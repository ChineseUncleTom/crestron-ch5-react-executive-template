using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ConfigFileManager.Data.Processor;
using ConfigFileManager.Data.Config;

namespace ConfigFileManager.ViewModel
{
    public class ProcessorModel
    {
        public ObservableCollection<ProcessorData> CurrentProcessors
        {
            get;
            set;
        }

        public static ObservableCollection<ConfigTemplate> ConfigTemplates
        {
            get;
            set;
        }

        public ProcessorModel()
        {
            CurrentProcessors = AppData.OpenProject.Processors;
            ProcessorModel.ConfigTemplates = AppData.OpenProject.ConfigTemplates;
        }
    }
}
