// Import our custom CSS
import '../scss/site.scss'

// Import all of Bootstrap's JS
import 'bootstrap'

const defaultInactivityTimeoutMinutes = 5;
let inactivityTimeoutId;
let inactivityTimeoutMilliseconds = defaultInactivityTimeoutMinutes * 60 * 1000;
let inactivityOverlay;
let inactivityListenersRegistered = false;
const inactivityActivityEvents = ['pointerdown', 'keydown', 'touchstart'];
function ensureInactivityOverlay() {
    if (inactivityOverlay) {
        return;
    }

    inactivityOverlay = document.createElement('div');
    inactivityOverlay.className = 'inactivity-screen-overlay';
    inactivityOverlay.setAttribute('aria-label', 'Touch the screen to wake the dashboard');
    inactivityOverlay.setAttribute('role', 'button');
    inactivityOverlay.setAttribute('tabindex', '0');
    inactivityOverlay.addEventListener('pointerdown', wakeScreen);
    inactivityOverlay.addEventListener('keydown', event => {
        if (event.key === 'Enter' || event.key === ' ') {
            event.preventDefault();
            wakeScreen();
        }
    });
    document.body.appendChild(inactivityOverlay);
}

function showDarkScreen() {
    ensureInactivityOverlay();
    inactivityOverlay.classList.add('is-visible');
    inactivityOverlay.focus();
}

function resetInactivityTimer() {
    window.clearTimeout(inactivityTimeoutId);
    inactivityTimeoutId = window.setTimeout(showDarkScreen, inactivityTimeoutMilliseconds);
}

function wakeScreen() {
    inactivityOverlay?.classList.remove('is-visible');
    resetInactivityTimer();
}

function registerInactivityListeners() {
    if (inactivityListenersRegistered) {
        return;
    }

    for (const eventName of inactivityActivityEvents) {
        document.addEventListener(eventName, resetInactivityTimer, { passive: true });
    }

    inactivityListenersRegistered = true;
}

function configureInactivityTimeout(timeoutMinutes) {
    const parsedTimeout = Number.parseInt(timeoutMinutes, 10);
    const validTimeout = Number.isInteger(parsedTimeout) && parsedTimeout > 0
        ? parsedTimeout
        : defaultInactivityTimeoutMinutes;

    inactivityTimeoutMilliseconds = validTimeout * 60 * 1000;
    ensureInactivityOverlay();
    registerInactivityListeners();
    wakeScreen();
}

window.familyDashboard = {
    getInactivityTimeoutMinutes: () => window.localStorage.getItem('inactivity_timeout_minutes'),
    getShowTopTiles: () => window.localStorage.getItem('show_top_tiles'),
    configureInactivityTimeout,
    setInactivityTimeoutMinutes: timeoutMinutes => {
        window.localStorage.setItem('inactivity_timeout_minutes', timeoutMinutes);
        configureInactivityTimeout(timeoutMinutes);
    },
    setShowTopTiles: showTopTiles => window.localStorage.setItem('show_top_tiles', showTopTiles),
    disposeInactivityTimeout: () => {
        window.clearTimeout(inactivityTimeoutId);
        for (const eventName of inactivityActivityEvents) {
            document.removeEventListener(eventName, resetInactivityTimer);
        }

        inactivityListenersRegistered = false;
        inactivityOverlay?.remove();
        inactivityOverlay = undefined;
    }
};
