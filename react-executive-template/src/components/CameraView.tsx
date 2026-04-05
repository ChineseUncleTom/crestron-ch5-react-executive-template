import React, { useCallback } from 'react';
import {
  ChevronUp, ChevronDown, ChevronLeft, ChevronRight,
  ZoomIn, ZoomOut, Power, Target,
} from 'lucide-react';
import { CrComLib } from '@crestron/ch5-crcomlib/build_bundles/cjs/cr-com-lib';
import {
  Joins,
  useDigitalJoin,
  useSerialJoin,
  useSendDigitalPulse,
} from '../hooks/useCrestron';

// ── Hold helper ───────────────────────────────────────────────────────────────
function useDigitalHold(joinNumber: number) {
  const press = useCallback(() => {
    CrComLib.publishEvent('b', joinNumber.toString(), true);
  }, [joinNumber]);
  const release = useCallback(() => {
    CrComLib.publishEvent('b', joinNumber.toString(), false);
  }, [joinNumber]);
  return { press, release };
}

// ── PresetTile ────────────────────────────────────────────────────────────────

interface PresetTileProps {
  index: number;
  join: number;
  nameJoin: number;
}

const PresetTile: React.FC<PresetTileProps> = ({ index, join, nameJoin }) => {
  const send  = useSendDigitalPulse(join);
  const name  = useSerialJoin(nameJoin);
  const label = name || `Preset ${index + 1}`;
  return (
    <button className="cv-preset-tile" onClick={send} aria-label={`Recall preset: ${label}`}>
      {label}
    </button>
  );
};

// ── CameraSelectBtn ───────────────────────────────────────────────────────────

interface CameraSelectBtnProps {
  index: number;
  btnJoin: number;
  fbJoin: number;
  nameJoin: number;
}

const CameraSelectBtn: React.FC<CameraSelectBtnProps> = ({ index, btnJoin, fbJoin, nameJoin }) => {
  const active = useDigitalJoin(fbJoin);
  const send   = useSendDigitalPulse(btnJoin);
  const name   = useSerialJoin(nameJoin);
  const label  = name || `Camera ${index + 1}`;
  return (
    <button
      className={`cv-cam-btn${active ? ' cv-cam-btn--active' : ''}`}
      onClick={send}
      aria-pressed={active}
    >
      {label}
    </button>
  );
};

// ── CameraView ────────────────────────────────────────────────────────────────

/**
 * CameraView
 *
 * Executive camera control page with a presets-first design.
 *
 * Layout (top to bottom):
 * 1. Camera selection row – choose the active camera.
 * 2. Preset tiles grid   – executives one-tap to a preset position.
 * 3. PTZ / Power row     – manual pan/tilt/zoom and power/tracking toggles
 *    (secondary; used by AV staff for fine-tuning).
 */
const CameraView: React.FC = () => {
  const panLeft  = useDigitalHold(Joins.CAM_PAN_LEFT_BTN);
  const panRight = useDigitalHold(Joins.CAM_PAN_RIGHT_BTN);
  const tiltUp   = useDigitalHold(Joins.CAM_TILT_UP_BTN);
  const tiltDown = useDigitalHold(Joins.CAM_TILT_DOWN_BTN);
  const zoomIn   = useDigitalHold(Joins.CAM_ZOOM_IN_BTN);
  const zoomOut  = useDigitalHold(Joins.CAM_ZOOM_OUT_BTN);

  const sendPower    = useSendDigitalPulse(Joins.CAM_POWER_BTN);
  const sendTracking = useSendDigitalPulse(Joins.CAM_TRACKING_BTN);
  const powerOn      = useDigitalJoin(Joins.CAM_POWER_FB);
  const trackingOn   = useDigitalJoin(Joins.CAM_TRACKING_FB);

  return (
    <div className="cv-page">

      {/* ── 1. Camera selection ───────────────────────────────────────────── */}
      <section className="cv-section" aria-label="Camera Selection">
        <h2 className="cv-section__title">Select Camera</h2>
        <div className="cv-cam-row">
          {Array.from({ length: 10 }, (_, i) => (
            <CameraSelectBtn
              key={i}
              index={i}
              btnJoin={Joins.CAM_SELECT_1 + i}
              fbJoin={Joins.CAM_ACTIVE_FB_1 + i}
              nameJoin={Joins.CAM_NAME_1 + i}
            />
          ))}
        </div>
      </section>

      {/* ── 2. Presets (primary executive control) ────────────────────────── */}
      <section className="cv-section" aria-label="Camera Presets">
        <h2 className="cv-section__title">Camera Presets</h2>
        <div className="cv-preset-grid">
          {Array.from({ length: 10 }, (_, i) => (
            <PresetTile
              key={i}
              index={i}
              join={Joins.CAM_PRESET_1 + i}
              nameJoin={Joins.CAM_PRESET_NAME_1 + i}
            />
          ))}
        </div>
      </section>

      {/* ── 3. Manual PTZ + power (secondary) ────────────────────────────── */}
      <section className="cv-section" aria-label="Manual Camera Control">
        <h2 className="cv-section__title">Manual Control</h2>
        <div className="cv-manual">

          {/* D-pad */}
          <div className="cv-dpad" aria-label="Pan and tilt">
            <button
              className="cv-dpad__btn cv-dpad__btn--up"
              aria-label="Tilt up"
              onPointerDown={tiltUp.press}
              onPointerUp={tiltUp.release}
              onPointerLeave={tiltUp.release}
            ><ChevronUp size={18} aria-hidden="true" /></button>

            <button
              className="cv-dpad__btn cv-dpad__btn--left"
              aria-label="Pan left"
              onPointerDown={panLeft.press}
              onPointerUp={panLeft.release}
              onPointerLeave={panLeft.release}
            ><ChevronLeft size={18} aria-hidden="true" /></button>

            <div className="cv-dpad__center" aria-hidden="true" />

            <button
              className="cv-dpad__btn cv-dpad__btn--right"
              aria-label="Pan right"
              onPointerDown={panRight.press}
              onPointerUp={panRight.release}
              onPointerLeave={panRight.release}
            ><ChevronRight size={18} aria-hidden="true" /></button>

            <button
              className="cv-dpad__btn cv-dpad__btn--down"
              aria-label="Tilt down"
              onPointerDown={tiltDown.press}
              onPointerUp={tiltDown.release}
              onPointerLeave={tiltDown.release}
            ><ChevronDown size={18} aria-hidden="true" /></button>
          </div>

          {/* Zoom + Power/Tracking */}
          <div className="cv-manual__actions">
            <button
              className="cv-action-btn"
              aria-label="Zoom in"
              onPointerDown={zoomIn.press}
              onPointerUp={zoomIn.release}
              onPointerLeave={zoomIn.release}
            >
              <ZoomIn size={16} aria-hidden="true" />
              Zoom In
            </button>
            <button
              className="cv-action-btn"
              aria-label="Zoom out"
              onPointerDown={zoomOut.press}
              onPointerUp={zoomOut.release}
              onPointerLeave={zoomOut.release}
            >
              <ZoomOut size={16} aria-hidden="true" />
              Zoom Out
            </button>
            <button
              className={`cv-action-btn${powerOn ? ' cv-action-btn--on' : ''}`}
              onClick={sendPower}
              aria-pressed={powerOn}
            >
              <Power size={16} aria-hidden="true" />
              Power {powerOn ? 'ON' : 'OFF'}
            </button>
            <button
              className={`cv-action-btn${trackingOn ? ' cv-action-btn--on' : ''}`}
              onClick={sendTracking}
              aria-pressed={trackingOn}
            >
              <Target size={16} aria-hidden="true" />
              Tracking {trackingOn ? 'ON' : 'OFF'}
            </button>
          </div>

        </div>
      </section>

    </div>
  );
};

export default CameraView;
