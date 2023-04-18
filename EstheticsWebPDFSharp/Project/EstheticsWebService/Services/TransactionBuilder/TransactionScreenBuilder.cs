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
    /// Класс для создания скриншота экрана
    /// </summary>
    class TransactionScreenBuilder
    {

        public string IMAGE_PATH = "Images/TEMPLATE.png"; // путь для обработки изображения
        public double OLD_BALANCE = 499.74; // баланс


        /// <summary>
        /// Создает скриншот экрана
        /// </summary>
        /// <param name="info">объект, описывающий скриншот</param>
        /// <returns></returns>
        public byte[] BuildScreen(TransactionInfoScreen info)
        {
            Image image; Graphics graphics;
 
            // получение изображения для работы с ним
            using (var stream = new FileStream(IMAGE_PATH, FileMode.Open, FileAccess.Read))
            {
                image = Bitmap.FromStream(stream); // само изображение
                graphics = Graphics.FromImage(image); //объект графики для работы с изображением

                // настройка объекта графики
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            }

            //цвета
            var redBrush = new SolidBrush(Color.FromArgb(204, 51, 51));
            var greenBrush = new SolidBrush(Color.FromArgb(28, 129, 77));
            var blackBrush = new SolidBrush(Color.FromArgb(0,0,0));
            var orangeBrush = new SolidBrush(Color.FromArgb(255, 141, 65));
            var ownerBrush = new SolidBrush(Color.FromArgb(102, 102, 102));
            //шрифты
            var arialFont = new Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Point);
            var priceFont = new Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Point);
            var dateFont = new Font("Arial", 11, FontStyle.Bold, GraphicsUnit.Point);

            var rightAlligment = new StringFormat // Выравнивание по правой стороне
            {
               Alignment = StringAlignment.Far
            };


            int shift = 0; // сдвиг строк на кол-во пикселей, если есть 2-я линия
            int lineCounter = 1; // счетчик линий 


            string[] remainderParts;
            //получение конечной суммы из поле ввода в виде double. с приведением типов ничего не работает
            if (info.UseSecondLine == "1")
            {
                remainderParts = info.secondLine[5].Split(',', '.');
            }
            else
            {
                remainderParts = info.secondLine[0].Split(',', '.');
            }
  
            double remainderDouble = Convert.ToDouble(remainderParts[0]);
            if (remainderParts.Length > 1)
            {
                if (remainderDouble >= 0)
                {
                    remainderDouble += Convert.ToDouble(remainderParts[1]) / 100;
                }
                else
                {
                    remainderDouble -= Convert.ToDouble(remainderParts[1]) / 100;
                }
            }
            double balance = remainderDouble; // остаток(начинается с конечной суммы)

            string provided; //подтверждена ли операция
            if (info.IsProvided == "1" || (string.IsNullOrEmpty(info.IsProvided)&&string.IsNullOrEmpty(info.UseSecondLine)))
            {
                provided = "PROVIDED";
            }
            else
            {
                provided = "NOTPROVIDED";
            }

            // Добавление 1-й линии
            List<ScreenLine> LinesList = new List<ScreenLine>();
            ScreenLine firstLine = new ScreenLine
            {
                FirstDate = info.Date,
                SecondDate = info.Date.AddDays(info.Delay),
                OperationText = info.Comment,
                Phone = info.PhoneNumber,
                Price = info.Price,
                Status = provided,
                Balance = OLD_BALANCE - info.Price,
                Coords = new Point(390, 579)
            };


                LinesList.Add(firstLine);



            double priceDouble=0;
            // добавление второй линии, которую выбрали в генераторе
            if (info.UseSecondLine == "1")
            {   
                //получение цены из поле ввода в виде double. с приведением типов ничего не работает
                string[] priceParts = info.secondLine[1].Split(',', '.');
                priceDouble = Convert.ToDouble(priceParts[0]);
                if (priceParts.Length > 1)
                {
                    if(priceDouble >= 0)
                    {
                        priceDouble += Convert.ToDouble(priceParts[1]) / 100;
                    }
                    else
                    {
                        priceDouble -= Convert.ToDouble(priceParts[1]) / 100;
                    }
                   
                }
                LinesList.Add(new ScreenLine
                {
                    FirstDate = Convert.ToDateTime(info.secondLine[0]),
                    SecondDate = Convert.ToDateTime(info.secondLine[0]).AddDays(Convert.ToDouble(info.secondLine[4])),
                    Price = priceDouble,
                    Phone = info.secondLine[2],
                    OperationText = info.secondLine[3],
                    Coords = new Point(390,616),
                    Status = "PROVIDED"                 
                }); 
               
                shift = 37;
            }


           //добавление дефолтных линий
            LinesList.Add(new ScreenLine()
            {
                FirstDate = info.Date.AddDays(-2),
                SecondDate = info.Date.AddDays(0),
                Phone = "+79094393734",
                Price= 9.89,
                Status = "PROVIDED",
                OperationText= "Poplatok Brawl Stars ...",
                Coords=new Point(390,616+shift)
            });
            LinesList.Add(new ScreenLine()
            {
                FirstDate = info.Date.AddDays(-5),
                SecondDate = info.Date.AddDays(-3),
                Phone = "4009918921673717",
                Price = -500.00,
                Status = "NOTPROVIDED",
                OperationText = "Doplnenie uctu",
                Coords = new Point(390, 653 + shift)
            });
            LinesList.Add(new ScreenLine()
            {
                FirstDate = info.Date.AddDays(-7),
                SecondDate = info.Date.AddDays(-5),
                Phone = "+79955004549",
                Price = 290.90,
                Status = "PROVIDED",
                OperationText = "Poplatok Brawl Stars ...",
                Coords = new Point(390, 689 + shift)
            });
            LinesList.Add(new ScreenLine()
            {
                FirstDate = info.Date.AddDays(-10),
                SecondDate = info.Date.AddDays(-7),
                Phone = "+4009918921673717",
                Price = -150.00,
                Status = "NOTPROVIDED",
                OperationText = "Doplnenie uctu",
                Coords = new Point(390, 726 + shift)
            });
            LinesList.Add(new ScreenLine()
            {
                FirstDate = info.Date.AddDays(-12),
                SecondDate = info.Date.AddDays(-10),
                Phone = "+79047151776",
                Price = 32.01,
                Status = "PROVIDED",
                OperationText = "Poplatok Brawl Stars ...",
                Coords = new Point(390, 762 + shift)
            });
            LinesList.Add(new ScreenLine()
            {
                FirstDate = info.Date.AddDays(-14),
                SecondDate = info.Date.AddDays(-12),
                Phone = "+79684474532",
                Price = 10.37,
                Status = "PROVIDED",
                OperationText = "Poplatky Nonstop banki…",
                Coords = new Point(390, 799 + shift)
            });
            LinesList.Add(new ScreenLine()
            {
                FirstDate = info.Date.AddDays(-17),
                SecondDate = info.Date.AddDays(-15),
                Phone = "+79049485261",
                Price = 51.54,
                Status = "PROVIDED",
                OperationText = "Vedenie konta Start kon…",
                Coords = new Point(390, 837 + shift)
            });
            LinesList.Add(new ScreenLine()
            {
                FirstDate = info.Date.AddDays(-19),
                SecondDate = info.Date.AddDays(-17),
                Phone = "4009918921673717",
                Price = -150.00,
                Status = "NOTPROVIDED",
                OperationText = "Doplnenie uctu",
                Coords = new Point(390, 874 + shift)
            });
            LinesList.Add(new ScreenLine()
            {
                FirstDate = info.Date.AddDays(-22),
                SecondDate = info.Date.AddDays(-20),
                Phone = "4214995172291465",
                Price = 490.00,
                Status = "NOTPROVIDED",
                OperationText = "Vyber s poplatkom",
                Coords = new Point(390, 911 + shift)
            });


            // Отрисовка владельца
            graphics.DrawString(info.AccountOwner, arialFont, ownerBrush, new Point(832, 78));
            //Отрисовка линий
            foreach (var line in LinesList)
            {
                                        
                if (lineCounter < 11)
                {
                    //if (lineCounter % 2 == 0) // фон на линии, кратной двум
                    //{
                    //    graphics.DrawRectangle(new Pen(Color.FromArgb(255, 249, 246, 246),31f),
                    //        new Rectangle(line.Coords.X - 140, line.Coords.Y +5, 1400, 5));
                    //}
                  
                    // Отрисовка левой даты(ранней даты)
                    graphics.DrawString(line.FirstDate.ToString("dd.MM.yyyy"), dateFont, orangeBrush, line.Coords);

                    // Отрисовка правой даты(поздней даты)
                    graphics.DrawString(line.SecondDate.ToString("dd.MM.yyyy"), arialFont, blackBrush, new Point(491, line.Coords.Y));

                    //Отрисовка цены
                    if (line.Price > 0)
                    {
                        graphics.DrawString(string.Format("-{0:F2}", line.Price) + " €", priceFont, redBrush, new Point(600+90, line.Coords.Y),rightAlligment);
                    }
                    else
                    {
                        string price = line.Price.ToString().Replace('-', ' ');

                        graphics.DrawString(string.Format("{0:F2}",Convert.ToDouble(price)) + " €", priceFont, greenBrush, new Point(600+90, line.Coords.Y), rightAlligment);
                    }
                    // Отрисовка иконки 
                   // graphics.DrawImage(line.Icon, new Rectangle(697, line.Coords.Y-10, 37, 37));
              

                    //Отрисовка телефона
                    graphics.DrawString(line.Phone, arialFont, blackBrush, new Point(758, line.Coords.Y));
                    //Отрисовка Provided
                    graphics.DrawString(line.Status, arialFont, blackBrush, new Point(1099, line.Coords.Y));
                    //Отрисовка комментария
                    graphics.DrawString(line.OperationText, arialFont, blackBrush, new Point(1246, line.Coords.Y));

                 

                    //Отрисовка остатка
                    if (info.UseSecondLine == "1")
                    {
                        if (lineCounter == 1)
                        {
                           
                            graphics.DrawString(string.Format("{0:F2}", remainderDouble-line.Price).ToString(), arialFont, blackBrush, new Point(1528, line.Coords.Y), rightAlligment);
                        }
                        else if (lineCounter == 2)
                        {
                            balance = remainderDouble;
                            graphics.DrawString(string.Format("{0:F2}", balance), arialFont, blackBrush, new Point(1528, line.Coords.Y), rightAlligment);
                        }
                        else
                        {
                            graphics.DrawString(string.Format("{0:F2}", balance), arialFont, blackBrush, new Point(1528, line.Coords.Y), rightAlligment);
                        }
                    }
                    else
                    {
                        graphics.DrawString(string.Format("{0:F2}", balance), arialFont, blackBrush, new Point(1528, line.Coords.Y), rightAlligment);
                    }
                    //подсчет остатка
                    balance += line.Price;
                }
                lineCounter++;
            }

          
            //перевод изображения в массив байтов для дальнейшего считывания,
            //возврат массива байтов методом
            using (var imgStream = new MemoryStream())
            {
                image.Save(imgStream, ImageFormat.Png);
                return imgStream.ToArray();
            }                     
        }       
    }

    /// <summary>
    /// Вспомогательный класс строки для генератора скриншота экрана
    /// </summary>
    class ScreenLine
    {   /// <summary>
        /// Ранняя дата
        /// </summary>
        public DateTime FirstDate { get; set; }
        /// <summary>
        /// Поздняя дата
        /// </summary>
        public DateTime SecondDate { get; set; }
        /// <summary>
        /// Стоимость операции
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// Значок операции
        /// </summary>
        public Image Icon { get; set; }
        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Статус операции
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Описание операции
        /// </summary>
        public string OperationText { get; set; }
        /// <summary>
        /// Остаток
        /// </summary>
        public double Balance { get; set; }
        /// <summary>
        /// Координаты первого элемента линии, по нему калибруются остальные
        /// </summary>
        public Point Coords { get; set; }
    }
}
