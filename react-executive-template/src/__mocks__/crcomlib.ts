// Mock for @crestron/ch5-crcomlib used in the Jest test environment.
// CrComLib requires WebSocket connections that are unavailable in jsdom,
// so we replace it with a lightweight no-op stub.
export const CrComLib = {
  subscribeState: jest.fn().mockReturnValue('mock-subscription-id'),
  unsubscribeState: jest.fn(),
  publishEvent: jest.fn(),
};

// Mirror the real useCrestron.ts behaviour: expose CrComLib globally so any
// code that accesses window.CrComLib (e.g. the WebXPanel worker callback)
// works correctly in the jsdom test environment.
(window as any).CrComLib = CrComLib;
