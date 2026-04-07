using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConfigFileManager.Data.Config
{
    /// Config template.
    public class ConfigTemplate : BaseConfig
    {
        [JsonProperty]
        public string TemplateName { get; set; }
        [JsonProperty]
        public Guid Guid { get; set; }

        public ConfigTemplate()
            : base()
        {
            Guid = Guid.NewGuid();
        }
    }
}