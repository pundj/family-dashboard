// Import our custom CSS
import '../scss/site.scss'
import '@fontsource/dseg7/classic-400.css'
import '@fontsource/dseg14/classic-400.css'

// Import all of Bootstrap's JS
import 'bootstrap'

const defaultInactivityTimeoutMinutes = 5;
const defaultScreensaverDateFormat = 'ddd MMM d yyyy';
const defaultScreensaverTimeFormat = 'h:mm:ss tt';
const dateTimeFormatTokenPattern = /yyyy|MMMM|dddd|MMM|ddd|yy|MM|dd|HH|hh|mm|ss|tt|M|d|H|h|m|s|t/g;
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
let screensaverDateFormat = defaultScreensaverDateFormat;
let screensaverTimeFormat = defaultScreensaverTimeFormat;
let screensaverActivationCallback;
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
    const formattedDate = formatDateTime(now, screensaverDateFormat);
    screensaverDate.textContent = screensaverDateFormat === defaultScreensaverDateFormat
        ? formattedDate.toUpperCase()
        : formattedDate;
    screensaverTime.dateTime = now.toTimeString().slice(0, 8);
    const formattedTime = formatTime(now, screensaverTimeFormat);
    screensaverTime.textContent = formattedTime.value;
    screensaverMeridiem.textContent = formattedTime.meridiem;
    screensaverMeridiem.hidden = !formattedTime.meridiem;
}

function formatTime(date, pattern) {
    const hasMeridiem = /tt|t/.test(pattern);
    return {
        value: formatDateTime(date, pattern.replace(/tt|t/g, '')).trim(),
        meridiem: hasMeridiem ? getDayPeriod(date) : ''
    };
}

function formatDateTime(date, pattern) {
    const safePattern = pattern || '';
    const tokenValues = {
        yyyy: String(date.getFullYear()),
        yy: String(date.getFullYear()).slice(-2),
        MMMM: getLocalizedPart(date, { month: 'long' }),
        MMM: getLocalizedPart(date, { month: 'short' }),
        MM: String(date.getMonth() + 1).padStart(2, '0'),
        M: String(date.getMonth() + 1),
        dddd: getLocalizedPart(date, { weekday: 'long' }),
        ddd: getLocalizedPart(date, { weekday: 'short' }),
        dd: String(date.getDate()).padStart(2, '0'),
        d: String(date.getDate()),
        HH: String(date.getHours()).padStart(2, '0'),
        H: String(date.getHours()),
        hh: String((date.getHours() % 12) || 12).padStart(2, '0'),
        h: String((date.getHours() % 12) || 12),
        mm: String(date.getMinutes()).padStart(2, '0'),
        m: String(date.getMinutes()),
        ss: String(date.getSeconds()).padStart(2, '0'),
        s: String(date.getSeconds()),
        tt: getDayPeriod(date),
        t: getDayPeriod(date).charAt(0)
    };

    return safePattern.replace(dateTimeFormatTokenPattern, token => tokenValues[token]);
}

function getLocalizedPart(date, options) {
    return new Intl.DateTimeFormat(undefined, options).format(date);
}

function getDayPeriod(date) {
    return new Intl.DateTimeFormat(undefined, {
        hour: 'numeric',
        hour12: true
    }).formatToParts(date)
        .find(part => part.type === 'dayPeriod')
        ?.value
        ?.toUpperCase() ?? '';
}

async function showDarkScreen() {
    ensureInactivityOverlay();
    inactivityOverlay.classList.add('is-visible');
    if (showScreensaverDateTime) {
        window.clearInterval(screensaverDateTimeIntervalId);
        updateScreensaverDateTime();
        screensaverDateTimeIntervalId = window.setInterval(updateScreensaverDateTime, 1000);
    }
    inactivityOverlay.focus();
    try {
        await screensaverActivationCallback?.invokeMethodAsync('OnScreensaverShown');
    } catch {
    }
}

function resetInactivityTimer() {
    window.clearTimeout(inactivityTimeoutId);
    if (!screensaverEnabled) {
        return;
    }

    inactivityTimeoutId = window.setTimeout(() => {
        void showDarkScreen();
    }, inactivityTimeoutMilliseconds);
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
    getScreensaverDateFormat: () => window.localStorage.getItem('screensaver_date_format'),
    getScreensaverTimeFormat: () => window.localStorage.getItem('screensaver_time_format'),
    setScreensaverActivationCallback: callback => {
        screensaverActivationCallback = callback;
    },
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
    setScreensaverDateFormat: format => {
        screensaverDateFormat = format || defaultScreensaverDateFormat;
        window.localStorage.setItem('screensaver_date_format', screensaverDateFormat);
        if (showScreensaverDateTime) {
            updateScreensaverDateTime();
        }
    },
    setScreensaverTimeFormat: format => {
        screensaverTimeFormat = format || defaultScreensaverTimeFormat;
        window.localStorage.setItem('screensaver_time_format', screensaverTimeFormat);
        if (showScreensaverDateTime) {
            updateScreensaverDateTime();
        }
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
        screensaverActivationCallback = undefined;
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
