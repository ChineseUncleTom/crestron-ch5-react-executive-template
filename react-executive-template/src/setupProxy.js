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

module.exports = function (app) {
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
