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
    selectJoin:     Joins.VIDEO_SRC_SELECT_1,
    activeFbJoin:   Joins.VIDEO_SRC_ACTIVE_1,
    availableFbJoin: Joins.VIDEO_SRC_AVAIL_1,
    nameSerialJoin: Joins.VIDEO_SRC_NAME_1,
    defaultName:    'Room PC',
  },
  {
    selectJoin:     Joins.VIDEO_SRC_SELECT_2,
    activeFbJoin:   Joins.VIDEO_SRC_ACTIVE_2,
    availableFbJoin: Joins.VIDEO_SRC_AVAIL_2,
    nameSerialJoin: Joins.VIDEO_SRC_NAME_2,
    defaultName:    'Laptop HDMI',
  },
  {
    selectJoin:     Joins.VIDEO_SRC_SELECT_3,
    activeFbJoin:   Joins.VIDEO_SRC_ACTIVE_3,
    availableFbJoin: Joins.VIDEO_SRC_AVAIL_3,
    nameSerialJoin: Joins.VIDEO_SRC_NAME_3,
    defaultName:    'Wireless Cast',
  },
  {
    selectJoin:     Joins.VIDEO_SRC_SELECT_4,
    activeFbJoin:   Joins.VIDEO_SRC_ACTIVE_4,
    availableFbJoin: Joins.VIDEO_SRC_AVAIL_4,
    nameSerialJoin: Joins.VIDEO_SRC_NAME_4,
    defaultName:    'Source 4',
  },
  {
    selectJoin:     Joins.VIDEO_SRC_SELECT_5,
    activeFbJoin:   Joins.VIDEO_SRC_ACTIVE_5,
    availableFbJoin: Joins.VIDEO_SRC_AVAIL_5,
    nameSerialJoin: Joins.VIDEO_SRC_NAME_5,
    defaultName:    'Source 5',
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
  {
    selectJoin:         Joins.VIDEO_DEST_SELECT_5,
    activeFbJoin:       Joins.VIDEO_DEST_ACTIVE_5,
    powerBtnJoin:       Joins.VIDEO_DEST_POWER_BTN_5,
    powerFbJoin:        Joins.VIDEO_DEST_POWER_FB_5,
    videoBtnJoin:       Joins.VIDEO_DEST_VIDEO_BTN_5,
    videoFbJoin:        Joins.VIDEO_DEST_VIDEO_FB_5,
    nameSerialJoin:     Joins.VIDEO_DEST_NAME_5,
    routedSrcSerialJoin: Joins.VIDEO_DEST_ROUTED_SRC_NAME_5,
    defaultName:        'Display 5',
    defaultRoutedSrc:   '—',
  },
  {
    selectJoin:         Joins.VIDEO_DEST_SELECT_6,
    activeFbJoin:       Joins.VIDEO_DEST_ACTIVE_6,
    powerBtnJoin:       Joins.VIDEO_DEST_POWER_BTN_6,
    powerFbJoin:        Joins.VIDEO_DEST_POWER_FB_6,
    videoBtnJoin:       Joins.VIDEO_DEST_VIDEO_BTN_6,
    videoFbJoin:        Joins.VIDEO_DEST_VIDEO_FB_6,
    nameSerialJoin:     Joins.VIDEO_DEST_NAME_6,
    routedSrcSerialJoin: Joins.VIDEO_DEST_ROUTED_SRC_NAME_6,
    defaultName:        'Display 6',
    defaultRoutedSrc:   '—',
  },
  {
    selectJoin:         Joins.VIDEO_DEST_SELECT_7,
    activeFbJoin:       Joins.VIDEO_DEST_ACTIVE_7,
    powerBtnJoin:       Joins.VIDEO_DEST_POWER_BTN_7,
    powerFbJoin:        Joins.VIDEO_DEST_POWER_FB_7,
    videoBtnJoin:       Joins.VIDEO_DEST_VIDEO_BTN_7,
    videoFbJoin:        Joins.VIDEO_DEST_VIDEO_FB_7,
    nameSerialJoin:     Joins.VIDEO_DEST_NAME_7,
    routedSrcSerialJoin: Joins.VIDEO_DEST_ROUTED_SRC_NAME_7,
    defaultName:        'Display 7',
    defaultRoutedSrc:   '—',
  },
  {
    selectJoin:         Joins.VIDEO_DEST_SELECT_8,
    activeFbJoin:       Joins.VIDEO_DEST_ACTIVE_8,
    powerBtnJoin:       Joins.VIDEO_DEST_POWER_BTN_8,
    powerFbJoin:        Joins.VIDEO_DEST_POWER_FB_8,
    videoBtnJoin:       Joins.VIDEO_DEST_VIDEO_BTN_8,
    videoFbJoin:        Joins.VIDEO_DEST_VIDEO_FB_8,
    nameSerialJoin:     Joins.VIDEO_DEST_NAME_8,
    routedSrcSerialJoin: Joins.VIDEO_DEST_ROUTED_SRC_NAME_8,
    defaultName:        'Display 8',
    defaultRoutedSrc:   '—',
  },
  {
    selectJoin:         Joins.VIDEO_DEST_SELECT_9,
    activeFbJoin:       Joins.VIDEO_DEST_ACTIVE_9,
    powerBtnJoin:       Joins.VIDEO_DEST_POWER_BTN_9,
    powerFbJoin:        Joins.VIDEO_DEST_POWER_FB_9,
    videoBtnJoin:       Joins.VIDEO_DEST_VIDEO_BTN_9,
    videoFbJoin:        Joins.VIDEO_DEST_VIDEO_FB_9,
    nameSerialJoin:     Joins.VIDEO_DEST_NAME_9,
    routedSrcSerialJoin: Joins.VIDEO_DEST_ROUTED_SRC_NAME_9,
    defaultName:        'Display 9',
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
 * - Top:    Teams / BYOD mode toggles.
 * - Middle: Source selection tiles (large, easy tap targets).
 * - Bottom: Destination display cards with independent power / video controls.
 */
const VideoView: React.FC = () => {
  const teamsModeOn  = useDigitalJoin(Joins.TEAMS_MODE_FB);
  const byodModeOn   = useDigitalJoin(Joins.BYOD_MODE_FB);
  const sendTeams    = useSendDigitalPulse(Joins.TEAMS_MODE_BTN);
  const sendByod     = useSendDigitalPulse(Joins.BYOD_MODE_BTN);
  const sendByodSel  = useSendDigitalPulse(Joins.BYOD_SELECT_BTN);

  return (
    <div className="ev-page">

      {/* ── Mode toggles ──────────────────────────────────────────────────── */}
      <div className="ev-mode-bar">
        <label className="ev-toggle-wrap">
          <span className="ev-toggle-wrap__text">
            Teams Mode <span aria-hidden="true">{teamsModeOn ? '(On)' : '(Off)'}</span>
          </span>
          <span
            className={`ev-toggle${teamsModeOn ? ' ev-toggle--on' : ''}`}
            role="switch"
            aria-checked={teamsModeOn}
            aria-label="Teams Mode"
            tabIndex={0}
            onClick={sendTeams}
            onKeyDown={(e) => e.key === 'Enter' && sendTeams()}
          >
            <span className="ev-toggle__thumb" />
          </span>
        </label>

        <span className="ev-mode-bar__hint">Select a source, then choose a display.</span>

        <label className="ev-toggle-wrap">
          <span className="ev-toggle-wrap__text">
            BYOD Mode <span aria-hidden="true">{byodModeOn ? '(On)' : '(Off)'}</span>
          </span>
          <span
            className={`ev-toggle${byodModeOn ? ' ev-toggle--on' : ''}`}
            role="switch"
            aria-checked={byodModeOn}
            aria-label="BYOD Mode"
            tabIndex={0}
            onClick={sendByod}
            onKeyDown={(e) => e.key === 'Enter' && sendByod()}
          >
            <span className="ev-toggle__thumb" />
          </span>
        </label>
      </div>

      {/* ── Sources ───────────────────────────────────────────────────────── */}
      <section className="ev-section" aria-label="Sources">
        <h2 className="ev-section__title">What do you want to show?</h2>
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
