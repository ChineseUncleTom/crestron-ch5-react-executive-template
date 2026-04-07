using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Collections.ObjectModel;

namespace ConfigFileManager.Discover
{
    public class DeviceDescover
    {
        private UdpClient Client;
        private List<DiscoveredDevice> DiscoveredDevices;

        private readonly byte[] responseCheck = { 0x15, 0x00, 0x00, 0x00 };

        public delegate void EventDeviceDiscovered(object o, DeviceDiscoveredArgs e);
        public event EventDeviceDiscovered OnDeviceDiscovered;

        public DeviceDescover(List<DiscoveredDevice> d)
        {
            DiscoveredDevices = d;
        }

        /// <summary>
        /// Start up the server to listen for responses to our query
        /// </summary>
        public void StartListener()
        {
            // New UDP client
            Client = new UdpClient(41794);

            // Try a begin Recieve
            try
            {
                Client.BeginReceive(new AsyncCallback(recv), null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void StopListener()
        {
            if(Client != null)
                Client.Close();
        }

        /// <summary>
        /// Callback for the async server recieve request
        /// </summary>
        /// <param name="res"></param>
        private void recv(IAsyncResult res)
        {
            // Recieve data from the client
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);

            try
            {
                byte[] received = Client.EndReceive(res, ref RemoteIpEndPoint);

                // Pull the first 4 bytes of the response
                byte[] header = received.Take(4).ToArray();

                // Make sure that the 4 bytes are what we are looking for.
                if (header.SequenceEqual(responseCheck))
                {
                    // Add the device to the list of what we were looking for.
                    addDiscoveredDevice(new DiscoveredDevice(Encoding.UTF8.GetString(received), RemoteIpEndPoint.Address));
                    // Let the end user know it responded
                    Console.WriteLine(String.Format("Found: {0} [{1}] {2}", DiscoveredDevices.Last().HostName, DiscoveredDevices.Last().Address, DiscoveredDevices.Last().Description));
                }

                // Start another listen.
                Client.BeginReceive(new AsyncCallback(recv), null);
            }
            catch
            {
            }
        }

        private void addDiscoveredDevice(DiscoveredDevice dev)
        {
            if (DiscoveredDevices == null)
                DiscoveredDevices = new List<DiscoveredDevice>();

            if (DiscoveredDevices.Where(p => p.HostName == dev.HostName).Count() == 0)
            {

                DiscoveredDevices.Add(dev);

                if (OnDeviceDiscovered != null)
                    OnDeviceDiscovered(this, new DeviceDiscoveredArgs(dev));
            }
        }

        /// <summary>
        /// Gets a list of active local IP addresses.
        /// </summary>
        /// <returns>List of IP Addresses</returns>
        private List<IPAddress> LocalIPAddress()
        {
            // Return nothing if none are avaliable
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            // Get the list of IPs based on our hostname
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            // Return only the ones that are actually us.
            return host
                .AddressList.ToList()
                .FindAll(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }

        public void Discover()
        {
            // Start the UDP Listener
            //StartListener();

            // Send the magic packet to the broadcast
            try
            {
                // Grab all the network interfaces
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    IPInterfaceProperties ip_properties = adapter.GetIPProperties();

                    // Skip adapters without multicast addresses
                    if (!adapter.GetIPProperties().MulticastAddresses.Any())
                        continue; 

                    // Skip if it doesn't support multicast
                    if (!adapter.SupportsMulticast)
                        continue;

                    // Skip if the interface doesn't have a link
                    if (OperationalStatus.Up != adapter.OperationalStatus)
                        continue;

                    // Get the IPv4 properties to make sure it is an IPv4 Address
                    IPv4InterfaceProperties p = adapter.GetIPProperties().GetIPv4Properties();
                    if (null == p)
                        continue;

                    foreach (UnicastIPAddressInformation UnicastAddress in ip_properties.UnicastAddresses)
                    {
                        // Make sure the IP address is a good addressable address
                        if (UnicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            // Break apart into chunks
                            var IPBytes = UnicastAddress.Address.GetAddressBytes();
                            var SubBytes = UnicastAddress.IPv4Mask.GetAddressBytes();
                            byte[] FinalBytes = new byte[4];

                            // Go though the netmask, check for 255, if all bits are masked, we are going to use the octet from the IP address
                            // otherwise we will use a broadcast address
                            for (int i = 0; i < 4; i++)
                                if (SubBytes[i] == 255)
                                    FinalBytes[i] = IPBytes[i];
                                else
                                    FinalBytes[i] = 255;

                            // Build the final Address
                            IPAddress broadcastAddress = new IPAddress(FinalBytes);

                            //Console.WriteLine("Sending to: {0} [{1}]", adapter.Description, broadcastAddress);

                            // Make the connection and send
                            UdpClient client = new UdpClient();
                            IPEndPoint ip = new IPEndPoint(broadcastAddress, 41794);
                            byte[] bytes = GetFixedBroadcast();

                            // Assign the client to the network interface we have been working on.
                            client.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, (int)IPAddress.HostToNetworkOrder(p.Index));

                            client.Send(bytes, bytes.Length, ip);

                            client.Close();
                        }
                    }
                }
            }
            catch (Exception send_exception)
            {
                Console.WriteLine(" Exception {0}", send_exception.Message);
            }
        }

        /// <summary>
        /// Build the magic packet for the Variable Broadcast type. Not all devices will respond to this query.
        /// </summary>
        /// <returns>The Magic Packet</returns>
        public byte[] GetVariableBroadcast()
        {
            string hostname = Dns.GetHostName();

            // Set up the sections
            byte[] headder = { 0x14, 0x00, 0x00, 0x00 };
            byte[] lenhost = BitConverter.GetBytes(Convert.ToInt16(hostname.Length + 4));
            byte[] cmd = { 0x00, 0x03, 0x00, 0x00 };
            byte[] host = Encoding.ASCII.GetBytes(hostname);

            // Add them to a list
            List<byte> message = new List<byte>();
            message.AddRange(headder);
            message.AddRange(lenhost);
            message.AddRange(cmd);
            message.AddRange(host);

            // Return the full string
            return message.ToArray();
        }

        /// <summary>
        /// Build the magic packet for the Fixed Broadcast type. All devices will respond to this query.
        /// </summary>
        /// <returns>The Magic Packet</returns>
        private byte[] GetFixedBroadcast()
        {
            string hostname = Dns.GetHostName();

            // Set up the sections
            byte[] headder = { 0x14, 0x00, 0x00, 0x00 };
            byte[] lenhost = { 0x01, 0x04 };
            byte[] cmd = { 0x00, 0x03, 0x00, 0x00 };
            byte[] host = Encoding.ASCII.GetBytes(hostname);

            // Add them to a list
            List<byte> message = new List<byte>();
            message.AddRange(headder);
            message.AddRange(lenhost);
            message.AddRange(cmd);
            message.AddRange(host);

            // Add padding to the end of the packet
            byte[] padding = new byte[(266 - message.Count)];
            message.AddRange(padding);

            // Return the full string
            return message.ToArray();
        }

    }
}
