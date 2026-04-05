using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using System;
using static ExecutiveControlSystem.SystemInfo;

namespace ExecutiveControlSystem
{
    /// <summary>
    /// Encapsulates video routing logic.
    /// Routes a selected source to a selected destination and updates the
    /// "currently routed source" serial join on the panel.
    /// </summary>
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
            _panel          = panel          ?? throw new ArgumentNullException(nameof(panel));
            _roomController = roomController ?? throw new ArgumentNullException(nameof(roomController));
            _systemInfo     = systemInfo     ?? throw new ArgumentNullException(nameof(systemInfo));
        }

        /// <summary>
        /// Routes the specified source to the specified destination and sends
        /// the routed source name to the corresponding serial join (25–28).
        /// Returns <c>true</c> if routing succeeded.
        /// </summary>
        public bool VideoRouting(ushort lastSelectedSource, ushort lastSelectedDestination)
        {
            if (lastSelectedSource == 0 || lastSelectedDestination == 0)
            {
                CrestronConsole.PrintLine("[Video] Cannot route – Source: {0}, Destination: {1}",
                    lastSelectedSource, lastSelectedDestination);
                return false;
            }

            if (_systemInfo.Config?.VideoSources == null)
            {
                CrestronConsole.PrintLine("[Video] Cannot route – config not loaded");
                return false;
            }

            int srcIndex = lastSelectedSource - 1;
            if (srcIndex < 0 || srcIndex >= _systemInfo.Config.VideoSources.Count)
            {
                CrestronConsole.PrintLine("[Video] Cannot route – invalid source index: {0}", lastSelectedSource);
                return false;
            }

            string sourceName = _systemInfo.Config.VideoSources[srcIndex].Name ?? string.Empty;

            // Update the "routed source name" serial join for the destination (joins 25–28)
            uint serialJoin = JoinMap.VIDEO_DEST_ROUTED_SRC_NAME_1 + lastSelectedDestination - 1;
            _panel.StringInput[serialJoin].StringValue = sourceName;

            CrestronConsole.PrintLine("[Video] Routed \"{0}\" (Source {1}) → Destination {2}",
                sourceName, lastSelectedSource, lastSelectedDestination);
            return true;
        }

        /// <summary>Routes video using the last selected source and destination from RoomController.</summary>
        public void VideoRouting()
        {
            VideoRouting(_roomController.LastSelectedSourceId, _roomController.LastSelectedDestinationId);
        }
    }
}
