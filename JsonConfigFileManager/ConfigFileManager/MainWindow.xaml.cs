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
using ConfigFileManager.Discover;
using ConfigFileManager.View;
using ConfigFileManager.Functions;
using Microsoft.Win32;
using ConfigFileManager.Data;

namespace ConfigFileManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Project OpenProject;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IconHelper.RemoveIcon(this);
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            if (openFileDialog1.ShowDialog() == true)
            {
                OpenTheProject(openFileDialog1.FileName);
            }
        }

        private void OpenTheProject(string p)
        {
            if (OpenProject != null)
            {
                var ret = MessageBox.Show("You currently have a project open. Save current changes?", "Warning!", MessageBoxButton.YesNoCancel);

                if (ret == MessageBoxResult.Yes)
                {
                    SaveTheProject(false);
                    ret = MessageBoxResult.No;
                }
                if (ret == MessageBoxResult.No)
                {
                    MainContent.Children.Remove(OpenProject);
                }
                if (ret == MessageBoxResult.Cancel)
                    return;
            }

            AppData.OpenProject = new ConfigProject();

            if (!String.IsNullOrEmpty(p))
            {
                AppData.OpenProject.FileName = p;

                try
                {
                    AppData.OpenProject.LoadFile();
                }
                catch
                {
                    MessageBox.Show("This seems to be an invalid project.");
                    AppData.OpenProject.FileName = null;
                }
            }

            OpenProject = new Project();

            MainContent.Children.Add(OpenProject);
        }

        private void SaveTheProject(bool NewName)
        {
            if (NewName || String.IsNullOrEmpty(AppData.OpenProject.FileName))
            {
                SaveFileDialog openFileDialog1 = new SaveFileDialog();

                openFileDialog1.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 1;

                if (openFileDialog1.ShowDialog() == true)
                {
                    AppData.OpenProject.FileName = openFileDialog1.FileName;
                    AppData.OpenProject.SaveFile();
                }
            }
            else
                AppData.OpenProject.SaveFile();
        }

        private void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            OpenTheProject(null);
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveTheProject(false);
        }

        private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveTheProject(true);
        }
    }
}
