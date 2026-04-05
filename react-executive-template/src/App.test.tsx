import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import App from './App';

describe('App – HomeView (default landing page)', () => {
  it('renders the Executive Boardroom title', () => {
    render(<App />);
    expect(screen.getByRole('heading', { name: 'Executive Boardroom' })).toBeInTheDocument();
  });

  it('renders the sidebar navigation', () => {
    render(<App />);
    expect(screen.getByRole('navigation', { name: 'Main navigation' })).toBeInTheDocument();
  });

  it('renders the Home nav item as active', () => {
    render(<App />);
    expect(screen.getByRole('button', { name: 'Home' })).toHaveAttribute('aria-current', 'page');
  });

  it('renders the scenario buttons', () => {
    render(<App />);
    expect(screen.getByRole('button', { name: 'Present to Room' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Video Conference' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'System Off' })).toBeInTheDocument();
  });

  it('renders the microphone quick-access button', () => {
    render(<App />);
    // aria-label changes based on mute state; both patterns should be findable
    expect(
      screen.getByRole('button', { name: /microphone/i })
    ).toBeInTheDocument();
  });
});

describe('App – navigation', () => {
  it('renders all four sidebar nav items', () => {
    render(<App />);
    expect(screen.getByRole('button', { name: 'Home' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Video' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Audio' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Camera' })).toBeInTheDocument();
  });

  it('renders the AV Help button in the top bar', () => {
    render(<App />);
    expect(screen.getByRole('button', { name: /av help/i })).toBeInTheDocument();
  });
});

describe('App – Settings drawer', () => {
  it('renders the Settings button in the sidebar', () => {
    render(<App />);
    expect(screen.getByRole('button', { name: 'Settings' })).toBeInTheDocument();
  });

  it('settings drawer is hidden by default', () => {
    render(<App />);
    const drawer = screen.getByTestId('settings-drawer');
    expect(drawer).toHaveAttribute('aria-hidden', 'true');
  });

  it('opens the settings drawer when Settings button is clicked', () => {
    render(<App />);
    fireEvent.click(screen.getByRole('button', { name: 'Settings' }));
    const drawer = screen.getByTestId('settings-drawer');
    expect(drawer).toHaveAttribute('aria-hidden', 'false');
  });

  it('closes the settings drawer when the close button is clicked', () => {
    render(<App />);
    fireEvent.click(screen.getByRole('button', { name: 'Settings' }));
    fireEvent.click(screen.getByRole('button', { name: 'Close settings' }));
    const drawer = screen.getByTestId('settings-drawer');
    expect(drawer).toHaveAttribute('aria-hidden', 'true');
  });

  it('renders the Theme section inside the drawer', () => {
    render(<App />);
    fireEvent.click(screen.getByRole('button', { name: 'Settings' }));
    expect(screen.getByRole('button', { name: 'Dark theme' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Light theme' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'High Contrast theme' })).toBeInTheDocument();
  });

  it('renders the BYOD toggles inside the drawer', () => {
    render(<App />);
    fireEvent.click(screen.getByRole('button', { name: 'Settings' }));
    expect(screen.getByRole('switch', { name: 'Auto-Switch on Connect' })).toBeInTheDocument();
    expect(screen.getByRole('switch', { name: 'Auto-Power On' })).toBeInTheDocument();
  });

  it('renders the panel brightness and startup volume sliders', () => {
    render(<App />);
    fireEvent.click(screen.getByRole('button', { name: 'Settings' }));
    expect(screen.getByRole('slider', { name: 'Panel Brightness' })).toBeInTheDocument();
    expect(screen.getByRole('slider', { name: 'Default Startup Volume' })).toBeInTheDocument();
  });

  it('renders the Help & Support section inside the drawer', () => {
    render(<App />);
    fireEvent.click(screen.getByRole('button', { name: 'Settings' }));
    expect(screen.getByLabelText('QR code for support ticket or quick start guide')).toBeInTheDocument();
  });

  it('renders the clock format segment control', () => {
    render(<App />);
    fireEvent.click(screen.getByRole('button', { name: 'Settings' }));
    expect(screen.getByRole('button', { name: '12h clock format' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: '24h clock format' })).toBeInTheDocument();
  });

  it('renders the Admin unlock button', () => {
    render(<App />);
    fireEvent.click(screen.getByRole('button', { name: 'Settings' }));
    expect(screen.getByRole('button', { name: /unlock admin area/i })).toBeInTheDocument();
  });

  it('opens the PIN modal when Unlock Admin Area is clicked', () => {
    render(<App />);
    fireEvent.click(screen.getByRole('button', { name: 'Settings' }));
    fireEvent.click(screen.getByRole('button', { name: /unlock admin area/i }));
    expect(screen.getByRole('dialog', { name: 'Admin PIN entry' })).toBeInTheDocument();
  });

  it('cancels the PIN modal', () => {
    render(<App />);
    fireEvent.click(screen.getByRole('button', { name: 'Settings' }));
    fireEvent.click(screen.getByRole('button', { name: /unlock admin area/i }));
    fireEvent.click(screen.getByRole('button', { name: 'Cancel' }));
    expect(screen.queryByRole('dialog', { name: 'Admin PIN entry' })).not.toBeInTheDocument();
  });

  it('unlocks admin area when correct PIN is entered', () => {
    render(<App />);
    fireEvent.click(screen.getByRole('button', { name: 'Settings' }));
    fireEvent.click(screen.getByRole('button', { name: /unlock admin area/i }));
    // Enter PIN 1234
    ['1','2','3','4'].forEach(d => {
      fireEvent.click(screen.getByRole('button', { name: `Digit ${d}` }));
    });
    expect(screen.queryByRole('dialog', { name: 'Admin PIN entry' })).not.toBeInTheDocument();
    expect(screen.getByRole('button', { name: /reboot touch panel/i })).toBeInTheDocument();
  });
});

