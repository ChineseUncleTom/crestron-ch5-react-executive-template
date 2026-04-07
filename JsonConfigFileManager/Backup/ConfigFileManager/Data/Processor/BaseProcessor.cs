using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel;

namespace ConfigFileManager.Data.Processor
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseProcessor : INotifyPropertyChanged
    {
        public BaseProcessor()
        {
        }

        private string _HostName;
        private int _Port;
        private string _UserName;
        private string _Password;
        private string _ConfigPath;
        private bool _RebootAfter;

        [JsonProperty]
        public string HostName
        {
            get
            {
                return _HostName;
            }
            set
            {
                _HostName = value;
                RaisePropertyChanged("HostName");
            }
        }
        [JsonProperty]
        public int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                _Port = value;
                RaisePropertyChanged("Port");
            }
        }
        [JsonProperty]
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
                RaisePropertyChanged("UserName");
            }
        }
        [JsonProperty]
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
                RaisePropertyChanged("Password");
            }
        }
        [JsonProperty]
        public string ConfigPath
        {
            get
            {
                return _ConfigPath;
            }
            set
            {
                _ConfigPath = value;
                RaisePropertyChanged("ConfigPath");
            }
        }
        [JsonProperty]
        public bool RebootAfter
        {
            get
            {
                return _RebootAfter;
            }
            set
            {
                _RebootAfter = value;
                RaisePropertyChanged("RebootAfter");
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
