using System;
using System.Collections.Generic;
using System.IO;

namespace backup
{
    enum MethodOfCreationBackup
    {
        OnlyFiles,
        FilesAndFolders
    }

    class FileManager
    {
        private List<string> OriginalPaths = null;
        private string TargetPath;
        private Log.Log Logger= null;

        public FileManager(string[] OriginalPaths = null, string TargetPath = null, Log.TypeOfLog typeOfLog = Log.TypeOfLog.Info)
        {
            if (!Directory.Exists(TargetPath))
            {
                throw new DirectoryNotFoundException("Target directory doesn\'t exist");
            }
            if (TargetPath == null)
            {
                throw new ArgumentNullException("Target directory are not set");
            }

            this.TargetPath = TargetPath + "\\" + DateTime.Now.ToString().Replace(":", "-");
            this.Logger = new Log.Log(this.TargetPath, typeOfLog);

            if (OriginalPaths == null)
            {
                this.Logger.WriteLog(Log.TypeOfLog.Fatal, "Source directory are not set");
                throw new DirectoryNotFoundException("Source directory are not set");
            }
            this.OriginalPaths = new List<string>();

            foreach (string path in OriginalPaths)
            {
                if (Directory.Exists(path))
                {
                    this.OriginalPaths.Add(path);
                }
                else
                {
                    this.Logger.WriteLog(Log.TypeOfLog.Error, $"Directory \"{path}\" doesn\'t exist");
                }
            }
            if (this.OriginalPaths.Count == 0)
            {
                this.Logger.WriteLog(Log.TypeOfLog.Fatal, "Any directories doesn\'t exist");
                throw new DirectoryNotFoundException("Any directories doesn\'t exist");
            }
        }
        public FileManager(string[] OriginalPath, string TargetPath) : this(OriginalPath, TargetPath, Log.TypeOfLog.Info) { }

        public FileManager(string OriginalPath, string TargetPath, Log.TypeOfLog typeOfLog) : this(new[] { OriginalPath }, TargetPath, typeOfLog) { }
        public FileManager(string OriginalPath, string TargetPath) : this( new[] { OriginalPath }, TargetPath, Log.TypeOfLog.Info) { }

        public void CreateBackup(MethodOfCreationBackup method = MethodOfCreationBackup.OnlyFiles)
        {
            try
            {
                Directory.CreateDirectory(TargetPath);
                this.Logger.WriteLog(Log.TypeOfLog.Info, "A directory with a time stamp was created");
            }
            catch (Exception ex)
            {
                this.Logger.WriteLog(Log.TypeOfLog.Error, ex.Message);
            }
            switch (method)
            {
                case MethodOfCreationBackup.FilesAndFolders:
                    foreach (string path in this.OriginalPaths)
                    {
                        string dirName = "\\" + path.Split("\\")[path.Split("\\").Length - 1];
                        this.TargetPath += dirName;

                        CopyFolder(path, path);
                        this.TargetPath = this.TargetPath.Substring(0, this.TargetPath.Length - dirName.Length);
                    }
                    break;
                case MethodOfCreationBackup.OnlyFiles:
                    foreach (string path in this.OriginalPaths)
                    {
                        this.CopyFiles(path, path);
                    }
                    break;
                default:
                    break;

            }

            this.Logger.Dispose();
        }

        private void CopyFolder(string path, string originalPath)
        {
            try
            {
                if (!Directory.Exists(this.TargetPath))
                {
                    Directory.CreateDirectory(this.TargetPath);
                    this.Logger.WriteLog(Log.TypeOfLog.Info, $"{this.TargetPath} was creatied successfully");
                }

                try
                {
                    string[] dirList = Directory.GetDirectories(path); // массив с именами директорий (+ пути)

                    if (dirList.Length > 0)
                    {
                        foreach (string dirPath in dirList)
                        {
                            try
                            {
                                int i = 1;
                                string dirName = dirPath.Substring(originalPath.Length);

                                while (i < 64)
                                {
                                    if (!Directory.Exists(TargetPath + dirName))
                                    {
                                        Directory.CreateDirectory(TargetPath + dirName);
                                        this.Logger.WriteLog(Log.TypeOfLog.Info, $"{dirName} was creatied successfully");
                                        break;
                                    }

                                    this.Logger.WriteLog(Log.TypeOfLog.Warning, $"{TargetPath + dirName} exist.");
                                    dirName = "_" + i.ToString() + "_" + dirPath.Substring(originalPath.Length);
                                    i++;
                                }

                                CopyFolder(dirPath, originalPath);
                            }
                            catch (IOException)
                            {
                                this.Logger.WriteLog(Log.TypeOfLog.Error, $"{dirPath} is file.");
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }

                    CopyFiles(path, originalPath);
                }
                catch (UnauthorizedAccessException ex)
                {
                    this.Logger.WriteLog(Log.TypeOfLog.Error, ex.Message);
                }
            }
            catch (Exception ex)
            {
                this.Logger.WriteLog(Log.TypeOfLog.Fatal, ex.Message);
                throw ex;
            }

        }

        private void CopyFiles(string path, string originalPath)
        {
            try
            {
                string[] fileList = Directory.GetFiles(path); // массив с именами файлов (+ пути)

                foreach (string filePath in fileList)
                {
                    string fileName = filePath.Substring(originalPath.Length + 1);
                    int i = 1;

                    while (i < 64)
                    {
                        if (!File.Exists(Path.Combine(TargetPath, fileName)))
                        {
                            File.Copy(filePath, Path.Combine(TargetPath, fileName));
                            this.Logger.WriteLog(Log.TypeOfLog.Info, $"{filePath} was copied successfully");
                            break;
                        }

                        this.Logger.WriteLog(Log.TypeOfLog.Warning, $"{Path.Combine(TargetPath, fileName)} exist.");
                        fileName = "_" + i.ToString() + "_" + filePath.Substring(originalPath.Length + 1);

                        i++;
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                this.Logger.WriteLog(Log.TypeOfLog.Error, ex.Message);
            }
            catch (Exception ex)
            {
                this.Logger.WriteLog(Log.TypeOfLog.Fatal, ex.Message);
                throw ex;
            }
        }
    }
}
