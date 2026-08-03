// In development, remove offline caches left by a previous published build so
// local changes are always loaded from the development server.
self.addEventListener('install', () => self.skipWaiting());
self.addEventListener('activate', event => event.waitUntil(
    Promise.all([
        self.clients.claim(),
        caches.keys().then(cacheKeys => Promise.all(
            cacheKeys
                .filter(cacheKey => cacheKey.startsWith('offline-cache-'))
                .map(cacheKey => caches.delete(cacheKey))))
    ])
));
