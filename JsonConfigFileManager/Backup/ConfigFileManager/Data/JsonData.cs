using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConfigFileManager.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonData
    {
        protected string fileName;

        public string FileName
        {
            set { fileName = value; }
            get { return fileName; }
        }

        public void SaveFile()
        {
            File.WriteAllText(fileName, JsonConvert.SerializeObject(this,Formatting.Indented));
        }

        public void LoadFile()
        {
            JsonConvert.PopulateObject(File.ReadAllText(fileName), this);
        }
    }
}
