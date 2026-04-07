using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ConfigFileManager.Data.Properties
{
    public class ConfigPropertyArray : INotifyPropertyChanged
    {
        public ConfigPropertyArray()
        {
            this.IsArray = true;
            this.Value = new List<string>();
        }

        public ConfigPropertyArray(string key, List<string> value)
        {
            this.Key = key;
            this.Value = new List<string>(value);
            this.IsArray = true;
        }

        private string _Key;
        private bool _IsArray;
        private List<string> _Value;

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
        public List<string> Value
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
