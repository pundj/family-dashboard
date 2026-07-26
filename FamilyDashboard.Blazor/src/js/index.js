// Import our custom CSS
import '../scss/site.scss'
import '@fontsource/dseg7/classic-400.css'
import '@fontsource/dseg14/classic-400.css'

// Import all of Bootstrap's JS
import 'bootstrap'

const defaultInactivityTimeoutMinutes = 5;
let inactivityTimeoutId;
let inactivityTimeoutMilliseconds = defaultInactivityTimeoutMinutes * 60 * 1000;
let inactivityOverlay;
let screensaverDateTimeContainer;
let screensaverDate;
let screensaverTime;
let screensaverMeridiem;
let screensaverDateTimeIntervalId;
let screensaverEnabled = false;
let showScreensaverDateTime = false;
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

    screensaverDateTimeContainer = document.createElement('div');
    screensaverDateTimeContainer.className = 'screensaver-date-time';
    screensaverDate = document.createElement('time');
    screensaverDate.className = 'screensaver-date';
    const screensaverClock = document.createElement('div');
    screensaverClock.className = 'screensaver-clock';
    screensaverTime = document.createElement('time');
    screensaverTime.className = 'screensaver-time';
    screensaverMeridiem = document.createElement('span');
    screensaverMeridiem.className = 'screensaver-meridiem';
    screensaverClock.append(screensaverTime, screensaverMeridiem);
    screensaverDateTimeContainer.append(screensaverDate, screensaverClock);

    inactivityOverlay.appendChild(screensaverDateTimeContainer);
    document.body.appendChild(inactivityOverlay);
}

function updateScreensaverDateTime() {
    if (!screensaverDate || !screensaverTime || !screensaverMeridiem) {
        return;
    }

    const now = new Date();
    screensaverDate.dateTime = now.toISOString().slice(0, 10);
    const dateParts = new Intl.DateTimeFormat(undefined, {
        weekday: 'short',
        month: 'short',
        day: 'numeric',
        year: 'numeric'
    }).formatToParts(now);
    screensaverDate.textContent = ['weekday', 'month', 'day', 'year']
        .map(type => dateParts.find(part => part.type === type)?.value)
        .filter(Boolean)
        .join(' ')
        .toUpperCase();
    screensaverTime.dateTime = now.toTimeString().slice(0, 8);
    const timeParts = new Intl.DateTimeFormat(undefined, {
        hour: 'numeric',
        minute: '2-digit',
        second: '2-digit'
    }).formatToParts(now);
    screensaverTime.textContent = timeParts
        .filter(part => part.type !== 'dayPeriod')
        .map(part => part.value)
        .join('')
        .trim();
    screensaverMeridiem.textContent = timeParts
        .filter(part => part.type === 'dayPeriod')
        .map(part => part.value)
        .join('')
        .toUpperCase();

}

function showDarkScreen() {
    ensureInactivityOverlay();
    inactivityOverlay.classList.add('is-visible');
    if (showScreensaverDateTime) {
        window.clearInterval(screensaverDateTimeIntervalId);
        updateScreensaverDateTime();
        screensaverDateTimeIntervalId = window.setInterval(updateScreensaverDateTime, 1000);
    }
    inactivityOverlay.focus();
}

function resetInactivityTimer() {
    window.clearTimeout(inactivityTimeoutId);
    if (!screensaverEnabled) {
        return;
    }

    inactivityTimeoutId = window.setTimeout(showDarkScreen, inactivityTimeoutMilliseconds);
}

function wakeScreen() {
    inactivityOverlay?.classList.remove('is-visible');
    window.clearInterval(screensaverDateTimeIntervalId);
    screensaverDateTimeIntervalId = undefined;
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
    if (screensaverEnabled) {
        wakeScreen();
    }
}

window.familyDashboard = {
    getInactivityTimeoutMinutes: () => window.localStorage.getItem('inactivity_timeout_minutes'),
    getShowTopTiles: () => window.localStorage.getItem('show_top_tiles'),
    getShowScreensaver: () => window.localStorage.getItem('show_screensaver'),
    getShowScreensaverDateTime: () => window.localStorage.getItem('show_screensaver_date_time'),
    getScreensaverDateTimeColor: () => window.localStorage.getItem('screensaver_date_time_color'),
    configureInactivityTimeout,
    setInactivityTimeoutMinutes: timeoutMinutes => {
        window.localStorage.setItem('inactivity_timeout_minutes', timeoutMinutes);
        configureInactivityTimeout(timeoutMinutes);
    },
    setShowTopTiles: showTopTiles => window.localStorage.setItem('show_top_tiles', showTopTiles),
    setShowScreensaver: enabled => {
        screensaverEnabled = enabled;
        window.localStorage.setItem('show_screensaver', enabled);
        if (enabled) {
            resetInactivityTimer();
        } else {
            window.clearTimeout(inactivityTimeoutId);
            inactivityOverlay?.classList.remove('is-visible');
        }
    },
    setScreensaverDateTimeColor: color => {
        window.localStorage.setItem('screensaver_date_time_color', color);
        inactivityOverlay?.style.setProperty('--screensaver-date-time-color', color);
    },
    setShowScreensaverDateTime: enabled => {
        showScreensaverDateTime = enabled;
        window.localStorage.setItem('show_screensaver_date_time', enabled);
        screensaverDateTimeContainer?.toggleAttribute('hidden', !enabled);
        if (inactivityOverlay?.classList.contains('is-visible')) {
            if (enabled) {
                window.clearInterval(screensaverDateTimeIntervalId);
                updateScreensaverDateTime();
                screensaverDateTimeIntervalId = window.setInterval(updateScreensaverDateTime, 1000);
            } else {
                window.clearInterval(screensaverDateTimeIntervalId);
                screensaverDateTimeIntervalId = undefined;
            }
        }
    },
    disposeInactivityTimeout: () => {
        window.clearTimeout(inactivityTimeoutId);
        window.clearInterval(screensaverDateTimeIntervalId);
        for (const eventName of inactivityActivityEvents) {
            document.removeEventListener(eventName, resetInactivityTimer);
        }

        inactivityListenersRegistered = false;
        inactivityOverlay?.remove();
        inactivityOverlay = undefined;
    }
};
