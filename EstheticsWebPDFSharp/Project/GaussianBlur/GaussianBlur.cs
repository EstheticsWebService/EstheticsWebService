using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GaussianBlur
{    /// <summary>
     /// Класс, содержащий логику размытия по Гауссу
     /// </summary>
    public class GaussianBlur
    {
        #region Переменные для работы алгоримта размытия по Гауссу
        private int[] _alpha;
        private int[] _red;
        private int[] _green;
        private int[] _blue;

        private int _width; //длина изображения, которое передается в конструктор класса
        private int _height; //высота изображения, которое передается в конструктор класса


        private ParallelOptions _pOptions = new ParallelOptions { MaxDegreeOfParallelism = 16 }; // Настройка параллелизма
        #endregion


        /// <summary>
        /// Словарь, хранящий точки, в пределах которых будет происходить размытие
        /// Ключ хранит структуру Point, которая хранит в себе лево-верхний угол зоны размытия
        /// Значение хранит структуру Point, которая хранит в себе длину и ширину зоны размытия
        /// </summary>
        private Dictionary<Point, Point> PointsForBluring { get; set; }

        /// <summary>
        /// Изображение, над которым производятся преобразования
        /// </summary>
        private Bitmap Image { get; set; }




        /// <summary>
        /// Конструктор класса, в который передается изображение 
        /// </summary>
        /// <param name="image">само изображение в виде bitmap</param>
        public GaussianBlur(Bitmap image)
        {
            this.Image = image;
        }
        /// <summary>
        /// Конструктор класса, в который передается изображение 
        /// и словарь зон для блюра
        /// </summary>
        /// <param name="image">само изображение в виде bitmap</param>
        /// <param name="zones">зоны для блюринга</param>
        public GaussianBlur(Bitmap image, Dictionary<Point, Point> zones)
        {
            this.Image = image;
            this.PointsForBluring = zones;
        }


        /// <summary>
        /// Метод создания размытия по Гауссу
        /// </summary>
        /// <param name="radial">Сила размытия. Чем больше число, тем сильнее размытие</param>
        /// <returns></returns>
        public Bitmap Process(int radial)
        {
            // получение пикселей входного изображения
            var rct = new Rectangle(0, 0, Image.Width, Image.Height);
            var source = new int[rct.Width * rct.Height];
            var bits = Image.LockBits(rct, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Marshal.Copy(bits.Scan0, source, 0, source.Length);
            Image.UnlockBits(bits);

            _width = Image.Width; _height = Image.Height; //ширина и высота входного изображения
            //массивы, хранящие числа для ARGB-цвета
            _alpha = new int[_width * _height]; _red = new int[_width * _height];
            _green = new int[_width * _height]; _blue = new int[_width * _height];

            Parallel.For(0, source.Length, _pOptions, i =>   //заполнение массивов значениями входного изображентя
            {
                _alpha[i] = (int)((source[i] & 0xff000000) >> 24);
                _red[i] = (source[i] & 0xff0000) >> 16;
                _green[i] = (source[i] & 0x00ff00) >> 8;
                _blue[i] = (source[i] & 0x0000ff);
            });

            Bitmap handlingImage = new Bitmap(_width, _height); //временное изображение для обработки
            Rectangle handlingRectangle = new Rectangle(0, 0, handlingImage.Width, handlingImage.Height); //зона обработки

            //Если есть точки в словаре, то размытие будет происходить только по ним
            //Иначе будет блюриться вся картинка
            if (PointsForBluring != null && PointsForBluring.Count > 0)
            {
                foreach (KeyValuePair<Point, Point> value in PointsForBluring)
                {
                    int x = value.Key.X, y = value.Key.Y; //лево-верхний угол прямоугольника
                    int w = value.Value.X, h = value.Value.Y; // ширина и высота
                    _width = w; _height = h;// ширина и высота для работы побочных методов


                    int startPoint = Image.Width * y + x; // получение точки обработки
                                                          //массивы, хранящие числа для ARGB-цвета
                    int[] selectedAlpha = new int[w * h]; int[] selectedRed = new int[w * h];
                    int[] selectedGreen = new int[w * h]; int[] selectedBlue = new int[w * h];
                    int[] selectedDest = new int[w * h];

                    // копирование пикселей с участка изображения, предназначенного для размытия
                    for (int shift = 0; shift < h; shift++)
                    {
                        for (int pixel = 0; pixel < w; pixel++)
                        {
                            selectedAlpha[(shift * h) + pixel] = _alpha[startPoint + (shift * Image.Width) + pixel];
                            selectedRed[(shift * h) + pixel] = _red[startPoint + (shift * Image.Width) + pixel];
                            selectedGreen[(shift * h) + pixel] = _green[startPoint + (shift * Image.Width) + pixel];
                            selectedBlue[(shift * h) + pixel] = _blue[startPoint + (shift * Image.Width) + pixel];
                        }
                    }
                    int[] selectedNewAlpha = new int[w * h]; int[] selectedNewRed = new int[w * h];
                    int[] selectedNewGreen = new int[w * h]; int[] selectedNewBlue = new int[w * h];
                    int[] selectedNewDest = new int[w * h];

                    Parallel.Invoke(        //Запуск параллельного расчета, замена старого пикселя на обработный по алгоритму
                   () => gaussBlur_4(selectedAlpha, selectedNewAlpha, radial),
                   () => gaussBlur_4(selectedRed, selectedNewRed, radial),
                   () => gaussBlur_4(selectedGreen, selectedNewGreen, radial),
                   () => gaussBlur_4(selectedBlue, selectedNewBlue, radial));

                    //Удержание значений в диапазоне от 0 до 255
                    Parallel.For(0, selectedNewDest.Length, _pOptions, i =>
                    {
                        if (selectedNewAlpha[i] > 255) selectedNewAlpha[i] = 255;
                        if (selectedNewRed[i] > 255) selectedNewRed[i] = 255;
                        if (selectedNewGreen[i] > 255) selectedNewGreen[i] = 255;
                        if (selectedNewBlue[i] > 255) selectedNewBlue[i] = 255;

                        if (selectedNewAlpha[i] < 0) selectedNewAlpha[i] = 0;
                        if (selectedNewRed[i] < 0) selectedNewRed[i] = 0;
                        if (selectedNewGreen[i] < 0) selectedNewGreen[i] = 0;
                        if (selectedNewBlue[i] < 0) selectedNewBlue[i] = 0;

                        selectedNewDest[i] = (int)((uint)(selectedNewAlpha[i] << 24) | (uint)(selectedNewRed[i] << 16) | (uint)(selectedNewGreen[i] << 8) | (uint)selectedNewBlue[i]);
                    });


                    // замена выделенных пикселей в итоговом изображении               
                    for (int shift = 0; shift < h; shift++)
                    {
                        for (int pixel = 0; pixel < w; pixel++)
                        {
                            _alpha[startPoint + (shift * Image.Width) + pixel] = selectedAlpha[pixel + (shift * w)];
                            _red[startPoint + (shift * Image.Width) + pixel] = selectedRed[pixel + (shift * w)];
                            _green[startPoint + (shift * Image.Width) + pixel] = selectedGreen[pixel + (shift * w)];
                            _blue[startPoint + (shift * Image.Width) + pixel] = selectedBlue[pixel + (shift * w)];
                        }
                    }
                    // Применение изменений на итоговой картинке
                    var destination = new int[Image.Width * Image.Height];
                    Parallel.For(0, destination.Length, _pOptions, i =>
                    {
                        destination[i] = (int)((uint)(_alpha[i] << 24) | (uint)(_red[i] << 16) | (uint)(_green[i] << 8) | (uint)_blue[i]);
                    });
                    var bitsToCopy = handlingImage.LockBits(handlingRectangle, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    Marshal.Copy(destination, 0, bitsToCopy.Scan0, destination.Length);
                    handlingImage.UnlockBits(bitsToCopy);
                }
            }
            else
            {
                //массивы, хранящие замененные числа для ARGB-цвета
                var newAlpha = new int[_width * _height]; var newRed = new int[_width * _height];
                var newGreen = new int[_width * _height]; var newBlue = new int[_width * _height];
                var dest = new int[_width * _height];

                //Запуск параллельного расчета, замена старого пикселя на обработный по алгоритму
                Parallel.Invoke(
                    () => gaussBlur_4(_alpha, newAlpha, radial),
                    () => gaussBlur_4(_red, newRed, radial),
                    () => gaussBlur_4(_green, newGreen, radial),
                    () => gaussBlur_4(_blue, newBlue, radial));

                //Удержание значений в диапазоне от 0 до 255
                Parallel.For(0, dest.Length, _pOptions, i =>
                {
                    if (newAlpha[i] > 255) newAlpha[i] = 255;
                    if (newRed[i] > 255) newRed[i] = 255;
                    if (newGreen[i] > 255) newGreen[i] = 255;
                    if (newBlue[i] > 255) newBlue[i] = 255;

                    if (newAlpha[i] < 0) newAlpha[i] = 0;
                    if (newRed[i] < 0) newRed[i] = 0;
                    if (newGreen[i] < 0) newGreen[i] = 0;
                    if (newBlue[i] < 0) newBlue[i] = 0;

                    dest[i] = (int)((uint)(newAlpha[i] << 24) | (uint)(newRed[i] << 16) | (uint)(newGreen[i] << 8) | (uint)newBlue[i]);
                });

                // Применение изменений к картинке
                rct = new Rectangle(0, 0, handlingImage.Width, handlingImage.Height);
                var bitsToCopy = handlingImage.LockBits(rct, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                Marshal.Copy(dest, 0, bitsToCopy.Scan0, dest.Length);
                handlingImage.UnlockBits(bitsToCopy);
            }

            return handlingImage; // возврат готового изображения
        }



        #region Внутренние методы для создания размытия по Гауссу
        private void gaussBlur_4(int[] source, int[] dest, int r)
        {
            var bxs = boxesForGauss(r, 3);
            boxBlur_4(source, dest, _width, _height, (bxs[0] - 1) / 2);
            boxBlur_4(dest, source, _width, _height, (bxs[1] - 1) / 2);
            boxBlur_4(source, dest, _width, _height, (bxs[2] - 1) / 2);
        }

        private int[] boxesForGauss(int sigma, int n)
        {
            var wIdeal = Math.Sqrt((12 * sigma * sigma / n) + 1);
            var wl = (int)Math.Floor(wIdeal);
            if (wl % 2 == 0) wl--;
            var wu = wl + 2;

            var mIdeal = (double)(12 * sigma * sigma - n * wl * wl - 4 * n * wl - 3 * n) / (-4 * wl - 4);
            var m = Math.Round(mIdeal);

            var sizes = new List<int>();
            for (var i = 0; i < n; i++) sizes.Add(i < m ? wl : wu);
            return sizes.ToArray();
        }

        private void boxBlur_4(int[] source, int[] dest, int w, int h, int r)
        {
            for (var i = 0; i < source.Length; i++) dest[i] = source[i];
            boxBlurH_4(dest, source, w, h, r);
            boxBlurT_4(source, dest, w, h, r);
        }

        private void boxBlurH_4(int[] source, int[] dest, int w, int h, int r)
        {
            var iar = (double)1 / (r + r + 1);
            Parallel.For(0, h, _pOptions, i =>
            {
                var ti = i * w;
                var li = ti;
                var ri = ti + r;
                var fv = source[ti];
                var lv = source[ti + w - 1];
                var val = (r + 1) * fv;
                for (var j = 0; j < r; j++) val += source[ti + j];
                for (var j = 0; j <= r; j++)
                {
                    val += source[ri++] - fv;
                    dest[ti++] = (int)Math.Round(val * iar);
                }
                for (var j = r + 1; j < w - r; j++)
                {
                    val += source[ri++] - dest[li++];
                    dest[ti++] = (int)Math.Round(val * iar);
                }
                for (var j = w - r; j < w; j++)
                {
                    val += lv - source[li++];
                    dest[ti++] = (int)Math.Round(val * iar);
                }
            });
        }

        private void boxBlurT_4(int[] source, int[] dest, int w, int h, int r)
        {
            var iar = (double)1 / (r + r + 1);
            Parallel.For(0, w, _pOptions, i =>
            {
                var ti = i;
                var li = ti;
                var ri = ti + r * w;
                var fv = source[ti];
                var lv = source[ti + w * (h - 1)];
                var val = (r + 1) * fv;
                for (var j = 0; j < r; j++) val += source[ti + j * w];
                for (var j = 0; j <= r; j++)
                {
                    val += source[ri] - fv;
                    dest[ti] = (int)Math.Round(val * iar);
                    ri += w;
                    ti += w;
                }
                for (var j = r + 1; j < h - r; j++)
                {
                    val += source[ri] - source[li];
                    dest[ti] = (int)Math.Round(val * iar);
                    li += w;
                    ri += w;
                    ti += w;
                }
                for (var j = h - r; j < h; j++)
                {
                    val += lv - source[li];
                    dest[ti] = (int)Math.Round(val * iar);
                    li += w;
                    ti += w;
                }
            });
        }
        #endregion
    }
}