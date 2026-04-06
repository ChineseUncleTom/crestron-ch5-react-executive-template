import React, { useState, useCallback, useEffect, useRef } from 'react';
import {
  X,
  Sun,
  Moon,
  Contrast,
  Palette,
  Wifi,
  WifiOff,
  Monitor,
  Mic,
  Cpu,
  Usb,
  Zap,
  SlidersHorizontal,
  Phone,
  QrCode,
  Clock,
  Thermometer,
  ShieldAlert,
  RefreshCw,
  Power,
  Lock,
  ChevronRight,
  Network,
} from 'lucide-react';
import {
  useDigitalJoin,
  useSerialJoin,
  useAnalogJoin,
  useSendAnalog,
  useSendSerial,
  useSendDigitalPulse,
  Joins,
} from '../hooks/useCrestron';

/* ── Types ──────────────────────────────────────────────────────────────────── */

type ThemeMode = 'dark' | 'light' | 'high-contrast';
type ClockFormat = '12h' | '24h';
type TempUnit = 'C' | 'F';

interface SettingsDrawerProps {
  isOpen: boolean;
  onClose: () => void;
}

/* ── PIN Modal ──────────────────────────────────────────────────────────────── */

/**
 * Default PIN used in the demo.  In a production deployment this should be
 * replaced by a value delivered from the Crestron processor via a serial join
 * so it never resides in client-side source code.
 */
const ADMIN_PIN = '1234';

interface PinModalProps {
  onSuccess: () => void;
  onCancel: () => void;
}

const PinModal: React.FC<PinModalProps> = ({ onSuccess, onCancel }) => {
  const [digits, setDigits] = useState('');
  const [error, setError] = useState(false);

  const handleDigit = useCallback((d: string) => {
    if (digits.length >= 4) return;
    const next = digits + d;
    setDigits(next);
    setError(false);
    if (next.length === 4) {
      if (next === ADMIN_PIN) {
        onSuccess();
      } else {
        setError(true);
        setTimeout(() => { setDigits(''); setError(false); }, 700);
      }
    }
  }, [digits, onSuccess]);

  const handleBackspace = useCallback(() => {
    setDigits(prev => prev.slice(0, -1));
    setError(false);
  }, []);

  const keys = ['1','2','3','4','5','6','7','8','9','','0','⌫'];

  return (
    <div className="settings-pin-overlay" role="dialog" aria-modal="true" aria-label="Admin PIN entry">
      <div className="settings-pin-modal">
        <h3 className="settings-pin-modal__title">
          <Lock size={18} aria-hidden="true" />
          Admin Access
        </h3>
        <p className="settings-pin-modal__sub">Enter the 4-digit admin PIN</p>

        <div className={`settings-pin-modal__dots${error ? ' settings-pin-modal__dots--error' : ''}`} aria-label={`${digits.length} of 4 digits entered`}>
          {[0,1,2,3].map(i => (
            <span key={i} className={`settings-pin-modal__dot${i < digits.length ? ' settings-pin-modal__dot--filled' : ''}`} aria-hidden="true" />
          ))}
        </div>

        {error && <p className="settings-pin-modal__error">Incorrect PIN. Try again.</p>}

        <div className="settings-pin-modal__keypad" role="group" aria-label="PIN keypad">
          {keys.map((k, idx) => (
            k === '' ? (
              <span key={idx} aria-hidden="true" />
            ) : k === '⌫' ? (
              <button
                key={idx}
                className="settings-pin-modal__key settings-pin-modal__key--back"
                onClick={handleBackspace}
                aria-label="Backspace"
              >
                {k}
              </button>
            ) : (
              <button
                key={idx}
                className="settings-pin-modal__key"
                onClick={() => handleDigit(k)}
                aria-label={`Digit ${k}`}
              >
                {k}
              </button>
            )
          ))}
        </div>

        <button className="settings-pin-modal__cancel" onClick={onCancel}>
          Cancel
        </button>
      </div>
    </div>
  );
};

/* ── Toggle Row helper ──────────────────────────────────────────────────────── */

interface ToggleRowProps {
  label: string;
  description?: string;
  checked: boolean;
  onChange: (v: boolean) => void;
  id: string;
}

const ToggleRow: React.FC<ToggleRowProps> = ({ label, description, checked, onChange, id }) => (
  <label className="settings-toggle-row" htmlFor={id}>
    <span className="settings-toggle-row__text">
      <span className="settings-toggle-row__label">{label}</span>
      {description && <span className="settings-toggle-row__desc">{description}</span>}
    </span>
    <span
      id={id}
      role="switch"
      aria-checked={checked}
      tabIndex={0}
      className={`settings-toggle${checked ? ' settings-toggle--on' : ''}`}
      onClick={() => onChange(!checked)}
      onKeyDown={e => { if (e.key === ' ' || e.key === 'Enter') { e.preventDefault(); onChange(!checked); } }}
      aria-label={label}
    >
      <span className="settings-toggle__thumb" />
    </span>
  </label>
);

/* ── Slider Row helper ──────────────────────────────────────────────────────── */

interface SliderRowProps {
  label: string;
  value: number;
  min?: number;
  max?: number;
  unit?: string;
  onChange: (v: number) => void;
  id: string;
}

const SliderRow: React.FC<SliderRowProps> = ({ label, value, min = 0, max = 100, unit = '%', onChange, id }) => (
  <div className="settings-slider-row">
    <div className="settings-slider-row__header">
      <span className="settings-slider-row__label">{label}</span>
      <span className="settings-slider-row__value">{value}{unit}</span>
    </div>
    <input
      id={id}
      type="range"
      min={min}
      max={max}
      value={value}
      className="settings-slider"
      aria-label={label}
      aria-valuenow={value}
      aria-valuemin={min}
      aria-valuemax={max}
      aria-valuetext={`${value}${unit}`}
      onChange={e => onChange(Number(e.target.value))}
    />
  </div>
);

/* ── Section header helper ──────────────────────────────────────────────────── */

const SectionHeader: React.FC<{ icon: React.ReactNode; title: string }> = ({ icon, title }) => (
  <h3 className="settings-section__title">
    <span className="settings-section__title-icon" aria-hidden="true">{icon}</span>
    {title}
  </h3>
);

/* ── Main SettingsDrawer ────────────────────────────────────────────────────── */

const SettingsDrawer: React.FC<SettingsDrawerProps> = ({ isOpen, onClose }) => {
  /* ── Theme state ────────────────────────────────────────────────────────── */
  const themeModeSerial = useSerialJoin(Joins.SETTINGS_THEME_MODE);
  const brandColorSerial = useSerialJoin(Joins.SETTINGS_BRAND_COLOR);
  const sendThemeMode  = useSendSerial(Joins.SETTINGS_THEME_MODE);
  const sendBrandColor = useSendSerial(Joins.SETTINGS_BRAND_COLOR);

  const [themeMode, setThemeModeLocal] = useState<ThemeMode>('dark');
  const [brandColor, setBrandColorLocal] = useState('#3b82f6');

  // Sync theme from processor when the join value arrives (e.g. on panel online or after reset)
  useEffect(() => {
    if (themeModeSerial === 'dark' || themeModeSerial === 'light' || themeModeSerial === 'high-contrast') {
      setThemeModeLocal(themeModeSerial);
    }
  }, [themeModeSerial]);

  useEffect(() => {
    if (brandColorSerial && /^#[0-9a-fA-F]{6}$/.test(brandColorSerial)) {
      setBrandColorLocal(brandColorSerial);
    }
  }, [brandColorSerial]);

  const setThemeMode = useCallback((mode: ThemeMode) => {
    setThemeModeLocal(mode);
    sendThemeMode(mode);
  }, [sendThemeMode]);

  const setBrandColor = useCallback((color: string) => {
    setBrandColorLocal(color);
    sendBrandColor(color);
  }, [sendBrandColor]);

  /* ── BYOD state ─────────────────────────────────────────────────────────── */
  const byodAutoSwitchFb  = useDigitalJoin(Joins.SETTINGS_BYOD_AUTO_SWITCH_FB);
  const byodAutoPowerOnFb = useDigitalJoin(Joins.SETTINGS_BYOD_AUTO_POWER_FB);
  const sendByodAutoSwitchToggle  = useSendDigitalPulse(Joins.SETTINGS_BYOD_AUTO_SWITCH_BTN);
  const sendByodAutoPowerToggle   = useSendDigitalPulse(Joins.SETTINGS_BYOD_AUTO_POWER_BTN);

  /* ── Display & Audio state ──────────────────────────────────────────────── */
  const brightnessFb   = useAnalogJoin(Joins.BRIGHTNESS_FB);
  const sendBrightness = useSendAnalog(Joins.BRIGHTNESS_SET);
  // Convert 0–65535 analog to 0–100 percent for UI display
  const panelBrightness = Math.round((brightnessFb / 65535) * 100);
  const handleBrightnessChange = useCallback((pct: number) => {
    sendBrightness(Math.round((pct / 100) * 65535));
  }, [sendBrightness]);

  const startupVolFb   = useAnalogJoin(Joins.SETTINGS_STARTUP_VOL_FB);
  const sendStartupVol = useSendAnalog(Joins.SETTINGS_STARTUP_VOL_SET);
  // Convert 0–65535 to 0–100 for display; use local state so slider is responsive
  const [startupVolume, setStartupVolumeLocal] = useState(30);
  useEffect(() => {
    setStartupVolumeLocal(Math.round((startupVolFb / 65535) * 100));
  }, [startupVolFb]);
  const setStartupVolume = useCallback((pct: number) => {
    setStartupVolumeLocal(pct);
    sendStartupVol(Math.round((pct / 100) * 65535));
  }, [sendStartupVol]);

  /* ── Environment state ──────────────────────────────────────────────────── */
  const clockFormatSerial = useSerialJoin(Joins.SETTINGS_CLOCK_FORMAT);
  const tempUnitSerial    = useSerialJoin(Joins.SETTINGS_TEMP_UNIT);
  const sendClockFormat = useSendSerial(Joins.SETTINGS_CLOCK_FORMAT);
  const sendTempUnit    = useSendSerial(Joins.SETTINGS_TEMP_UNIT);

  const [clockFormat, setClockFormatLocal] = useState<ClockFormat>('12h');
  const [tempUnit, setTempUnitLocal]       = useState<TempUnit>('F');

  useEffect(() => {
    if (clockFormatSerial === '12h' || clockFormatSerial === '24h') {
      setClockFormatLocal(clockFormatSerial);
    }
  }, [clockFormatSerial]);

  useEffect(() => {
    if (tempUnitSerial === 'F' || tempUnitSerial === 'C') {
      setTempUnitLocal(tempUnitSerial as TempUnit);
    }
  }, [tempUnitSerial]);

  const setClockFormat = useCallback((fmt: ClockFormat) => {
    setClockFormatLocal(fmt);
    sendClockFormat(fmt);
  }, [sendClockFormat]);

  const setTempUnit = useCallback((unit: TempUnit) => {
    setTempUnitLocal(unit);
    sendTempUnit(unit);
  }, [sendTempUnit]);

  /* ── Help & Support info from SystemConfig ──────────────────────────────── */
  const helpItPhone      = useSerialJoin(Joins.HELP_IT_PHONE_SERIAL);
  const helpSupportEmail = useSerialJoin(Joins.HELP_SUPPORT_EMAIL_SERIAL);
  const helpQrLabel      = useSerialJoin(Joins.HELP_QR_LABEL_SERIAL);

  /* ── Network info from control system ───────────────────────────────────── */
  const networkPanelIp = useSerialJoin(Joins.NETWORK_PANEL_IP_SERIAL);
  const networkSubnet  = useSerialJoin(Joins.NETWORK_SUBNET_SERIAL);
  const networkMac     = useSerialJoin(Joins.NETWORK_MAC_SERIAL);
  const networkCsIp    = useSerialJoin(Joins.NETWORK_CS_IP_SERIAL);

  /* ── Admin state ────────────────────────────────────────────────────────── */
  const [showPinModal, setShowPinModal] = useState(false);
  const [adminUnlocked, setAdminUnlocked] = useState(false);
  const [rebootConfirm, setRebootConfirm] = useState(false);
  const [resetConfirm, setResetConfirm] = useState(false);
  const [adminStatus, setAdminStatus] = useState<string | null>(null);
  const sendResetBtn = useSendDigitalPulse(Joins.SETTINGS_RESET_BTN);

  /* ── CH5 joins (read-only for diagnostics display) ──────────────────────── */
  /**
   * SYSTEM_OFF_FB (join 5) is used here as a proxy for the CH5 WebSocket
   * connection health: it carries a non-zero value only after the processor has
   * established a session and sent at least one feedback signal.  A dedicated
   * WS_CONNECTED serial/digital join would be more semantically correct but is
   * not yet defined in the current JoinMap.
   */
  const ch5Connected  = useDigitalJoin(Joins.SYSTEM_OFF_FB);  // proxy for WS health
  const roomName      = useSerialJoin(Joins.ROOM_NAME_SERIAL);

  /* ── Config file persistence (dev-server API) ──────────────────────────── */

  // Guard that is set to true once the initial GET /api/user-config response
  // has been processed (or has failed).  The save effect checks this flag so
  // it never overwrites the file with React defaults before the initial load
  // has completed.
  const apiLoadedRef = useRef(false);

  // On mount: seed local state from CurrentUserConfig.json via the dev-server
  // API defined in setupProxy.js.  Only runs in development; the endpoint does
  // not exist in production (CH5 archive on a Crestron panel).
  // Crestron join values always take precedence when the processor is connected.
  useEffect(() => {
    if (process.env.NODE_ENV !== 'development') {
      apiLoadedRef.current = true;
      return;
    }
    fetch('/api/user-config')
      .then(r => (r.ok ? r.json() : null))
      .then((cfg: Record<string, unknown> | null) => {
        if (cfg) {
          if (cfg.ThemeMode === 'dark' || cfg.ThemeMode === 'light' || cfg.ThemeMode === 'high-contrast') {
            setThemeModeLocal(cfg.ThemeMode as ThemeMode);
          }
          if (typeof cfg.BrandColor === 'string' && /^#[0-9a-fA-F]{6}$/.test(cfg.BrandColor)) {
            setBrandColorLocal(cfg.BrandColor);
          }
          if (cfg.ClockFormat === '12h' || cfg.ClockFormat === '24h') {
            setClockFormatLocal(cfg.ClockFormat as ClockFormat);
          }
          if (cfg.TempUnit === 'F' || cfg.TempUnit === 'C') {
            setTempUnitLocal(cfg.TempUnit as TempUnit);
          }
          if (typeof cfg.StartupVolume === 'number') {
            setStartupVolumeLocal(Math.max(0, Math.min(100, Math.round(cfg.StartupVolume))));
          }
        }
        apiLoadedRef.current = true;
      })
      .catch((err: unknown) => {
        // Endpoint is absent in production – this is expected and not an error.
        // Log unexpected failures during development to aid debugging.
        if (process.env.NODE_ENV === 'development') {
          console.warn('[SettingsDrawer] Could not load user config from dev API:', err);
        }
        apiLoadedRef.current = true;
      });
  }, []); // useState setters are stable references; this effect intentionally runs once on mount

  // When settings change: persist them back to CurrentUserConfig.json via the
  // dev-server API (debounced to avoid spamming on slider drags).
  // Only runs in development; the endpoint is absent in production.
  useEffect(() => {
    if (!apiLoadedRef.current) return; // skip until initial load is complete
    if (process.env.NODE_ENV !== 'development') return;
    const timer = setTimeout(() => {
      fetch('/api/user-config', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          ThemeMode:       themeMode,
          BrandColor:      brandColor,
          ByodAutoSwitch:  byodAutoSwitchFb,
          ByodAutoPowerOn: byodAutoPowerOnFb,
          StartupVolume:   startupVolume,
          ClockFormat:     clockFormat,
          TempUnit:        tempUnit,
        }),
      }).catch((err: unknown) => {
        if (process.env.NODE_ENV === 'development') {
          console.warn('[SettingsDrawer] Could not save user config to dev API:', err);
        }
      });
    }, 500);
    return () => clearTimeout(timer);
  }, [themeMode, brandColor, byodAutoSwitchFb, byodAutoPowerOnFb, startupVolume, clockFormat, tempUnit]);

  /* ── Apply theme to document root ──────────────────────────────────────── */
  useEffect(() => {
    const root = document.documentElement;
    root.setAttribute('data-theme', themeMode);
    root.style.setProperty('--accent', brandColor);
    // --accent-hover intentionally mirrors --accent; the chosen brand colour
    // is used as-is for hover states to preserve full saturation.
    root.style.setProperty('--accent-hover', brandColor);
    root.style.setProperty('--accent-glow', `${brandColor}40`);
    root.style.setProperty('--border-accent', `${brandColor}80`);
  }, [themeMode, brandColor]);

  /* ── Lock admin section when drawer closes ──────────────────────────────── */
  useEffect(() => {
    if (!isOpen) {
      setAdminUnlocked(false);
      setRebootConfirm(false);
      setResetConfirm(false);
      setAdminStatus(null);
    }
  }, [isOpen]);

  /* ── Keyboard close ─────────────────────────────────────────────────────── */
  useEffect(() => {
    if (!isOpen) return;
    const handler = (e: KeyboardEvent) => {
      if (e.key === 'Escape') onClose();
    };
    window.addEventListener('keydown', handler);
    return () => window.removeEventListener('keydown', handler);
  }, [isOpen, onClose]);

  const handleAdminUnlock = useCallback(() => {
    setShowPinModal(false);
    setAdminUnlocked(true);
  }, []);

  const handleReboot = useCallback(() => {
    if (!rebootConfirm) { setRebootConfirm(true); return; }
    // Placeholder: in a real deployment this would pulse a Crestron digital join
    setRebootConfirm(false);
    setAdminStatus('Reboot command sent to touch panel.');
  }, [rebootConfirm]);

  const handleReset = useCallback(() => {
    if (!resetConfirm) { setResetConfirm(true); return; }
    sendResetBtn();
    setResetConfirm(false);
    setAdminStatus('Settings reset to defaults.');
  }, [resetConfirm, sendResetBtn]);

  return (
    <>
      {/* ── Backdrop ──────────────────────────────────────────────────────── */}
      <div
        className={`settings-backdrop${isOpen ? ' settings-backdrop--visible' : ''}`}
        onClick={onClose}
        aria-hidden="true"
      />

      {/* ── Drawer panel ──────────────────────────────────────────────────── */}
      <aside
        className={`settings-drawer${isOpen ? ' settings-drawer--open' : ''}`}
        aria-label="Settings"
        aria-hidden={!isOpen}
        data-testid="settings-drawer"
      >
        {/* Header */}
        <div className="settings-drawer__header">
          <h2 className="settings-drawer__title">Settings</h2>
          <button
            className="settings-drawer__close"
            onClick={onClose}
            aria-label="Close settings"
          >
            <X size={20} aria-hidden="true" />
          </button>
        </div>

        <div className="settings-drawer__body">

          {/* ══ 1. Theme Selections ══════════════════════════════════════════ */}
          <section className="settings-section" aria-labelledby="settings-theme-title">
            <SectionHeader icon={<Palette size={16} />} title="Theme" />
            <div id="settings-theme-title" className="sr-only">Theme Settings</div>

            <div className="settings-theme-modes" role="group" aria-label="Color theme">
              {([
                { id: 'dark',          label: 'Dark',          icon: <Moon size={18} /> },
                { id: 'light',         label: 'Light',         icon: <Sun  size={18} /> },
                { id: 'high-contrast', label: 'High Contrast', icon: <Contrast size={18} /> },
              ] as { id: ThemeMode; label: string; icon: React.ReactNode }[]).map(t => (
                <button
                  key={t.id}
                  className={`settings-theme-btn${themeMode === t.id ? ' settings-theme-btn--active' : ''}`}
                  onClick={() => setThemeMode(t.id)}
                  aria-pressed={themeMode === t.id}
                  aria-label={`${t.label} theme`}
                >
                  <span aria-hidden="true">{t.icon}</span>
                  <span>{t.label}</span>
                </button>
              ))}
            </div>

            <div className="settings-color-row">
              <label className="settings-color-row__label" htmlFor="brand-color-picker">
                Brand Color
              </label>
              <div className="settings-color-row__right">
                <span className="settings-color-row__hex">{brandColor}</span>
                <input
                  id="brand-color-picker"
                  type="color"
                  value={brandColor}
                  className="settings-color-picker"
                  aria-label="Brand color picker"
                  onChange={e => setBrandColor(e.target.value)}
                />
              </div>
            </div>
          </section>

          <div className="settings-divider" aria-hidden="true" />

          {/* ══ 2. Diagnostics ══════════════════════════════════════════════ */}
          <section className="settings-section" aria-labelledby="settings-diag-title">
            <SectionHeader icon={<Network size={16} />} title="Diagnostics" />
            <div id="settings-diag-title" className="sr-only">Diagnostics</div>

            {/* Connection status */}
            <div className="settings-diag-row">
              <span className="settings-diag-row__icon" aria-hidden="true">
                {ch5Connected ? <Wifi size={15} /> : <WifiOff size={15} />}
              </span>
              <span className="settings-diag-row__label">CH5 Websocket</span>
              <span className={`settings-diag-row__badge${ch5Connected ? ' settings-diag-row__badge--ok' : ' settings-diag-row__badge--err'}`}>
                {ch5Connected ? 'Connected' : 'Disconnected'}
              </span>
            </div>

            {/* Network info */}
            <div className="settings-diag-group">
              <span className="settings-diag-group__title">Network Info</span>
              {[
                { label: 'Room Name',        value: roomName      || '—' },
                { label: 'Panel IP',         value: networkPanelIp || '—' },
                { label: 'Subnet',           value: networkSubnet  || '—' },
                { label: 'MAC Address',      value: networkMac     || '—' },
                { label: 'Control System IP',value: networkCsIp    || '—' },
              ].map(row => (
                <div key={row.label} className="settings-diag-kv">
                  <span className="settings-diag-kv__key">{row.label}</span>
                  <span className="settings-diag-kv__val">{row.value}</span>
                </div>
              ))}
            </div>

            {/* Hardware status */}
            <div className="settings-diag-group">
              <span className="settings-diag-group__title">Hardware Status</span>
              {[
                { icon: <Monitor size={14} />, label: 'Displays',    status: 'Online' },
                { icon: <Mic     size={14} />, label: 'Microphones', status: 'Online' },
                { icon: <Cpu     size={14} />, label: 'Room PC',     status: 'Online' },
              ].map(hw => (
                <div key={hw.label} className="settings-diag-hw">
                  <span className="settings-diag-hw__icon" aria-hidden="true">{hw.icon}</span>
                  <span className="settings-diag-hw__label">{hw.label}</span>
                  <span className="settings-diag-hw__badge settings-diag-hw__badge--ok">{hw.status}</span>
                </div>
              ))}
            </div>
          </section>

          <div className="settings-divider" aria-hidden="true" />

          {/* ══ 3. BYOD Behavior ════════════════════════════════════════════ */}
          <section className="settings-section" aria-labelledby="settings-byod-title">
            <SectionHeader icon={<Usb size={16} />} title="BYOD Behavior" />
            <div id="settings-byod-title" className="sr-only">BYOD Behavior</div>

            <ToggleRow
              id="byod-auto-switch"
              label="Auto-Switch on Connect"
              description="Automatically route video when a BYOD device is plugged in"
              checked={byodAutoSwitchFb}
              onChange={sendByodAutoSwitchToggle}
            />
            <ToggleRow
              id="byod-auto-power"
              label="Auto-Power On"
              description="Wake up the room (displays + audio) when a BYOD device connects"
              checked={byodAutoPowerOnFb}
              onChange={sendByodAutoPowerToggle}
            />
          </section>

          <div className="settings-divider" aria-hidden="true" />

          {/* ══ 4. Display & Audio Preferences ══════════════════════════════ */}
          <section className="settings-section" aria-labelledby="settings-disp-title">
            <SectionHeader icon={<SlidersHorizontal size={16} />} title="Display & Audio Preferences" />
            <div id="settings-disp-title" className="sr-only">Display and Audio Preferences</div>

            <SliderRow
              id="panel-brightness"
              label="Panel Brightness"
              value={panelBrightness}
              onChange={handleBrightnessChange}
            />
            <SliderRow
              id="startup-volume"
              label="Default Startup Volume"
              value={startupVolume}
              onChange={setStartupVolume}
            />
          </section>

          <div className="settings-divider" aria-hidden="true" />

          {/* ══ 5. Help & Support ═══════════════════════════════════════════ */}
          <section className="settings-section" aria-labelledby="settings-help-title">
            <SectionHeader icon={<Phone size={16} />} title="Help & Support" />
            <div id="settings-help-title" className="sr-only">Help and Support</div>

            <div className="settings-help-grid">
              <div className="settings-diag-group settings-help-info">
                {[
                  { label: 'Room Name / ID', value: roomName || '—' },
                  { label: 'IT Helpdesk',    value: helpItPhone      || '—' },
                  { label: 'Support Email',  value: helpSupportEmail || '—' },
                ].map(row => (
                  <div key={row.label} className="settings-diag-kv">
                    <span className="settings-diag-kv__key">{row.label}</span>
                    <span className="settings-diag-kv__val">{row.value}</span>
                  </div>
                ))}
              </div>

              <div className="settings-qr-placeholder" aria-label="QR code for support ticket or quick start guide">
                <QrCode size={64} aria-hidden="true" />
                <span className="settings-qr-placeholder__label">{helpQrLabel || 'Scan for Quick Start Guide'}</span>
              </div>
            </div>
          </section>

          <div className="settings-divider" aria-hidden="true" />

          {/* ══ 6. Environment & Localization ═══════════════════════════════ */}
          <section className="settings-section" aria-labelledby="settings-locale-title">
            <SectionHeader icon={<Clock size={16} />} title="Environment & Localization" />
            <div id="settings-locale-title" className="sr-only">Environment and Localization</div>

            {/* Clock format */}
            <div className="settings-segment-row">
              <span className="settings-segment-row__label">Clock Format</span>
              <div className="settings-segment" role="group" aria-label="Clock format">
                {(['12h', '24h'] as ClockFormat[]).map(f => (
                  <button
                    key={f}
                    className={`settings-segment__btn${clockFormat === f ? ' settings-segment__btn--active' : ''}`}
                    onClick={() => setClockFormat(f)}
                    aria-pressed={clockFormat === f}
                    aria-label={`${f} clock format`}
                  >
                    {f}
                  </button>
                ))}
              </div>
            </div>

            {/* Temperature units */}
            <div className="settings-segment-row">
              <span className="settings-segment-row__label">
                <Thermometer size={14} aria-hidden="true" /> Temperature
              </span>
              <div className="settings-segment" role="group" aria-label="Temperature unit">
                {(['F', 'C'] as TempUnit[]).map(u => (
                  <button
                    key={u}
                    className={`settings-segment__btn${tempUnit === u ? ' settings-segment__btn--active' : ''}`}
                    onClick={() => setTempUnit(u)}
                    aria-pressed={tempUnit === u}
                    aria-label={`Degrees ${u === 'F' ? 'Fahrenheit' : 'Celsius'}`}
                  >
                    °{u}
                  </button>
                ))}
              </div>
            </div>
          </section>

          <div className="settings-divider" aria-hidden="true" />

          {/* ══ 7. Advanced / Admin ══════════════════════════════════════════ */}
          <section className="settings-section" aria-labelledby="settings-admin-title">
            <SectionHeader icon={<ShieldAlert size={16} />} title="Advanced / Admin" />
            <div id="settings-admin-title" className="sr-only">Advanced and Admin</div>

            {!adminUnlocked ? (
              <button
                className="settings-admin-lock-btn"
                onClick={() => setShowPinModal(true)}
                aria-label="Unlock admin area – requires PIN"
              >
                <Lock size={16} aria-hidden="true" />
                <span>Unlock Admin Area</span>
                <ChevronRight size={16} aria-hidden="true" className="settings-admin-lock-btn__chevron" />
              </button>
            ) : (
              <div className="settings-admin-unlocked" role="group" aria-label="Admin controls">
                <div className="settings-admin-unlocked__badge" aria-live="polite">
                  <ShieldAlert size={14} aria-hidden="true" /> Admin Unlocked
                </div>

                {adminStatus && (
                  <p className="settings-admin-status" role="status" aria-live="polite">
                    {adminStatus}
                  </p>
                )}

                <button
                  className={`settings-admin-action${rebootConfirm ? ' settings-admin-action--confirm' : ''}`}
                  onClick={handleReboot}
                  aria-label={rebootConfirm ? 'Confirm reboot touch panel' : 'Reboot touch panel'}
                >
                  <Power size={16} aria-hidden="true" />
                  <span>{rebootConfirm ? 'Confirm Reboot?' : 'Reboot Touch Panel'}</span>
                </button>

                <button
                  className={`settings-admin-action${resetConfirm ? ' settings-admin-action--confirm' : ''}`}
                  onClick={handleReset}
                  aria-label={resetConfirm ? 'Confirm reset UI' : 'Reset UI'}
                >
                  <RefreshCw size={16} aria-hidden="true" />
                  <span>{resetConfirm ? 'Confirm Reset?' : 'Reset UI'}</span>
                </button>

                <button
                  className="settings-admin-action settings-admin-action--lock"
                  onClick={() => { setAdminUnlocked(false); setRebootConfirm(false); setResetConfirm(false); setAdminStatus(null); }}
                  aria-label="Lock admin area"
                >
                  <Lock size={16} aria-hidden="true" />
                  <span>Lock Admin Area</span>
                </button>
              </div>
            )}
          </section>

        </div>{/* /body */}

        {/* Zap icon watermark */}
        <div className="settings-drawer__footer" aria-hidden="true">
          <Zap size={12} />
          <span>Executive AV · CH5 Platform</span>
        </div>
      </aside>

      {/* PIN Modal */}
      {showPinModal && (
        <PinModal
          onSuccess={handleAdminUnlock}
          onCancel={() => setShowPinModal(false)}
        />
      )}
    </>
  );
};

export default SettingsDrawer;
