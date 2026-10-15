const OFFLINE = 'airy-offline'

self.addEventListener('install', event => {
  event.waitUntil(caches.open(OFFLINE).then(cache => cache.add('/offline.html')).then(() => self.skipWaiting()))
})
self.addEventListener('activate', event => event.waitUntil(self.clients.claim()))
self.addEventListener('fetch', event => {
  if (event.request.mode !== 'navigate') return
  event.respondWith(fetch(event.request).catch(() => caches.match('/offline.html')))
})
