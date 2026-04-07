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
using ConfigFileManager.Data.Processor;
using Microsoft.Windows.Controls;

namespace ConfigFileManager.View
{
    /// <summary>
    /// Interaction logic for Processor.xaml
    /// </summary>
    public partial class Processor : UserControl
    {
        public Processor()
        {
            InitializeComponent();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            AppData.OpenProject.Processors.Add(new ProcessorData());
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            AppData.OpenProject.Processors.Remove((ProcessorData)TheGrid.Items[TheGrid.SelectedIndex]);
        }

        private void ButtonDiscover_Click(object sender, RoutedEventArgs e)
        {
            TheStack.Children.Add(new ImportDiscovery());
        }
    }
}
