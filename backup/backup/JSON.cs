using System.IO;
using System.Text.Json;

namespace backup
{
    class JSON
    {
        private string Path;

        public JSON(string path)
        {
            if (File.Exists(path))
            {
                this.Path = path;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public Settings ReadJSON()
        {
            StreamReader stream = new StreamReader(this.Path);
            string json = stream.ReadToEnd();

            Settings s = JsonSerializer.Deserialize<Settings>(json);

            return s;
        }

        public void WriteJSON(Settings s)
        {
            if (!File.Exists(this.Path)) File.Create(this.Path); 

            StreamWriter sw = new StreamWriter(this.Path, false, System.Text.Encoding.Default);
            string json = JsonSerializer.Serialize<Settings>(s);
            sw.Write(json);
            sw.Close();
        }
    }
}
