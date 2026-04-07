using System;
using Newtonsoft.Json;
using System.ComponentModel;

namespace ConfigFileManager.Data.Processor
{
	/// Processor template.
    [JsonObject(MemberSerialization.OptIn)]
    public class ProcessorTemplate : BaseProcessor
	{
        public ProcessorTemplate()
        {
            Guid = Guid.NewGuid();
        }
        
        private string _TemplateName;

        [JsonProperty]
        public string TemplateName
        {
            get
            {
                return _TemplateName;
            }
            set
            {
                _TemplateName = value;
                RaisePropertyChanged("TemplateName");
            }
        }

        [JsonProperty]
        public Guid Guid { get; set; }
	}
}