# crestron-ch5-react-fullstack-starter

Full Stack Crestron AV: C# SIMPL# Pro + CH5 React TypeScript

A complete starter project that connects a **Crestron control system** (SIMPL# Pro C#) to a **CH5 touch-panel UI** built with React + TypeScript.

---

## Repository Layout

```
crestron-ch5-react-fullstack-starter/
│
├── ConstrolSystemTemplate/          – C# SIMPL# Pro control system (XPanel / browser panel)
│   ├── ControlSystem.cs             – Entry point: IP-ID setup, XPanel registration, join routing
│   ├── RoomController.cs            – Room logic (lights, volume, source selection)
│   └── JoinMap.cs                   – Centralised join number constants
│
├── react-template/                  – Panel UI: Create-React-App (CRA) + TypeScript
│   ├── package.json                 – React 19, CrComLib 2.17.2, CH5 CLI dependencies
│   └── src/
│       ├── index.tsx                – Entry point: WebXPanel init before React renders
│       ├── App.tsx                  – Root component wiring everything together
│       ├── setupProxy.js            – CRA dev-server proxy for /cws/ (CORS fix)
│       ├── hooks/
│       │   └── useCrestron.ts       – CrComLib hooks (digital / analog / serial)
│       ├── components/
│       │   ├── LightButton.tsx      – Digital join on/off button with feedback
│       │   ├── VolumeSlider.tsx     – Analog join range slider with feedback
│       │   └── SourceLabel.tsx      – Serial join read-only label
│       └── __mocks__/
│           ├── crcomlib.ts          – CrComLib mock for Jest unit tests
│           └── webxpanel.ts         – WebXPanel mock for Jest unit tests
│
├── vite-template/                   – Panel UI: Vite + React 18 + TypeScript (recommended)
│   ├── package.json                 – Vite 5, React 18.2, CrComLib 2.17.2
│   ├── vite.config.ts               – Vite dev-server proxy + CrComLib resolve alias
│   └── src/
│       ├── main.tsx                 – Entry point: WebXPanel init before React renders
│       ├── App.tsx                  – Root component (identical structure to react-template)
│       ├── hooks/
│       │   └── useCrestron.ts       – CrComLib hooks (identical to react-template)
│       └── components/
│           ├── LightButton.tsx
│           ├── VolumeSlider.tsx
│           └── SourceLabel.tsx
│
├── .gitignore
└── README.md
```

> **Which template should I use?**
> - **`react-template/`** (CRA) — simplest setup, best for learning; uses `npm start` / `npm run build`.
> - **`vite-template/`** (Vite) — faster builds and hot-reload, recommended for production projects; uses `npm run dev` / `npm run build`.

---

## Prerequisites

| Tool | Version |
|------|---------|
| Crestron SIMPL# Pro SDK | 2.x or later |
| Visual Studio 2019/2022 | — |
| Node.js | 18 LTS or later |
| npm | 9+ |
| Crestron CH5 CLI | included via `package.json` |

---

## Control System – C# Setup

### 1. Open the SIMPL# Pro project

Open `ConstrolSystemTemplate/ConstrolSystemTemplate.slnx` in **Visual Studio**.  The solution already contains all three source files:

- `ControlSystem.cs` — registers an `XpanelForHtml5` device at IP-ID `0x03` and wires join callbacks
- `RoomController.cs` — encapsulates room logic (lights, volume, source)
- `JoinMap.cs` — join number constants shared with the React frontend

> If you prefer to target a **physical Tsw1060 panel** instead of an XPanel / browser panel, use the alternative template in `startupDay0/control-system/` which replaces `XpanelForHtml5` with a `Tsw1060` device object.

### 2. Configure the IP-ID

In `ControlSystem.cs`, locate the constant and change it to match the IP-ID configured on your panel or XPanel device:

```csharp
private const uint PANEL_IPID = 0x03; // ← change to your panel's IP-ID
```

### 3. Build & deploy

Build the project in Visual Studio and deploy to your Crestron processor using **Crestron Toolbox** or the **SSH / FTP** workflow you prefer.

---

## Panel UI – React Setup

Two ready-to-use React templates are included.  Both implement identical UI components and join logic; the only difference is the build toolchain.

### Option A — `react-template/` (Create-React-App)

```bash
cd react-template
npm install
```

| Task | Command | Notes |
|------|---------|-------|
| Dev server | `npm start` | Hot-reload at `http://localhost:3000` |
| Production build | `npm run build` | Output → `react-template/build/` |
| Package for CH5 | `npm run build:ch5` | Creates `.ch5z` archive in `react-template/dist/` |
| Run tests | `npm test` | Jest + Testing Library; mocks CrComLib/WebXPanel |

### Option B — `vite-template/` (Vite) *(recommended)*

```bash
cd vite-template
npm install
```

| Task | Command | Notes |
|------|---------|-------|
| Dev server | `npm run dev` | Hot-reload at `http://localhost:5173` |
| Production build | `npm run build` | TypeScript compile + Vite bundle → `dist/` |
| Package for CH5 | `npm run build:ch5` | Builds then creates a `.ch5z` archive in `vite-template/dist/` |

### Deploy to the panel

Upload the `.ch5z` file via **Crestron Toolbox** (Tool → CH5 → Load Project) or via the panel's web configuration interface.

---

## Join Map Reference

Join numbers are defined in `ConstrolSystemTemplate/JoinMap.cs` and mirrored as constants in each template's `src/hooks/useCrestron.ts`.

| Join Type | Number | Signal Name   | Direction          | Description                       |
|-----------|--------|---------------|--------------------|-----------------------------------|
| Digital   | 1      | `LIGHT_ON`    | Panel → Processor  | Press to turn lights on           |
| Digital   | 2      | `LIGHT_OFF`   | Panel → Processor  | Press to turn lights off          |
| Digital   | 3      | `LIGHT_IS_ON` | Processor → Panel  | Feedback – lights are currently on|
| Analog    | 1      | `VOLUME_SET`  | Panel → Processor  | Set volume level (0 – 65 535)     |
| Analog    | 2      | `VOLUME_FB`   | Processor → Panel  | Volume feedback from processor    |
| Serial    | 1      | `SOURCE_NAME` | Processor → Panel  | Currently selected source name    |

---

## XPanel (Virtual Panel) Support

The UI can be accessed from a Crestron XPanel (Virtual Control, VC-4, or any browser-based panel) using the `@crestron/ch5-webxpanel` library.  WebXPanel is initialized **before** React renders so that the Crestron transport layer is established as the very first thing the page does.

### How it works

`src/index.tsx` (CRA) / `src/main.tsx` (Vite) call `WebXPanel.initialize()` with the connection parameters before `ReactDOM.createRoot()`:

```typescript
import getWebXPanel from '@crestron/ch5-webxpanel';

const { WebXPanel, isActive } = getWebXPanel(typeof window !== 'undefined');

const xPanelConfig = {
  host:   'ip_address | hostname', // Crestron processor IP (defaults to window.location.hostname)
  ipId:   '0x03',                  // IP-ID set on the XPanel device (hex string)
  roomId: '',                      // Virtual Control room ID (leave empty for physical panels)
};

if (isActive) {
  WebXPanel.initialize(xPanelConfig);
}
```

`isActive` is `true` only when the page is served inside a real XPanel context, so ordinary browser development is unaffected.

### Configuration via environment variables

| Variable | CRA (`react-template`) | Vite (`panel-ui`) | Default |
|---|---|---|---|
| Processor host | `REACT_APP_XPANEL_HOST` | `VITE_XPANEL_HOST` | `window.location.hostname` |
| IP-ID | `REACT_APP_XPANEL_IP_ID` | `VITE_XPANEL_IP_ID` | `0x03` |
| Room ID | `REACT_APP_XPANEL_ROOM_ID` | `VITE_XPANEL_ROOM_ID` | *(empty)* |

Create a `.env` file in the relevant template root to override defaults at build time:

**`react-template/.env`**
```env
REACT_APP_XPANEL_HOST=192.168.1.100
REACT_APP_XPANEL_IP_ID=0x03
REACT_APP_XPANEL_ROOM_ID=
```

**`vite-template/.env`**
```env
VITE_XPANEL_HOST=192.168.1.100
VITE_XPANEL_IP_ID=0x03
VITE_XPANEL_ROOM_ID=
```

### Development-server proxy (CORS fix)

During local development the React app is served from a different origin (`http://localhost:3000` for CRA, `http://localhost:5173` for Vite) than the Crestron processor (`https://<host>`).  Browsers block those cross-origin requests with a CORS error like:

```
Access to fetch at 'https://localhost/cws/websocket/getWebSocketToken' from origin 'http://localhost:3000'
has been blocked by CORS policy: No 'Access-Control-Allow-Origin' header is present on the requested resource.
```

Both templates include a development-server proxy that forwards every `/cws/` request to the processor so the browser sees it as same-origin:

| Template | Proxy configuration |
|---|---|
| CRA (`react-template`) | `src/setupProxy.js` — read by CRA's Express dev server automatically |
| Vite (`startupDay0/panel-ui`) | `server.proxy` block in `vite.config.ts` |

The target host is taken from the same environment variable used for `WebXPanel.initialize()`:

| Template | Variable | Default |
|---|---|---|
| CRA | `REACT_APP_XPANEL_HOST` | `localhost` |
| Vite | `VITE_XPANEL_HOST` | `localhost` |

The proxy also sets `secure: false` so the dev server accepts the self-signed TLS certificate that Crestron processors ship with by default.

---

## Architecture Overview

```
┌─────────────────────────────────┐         ┌──────────────────────────────────────┐
│  Crestron Processor (C#)        │◄───────►│  Touch Panel / Browser               │
│                                 │  TCP/IP │                                      │
│  ConstrolSystemTemplate/        │  IP-ID  │  react-template/  (CRA)              │
│  └─ ControlSystem.cs            │         │  └─ src/                             │
│      └─ RoomController.cs       │         │      ├─ index.tsx  (WebXPanel init)  │
│          └─ JoinMap.cs          │         │      ├─ hooks/useCrestron.ts         │
│                                 │         │      └─ components/                  │
│                                 │         │          ├─ LightButton.tsx          │
│                                 │         │          ├─ VolumeSlider.tsx         │
│                                 │         │          └─ SourceLabel.tsx          │
│                                 │         │                                      │
│                                 │         │  vite-template/  (Vite)              │
│                                 │         │  └─ src/  (identical structure)      │
└─────────────────────────────────┘         └──────────────────────────────────────┘
```

---

## Contributing

Pull requests are welcome. Please open an issue first to discuss significant changes.

## License

MIT
