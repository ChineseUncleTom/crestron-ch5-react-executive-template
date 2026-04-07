using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ConfigFileManager.Data.Config;
using Microsoft.Windows.Controls;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows;
using ConfigFileManager.Functions;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace ConfigFileManager.ViewModel
{
    class ConfigTemplateModel
    {
        public ObservableCollection<ConfigTemplate> CurrentConfigTemplate
        {
            get;
            set;
        }

        protected FileSystemWatcher theWatcher;
        protected string editingFile;
        protected ConfigTemplate editingTemplate;

        public ConfigTemplateModel()
        {
            CurrentConfigTemplate = AppData.OpenProject.ConfigTemplates;
        }

        public void ShowModify(DataGrid dataGrid, StackPanel TheStack)
        {

        }

        public void ImportTemplate()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            if (openFileDialog1.ShowDialog() == true)
            {
                string fileName = openFileDialog1.FileName;

                try
                {
                    ConfigTemplate newTemplate = new ConfigTemplate();
                    newTemplate.TemplateName = Path.GetFileNameWithoutExtension(fileName);
                    newTemplate.Properties = JObject.Parse(File.ReadAllText(fileName));

                    AppData.OpenProject.ConfigTemplates.Add(newTemplate);

                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message, "File Import");
                }

            }
        }

        public void ExportToFile(ConfigTemplate template)
        {
            SaveFileDialog openFileDialog1 = new SaveFileDialog();

            openFileDialog1.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            if (openFileDialog1.ShowDialog() == true)
            {
                string fileName = openFileDialog1.FileName;

                File.WriteAllText(fileName, JsonConvert.SerializeObject(template.Properties, Formatting.Indented));
            }
        }

        public void EditTemplate(ConfigTemplate template)
        {
            if (!String.IsNullOrEmpty(editingFile))
            {
                MessageBoxResult res = MessageBox.Show("There is currently a file being edited. Editing a new file will overwrite the existing one. Continue?", "", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.No)
                {
                    return;
                }
            }

            editingTemplate = template;
            editingFile = Path.Combine(Path.GetTempPath(), String.Format("{0}.json", template.TemplateName));

            File.WriteAllText(editingFile, JsonConvert.SerializeObject(template.Properties, Formatting.Indented));

            if (editingFile != null)
            {
                Process process = new Process();
                process.StartInfo.FileName = editingFile;

                theWatcher = new FileSystemWatcher();
                theWatcher.Path = Path.GetDirectoryName(editingFile);

                theWatcher.Changed += new FileSystemEventHandler(EditTemplateEditorExited);

                theWatcher.EnableRaisingEvents = true;

                process.Start();
            }
        }

        void EditTemplateEditorExited(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath == editingFile)
            {
                MessageBoxResult res = MessageBox.Show("The file being edited has changed. Import back in?", "", MessageBoxButton.YesNo);

                theWatcher.EnableRaisingEvents = false;
                theWatcher.Dispose();

                if (res == MessageBoxResult.Yes)
                {
                    //JObject newTemp = JObject.Parse(File.ReadAllText(editingFile));

                    //MessageBox.Show(JSONDiff.CompareObjects(editingTemplate.Properties, newTemp).ToString());

                    editingTemplate.Properties = JObject.Parse(File.ReadAllText(editingFile));
                }

                editingFile = null;
                editingTemplate = null;
            }
        }
    }
}
