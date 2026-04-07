using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ConfigFileManager.Discover;
using System.Net;
using System.Threading;
using ConfigFileManager.Data.Processor;
using Microsoft.Windows.Controls;

namespace ConfigFileManager.ViewModel
{
    class ImportDiscoveryModel
    {
        System.Windows.Threading.Dispatcher _dispatcher;
        DeviceDescover DiscoverEngine;

        public static ObservableCollection<ProcessorTemplate> ProcessorTemplates
        {
            get;
            set;
        }

        public ImportDiscoveryModel()
        {
            ImportDiscoveryModel.ProcessorTemplates = AppData.OpenProject.ProcessorTemplates;

            Devices = new ObservableCollection<DiscoveredDevice>();
            _dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

            DiscoverEngine = new DeviceDescover(null);

            DiscoverEngine.OnDeviceDiscovered += new DeviceDescover.EventDeviceDiscovered(DiscoverEngine_OnDeviceDiscovered);
        }

        void DiscoverEngine_OnDeviceDiscovered(object o, DeviceDiscoveredArgs e)
        {
            _dispatcher.Invoke(new Action(() => { Devices.Add(e.Device); }));
        }

        public ObservableCollection<DiscoveredDevice> Devices
        {
            get;
            set;
        }

        public void DiscoverStart()
        {
            DiscoverEngine.StartListener();

            AppData.DiscoverThreadControl = true;

            ThreadPool.QueueUserWorkItem((o) =>
            {
                while (AppData.DiscoverThreadControl)
                {
                    DiscoverEngine.Discover();
                    Thread.Sleep(5000);
                }
            });
        }

        public void DiscoverStop()
        {
            DiscoverEngine.StopListener();
            AppData.DiscoverThreadControl = false;
        }

        public void AddSelected(int index)
        {
            if (Devices[index].ProcessorTemplate != null)
            {
                ProcessorTemplate temp = ProcessorTemplates.First(p => p.Guid == Devices[index].ProcessorTemplate);

                if (temp != null)
                {
                    AppData.OpenProject.Processors.Add(new ProcessorData()
                        {
                            HostName = Devices[index].HostName,
                            Port = temp.Port,
                            UserName = temp.UserName,
                            Password = temp.Password,
                            ConfigPath = temp.ConfigPath,
                            RebootAfter = temp.RebootAfter
                        });

                    return;
                }
            }

            AppData.OpenProject.Processors.Add(new ProcessorData()
            {
                HostName = Devices[index].HostName
            });
        }
    }
}
