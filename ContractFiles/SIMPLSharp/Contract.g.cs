using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace ExecutiveContract
{
    /// <summary>
    /// Common Interface for Root Contracts.
    /// </summary>
    public interface IContract
    {
        object UserObject { get; set; }
        void AddDevice(BasicTriListWithSmartObject device);
        void RemoveDevice(BasicTriListWithSmartObject device);
    }

    /// <summary>
    /// C# + React Contract
    /// </summary>
    public class Contract : IContract, IDisposable
    {
        #region Components

        private ComponentMediator ComponentMediator { get; set; }

        public ExecutiveContract.IPowerButton PowerButton { get { return (ExecutiveContract.IPowerButton)InternalPowerButton; } }
        private ExecutiveContract.PowerButton InternalPowerButton { get; set; }

        #endregion

        #region Construction and Initialization

        public Contract()
            : this(new List<BasicTriListWithSmartObject>().ToArray())
        {
        }

        public Contract(BasicTriListWithSmartObject device)
            : this(new [] { device })
        {
        }

        public Contract(BasicTriListWithSmartObject[] devices)
        {
            if (devices == null)
                throw new ArgumentNullException("Devices is null");

            ComponentMediator = new ComponentMediator();

            InternalPowerButton = new ExecutiveContract.PowerButton(ComponentMediator, 1);

            for (int index = 0; index < devices.Length; index++)
            {
                AddDevice(devices[index]);
            }
        }

        #endregion

        #region Standard Contract Members

        public object UserObject { get; set; }

        public void AddDevice(BasicTriListWithSmartObject device)
        {
            InternalPowerButton.AddDevice(device);
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            InternalPowerButton.RemoveDevice(device);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            InternalPowerButton.Dispose();
            ComponentMediator.Dispose(); 
        }

        #endregion

    }
}
