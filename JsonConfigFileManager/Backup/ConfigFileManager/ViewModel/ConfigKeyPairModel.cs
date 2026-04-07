using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConfigFileManager.Data.Config;
using System.Collections.ObjectModel;

namespace ConfigFileManager.ViewModel
{
    class ConfigKeyPairModel
    {
        public ConfigTemplate CurrentConfig
        {
            get;
            set;
        }

        public ConfigKeyPairModel()
        {
        }
    }
}
