namespace backup
{
    class Program
    {
        private static string settingsPath = "settings.json";

        static void Main(string[] args)
        {
            Settings s = new Settings
            {
                OriginalPaths = new[] { "P:\\C#\\Sokets (Server)", "P:\\C#\\Sokets (Client)" },
                TargetPath = "C:\\Users\\sksfb\\OneDrive\\\u0420\u0430\u0431\u043E\u0447\u0438\u0439 \u0441\u0442\u043E\u043B\\backup",
                TypeOfLog = Log.TypeOfLog.Info
            };

            new JSON(settingsPath).WriteJSON(s);

            Settings settings = new JSON(settingsPath).ReadJSON();

            FileManager fileManager = new FileManager(settings.OriginalPaths, settings.TargetPath, settings.TypeOfLog);

            fileManager.CreateBackup();
        }
    }
}
