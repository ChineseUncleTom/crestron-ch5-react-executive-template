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
using Newtonsoft.Json.Linq;

namespace ConfigFileManager.View
{
    /// <summary>
    /// Interaction logic for JsonTree.xaml
    /// </summary>
    public partial class JsonTree : UserControl
    {
        public ConfigFileManager.Data.Config.ConfigTemplate _template;

        public JsonTree(ConfigFileManager.Data.Config.ConfigTemplate t)
        {
            _template = t;
            InitializeComponent();

            BuildTree();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            ((StackPanel)this.Parent).Children.Remove(this);
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BuildTree()
        {
            theStack.Children.Clear();

            foreach (var x in _template.Properties)
            {
                var newExpander = BuildExpander(x.Key, 0);
                theStack.Children.Add(newExpander.Key);
                AddProperty(newExpander.Value, x.Value, 1);
            }
        }

        private KeyValuePair<StackPanel, StackPanel> BuildExpander(string Title, int level)
        {
            StackPanel basePanel = new StackPanel();
            basePanel.Orientation = Orientation.Horizontal;

            if (level > 0)
            {
                Separator newSeperator = new Separator();
                newSeperator.Background = Brushes.Transparent;
                newSeperator.Width = 25 * level;

                basePanel.Children.Add(newSeperator);
            }
            
            Expander newExpander = new Expander();

            StackPanel header = new StackPanel();
            header.Orientation = Orientation.Horizontal;

            header.Children.Add(new TextBox { Text = Title } );
            header.Children.Add(new Button() { Content = "X" });
            header.Children.Add(new Button() { Content = "+" });

            newExpander.Header = header;
            newExpander.Content = new StackPanel();

            basePanel.Children.Add(newExpander);

            return new KeyValuePair<StackPanel, StackPanel>(basePanel, (StackPanel)newExpander.Content);
        }

        private void AddProperty(StackPanel panel, JToken data, int level)
        {
            if (data.Type == JTokenType.Object)
            {
                foreach (var pair in data as JObject)
                {
                    var newExpander = BuildExpander(pair.Key, level);
                    panel.Children.Add(newExpander.Key);
                    AddProperty(newExpander.Value, pair.Value, level + 1);
                }
            }
            else if (data.Type == JTokenType.Array)
            {
                Expander newExpander = new Expander();
                newExpander.Content = new StackPanel();

                foreach (var child in data.Children())
                {
                    AddProperty(((StackPanel)newExpander.Content), child, level + 1);
                }

                panel.Children.Add(newExpander);          
            }
            else
            {
                StackPanel basePanel = new StackPanel();
                basePanel.Orientation = Orientation.Horizontal;

                Separator newSeperator = new Separator();
                newSeperator.Background = Brushes.Transparent;
                newSeperator.Width = 25;

                basePanel.Children.Add(newSeperator);

                basePanel.Children.Add(new TextBox { Text = data.ToString() });

                panel.Children.Add(basePanel);
            }
            
        }
    }
}
