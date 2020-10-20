using System;
using System.IO;

namespace Log
{
    enum TypeOfLog
    {
        Info,
        Warning,
        Error,
        Fatal
    }

    class Log : IDisposable
    {
        private string Path;
        private StreamWriter file = null;
        private TypeOfLog TypeOfLog = TypeOfLog.Info;

        public Log(string path, TypeOfLog typeOfLog)
        {
            this.Path = path + " -- .log";
            this.TypeOfLog = typeOfLog;

            this.file = new StreamWriter(this.Path);
        }

        public Log(string path) : this(path, TypeOfLog.Info) { }

        public void WriteLog(TypeOfLog ClassOfMessage, string Message) //(запись нового лога) параметры - 1. класс сообщения, 2. текст сообщения
        {
            if (ClassOfMessage >= TypeOfLog) {
                string S_ClassOfMessage = null;

                switch (ClassOfMessage)
                {
                    case TypeOfLog.Info:
                        S_ClassOfMessage = "[Info] ";
                        break;
                    case TypeOfLog.Warning:
                        S_ClassOfMessage = "[Warn] ";
                        break;
                    case TypeOfLog.Error:
                        S_ClassOfMessage = "[Error] ";
                        break;
                    case TypeOfLog.Fatal:
                        S_ClassOfMessage = "[Fatal] ";
                        break;
                }

                S_ClassOfMessage += Message;
                S_ClassOfMessage += $" {DateTime.Now.ToString()}";
                this.file.WriteLine(S_ClassOfMessage);
            }
        }

        public void Dispose()
        {
            this.file.Close();
        }
    }
}
