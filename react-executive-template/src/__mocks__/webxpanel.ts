// Mock for @crestron/ch5-webxpanel used in the Jest test environment.
// WebXPanel requires a live WebSocket / XPanel context that is unavailable in
// jsdom, so we replace it with lightweight no-op stubs.
//
// The real package exports a factory: getWebXPanel(isBrowser) → { WebXPanel, isActive, … }
// We mock the factory to return a stable no-op object.
const mockWebXPanel = {
  initialize: jest.fn(),
};

const getWebXPanel = jest.fn().mockReturnValue({
  WebXPanel: mockWebXPanel,
  // Simulate a non-XPanel browser environment (the common test scenario).
  isActive: false,
  WebXPanelConfigParams: {},
});

export default getWebXPanel;
export { getWebXPanel };
