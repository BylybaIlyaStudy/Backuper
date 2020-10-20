using System;

namespace backup
{
    class Settings
    {
        public string[] OriginalPaths { get; set; }
        public string TargetPath { get; set; }
        public Log.TypeOfLog TypeOfLog { get; set; }
    }
}
