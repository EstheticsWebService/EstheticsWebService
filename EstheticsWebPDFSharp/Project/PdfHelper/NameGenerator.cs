using System;
using System.Text;

namespace PdfHelper
{
    /// <summary>
    /// Статический класс для генерации названия
    /// </summary>
    public static class NameGenerator
    {
        /// <summary>
        /// Метод генерации названия файла
        /// </summary>
        /// <returns></returns>
		public static string GenerateName()
        {
            // transakcie_113-7882890-9842624.docx - шаблон 
            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            sb.Append("transakcie_");

            for (int i = 0; i < 3; i++)
                sb.Append(random.Next(0, 9));

            sb.Append("-");

            for (int i = 0; i < 7; i++)
                sb.Append(random.Next(0, 9));

            sb.Append("-");

            for (int i = 0; i < 7; i++)
                sb.Append(random.Next(0, 9));

            sb.Append("");

            return sb.ToString();

        }
    }
}
