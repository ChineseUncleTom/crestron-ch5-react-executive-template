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
using ConfigFileManager.VML;
using ConfigFileManager.ViewModel;

namespace ConfigFileManager.View
{
    /// <summary>
    /// Interaction logic for ImportDiscovery.xaml
    /// </summary>
    public partial class ImportDiscovery : UserControl
    {
        public ImportDiscovery()
        {
            InitializeComponent();
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            ((ImportDiscoveryModel)DataContext).DiscoverStart();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            ((ImportDiscoveryModel)DataContext).DiscoverStop();
        }

        private void ButtonAddSelected_Click(object sender, RoutedEventArgs e)
        {
            ((ImportDiscoveryModel)DataContext).AddSelected(TheGrid.SelectedIndex);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            ((ImportDiscoveryModel)DataContext).DiscoverStop();

            //MessageBox.Show(this.Parent.ToString());

            if (this.Parent is DockPanel)
            {
                //MessageBox.Show("This is a stackpanel");
                ((DockPanel)this.Parent).Children.Remove(this);
            }
        }
    }
}
