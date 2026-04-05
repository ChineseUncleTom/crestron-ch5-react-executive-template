// Development-server proxy for the Crestron WebXPanel token endpoint.
//
// During local development the React app is served from http://localhost:3000
// while the Crestron processor (or VC-4) runs over HTTPS on a different origin.
// Browsers block those cross-origin requests (CORS).  CRA's Express dev server
// forwards every /cws/ request through this proxy so they appear same-origin.
//
// Set REACT_APP_XPANEL_HOST in your .env file (or shell environment) to point
// at your processor.  It defaults to "localhost" when unset.
//
// Reference: https://create-react-app.dev/docs/proxying-api-requests-in-development/

const { createProxyMiddleware } = require('http-proxy-middleware');
const path = require('path');
const fs   = require('fs');

// Path to the persisted user settings file (relative to this file's location).
const CONFIG_PATH = path.join(
  __dirname, '..', '..', 'ExecutiveControlSystem', 'Nvram', 'CurrentUserConfig.json',
);

module.exports = function (app) {
  // ── Local settings API ──────────────────────────────────────────────────────
  // These two routes let the React settings drawer read and write
  // CurrentUserConfig.json during development so settings survive a page
  // refresh even when no Crestron processor is connected.
  // In production the routes are absent; the Crestron processor handles
  // persistence via CH5 joins and UserConfig.cs.

  // GET /api/user-config – return the current user configuration as JSON.
  app.get('/api/user-config', (_req, res) => {
    try {
      const raw = fs.readFileSync(CONFIG_PATH, 'utf8');
      res.setHeader('Content-Type', 'application/json');
      res.end(raw);
    } catch (_e) {
      res.statusCode = 404;
      res.setHeader('Content-Type', 'application/json');
      res.end(JSON.stringify({ error: 'CurrentUserConfig.json not found' }));
    }
  });

  // POST /api/user-config – persist an updated user configuration to the file.
  app.post('/api/user-config', (req, res) => {
    const MAX_BODY_BYTES = 4096; // generous limit for a small settings object
    let body = '';
    let overflow = false;

    req.on('data', chunk => {
      if (overflow) return;
      body += chunk;
      if (Buffer.byteLength(body, 'utf8') > MAX_BODY_BYTES) {
        overflow = true;
        res.statusCode = 413;
        res.setHeader('Content-Type', 'application/json');
        res.end(JSON.stringify({ error: 'Request body too large' }));
      }
    });
    req.on('end', () => {
      if (overflow) return;
      try {
        const data = JSON.parse(body);

        // Validate that the payload matches the UserConfigData schema before
        // writing to disk.  Unexpected keys are ignored; missing keys retain
        // their current value in the file.
        const THEME_MODES  = ['dark', 'light', 'high-contrast'];
        const CLOCK_FMTS   = ['12h', '24h'];
        const TEMP_UNITS   = ['F', 'C'];

        if (
          ('ThemeMode'      in data && !THEME_MODES.includes(data.ThemeMode))     ||
          ('BrandColor'     in data && (typeof data.BrandColor !== 'string'
                                        || !/^#[0-9a-fA-F]{6}$/.test(data.BrandColor))) ||
          ('ByodAutoSwitch' in data && typeof data.ByodAutoSwitch  !== 'boolean') ||
          ('ByodAutoPowerOn' in data && typeof data.ByodAutoPowerOn !== 'boolean') ||
          ('StartupVolume'  in data && (typeof data.StartupVolume  !== 'number'
                                        || data.StartupVolume < 0
                                        || data.StartupVolume > 100))             ||
          ('ClockFormat'    in data && !CLOCK_FMTS.includes(data.ClockFormat))    ||
          ('TempUnit'       in data && !TEMP_UNITS.includes(data.TempUnit))
        ) {
          res.statusCode = 400;
          res.setHeader('Content-Type', 'application/json');
          res.end(JSON.stringify({ error: 'Invalid settings value' }));
          return;
        }

        // Merge validated fields with the existing file so unknown keys are
        // preserved and unset fields fall back to their saved value.
        let existing = {};
        try {
          existing = JSON.parse(fs.readFileSync(CONFIG_PATH, 'utf8'));
        } catch (readErr) {
          if (readErr.code !== 'ENOENT') {
            console.warn('[api/user-config] Could not read existing config:', readErr.message);
          }
        }
        const merged = { ...existing, ...data };

        fs.writeFileSync(CONFIG_PATH, JSON.stringify(merged, null, 4), 'utf8');
        res.setHeader('Content-Type', 'application/json');
        res.end(JSON.stringify({ ok: true }));
      } catch (e) {
        res.statusCode = 500;
        res.setHeader('Content-Type', 'application/json');
        res.end(JSON.stringify({ error: String(e) }));
      }
    });
  });

  // ── Crestron WebXPanel proxy ────────────────────────────────────────────────
  const host   = process.env.REACT_APP_XPANEL_HOST || 'localhost';
  const target = `https://${host}`;

  app.use(
    '/cws',
    createProxyMiddleware({
      target,
      changeOrigin: true, // rewrite the Host header to match the target
      secure: false,      // accept self-signed TLS certificates (common on Crestron processors)
      ws: true,           // also proxy WebSocket upgrade requests
    }),
  );
};
