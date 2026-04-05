using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConstrolSystemTemplate.SystemInfo;

namespace ConstrolSystemTemplate
{
    internal class Video
    {
        private readonly BasicTriListWithSmartObject _panel;
        private readonly RoomController _roomController;
        private readonly SystemInfo _systemInfo;

        /// <summary>Constructor for Video class.</summary>
        /// <param name="panel">The panel device.</param>
        /// <param name="roomController">The room controller instance.</param>
        /// <param name="systemInfo">The system info instance.</param>
        public Video(BasicTriListWithSmartObject panel, RoomController roomController, SystemInfo systemInfo)
        {
            _panel = panel ?? throw new ArgumentNullException(nameof(panel));
            _roomController = roomController ?? throw new ArgumentNullException(nameof(roomController));
            _systemInfo = systemInfo ?? throw new ArgumentNullException(nameof(systemInfo));
        }

        public bool VideoRouting(ushort lastSelectedSource, ushort lastSelectedDestination)
        {
            if (lastSelectedSource != 0 && lastSelectedDestination != 0)
            {
                // Check if config is loaded and has video sources
                if (_systemInfo.Config == null || _systemInfo.Config.VideoSources == null)
                {
                    CrestronConsole.PrintLine("[Video] Cannot route - Config not loaded");
                    return false;
                }

                // Check if source index is valid (list is 0-based, but IDs are 1-based)
                if (lastSelectedSource - 1 < 0 || lastSelectedSource - 1 >= _systemInfo.Config.VideoSources.Count)
                {
                    CrestronConsole.PrintLine("[Video] Cannot route - Invalid source index: {0}", lastSelectedSource);
                    return false;
                }

                string source = _systemInfo.Config.VideoSources[lastSelectedSource - 1].Name;

                // Calculate the correct serial join for the destination (joins 25-28)
                uint serialJoin = JoinMap.VIDEO_DEST_ROUTED_SRC_NAME_1 + lastSelectedDestination - 1;
                _panel.StringInput[serialJoin].StringValue = source;

                CrestronConsole.PrintLine("[Video] Routed {0} (Source {1}) to Destination {2}", 
                    source, lastSelectedSource, lastSelectedDestination);
                return true;
            }
            else
            {
                CrestronConsole.PrintLine("[Video] Cannot route - Source: {0}, Destination: {1}", 
                    lastSelectedSource, lastSelectedDestination);
                return false;
            }
        }

        /// <summary>Routes video using the last selected source and destination from RoomController.</summary>
        public void VideoRouting()
        {
            VideoRouting(_roomController.LastSelectedSourceId, _roomController.LastSelectedDestinationId);
        }
    }
}
