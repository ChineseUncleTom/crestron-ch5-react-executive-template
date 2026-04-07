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

namespace ConfigFileManager.View
{
    /// <summary>
    /// Interaction logic for ConfigKeyPair.xaml
    /// </summary>
    public partial class ConfigKeyPair : UserControl
    {
        public ConfigKeyPair(ConfigFileManager.Data.Config.ConfigTemplate t)
        {
            InitializeComponent();
            ((ConfigKeyPairModel)this.DataContext).CurrentConfig = t;
        }

        private void ButtonAddSingle_Click(object sender, RoutedEventArgs e)
        {
            ((ConfigKeyPairModel)this.DataContext).CurrentConfig.Properties.Add(new ConfigFileManager.Data.Properties.ConfigProperty("test","test"));
        }
        private void ButtonAddMultiple_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            ((StackPanel)this.Parent).Children.Remove(this);
        }
    }
}
