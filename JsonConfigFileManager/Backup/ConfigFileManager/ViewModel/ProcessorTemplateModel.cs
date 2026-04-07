using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ConfigFileManager.Data.Processor;

namespace ConfigFileManager.ViewModel
{
    class ProcessorTemplateModel
    {
        public ObservableCollection<ProcessorTemplate> CurrentProcessorTemplate
        {
            get;
            set;
        }

        public ProcessorTemplateModel()
        {
            CurrentProcessorTemplate = AppData.OpenProject.ProcessorTemplates;
        }
    }
}
