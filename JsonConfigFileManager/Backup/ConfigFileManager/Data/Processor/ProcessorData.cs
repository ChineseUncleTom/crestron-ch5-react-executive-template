using System;
using Newtonsoft.Json;
using ConfigFileManager.Data.Config;
using System.ComponentModel;

namespace ConfigFileManager.Data.Processor
{
	/// Processor
    [JsonObject(MemberSerialization.OptIn)]
    public class ProcessorData : BaseProcessor
	{
        public ProcessorData()
        {
            Guid = Guid.NewGuid();
        }

        private Guid? _ConfigTemplate;
        private ConfigChanges _ConfigChanges;

        [JsonProperty]
        public Guid? ConfigTemplate
        {
            get
            {
                return _ConfigTemplate;
            }
            set
            {
                _ConfigTemplate = value;
                RaisePropertyChanged("ConfigTemplate");
            }
        }
        [JsonProperty]
        public ConfigChanges ConfigChanges
        {
            get
            {
                return _ConfigChanges;
            }
            set
            {
                _ConfigChanges = value;
                RaisePropertyChanged("ConfigChanges");
            }
        }

        [JsonProperty]
        public Guid Guid { get; set; }
	}
}