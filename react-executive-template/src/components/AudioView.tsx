import React, { useCallback, useRef, useState } from 'react';
import { Mic, MicOff, Volume2, Volume1, VolumeX } from 'lucide-react';
import { CrComLib } from '@crestron/ch5-crcomlib/build_bundles/cjs/cr-com-lib';
import {
  Joins,
  useAnalogJoin,
  useDigitalJoin,
  useSerialJoin,
  useSendAnalog,
  useSendDigitalPulse,
} from '../hooks/useCrestron';

// ── DragSlider ────────────────────────────────────────────────────────────────
// Custom pointer-drag slider matching the home-page master-volume feel.
// value / onChange both use the raw Crestron 0–65535 scale.

interface DragSliderProps {
  value: number;
  onChange: (newValue: number) => void;
  ariaLabel?: string;
}

const DragSlider: React.FC<DragSliderProps> = ({ value, onChange, ariaLabel }) => {
  const pct = Math.round((value / 65535) * 100);
  const valueRef = useRef(value);
  valueRef.current = value;

  const isDraggingRef   = useRef(false);
  const dragStartXRef   = useRef(0);
  const dragStartValRef = useRef(0);
  const trackWidthRef  = useRef(1);

  const handlePointerDown = useCallback((e: React.PointerEvent<HTMLDivElement>) => {
    e.currentTarget.setPointerCapture(e.pointerId);
    const rect = e.currentTarget.getBoundingClientRect();
    const relX = e.clientX - rect.left;
    const clickedVal = Math.max(0, Math.min(65535, Math.round((relX / rect.width) * 65535)));
    onChange(clickedVal);
    dragStartXRef.current  = e.clientX;
    dragStartValRef.current = clickedVal;
    trackWidthRef.current  = rect.width || 1;
    isDraggingRef.current  = true;
  }, [onChange]);

  const handlePointerMove = useCallback((e: React.PointerEvent<HTMLDivElement>) => {
    if (!isDraggingRef.current) return;
    const deltaX   = e.clientX - dragStartXRef.current;
    const deltaRaw = Math.round((deltaX / trackWidthRef.current) * 65535);
    onChange(Math.max(0, Math.min(65535, dragStartValRef.current + deltaRaw)));
  }, [onChange]);

  const stopDrag = useCallback((e: React.PointerEvent<HTMLDivElement>) => {
    if (!isDraggingRef.current) return;
    e.currentTarget.releasePointerCapture(e.pointerId);
    isDraggingRef.current = false;
  }, []);

  const handleKeyDown = useCallback((e: React.KeyboardEvent<HTMLDivElement>) => {
    const STEP = Math.round(65535 * 0.05);
    if (e.key === 'ArrowRight' || e.key === 'ArrowUp') {
      e.preventDefault();
      onChange(Math.min(65535, valueRef.current + STEP));
    } else if (e.key === 'ArrowLeft' || e.key === 'ArrowDown') {
      e.preventDefault();
      onChange(Math.max(0, valueRef.current - STEP));
    }
  }, [onChange]);

  return (
    <div
      className="av-drag-slider"
      role="slider"
      aria-valuenow={pct}
      aria-valuemin={0}
      aria-valuemax={100}
      aria-valuetext={`${pct}%`}
      aria-label={ariaLabel}
      tabIndex={0}
      onPointerDown={handlePointerDown}
      onPointerMove={handlePointerMove}
      onPointerUp={stopDrag}
      onPointerCancel={stopDrag}
      onKeyDown={handleKeyDown}
    >
      <div className="av-drag-slider__track" aria-hidden="true">
        <div className="av-drag-slider__fill" style={{ width: `${pct}%` }} />
        <div className="av-drag-slider__thumb" style={{ left: `${pct}%` }} />
      </div>
    </div>
  );
};

// ── Hold helper (press = true, release = false) ───────────────────────────────
function useDigitalHold(joinNumber: number) {
  const press = useCallback(() => {
    CrComLib.publishEvent('b', joinNumber.toString(), true);
  }, [joinNumber]);
  const release = useCallback(() => {
    CrComLib.publishEvent('b', joinNumber.toString(), false);
  }, [joinNumber]);
  return { press, release };
}

// ── Audio source data ─────────────────────────────────────────────────────────

const AUDIO_SOURCES = [
  {
    volUpJoin:      Joins.AUDIO_SRC_VOL_UP_1,
    volDownJoin:    Joins.AUDIO_SRC_VOL_DOWN_1,
    defaultJoin:    Joins.AUDIO_SRC_DEFAULT_1,
    muteBtnJoin:    Joins.AUDIO_SRC_MUTE_BTN_1,
    muteFbJoin:     Joins.AUDIO_SRC_MUTE_FB_1,
    volSetJoin:     Joins.AUDIO_SRC_VOL_SET_1,
    volFbJoin:      Joins.AUDIO_SRC_VOL_FB_1,
    nameSerialJoin: Joins.AUDIO_SRC_NAME_1,
    defaultName:    'Source 1',
  },
  {
    volUpJoin:      Joins.AUDIO_SRC_VOL_UP_2,
    volDownJoin:    Joins.AUDIO_SRC_VOL_DOWN_2,
    defaultJoin:    Joins.AUDIO_SRC_DEFAULT_2,
    muteBtnJoin:    Joins.AUDIO_SRC_MUTE_BTN_2,
    muteFbJoin:     Joins.AUDIO_SRC_MUTE_FB_2,
    volSetJoin:     Joins.AUDIO_SRC_VOL_SET_2,
    volFbJoin:      Joins.AUDIO_SRC_VOL_FB_2,
    nameSerialJoin: Joins.AUDIO_SRC_NAME_2,
    defaultName:    'Source 2',
  },
  {
    volUpJoin:      Joins.AUDIO_SRC_VOL_UP_3,
    volDownJoin:    Joins.AUDIO_SRC_VOL_DOWN_3,
    defaultJoin:    Joins.AUDIO_SRC_DEFAULT_3,
    muteBtnJoin:    Joins.AUDIO_SRC_MUTE_BTN_3,
    muteFbJoin:     Joins.AUDIO_SRC_MUTE_FB_3,
    volSetJoin:     Joins.AUDIO_SRC_VOL_SET_3,
    volFbJoin:      Joins.AUDIO_SRC_VOL_FB_3,
    nameSerialJoin: Joins.AUDIO_SRC_NAME_3,
    defaultName:    'Source 3',
  },
  {
    volUpJoin:      Joins.AUDIO_SRC_VOL_UP_4,
    volDownJoin:    Joins.AUDIO_SRC_VOL_DOWN_4,
    defaultJoin:    Joins.AUDIO_SRC_DEFAULT_4,
    muteBtnJoin:    Joins.AUDIO_SRC_MUTE_BTN_4,
    muteFbJoin:     Joins.AUDIO_SRC_MUTE_FB_4,
    volSetJoin:     Joins.AUDIO_SRC_VOL_SET_4,
    volFbJoin:      Joins.AUDIO_SRC_VOL_FB_4,
    nameSerialJoin: Joins.AUDIO_SRC_NAME_4,
    defaultName:    'Source 4',
  },
  {
    volUpJoin:      Joins.AUDIO_SRC_VOL_UP_5,
    volDownJoin:    Joins.AUDIO_SRC_VOL_DOWN_5,
    defaultJoin:    Joins.AUDIO_SRC_DEFAULT_5,
    muteBtnJoin:    Joins.AUDIO_SRC_MUTE_BTN_5,
    muteFbJoin:     Joins.AUDIO_SRC_MUTE_FB_5,
    volSetJoin:     Joins.AUDIO_SRC_VOL_SET_5,
    volFbJoin:      Joins.AUDIO_SRC_VOL_FB_5,
    nameSerialJoin: Joins.AUDIO_SRC_NAME_5,
    defaultName:    'Source 5',
  },
] as const;

// ── SourceRow ─────────────────────────────────────────────────────────────────

interface SourceRowProps {
  volUpJoin: number;
  volDownJoin: number;
  defaultJoin: number;
  muteBtnJoin: number;
  muteFbJoin: number;
  volSetJoin: number;
  volFbJoin: number;
  nameSerialJoin: number;
  defaultName: string;
}

const SourceRow: React.FC<SourceRowProps> = ({
  volUpJoin,
  volDownJoin,
  defaultJoin,
  muteBtnJoin,
  muteFbJoin,
  volSetJoin,
  volFbJoin,
  nameSerialJoin,
  defaultName,
}) => {
  const name        = useSerialJoin(nameSerialJoin);
  const muted       = useDigitalJoin(muteFbJoin);
  const volFb       = useAnalogJoin(volFbJoin);
  const sendDefault = useSendDigitalPulse(defaultJoin);
  const sendMute    = useSendDigitalPulse(muteBtnJoin);
  const sendVolSet  = useSendAnalog(volSetJoin);
  const volUp       = useDigitalHold(volUpJoin);
  const volDown     = useDigitalHold(volDownJoin);

  const sliderValue = Math.round((volFb / 65535) * 100);

  const displayName = name || defaultName;

  return (
    <div className={`av-source-row${muted ? ' av-source-row--muted' : ''}`}>
      <span className="av-source-row__name">{displayName}</span>

      <div className="av-source-row__controls">
        <button
          className="av-src-btn"
          aria-label={`${displayName} volume down`}
          onPointerDown={volDown.press}
          onPointerUp={volDown.release}
          onPointerLeave={volDown.release}
          onPointerCancel={volDown.release}
        >
          <Volume1 size={15} aria-hidden="true" />
        </button>

        <DragSlider
          value={volFb}
          onChange={sendVolSet}
          ariaLabel={`${displayName} volume`}
        />

        <button
          className="av-src-btn av-src-btn--default"
          aria-label={`${displayName} default volume`}
          onClick={sendDefault}
        >
          Def
        </button>

        <button
          className="av-src-btn"
          aria-label={`${displayName} volume up`}
          onPointerDown={volUp.press}
          onPointerUp={volUp.release}
          onPointerLeave={volUp.release}
          onPointerCancel={volUp.release}
        >
          <Volume2 size={15} aria-hidden="true" />
        </button>

        <button
          className={`av-src-btn${muted ? ' av-src-btn--muted' : ''}`}
          aria-label={`${displayName} ${muted ? 'unmute' : 'mute'}`}
          onClick={sendMute}
          aria-pressed={muted}
        >
          {muted
            ? <VolumeX size={15} aria-hidden="true" />
            : <Volume2 size={15} aria-hidden="true" />
          }
        </button>
      </div>

      <span className="av-source-row__level">{sliderValue}%</span>
    </div>
  );
};

// ── AudioView ─────────────────────────────────────────────────────────────────

/**
 * AudioView
 *
 * Executive audio control page.
 *
 * Primary controls (immediately visible, zero training required):
 * - Master Volume: large horizontal slider with +/- nudge buttons.
 * - Microphone Mute: oversized toggle button, red when muted.
 *
 * Secondary controls (Advanced section – for AV staff):
 * - Per-source volume rows (expandable).
 * - Far-end mic mutes: Wireless Mic, Ceiling Mic.
 */
const AudioView: React.FC = () => {
  const [showAdvanced, setShowAdvanced] = useState(false);

  // Master volume
  const masterVolFb   = useAnalogJoin(Joins.VOLUME_FB);
  const sendMasterVol = useSendAnalog(Joins.VOLUME_SET);

  // Track the latest feedback value in a ref so interval callbacks stay current.
  const masterVolFbRef = useRef(masterVolFb);
  masterVolFbRef.current = masterVolFb;

  // Step = 5% of 65535 ≈ 3277 per nudge tick
  const NUDGE_STEP = 3277;
  const nudgeIntervalRef = useRef<ReturnType<typeof setInterval> | null>(null);

  const startNudge = useCallback(
    (direction: 1 | -1) => {
      const applyNudge = () => {
        const next = Math.max(0, Math.min(65535, masterVolFbRef.current + direction * NUDGE_STEP));
        sendMasterVol(next);
      };
      applyNudge();
      nudgeIntervalRef.current = setInterval(applyNudge, 250);
    },
    [sendMasterVol],
  );

  const stopNudge = useCallback(() => {
    if (nudgeIntervalRef.current !== null) {
      clearInterval(nudgeIntervalRef.current);
      nudgeIntervalRef.current = null;
    }
  }, []);

  const masterVolPct = Math.round((masterVolFb / 65535) * 100);

  // Master volume mute (separate from microphone privacy mute)
  const masterVolMuted    = useDigitalJoin(Joins.MASTER_VOL_MUTE_FB);
  const sendMasterVolMute = useSendDigitalPulse(Joins.MASTER_VOL_MUTE_BTN);
  const sendMasterVolDef  = useSendDigitalPulse(Joins.MASTER_VOL_DEFAULT_BTN);

  // Primary mic mute (privacy mute = room-wide microphone mute)
  const privacyMuted    = useDigitalJoin(Joins.AUDIO_PRIVACY_MUTE_FB);
  const sendPrivacyMute = useSendDigitalPulse(Joins.AUDIO_PRIVACY_MUTE_BTN);

  // Far-end mutes (advanced)
  const wirelessMicMuted    = useDigitalJoin(Joins.AUDIO_WIRELESS_MIC_MUTE_FB);
  const ceilingMicMuted     = useDigitalJoin(Joins.AUDIO_CEILING_MIC_MUTE_FB);
  const sendWirelessMicMute = useSendDigitalPulse(Joins.AUDIO_WIRELESS_MIC_MUTE_BTN);
  const sendCeilingMicMute  = useSendDigitalPulse(Joins.AUDIO_CEILING_MIC_MUTE_BTN);

  return (
    <div className="av-page">

      {/* ── Primary: Master Volume ────────────────────────────────────────── */}
      <section className="av-section" aria-label="Master Volume">
        <h2 className="av-section__title">Master Volume</h2>
        <div className="av-source-row av-source-row--master">
          <button
            className="av-src-btn"
            aria-label="Volume down"
            onPointerDown={() => startNudge(-1)}
            onPointerUp={stopNudge}
            onPointerLeave={stopNudge}
            onPointerCancel={stopNudge}
          >
            <Volume1 size={15} aria-hidden="true" />
          </button>

          <DragSlider
            value={masterVolFb}
            onChange={sendMasterVol}
            ariaLabel="Master volume"
          />

          <button
            className="av-src-btn av-src-btn--default"
            aria-label="Master volume default"
            onClick={sendMasterVolDef}
          >
            Def
          </button>

          <button
            className="av-src-btn"
            aria-label="Volume up"
            onPointerDown={() => startNudge(1)}
            onPointerUp={stopNudge}
            onPointerLeave={stopNudge}
            onPointerCancel={stopNudge}
          >
            <Volume2 size={15} aria-hidden="true" />
          </button>

          <button
            className={`av-src-btn${masterVolMuted ? ' av-src-btn--muted' : ''}`}
            aria-label={masterVolMuted ? 'Unmute master volume' : 'Mute master volume'}
            aria-pressed={masterVolMuted}
            onClick={sendMasterVolMute}
          >
            {masterVolMuted
              ? <VolumeX size={15} aria-hidden="true" />
              : <Volume2 size={15} aria-hidden="true" />
            }
          </button>

          <span className="av-source-row__level">{masterVolPct}%</span>
        </div>
      </section>

      {/* ── Primary: Microphone Mute ──────────────────────────────────────── */}
      <section className="av-section" aria-label="Microphone">
        <h2 className="av-section__title">Microphone</h2>
        <button
          className={`av-mute-btn${privacyMuted ? ' av-mute-btn--muted' : ''}`}
          onClick={sendPrivacyMute}
          aria-pressed={privacyMuted}
          aria-label={privacyMuted ? 'Microphone Muted – tap to unmute' : 'Microphone Active – tap to mute'}
        >
          <span className="av-mute-btn__icon" aria-hidden="true">
            {privacyMuted ? <MicOff size={48} /> : <Mic size={48} />}
          </span>
          <span className="av-mute-btn__label">
            {privacyMuted ? 'MUTED' : 'ACTIVE'}
          </span>
          <span className="av-mute-btn__hint">
            {privacyMuted ? 'Tap to unmute microphone' : 'Tap to mute microphone'}
          </span>
        </button>
      </section>

      {/* ── Advanced (collapsible) ────────────────────────────────────────── */}
      <section className="av-section av-section--advanced" aria-label="Advanced Audio Controls">
        <button
          className="av-advanced-toggle"
          onClick={() => setShowAdvanced((v) => !v)}
          aria-expanded={showAdvanced}
        >
          <span>Advanced Controls</span>
          <span className="av-advanced-toggle__arrow" aria-hidden="true">
            {showAdvanced ? '▲' : '▼'}
          </span>
        </button>

        {showAdvanced && (
          <div className="av-advanced-content">
            {/* Per-source volume rows */}
            <div className="av-sources">
              {AUDIO_SOURCES.map((src) => (
                <SourceRow key={src.volSetJoin} {...src} />
              ))}
            </div>

            {/* Far-end mic mutes */}
            <div className="av-farend">
              <h3 className="av-farend__title">Far-end Microphones</h3>
              <div className="av-farend__btns">
                <button
                  className={`av-farend-btn${wirelessMicMuted ? ' av-farend-btn--muted' : ''}`}
                  onClick={sendWirelessMicMute}
                  aria-pressed={wirelessMicMuted}
                >
                  <Mic size={16} aria-hidden="true" />
                  Wireless Mic {wirelessMicMuted ? '(Muted)' : '(Active)'}
                </button>
                <button
                  className={`av-farend-btn${ceilingMicMuted ? ' av-farend-btn--muted' : ''}`}
                  onClick={sendCeilingMicMute}
                  aria-pressed={ceilingMicMuted}
                >
                  <Mic size={16} aria-hidden="true" />
                  Ceiling Mic {ceilingMicMuted ? '(Muted)' : '(Active)'}
                </button>
              </div>
            </div>
          </div>
        )}
      </section>

    </div>
  );
};

export default AudioView;
