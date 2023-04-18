using System;

namespace EstheticsWebService.Services.TransactionBuilder
{

    /// <summary>
    /// структура, описывающая входные данные для создания скриншота экрана
    /// </summary>
    public struct TransactionInfoScreen
    {
        /// <summary>
        /// Дата 
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Цена
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// Номер телефона
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Отсрочка в днях
        /// </summary>
        public int Delay { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Владелец аккаунта
        /// </summary>
        public string AccountOwner { get; set; }
        /// <summary>
        /// Массив значений второй строки
        /// </summary>
        public string []secondLine { get; set; }
        /// <summary>
        /// Флаг отображения второй строки. 1 - да
        /// </summary>
        public string UseSecondLine { get; set; }
        /// <summary>
        /// Подтверждена ли первая операция. 1 - да
        /// </summary>
        public string IsProvided { get; set; }
    }
}
