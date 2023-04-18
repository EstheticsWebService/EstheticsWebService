using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Globalization;
using System.Drawing.Text;



namespace EstheticsWebService.Services.TransactionBuilder
{
    /// <summary>
    /// Класс для создания чека
    /// </summary>
    class TransactionCheckBuilder
    {

        public string IMAGE_PATH = "Images/CHECK.png"; // путь для обработки изображения
        public float OLD_BALANCE = 499.74f;// баланс


        /// <summary>
        /// Создает чек
        /// </summary>
        /// <param name="info">объект, описывающий чек</param>
        /// <returns></returns>
        public byte[] BuildCheck(TransactionInfoCheck info)
        {
            Image image; Graphics graphics;
            // получение изображения для работы с ним
            using (var stream = new FileStream(IMAGE_PATH, FileMode.Open, FileAccess.Read))
            {
                image = Bitmap.FromStream(stream); // само изображение
                graphics = Graphics.FromImage(image); //объект графики для работы с изображением

               // настройка объекта графики
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            }
                
                Font VAR_font;
                SolidBrush VAR_brush;

                // Отрисовка даты
                VAR_font = new Font("Arial", 25, FontStyle.Regular, GraphicsUnit.Point);
                VAR_brush = new SolidBrush(Color.FromArgb(134, 134, 144));
                graphics.DrawString(info.DateOfCheck.ToString("dd.MM.yyyy"), VAR_font, VAR_brush, new Point(543, 915));

                // Отрисовка времени
                VAR_font = new Font("Arial", 25, FontStyle.Regular, GraphicsUnit.Point);
                VAR_brush = new SolidBrush(Color.FromArgb(134, 134, 144));
                graphics.DrawString(info.Time.AddMinutes(info.AdditionalTime).ToString("HH:mm"), VAR_font, VAR_brush, new Point(744, 1033));
                // ===========================


                // Отрисовка слова "счета" или "карты"(выбирает пользователь)
                VAR_font = new Font("Arial", 22, FontStyle.Regular, GraphicsUnit.Point);
                VAR_brush = new SolidBrush(Color.FromArgb(125, 125, 127));
                graphics.DrawString(info.CardOrAccount, VAR_font, VAR_brush, new Point(521, 1388));

                // Отрисовка числа "5295"
                VAR_font = new Font("Arial", 24, FontStyle.Regular, GraphicsUnit.Point);
                VAR_brush = new SolidBrush(Color.FromArgb(125, 125, 127));
                graphics.DrawString("5295", VAR_font, VAR_brush, new Point(848, 1268));

                 // Отрисовка четырех чисел счета или карты(выбирает пользователь)
                VAR_font = new Font("Arial", 24, FontStyle.Regular, GraphicsUnit.Point);
                VAR_brush = new SolidBrush(Color.FromArgb(125, 125, 127));
                graphics.DrawString(info.FourDigits, VAR_font, VAR_brush, new Point(818, 1381));
                
                // Отрисовка суммы операции
                VAR_font = new Font("Arial", 25, FontStyle.Regular, GraphicsUnit.Point);
                VAR_brush = new SolidBrush(Color.FromArgb(125, 125, 127));
                graphics.DrawString($"{info.OperationSum} {info.Currency}", VAR_font, VAR_brush, new Point(593, 1500));
                // ===========================

                // Отрисовка комиссии
                VAR_font = new Font("Arial", 23, FontStyle.Regular, GraphicsUnit.Point);
                VAR_brush = new SolidBrush(Color.FromArgb(117, 117, 126));
                graphics.DrawString($"0.00 {info.Currency}", VAR_font, VAR_brush, new Point(412, 1619));
                // ===========================

                // Отрисовка кода авторизации
                VAR_font = new Font("Arial", 25, FontStyle.Regular, GraphicsUnit.Point);
                VAR_brush = new SolidBrush(Color.FromArgb(125, 125, 127));
                graphics.DrawString(info.AuthCode, VAR_font, VAR_brush, new Point(626, 1736));


            //перевод изображения в массив байтов для дальнейшего считывания,
            //возврат массива байтов методом
                using (var imgStream = new MemoryStream())
                {
                    image.Save(imgStream, ImageFormat.Png);
                    return imgStream.ToArray();
                }                       
        }
    }
}
