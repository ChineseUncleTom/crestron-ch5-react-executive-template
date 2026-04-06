import React from 'react';
import { Circle, Monitor, Power, Tv2 } from 'lucide-react';
import {
  Joins,
  useDigitalJoin,
  useSerialJoin,
  useSendDigitalPulse,
} from '../hooks/useCrestron';

// ── Source data ───────────────────────────────────────────────────────────────

const SOURCES = [
  {
    selectJoin:          Joins.VIDEO_SRC_SELECT_1,
    activeFbJoin:        Joins.VIDEO_SRC_ACTIVE_1,
    availableFbJoin:     Joins.VIDEO_SRC_AVAIL_1,
    nameSerialJoin:      Joins.VIDEO_SRC_NAME_1,
    farEndVisibleFbJoin: Joins.VIDEO_SRC_FAR_END_VISIBLE_1,
    defaultName:         'Room PC',
  },
  {
    selectJoin:          Joins.VIDEO_SRC_SELECT_2,
    activeFbJoin:        Joins.VIDEO_SRC_ACTIVE_2,
    availableFbJoin:     Joins.VIDEO_SRC_AVAIL_2,
    nameSerialJoin:      Joins.VIDEO_SRC_NAME_2,
    farEndVisibleFbJoin: Joins.VIDEO_SRC_FAR_END_VISIBLE_2,
    defaultName:         'Laptop HDMI',
  },
  {
    selectJoin:          Joins.VIDEO_SRC_SELECT_3,
    activeFbJoin:        Joins.VIDEO_SRC_ACTIVE_3,
    availableFbJoin:     Joins.VIDEO_SRC_AVAIL_3,
    nameSerialJoin:      Joins.VIDEO_SRC_NAME_3,
    farEndVisibleFbJoin: Joins.VIDEO_SRC_FAR_END_VISIBLE_3,
    defaultName:         'Wireless Cast',
  },
  {
    selectJoin:          Joins.VIDEO_SRC_SELECT_4,
    activeFbJoin:        Joins.VIDEO_SRC_ACTIVE_4,
    availableFbJoin:     Joins.VIDEO_SRC_AVAIL_4,
    nameSerialJoin:      Joins.VIDEO_SRC_NAME_4,
    farEndVisibleFbJoin: Joins.VIDEO_SRC_FAR_END_VISIBLE_4,
    defaultName:         'Source 4',
  },
  {
    selectJoin:          Joins.VIDEO_SRC_SELECT_5,
    activeFbJoin:        Joins.VIDEO_SRC_ACTIVE_5,
    availableFbJoin:     Joins.VIDEO_SRC_AVAIL_5,
    nameSerialJoin:      Joins.VIDEO_SRC_NAME_5,
    farEndVisibleFbJoin: Joins.VIDEO_SRC_FAR_END_VISIBLE_5,
    defaultName:         'Source 5',
  },
] as const;

// ── Destination data ──────────────────────────────────────────────────────────

const DESTINATIONS = [
  {
    selectJoin:         Joins.VIDEO_DEST_SELECT_1,
    activeFbJoin:       Joins.VIDEO_DEST_ACTIVE_1,
    powerBtnJoin:       Joins.VIDEO_DEST_POWER_BTN_1,
    powerFbJoin:        Joins.VIDEO_DEST_POWER_FB_1,
    videoBtnJoin:       Joins.VIDEO_DEST_VIDEO_BTN_1,
    videoFbJoin:        Joins.VIDEO_DEST_VIDEO_FB_1,
    nameSerialJoin:     Joins.VIDEO_DEST_NAME_1,
    routedSrcSerialJoin: Joins.VIDEO_DEST_ROUTED_SRC_NAME_1,
    defaultName:        'Display Left',
    defaultRoutedSrc:   '—',
  },
  {
    selectJoin:         Joins.VIDEO_DEST_SELECT_2,
    activeFbJoin:       Joins.VIDEO_DEST_ACTIVE_2,
    powerBtnJoin:       Joins.VIDEO_DEST_POWER_BTN_2,
    powerFbJoin:        Joins.VIDEO_DEST_POWER_FB_2,
    videoBtnJoin:       Joins.VIDEO_DEST_VIDEO_BTN_2,
    videoFbJoin:        Joins.VIDEO_DEST_VIDEO_FB_2,
    nameSerialJoin:     Joins.VIDEO_DEST_NAME_2,
    routedSrcSerialJoin: Joins.VIDEO_DEST_ROUTED_SRC_NAME_2,
    defaultName:        'Display Center',
    defaultRoutedSrc:   '—',
  },
  {
    selectJoin:         Joins.VIDEO_DEST_SELECT_3,
    activeFbJoin:       Joins.VIDEO_DEST_ACTIVE_3,
    powerBtnJoin:       Joins.VIDEO_DEST_POWER_BTN_3,
    powerFbJoin:        Joins.VIDEO_DEST_POWER_FB_3,
    videoBtnJoin:       Joins.VIDEO_DEST_VIDEO_BTN_3,
    videoFbJoin:        Joins.VIDEO_DEST_VIDEO_FB_3,
    nameSerialJoin:     Joins.VIDEO_DEST_NAME_3,
    routedSrcSerialJoin: Joins.VIDEO_DEST_ROUTED_SRC_NAME_3,
    defaultName:        'Display Right',
    defaultRoutedSrc:   '—',
  },
  {
    selectJoin:         Joins.VIDEO_DEST_SELECT_4,
    activeFbJoin:       Joins.VIDEO_DEST_ACTIVE_4,
    powerBtnJoin:       Joins.VIDEO_DEST_POWER_BTN_4,
    powerFbJoin:        Joins.VIDEO_DEST_POWER_FB_4,
    videoBtnJoin:       Joins.VIDEO_DEST_VIDEO_BTN_4,
    videoFbJoin:        Joins.VIDEO_DEST_VIDEO_FB_4,
    nameSerialJoin:     Joins.VIDEO_DEST_NAME_4,
    routedSrcSerialJoin: Joins.VIDEO_DEST_ROUTED_SRC_NAME_4,
    defaultName:        'Display Rear',
    defaultRoutedSrc:   '—',
  },
] as const;

// ── SourceTile ────────────────────────────────────────────────────────────────

interface SourceTileProps {
  selectJoin: number;
  activeFbJoin: number;
  availableFbJoin: number;
  nameSerialJoin: number;
  defaultName: string;
}

const SourceTile: React.FC<SourceTileProps> = ({
  selectJoin,
  activeFbJoin,
  availableFbJoin,
  nameSerialJoin,
  defaultName,
}) => {
  const isActive    = useDigitalJoin(activeFbJoin);
  const isAvailable = useDigitalJoin(availableFbJoin);
  const name        = useSerialJoin(nameSerialJoin);
  const sendSelect  = useSendDigitalPulse(selectJoin);

  return (
    <button
      className={`ev-source-tile${isActive ? ' ev-source-tile--active' : ''}`}
      onClick={sendSelect}
      aria-pressed={isActive}
    >
      <span
        className={`ev-source-tile__dot${isAvailable ? ' ev-source-tile__dot--on' : ''}`}
        aria-hidden="true"
      >
        <Circle size={8} />
      </span>
      <span className="ev-source-tile__name">{name || defaultName}</span>
    </button>
  );
};

// ── DestDisplay ───────────────────────────────────────────────────────────────

interface DestDisplayProps {
  selectJoin: number;
  activeFbJoin: number;
  powerBtnJoin: number;
  powerFbJoin: number;
  videoBtnJoin: number;
  videoFbJoin: number;
  nameSerialJoin: number;
  routedSrcSerialJoin: number;
  defaultName: string;
  defaultRoutedSrc: string;
}

const DestDisplay: React.FC<DestDisplayProps> = ({
  selectJoin,
  activeFbJoin,
  powerBtnJoin,
  powerFbJoin,
  videoBtnJoin,
  videoFbJoin,
  nameSerialJoin,
  routedSrcSerialJoin,
  defaultName,
  defaultRoutedSrc,
}) => {
  const isActive      = useDigitalJoin(activeFbJoin);
  const powerOn       = useDigitalJoin(powerFbJoin);
  const videoOn       = useDigitalJoin(videoFbJoin);
  const name          = useSerialJoin(nameSerialJoin);
  const routedSrcName = useSerialJoin(routedSrcSerialJoin);
  const sendSelect    = useSendDigitalPulse(selectJoin);
  const sendPower     = useSendDigitalPulse(powerBtnJoin);
  const sendVideo     = useSendDigitalPulse(videoBtnJoin);

  return (
    <div className={`ev-dest-card${isActive ? ' ev-dest-card--active' : ''}`}>
      <button
        className="ev-dest-card__select"
        onClick={sendSelect}
        aria-pressed={isActive}
        aria-label={`Select ${name || defaultName}`}
      >
        <Monitor size={24} aria-hidden="true" className="ev-dest-card__icon" />
        <span className="ev-dest-card__name">{name || defaultName}</span>
        <span className="ev-dest-card__src">{routedSrcName || defaultRoutedSrc}</span>
      </button>
      <div className="ev-dest-card__controls">
        <button
          className={`ev-dest-card__ctrl-btn${powerOn ? ' ev-dest-card__ctrl-btn--on' : ' ev-dest-card__ctrl-btn--off'}`}
          onClick={sendPower}
          aria-pressed={powerOn}
        >
          <Power size={13} aria-hidden="true" />
          {powerOn ? 'Power ON' : 'Power OFF'}
        </button>
        <button
          className={`ev-dest-card__ctrl-btn${videoOn ? ' ev-dest-card__ctrl-btn--video-on' : ' ev-dest-card__ctrl-btn--off'}`}
          onClick={sendVideo}
          aria-pressed={videoOn}
        >
          <Tv2 size={13} aria-hidden="true" />
          {videoOn ? 'Video ON' : 'Video OFF'}
        </button>
      </div>
    </div>
  );
};

// ── VideoView ─────────────────────────────────────────────────────────────────

/**
 * VideoView
 *
 * Executive video routing page.
 *
 * Design approach – action-oriented ("What do you want to show? → Where?"):
 * - Top:    "What do you want to show to far end?" – non-Room-PC sources
 *           (controlled by VIDEO_SRC_FAR_END_VISIBLE signals from processor),
 *           routed to the far-end output port of the video switcher.
 * - Middle: "What do you want to show locally?" – all sources routed to the
 *           connected displays (HDMI OUT 1–4).
 * - Bottom: Destination display cards with independent power / video controls,
 *           followed by a BYOD connection button.
 */
const VideoView: React.FC = () => {
  const sendByodSel = useSendDigitalPulse(Joins.BYOD_SELECT_BTN);

  // Per-source far-end visibility feedback (from processor via VIDEO_SRC_FAR_END_VISIBLE_1-5)
  const farEndVis1 = useDigitalJoin(Joins.VIDEO_SRC_FAR_END_VISIBLE_1);
  const farEndVis2 = useDigitalJoin(Joins.VIDEO_SRC_FAR_END_VISIBLE_2);
  const farEndVis3 = useDigitalJoin(Joins.VIDEO_SRC_FAR_END_VISIBLE_3);
  const farEndVis4 = useDigitalJoin(Joins.VIDEO_SRC_FAR_END_VISIBLE_4);
  const farEndVis5 = useDigitalJoin(Joins.VIDEO_SRC_FAR_END_VISIBLE_5);
  const farEndVisibility = [farEndVis1, farEndVis2, farEndVis3, farEndVis4, farEndVis5];

  // Non-Room-PC sources shown in the far-end section (processor controls visibility)
  const farEndSources = SOURCES.filter((_, i) => farEndVisibility[i]);

  return (
    <div className="ev-page">

      {/* ── Far-end sources ───────────────────────────────────────────────── */}
      <section className="ev-section" aria-label="Far-end sources">
        <h2 className="ev-section__title">What do you want to show to far end?</h2>
        <div className="ev-sources">
          {farEndSources.map((src) => (
            <SourceTile key={src.selectJoin} {...src} />
          ))}
        </div>
      </section>

      {/* ── Local sources ────────────────────────────────────────────────── */}
      <section className="ev-section" aria-label="Local sources">
        <h2 className="ev-section__title">What do you want to show locally?</h2>
        <div className="ev-sources">
          {SOURCES.map((src) => (
            <SourceTile key={src.selectJoin} {...src} />
          ))}
        </div>
      </section>

      {/* ── Destinations ─────────────────────────────────────────────────── */}
      <section className="ev-section" aria-label="Displays">
        <h2 className="ev-section__title">Where do you want to show it?</h2>
        <div className="ev-destinations">
          {DESTINATIONS.map((dest) => (
            <DestDisplay key={dest.selectJoin} {...dest} />
          ))}
        </div>
      </section>

      {/* ── BYOD connection ───────────────────────────────────────────────── */}
      <section className="ev-section" aria-label="BYOD Connection">
        <h2 className="ev-section__title">BYOD Connection</h2>
        <button className="ev-byod-btn" onClick={sendByodSel}>
          Connect BYOD Device
        </button>
      </section>

    </div>
  );
};

export default VideoView;
