import React from 'react';
import { Home, Monitor, Volume2, Camera, Settings } from 'lucide-react';

export type Page = 'home' | 'video' | 'audio' | 'camera';

interface NavItem {
  id: Page;
  label: string;
  icon: React.ReactNode;
}

const NAV_ITEMS: NavItem[] = [
  { id: 'home',   label: 'Home',   icon: <Home   size={22} /> },
  { id: 'video',  label: 'Video',  icon: <Monitor size={22} /> },
  { id: 'audio',  label: 'Audio',  icon: <Volume2 size={22} /> },
  { id: 'camera', label: 'Camera', icon: <Camera  size={22} /> },
];

interface SidebarNavProps {
  currentPage: Page;
  onNavigate: (page: Page) => void;
  onOpenSettings: () => void;
}

/**
 * SidebarNav
 *
 * Fixed left-sidebar navigation for the executive template.
 * Highlights the active page tab and provides large tap targets.
 * A Settings gear button sits pinned at the bottom of the sidebar.
 */
const SidebarNav: React.FC<SidebarNavProps> = ({ currentPage, onNavigate, onOpenSettings }) => {
  return (
    <nav className="exec-sidebar" aria-label="Main navigation">
      <div className="exec-sidebar__logo" aria-hidden="true">AV</div>
      <ul className="exec-sidebar__list">
        {NAV_ITEMS.map((item) => (
          <li key={item.id}>
            <button
              className={`exec-sidebar__item${currentPage === item.id ? ' exec-sidebar__item--active' : ''}`}
              onClick={() => onNavigate(item.id)}
              aria-current={currentPage === item.id ? 'page' : undefined}
              aria-label={item.label}
            >
              <span className="exec-sidebar__icon" aria-hidden="true">{item.icon}</span>
              <span className="exec-sidebar__label">{item.label}</span>
            </button>
          </li>
        ))}
      </ul>

      {/* Settings button pinned to the bottom of the sidebar */}
      <button
        className="exec-sidebar__settings-btn"
        onClick={onOpenSettings}
        aria-label="Settings"
        aria-haspopup="dialog"
      >
        <span className="exec-sidebar__icon" aria-hidden="true">
          <Settings size={22} />
        </span>
        <span className="exec-sidebar__label">Settings</span>
      </button>
    </nav>
  );
};

export default SidebarNav;
