import React, { useState } from 'react';
import TopBar from './components/TopBar';
import SidebarNav, { Page } from './components/SidebarNav';
import HomeView from './components/HomeView';
import VideoView from './components/VideoView';
import AudioView from './components/AudioView';
import CameraView from './components/CameraView';
import SettingsDrawer from './components/SettingsDrawer';
import './App.css';

const PAGE_TITLES: Record<Page, string> = {
  home:   'Executive Boardroom',
  video:  'Video Control',
  audio:  'Audio Control',
  camera: 'Camera Control',
};

/**
 * App
 *
 * Root component for the CH5 executive touch-panel UI.
 *
 * Layout:
 * ┌──────────────────────────────────────────────────────┐
 * │  Sidebar  │  TopBar                                  │
 * │           ├──────────────────────────────────────────┤
 * │  Nav      │  Main content (Home / Video / Audio / …) │
 * └──────────────────────────────────────────────────────┘
 * A Settings gear button at the bottom of the Sidebar opens a slide-in
 * SettingsDrawer overlay with all panel configuration options.
 */
function App() {
  const [currentPage, setCurrentPage] = useState<Page>('home');
  const [settingsOpen, setSettingsOpen] = useState(false);

  return (
    <div className="exec-shell">
      <SidebarNav
        currentPage={currentPage}
        onNavigate={setCurrentPage}
        onOpenSettings={() => setSettingsOpen(true)}
      />

      <div className="exec-shell__main">
        <TopBar title={PAGE_TITLES[currentPage]} />

        <main className="exec-shell__content">
          {currentPage === 'home'   && <HomeView />}
          {currentPage === 'video'  && <VideoView />}
          {currentPage === 'audio'  && <AudioView />}
          {currentPage === 'camera' && <CameraView />}
        </main>
      </div>

      <SettingsDrawer
        isOpen={settingsOpen}
        onClose={() => setSettingsOpen(false)}
      />
    </div>
  );
}

export default App;
