using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel;

namespace ConfigFileManager.Data.Properties
{
    public class ConfigProperty : INotifyPropertyChanged
    {

        public ConfigProperty()
        {
            this.IsArray = false;
        }

        public ConfigProperty(string key, string value)
        {
            this.Key = key;
            this.Value = value;
            this.IsArray = false;
        }

        private string _Key;
        private bool _IsArray;
        private string _Value;

        public string Key
        {
            get
            {
                return _Key;
            }
            set
            {
                _Key = value;
                RaisePropertyChanged("Key");
            }
        }
        public bool IsArray
        {
            get
            {
                return _IsArray;
            }
            set
            {
                _IsArray = value;
                RaisePropertyChanged("IsArray");
            }
        }
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                RaisePropertyChanged("Value");
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
