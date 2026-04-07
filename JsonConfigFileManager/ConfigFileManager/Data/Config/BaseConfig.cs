using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConfigFileManager.Data.Properties;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace ConfigFileManager.Data.Config
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseConfig : INotifyPropertyChanged
    {
        public BaseConfig()
        {
        }

        private JObject _Properties;

        [JsonProperty]
        public JObject Properties
        {
            get
            {
                if (_Properties == null)
                    _Properties = new JObject();

                return _Properties;
            }
            set
            {
                _Properties = value;
                RaisePropertyChanged("Properties");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
