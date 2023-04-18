using System;

namespace EstheticsWebService.Services.TransactionBuilder
{ 
    /// <summary>
    /// Описывает чек
    /// </summary>
    public struct TransactionInfoCheck
    {
        /// <summary>
        /// Дата чека
        /// </summary>
        public DateTime DateOfCheck { get; set; } 
        /// <summary>
        /// Дополнительное время
        /// </summary>
        public Double AdditionalTime { get; set; }
        /// <summary>
        /// Выбор карты или счета
        /// </summary>
        public string CardOrAccount { get; set; }
        /// <summary>
        /// 4 цифры
        /// </summary>
        public string FourDigits { get; set; }
        /// <summary>
        /// Сумма операции
        /// </summary>
        public string OperationSum { get; set; }
        /// <summary>
        /// Валюта
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Выбранный 7 цифренный код
        /// </summary>
        public string AuthCode { get; set; }
        /// <summary>
        /// Время
        /// </summary>
        public DateTime Time{ get; set; }
    }
}
