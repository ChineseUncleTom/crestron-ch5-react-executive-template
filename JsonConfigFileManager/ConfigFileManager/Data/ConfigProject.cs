using System;
using Newtonsoft.Json;
using ConfigFileManager.Data.Config;
using ConfigFileManager.Data.Processor;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ConfigFileManager.Data
{
	/// So this will be the main data class for a project.

    [JsonObject(MemberSerialization.OptIn)]
	public class ConfigProject : JsonData, INotifyPropertyChanged
	{
		public ConfigProject()
		{
            // Initialize
            _ConfigTemplates = new ObservableCollection<ConfigTemplate>();
            _ProcessorTemplates = new ObservableCollection<ProcessorTemplate>();
            _Processors = new ObservableCollection<ProcessorData>();
		}

        private ObservableCollection<ConfigTemplate> _ConfigTemplates;
        private ObservableCollection<ProcessorTemplate> _ProcessorTemplates;
        public ObservableCollection<ProcessorData> _Processors;

        [JsonProperty]
        public ObservableCollection<ConfigTemplate> ConfigTemplates 
        {
            get
            {
                return _ConfigTemplates;
            }
            set
            {
                _ConfigTemplates = value;
                RaisePropertyChanged("ConfigTemplates");
            }
        }
        [JsonProperty]
        public ObservableCollection<ProcessorTemplate> ProcessorTemplates
        {
            get
            {
                return _ProcessorTemplates;
            }
            set
            {
                _ProcessorTemplates = value;
                RaisePropertyChanged("ProcessorTemplates");
            }
        }
        [JsonProperty]
        public ObservableCollection<ProcessorData> Processors
        {
            get
            {
                return _Processors;
            }
            set
            {
                _Processors = value;
                RaisePropertyChanged("Processors");
            }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}