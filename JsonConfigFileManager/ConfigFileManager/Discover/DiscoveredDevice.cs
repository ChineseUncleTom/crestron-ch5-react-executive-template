using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using ConfigFileManager.ViewModel;

namespace ConfigFileManager.Discover
{
    public class DiscoveredDevice : BaseViewModel
    {
        private readonly string responseParse = "(?<hostname>[\\w\\d- ]*\\b)(?:.*?)(?<description>[\\w\\d].*)";

        private string _HostName;
        private string _Description;
        private IPAddress _Address;

        public string HostName 
        {
            get
            {
                return _HostName;
            }
            set
            {
                _HostName = value;
                OnPropertyChanged("HostName");
            }
        }
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
                OnPropertyChanged("Description");
            }
        }
        public string Address
        {
            get
            {
                return _Address.ToString();
            }
            set
            {
                _Address = IPAddress.Parse(value);
                OnPropertyChanged("Address");
            }
        }

        private Guid? _ProcessorTemplate;

        public Guid? ProcessorTemplate
        {
            get
            {
                return _ProcessorTemplate;
            }
            set
            {
                _ProcessorTemplate = value;
                OnPropertyChanged("ProcessorTemplate");
            }
        }

        public DiscoveredDevice()
        {
        }

        public DiscoveredDevice(string s)
        {
            ParseResponse(s);
        }

        public DiscoveredDevice(string s, IPAddress ip)
        {
            ParseResponse(s);
            _Address = ip;
        }

        /// <summary>
        /// Parse the response from the UDP Query. This might probably should be in Device Descovery, but meh.
        /// </summary>
        /// <param name="s">Datapacket from UDP server.</param>
        private void ParseResponse(string s)
        {
            string cleaned = s.Trim((char)0, ' ', (char)10, (char)13);

            Regex r = new Regex(responseParse, RegexOptions.IgnoreCase);
            Match m = r.Match(cleaned);

            if (m.Success)
            {
                HostName = m.Groups["hostname"].Value;
                Description = m.Groups["description"].Value;
            }
        }
    }
}
