// Cross-device test: a fresh browser (no localStorage cache) should pull the
// per-bot theme from the SERVER and apply it (App.vue's getTheme → applyServerTheme).
const { chromium } = require('playwright');
const JWT = process.env.JWT, BASE = process.env.BASE || 'http://127.0.0.1:10000', OUT = process.env.OUT || '/out';
const readAccent = (p) => p.evaluate(() => document.documentElement.style.getPropertyValue('--accent-hue').trim());
(async () => {
  const b = await chromium.launch({ args: ['--no-sandbox'] });
  const ctx = await b.newContext({ ignoreHTTPSErrors: true });
  await ctx.addCookies([{ name: 'jwt', value: JWT, domain: new URL(BASE).hostname, path: '/' }]);
  const p = await ctx.newPage();
  // Simulate a brand-new device: wipe localStorage before the app boots.
  await p.addInitScript(() => { try { localStorage.clear(); } catch {} });
  await p.goto(`${BASE}/`, { waitUntil: 'networkidle', timeout: 30000 });
  await p.waitForTimeout(2000); // allow the async server sync (getTheme) to land
  const accent = await readAccent(p);
  const cached = await p.evaluate(() => Object.keys(localStorage).filter(k => k.startsWith('airy.imageTheme')));
  console.log('fresh-device accent-hue:', JSON.stringify(accent), accent ? '✓ pulled theme from SERVER' : '❌ no theme (server sync failed)');
  console.log('localStorage re-cached from server:', cached);
  await p.screenshot({ path: `${OUT}/airy-crossdevice.png`, fullPage: true });
  await b.close();
  console.log('DONE');
})().catch(e => { console.log('FATAL', e.message); process.exit(1); });
