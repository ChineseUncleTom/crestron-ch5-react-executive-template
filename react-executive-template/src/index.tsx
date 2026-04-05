import React from 'react';
import ReactDOM from 'react-dom/client';
// WebXPanel must be the first Crestron library initialized.
// getWebXPanel(isBrowser) returns the panel object and isActive flag.
// isActive is true only when the page is loaded inside a Crestron XPanel
// (e.g. VC-4, Virtual Control, or a TSW running in browser mode).
import getWebXPanel from '@crestron/ch5-webxpanel';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';

const { WebXPanel, isActive } = getWebXPanel(typeof window !== 'undefined');

// ── XPanel configuration ──────────────────────────────────────────────────────
// The host defaults to window.location.hostname so the panel connects back to
// the same server that served the CH5 bundle (correct for on-panel deployments).
// Override host / ipId / roomId here or via build-time environment variables.
const xPanelConfig = {
  host:   process.env.REACT_APP_XPANEL_HOST    ?? window.location.hostname,
  ipId:   process.env.REACT_APP_XPANEL_IP_ID   ?? '0x03',
  roomId: process.env.REACT_APP_XPANEL_ROOM_ID ?? '001',
};

if (isActive) {
  WebXPanel.initialize(xPanelConfig);
  // Broadcast WebXPanel connection state changes as window-level CustomEvents
  // so that the useWebXPanelOnline hook in useCrestron.ts can subscribe
  // without needing a second import of this package.
  WebXPanel.addEventListener('connect',    () => window.dispatchEvent(new CustomEvent('ch5:connect')));
  WebXPanel.addEventListener('disconnect', () => window.dispatchEvent(new CustomEvent('ch5:disconnect')));
}

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
