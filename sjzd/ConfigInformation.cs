namespace sjzd
{
    internal class ConfigInformation
    {
        private static ConfigInformation _configInformation;

        // 数据库链接字符串加解密 Key Value
        public static string Key = "27e16963-9999-4bc1-bea0-c8781a9f01cb";
        public static string Vector = "8280d369-f9bf-6666-bbfa-5e0b4b672958";

        public ConfigInformation Instance
        {
            get
            {
                if (_configInformation == null) _configInformation = new ConfigInformation();
                return _configInformation;
            }
        }
    }
}