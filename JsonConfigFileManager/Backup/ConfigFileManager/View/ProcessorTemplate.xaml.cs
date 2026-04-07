using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConfigFileManager.View
{
    /// <summary>
    /// Interaction logic for ProcessorTemplate.xaml
    /// </summary>
    public partial class ProcessorTemplate : UserControl
    {
        public ProcessorTemplate()
        {
            InitializeComponent();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            AppData.OpenProject.ProcessorTemplates.Add(new ConfigFileManager.Data.Processor.ProcessorTemplate());
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            AppData.OpenProject.ProcessorTemplates.Remove((ConfigFileManager.Data.Processor.ProcessorTemplate)TheGrid.Items[TheGrid.SelectedIndex]);
        }
    }
}
