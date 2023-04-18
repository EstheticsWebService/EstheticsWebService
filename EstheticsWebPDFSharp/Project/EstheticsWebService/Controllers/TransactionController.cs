using System;
using System.Collections.Generic;
using EstheticsWebService.Services.TransactionBuilder;
using Microsoft.AspNetCore.Mvc;
using DocxWorker;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace EstheticsWebService.Controllers
{
    /// <summary>
    /// Контроллер для работы web-приложения
    /// </summary>
    public class TransactionController : Controller
    {
        static private object obj = new object(); 
        /// <summary>
        /// Открытие страницы генераторов
        /// </summary>
        /// <returns></returns>
        [Route("")]
        public IActionResult Generators()
        {
            return View();
        }
       

        /// <summary>
        /// Выводит сгенерированный чек
        /// </summary>
        [HttpPost]
        [HttpGet]

        public IActionResult Check(DateTime dateOfCheck, double additionalTime, string cardOrAccount, string fourDigits, string operationSum, string currency, string authCode,string time)
        {
            ///Создание структуры, который описывает чек, заполнение данными, введенными пользователем
            TransactionInfoCheck infoCheck = new TransactionInfoCheck()
            {
                DateOfCheck = dateOfCheck,
                AdditionalTime = additionalTime,
                CardOrAccount = cardOrAccount,
                FourDigits = fourDigits,
                OperationSum = operationSum,
                Currency = currency,
                AuthCode = authCode
            };
            string[] timeParts = time.Split(':');
            infoCheck.Time = new DateTime(1, 1, 1, Convert.ToInt32(timeParts[0]), Convert.ToInt32(timeParts[1]),0);
            TransactionCheckBuilder builder = new TransactionCheckBuilder();  //Создание генератора чека
            return File(builder.BuildCheck(infoCheck), "image/png"); //Генерация чека, вывод чека png файлом
        }

        /// <summary>
        /// Выводит сгенерированный экран
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [HttpGet]
        public IActionResult Screen(string date,  string phoneNumber, string delay, string comment, string accountOwner, string price, string isProvided,
            string[]secondLine,string useSecondLine)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                date = DateTime.Now.ToString("dd-MM-yyyy");
            }
            if (string.IsNullOrWhiteSpace(delay))
            {
                delay = "3";
            }
            ///Создание класса, который описывает скриншот, заполнение данными, введенными пользователем
            TransactionInfoScreen infoScreen = new TransactionInfoScreen()
            {
                PhoneNumber = phoneNumber,
                Comment = comment,
                AccountOwner = accountOwner,
                Date = Convert.ToDateTime(date),
                Delay = Convert.ToInt32(delay),
                secondLine = secondLine,
                UseSecondLine = useSecondLine,
                IsProvided = isProvided
            };
            
            //получение цены из поле ввода в виде double. с приведением типов ничего не работает
            string[] priceParts = price.Split(',', '.');
            double priceDouble = Convert.ToDouble(priceParts[0]);
            if (priceParts.Length > 1) 
            {
                if (priceDouble >= 0)
                {
                    priceDouble += Convert.ToDouble(priceParts[1]) / 100;
                }
                else
                {
                    priceDouble -= Convert.ToDouble(priceParts[1]) / 100;
                }
            }
            infoScreen.Price = priceDouble;
            Debug.WriteLine(useSecondLine);
            foreach(var i in secondLine)
            {
                Debug.WriteLine(i);
            }





            TransactionScreenBuilder builder = new TransactionScreenBuilder(); //Создание генератора скриншота
            return File(builder.BuildScreen(infoScreen), "image/png"); // Генерация скриншота, вывод скриншота png файлом
        }


        // инициализация Pdf генератора и словаря, который используется для замен слов
        static Dictionary<string, string> words = null;
        DocxWorker.DocxWorker docxWorker;

        /// <summary>
        /// Генерация pdf файла 
        /// </summary>

        [HttpPost]
        public IActionResult CreatePDF(string date, string number, string sum, string currency, string delay, string accountOwner, string swift, string title = null)
        {
            FileContentResult res = null;
            //Слова для замены
            lock (obj)
            {
                try
                {
                    words = new Dictionary<string, string>();
                    words.Add("firstdate", Convert.ToDateTime(date).ToString("dd.MM.yyyy"));
                    words.Add("first_date", Convert.ToDateTime(date).ToString("dd.MM.yyyy"));

                    if (title.Contains("Transakcie", StringComparison.OrdinalIgnoreCase))
                    {
                        string withoutTransactie = title.Replace("Transakcie_", "", StringComparison.OrdinalIgnoreCase);
                        //  words.Add("Transakcie title", withoutTransactie);
                        words.Add("title", withoutTransactie);
                    }
                    else
                    {
                        // words.Add("Transakcie title", title);
                        words.Add("title", title);
                    }


                    words.Add("PBANRU4E", swift); words.Add("SWIFT1", swift);
                    words.Add("bank_holder", accountOwner);
                    words.Add("reqizit", number);
                    words.Add("summ_val", sum + " " + currency);
                    words.Add("summ", sum); words.Add("val", currency);
                    words.Add("second_date", Convert.ToDateTime(date).AddDays(Convert.ToDouble(delay)).ToString("dd.MM.yyyy"));
                    words.Add("seconddate", Convert.ToDateTime(date).AddDays(Convert.ToDouble(delay)).ToString("dd.MM.yyyy"));
                    docxWorker = new DocxWorker.DocxWorker("1.pdf", words);
                 
                    byte[] bytes = docxWorker.GeneratePDF();
                    res = File(bytes, "application/pdf", title + ".pdf");
              
                }
                catch { }
            }
            return res;// возврат пользователю файла



        }

      


    }
}