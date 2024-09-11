namespace WelldoneAuth.Lib.Models
{
    public class CommonAPIModel<T>
    {
        /// <summary>
        /// api狀態
        /// </summary>
        public bool Success { get; set; } = true;
        /// <summary>
        /// 說明內容
        /// </summary>
        public string Msg { get; set; } = "";

        public T Data { get; set; }
    }
}
