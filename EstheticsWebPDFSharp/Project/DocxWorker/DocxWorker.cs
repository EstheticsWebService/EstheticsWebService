//using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
//using Word = Microsoft.Office.Interop.Word;
using PdfSharp.Drawing;
using PdfSharp.Pdf;



namespace DocxWorker
{
    /// <summary>
    /// Класс для работы с docx файлом
    /// Реализован функционал замены слов и попутной конвертации в pdf
    /// </summary>
    public class DocxWorker
    {
        /// <summary>
        /// Исходный путь к корневой папки библиотеки, для подгрузки файлов
        /// </summary>
        private static string binaryDirectory { get; set; } = Environment.CurrentDirectory;

        /// <summary>
        /// Путь для создания временного файла, в котором пишутся изменения
        /// и который конвертируется в .pdf файл
        /// </summary>
        private string tempFilePath { get; set; } = binaryDirectory + @"\temp.docx";

        /// <summary>
        /// Файл-шаблон в формате .docx
        /// </summary>
        private string FilePath { get; set; } = binaryDirectory + @"\test.docx";

        /// <summary>
        /// Словарь, хранящий слова, которые надо заменить и слова для замены 
        /// </summary>
        private Dictionary<string, string> WordsToReplace { get; set; }

        /// <summary>
        /// Название PDF файла и его расширение
        /// </summary>
        public string PDFFileName { get; set; }
        private XFont fontDateHeader = new XFont("Times New Roman", 9);
        private XFont fontTimes11 = new XFont("Times New Roman", 11);
        private XFont font = new XFont("Arial", 11);
        private XFont fontTop = new XFont("Arial", 10);
        private XFont fontBottom = new XFont("Arial", 8);
        private XFont fontHeader = new XFont("Arial", 12, XFontStyle.Bold);
        private XFont fontName = new XFont("Arial", 16, XFontStyle.Bold);
        private XImage imageFon = XImage.FromFile(@"Images\image-fon.png");
        /// <summary>
        /// Стандартный конструктор класса со стандартным путем к файлу
        /// </summary>
        /// <param name="replaceWords">словарь со словами для замены, необязательный параметр
        /// <param name="pdfFileName">имя pdf файла, обязательный параметр
        public DocxWorker(string pdfFileName, Dictionary<string, string> replaceWords = null)
        {
            this.PDFFileName = pdfFileName;
            this.WordsToReplace = replaceWords;
        }

        /// <summary>
        /// Конструктор класса для задания своего .docx файла 
        /// </summary>
        /// <param name="fileName">имя файла и расширение docx. пример: name.docx, обязательный параметр</param>
        /// <param name="pdfFileName">имя pdf файла, обязательный параметр
        /// <param name="replaceWords">словарь со словами для замены, необязательный параметр
        /// необязательный параметр</param>
        public DocxWorker(string fileName, string pdfFileName, Dictionary<string, string> replaceWords = null)
        {
            this.FilePath = @"\" + binaryDirectory + fileName;
            this.WordsToReplace = replaceWords;
            this.PDFFileName = pdfFileName;
        }
        /// <summary>
        /// Метод генерации PDF-файла и замены слов
        /// </summary>
        public byte[] GeneratePDF()
        {
            byte[] bytes = null;
            PdfDocument pdfDocument = new PdfDocument();
            try
            {
                pdfDocument.Info.Title = "transakcie_" + WordsToReplace["title"];
                // Create an empty page
                PdfPage page = pdfDocument.AddPage();
                // Get an XGraphics object for drawing
                XGraphics gfx = XGraphics.FromPdfPage(page);
                // Create a font
                //XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);
                // Draw the text

                gfx.DrawImage(imageFon, 23, 9, 555, 804);

                gfx.DrawString(WordsToReplace["first_date"], fontDateHeader, XBrushes.Black, new XPoint(35, 26));

                gfx.DrawString("vub.sk – Transakcie", fontTop, XBrushes.Black, new XPoint(215, 26));
                gfx.DrawString(WordsToReplace["title"], fontTop, XBrushes.Black, new XPoint(309, 26));

                gfx.DrawString("Číslo účtu:", fontHeader, XBrushes.Black, new XPoint(61, 130));
                gfx.DrawString("SK2402000000003565763359", font, XBrushes.Black, new XPoint(374, 129));
                gfx.DrawString("Majiteľ účtu:", fontHeader, XBrushes.Black, new XPoint(61, 147));
                gfx.DrawString(WordsToReplace["bank_holder"], fontTop, XBrushes.Black, new XPoint(374, 146));

                gfx.DrawString("Potvrdenie o zrealizovaní transakcie:", fontName, XBrushes.Black, new XPoint(165, 192));

                gfx.DrawString("Dátum vykonania:", fontHeader, XBrushes.Black, new XPoint(57, 265));
                gfx.DrawString(WordsToReplace["first_date"], fontTimes11, XBrushes.Black, new XPoint(354, 265));

                gfx.DrawString("Číslo dokladu:", fontHeader, XBrushes.Black, new XPoint(57, 282));
                gfx.DrawString(WordsToReplace["title"], font, XBrushes.Black, new XPoint(354, 282));

                gfx.DrawString("Typ transakcie:", fontHeader, XBrushes.Black, new XPoint(57, 312));
                gfx.DrawString("debet", font, XBrushes.Black, new XPoint(354, 314));

                gfx.DrawString("BIC:", fontHeader, XBrushes.Black, new XPoint(57, 329));

                gfx.DrawString("SWIFT:", fontHeader, XBrushes.Black, new XPoint(57, 344));
                gfx.DrawString(WordsToReplace["SWIFT1"], font, XBrushes.Black, new XPoint(354, 344));

                gfx.DrawString("Príjemca:", fontHeader, XBrushes.Black, new XPoint(57, 359));
                gfx.DrawString(WordsToReplace["reqizit"], fontTop, XBrushes.Black, new XPoint(354, 359));

                gfx.DrawString("Zúčtovaná suma:", fontHeader, XBrushes.Black, new XPoint(57, 374));
                gfx.DrawString(WordsToReplace["summ_val"], font, XBrushes.Black, new XPoint(354, 374));

                gfx.DrawString("Referencia platiteľa:", fontHeader, XBrushes.Black, new XPoint(57, 389));
                gfx.DrawString(WordsToReplace["reqizit"], font, XBrushes.Black, new XPoint(354, 389));

                gfx.DrawString("Variabilný symbol:", fontHeader, XBrushes.Black, new XPoint(57, 404));
                gfx.DrawString("Špecifický symbol:", fontHeader, XBrushes.Black, new XPoint(57, 419));
                gfx.DrawString("Konštantný symbol:", fontHeader, XBrushes.Black, new XPoint(57, 434));
                gfx.DrawString("Informácie pre príjemcu:", fontHeader, XBrushes.Black, new XPoint(57, 449));
                gfx.DrawString("Účel prevodu:", fontHeader, XBrushes.Black, new XPoint(57, 464));
                gfx.DrawString("Kategória účelu prevodu:", fontHeader, XBrushes.Black, new XPoint(57, 479));
                gfx.DrawString("Pôvodný odosielateľ:", fontHeader, XBrushes.Black, new XPoint(57, 494));
                gfx.DrawString("Konečný príjemca:", fontHeader, XBrushes.Black, new XPoint(57, 509));//-12

                gfx.DrawString("Spotový obchod:", fontHeader, XBrushes.Black, new XPoint(57, 524));
                gfx.DrawString("nie", font, XBrushes.Black, new XPoint(354, 524));

                gfx.DrawString("Rýchlosť platby:", fontHeader, XBrushes.Black, new XPoint(57, 539));
                gfx.DrawString("Štandardná", font, XBrushes.Black, new XPoint(354, 539));

                gfx.DrawString("Dátum zaúčtovania:", fontHeader, XBrushes.Black, new XPoint(57, 554));
                gfx.DrawString(WordsToReplace["second_date"], font, XBrushes.Black, new XPoint(354, 554));



                gfx.DrawString("Služba Kontakt: 0850 123 000 (v rámci SR)", fontBottom, XBrushes.DimGray, new XPoint(50, 753));
                gfx.DrawString("+421 2 4855 59 70 (zo zahraničia) E-mail:", fontBottom, XBrushes.DimGray, new XPoint(50, 763));
                gfx.DrawString("kontakt@vub.sk www.vub.sk", fontBottom, XBrushes.DimGray, new XPoint(50, 773));


                gfx.DrawString("Všeobecná úverová banka, a.s. (BIC: SUBASKBX)", fontBottom, XBrushes.DimGray, new XPoint(322, 748));
                gfx.DrawString("Mlynské nivy 1, 829 90 Bratislava 25 Obchodný", fontBottom, XBrushes.DimGray, new XPoint(322, 758));
                gfx.DrawString("register: Okresný súd Bratislava I Oddiel: Sa, vložka", fontBottom, XBrushes.DimGray, new XPoint(322, 768));
                gfx.DrawString("číslo: 341/B, IČO: 31320155", fontBottom, XBrushes.DimGray, new XPoint(322, 778));

                gfx.DrawString("1 / 1", fontBottom, XBrushes.DimGray, new XPoint(276, 798));

                //gfx.DrawString("Hello, World!", font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);
                // Save the document...
                string filename = binaryDirectory + @"\" + PDFFileName;
                //pdfDocument.Save(filename);

                MemoryStream stream = new MemoryStream();
                pdfDocument.Save(stream, false);
                bytes = stream.ToArray();


            }
            catch
            {

            }
            return bytes;


        }




    }
}
