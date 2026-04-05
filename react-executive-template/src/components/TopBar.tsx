import React, { useState, useRef, useCallback, useEffect } from 'react';
import { Building2, Phone, HelpCircle, X, Phone as PhoneIcon, Mail, MessageSquare, Monitor, Wifi, Volume2 } from 'lucide-react';
import { Joins, useSerialJoin, useDigitalJoin } from '../hooks/useCrestron';

interface TopBarProps {
  /** Page-specific title displayed in the centre of the bar. */
  title: string;
}

/**
 * Converts a time string received from the control system to the requested
 * display format.
 *
 * @param raw         - Raw time string from the processor, expected as
 *                      "HH:MM", "HH:MM:SS" (24-hour), or "H:MM AM/PM".
 * @param clockFormat - Desired output format: "12h" or "24h".
 * @returns The formatted time string, or `raw` unchanged if it cannot be
 *          parsed (so the original value is always preserved as a fallback).
 */
function formatTime(raw: string, clockFormat: string): string {
  if (!raw) return raw;
  const match = raw.match(/^(\d{1,2}):(\d{2})(:\d{2})?(\s*[AaPp][Mm])?$/);
  if (!match) return raw;

  let hours = parseInt(match[1], 10);
  const minutes = match[2];
  const suffix = (match[4] || '').trim().toUpperCase();

  // Normalize to 24-hour value first
  if (suffix === 'PM' && hours !== 12) hours += 12;
  if (suffix === 'AM' && hours === 12) hours = 0;

  if (clockFormat === '24h') {
    return `${String(hours).padStart(2, '0')}:${minutes}`;
  }

  // 12-hour format
  const period = hours >= 12 ? 'PM' : 'AM';
  const h12 = hours % 12 || 12;
  return `${h12}:${minutes} ${period}`;
}

/** AV Help modal — shown when the AV Help button is pressed. */
const AVHelpModal: React.FC<{ onClose: () => void; itPhone: string; supportEmail: string }> = ({ onClose, itPhone, supportEmail }) => {
  const backdropRef = useRef<HTMLDivElement>(null);

  const handleBackdropClick = useCallback((e: React.MouseEvent<HTMLDivElement>) => {
    if (e.target === backdropRef.current) {
      onClose();
    }
  }, [onClose]);

  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Escape') {
        onClose();
      }
    };
    document.addEventListener('keydown', handleKeyDown);
    return () => document.removeEventListener('keydown', handleKeyDown);
  }, [onClose]);

  return (
    <div
      className="av-help-backdrop"
      ref={backdropRef}
      onClick={handleBackdropClick}
    >
      <div
        className="av-help-modal"
        role="dialog"
        aria-modal="true"
        aria-label="AV Help and Support"
      >
        {/* Header */}
        <div className="av-help-modal__header">
          <div className="av-help-modal__title-group">
            <HelpCircle size={20} aria-hidden="true" />
            <h2 className="av-help-modal__title">AV Help &amp; Support</h2>
          </div>
          <button
            className="av-help-modal__close-btn"
            onClick={onClose}
            aria-label="Close AV Help"
          >
            <X size={18} aria-hidden="true" />
          </button>
        </div>

        {/* Body */}
        <div className="av-help-modal__body">
          {/* Quick tips */}
          <section className="av-help-modal__section">
            <h3 className="av-help-modal__section-title">Quick Troubleshooting</h3>
            <ul className="av-help-modal__tip-list">
              <li className="av-help-modal__tip">
                <Monitor size={15} aria-hidden="true" />
                <span>No display? Check that the source is powered on and the correct input is selected.</span>
              </li>
              <li className="av-help-modal__tip">
                <Volume2 size={15} aria-hidden="true" />
                <span>No audio? Verify the volume is not muted and the correct source is routed.</span>
              </li>
              <li className="av-help-modal__tip">
                <Wifi size={15} aria-hidden="true" />
                <span>Network issues? Ensure the room PC is connected to the corporate network.</span>
              </li>
              <li className="av-help-modal__tip">
                <MessageSquare size={15} aria-hidden="true" />
                <span>For Teams or BYOD calls, use the dedicated Teams/BYOD buttons in the Video tab.</span>
              </li>
            </ul>
          </section>

          {/* Contact */}
          <section className="av-help-modal__section">
            <h3 className="av-help-modal__section-title">Contact AV Support</h3>
            <div className="av-help-modal__contact-list">
              <a className="av-help-modal__contact-item" href={`tel:${itPhone || '+15550100'}`}>
                <PhoneIcon size={15} aria-hidden="true" />
                <span>{itPhone || '+1 (555) 010-0100'}</span>
              </a>
              <a className="av-help-modal__contact-item" href={`mailto:${supportEmail || 'av-support@example.com'}`}>
                <Mail size={15} aria-hidden="true" />
                <span>{supportEmail || 'av-support@example.com'}</span>
              </a>
            </div>
          </section>
        </div>

        {/* Footer */}
        <div className="av-help-modal__footer">
          <button className="av-help-modal__dismiss-btn" onClick={onClose}>
            Close
          </button>
        </div>
      </div>
    </div>
  );
};

/**
 * TopBar
 *
 * Persistent executive top bar rendered on every page.
 * - Left:   current time (large) and date from serial joins.
 * - Centre: current page title and label from serial joins.
 * - Right:  room name / number and an AV Help button.
 */
const TopBar: React.FC<TopBarProps> = ({ title }) => {
  const date        = useSerialJoin(Joins.DATE_SERIAL);
  const timeRaw     = useSerialJoin(Joins.TIME_SERIAL);
  const label       = useSerialJoin(Joins.LABEL_SERIAL);
  const roomName    = useSerialJoin(Joins.ROOM_NAME_SERIAL);
  const roomNumber  = useSerialJoin(Joins.ROOM_NUMBER_SERIAL);
  const inMeeting   = useDigitalJoin(Joins.TEAMS_MODE_FB);
  const byodActive  = useDigitalJoin(Joins.BYOD_MODE_FB);
  const clockFormat = useSerialJoin(Joins.SETTINGS_CLOCK_FORMAT);
  const itPhone     = useSerialJoin(Joins.HELP_IT_PHONE_SERIAL);
  const supportEmail = useSerialJoin(Joins.HELP_SUPPORT_EMAIL_SERIAL);

  // Apply clock format preference to the raw time string from the processor
  const time = formatTime(timeRaw, clockFormat || '12h');

  const [showHelp, setShowHelp] = useState(false);

  return (
    <>
      <header className="exec-topbar">
        {/* Left — time / date */}
        <div className="exec-topbar__left">
          <span className="exec-topbar__time">{time || '00:00'}</span>
          <span className="exec-topbar__date">{date || 'Today'}</span>
        </div>

        {/* Centre — page title */}
        <div className="exec-topbar__center">
          {label && <span className="exec-topbar__label">{label}</span>}
          <h1 className="exec-topbar__title">{title}</h1>
          {(inMeeting || byodActive) && (
            <span className={`exec-topbar__badge${byodActive ? ' exec-topbar__badge--byod' : ' exec-topbar__badge--meeting'}`}>
              {byodActive ? 'BYOD Active' : 'In Meeting'}
            </span>
          )}
        </div>

        {/* Right — room info + help */}
        <div className="exec-topbar__right">
          <div className="exec-topbar__room">
            <span aria-label={`Room: ${roomName || 'Boardroom'}`}>
              <Building2 size={13} aria-hidden="true" />
              {roomName || 'Boardroom'}
            </span>
            <span aria-label={`Extension: ${roomNumber || 'Ext'}`}>
              <Phone size={13} aria-hidden="true" />
              {roomNumber || 'Ext'}
            </span>
          </div>
          <button
            className="exec-topbar__help-btn"
            aria-label="AV Help and Support"
            onClick={() => setShowHelp(true)}
          >
            <HelpCircle size={16} aria-hidden="true" />
            <span>AV Help</span>
          </button>
        </div>
      </header>

      {showHelp && <AVHelpModal onClose={() => setShowHelp(false)} itPhone={itPhone} supportEmail={supportEmail} />}
    </>
  );
};

export default TopBar;
