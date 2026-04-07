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
using ConfigFileManager.ViewModel;
using Microsoft.Windows.Controls;

namespace ConfigFileManager.View
{
    /// <summary>
    /// Interaction logic for ConfigTemplate.xaml
    /// </summary>
    public partial class ConfigTemplate : UserControl
    {
        public ConfigTemplate()
        {
            InitializeComponent();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            AppData.OpenProject.ConfigTemplates.Add(new ConfigFileManager.Data.Config.ConfigTemplate());
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            AppData.OpenProject.ConfigTemplates.Remove((ConfigFileManager.Data.Config.ConfigTemplate)TheGrid.Items[TheGrid.SelectedIndex]);
        }

        private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {
            ((ConfigTemplateModel)this.DataContext).ImportTemplate();
        }

        private void ButtonModify_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(((sender as Button).DataContext as ConfigFileManager.Data.Config.ConfigTemplate).TemplateName);
            //((ConfigTemplateModel)DataContext).ShowModify((DataGrid)sender, TheStack);
            //TheStack.Children.Add(new JsonTree(((sender as Button).DataContext as ConfigFileManager.Data.Config.ConfigTemplate)));

            ((ConfigTemplateModel)this.DataContext).EditTemplate(((sender as Button).DataContext as ConfigFileManager.Data.Config.ConfigTemplate));

        }

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            ((ConfigTemplateModel)this.DataContext).ExportToFile(((sender as Button).DataContext as ConfigFileManager.Data.Config.ConfigTemplate));
        }
    }
}
