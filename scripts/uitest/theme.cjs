// Airy panel browser test — drives the per-bot image-theme flow end-to-end and
// the migrated views, asserting the things that couldn't be checked headlessly.
// Run via the hive-uitest image with --network host (panel is on 127.0.0.1:10000):
//   docker run --rm --network host -e JWT="$JWT" -e BASE=http://127.0.0.1:10000 \
//     -v "$PWD/scripts/uitest:/work" -v /tmp/airy-uitest:/out hive-uitest node /work/theme.cjs
const { chromium } = require('playwright');
const JWT = process.env.JWT, BASE = process.env.BASE || 'http://127.0.0.1:10000', OUT = process.env.OUT || '/out';
const IMG = process.env.IMG || '/work/test-image.png';

const readAccent = (p) => p.evaluate(() => document.documentElement.style.getPropertyValue('--accent-hue').trim());

(async () => {
  const b = await chromium.launch({ args: ['--no-sandbox'] });
  const ctx = await b.newContext({ ignoreHTTPSErrors: true });
  await ctx.addCookies([{ name: 'jwt', value: JWT, domain: new URL(BASE).hostname, path: '/' }]);
  const p = await ctx.newPage();
  p.on('pageerror', e => console.log('PAGEERROR:', e.message));
  p.on('console', m => { if (m.type() === 'error') console.log('CONSOLEERR:', m.text()); });

  // 1. Home — confirms auth passed the router guard (else we'd be bounced to /login)
  await p.goto(`${BASE}/`, { waitUntil: 'networkidle', timeout: 30000 });
  console.log('1. URL after load:', p.url(), p.url().includes('/login') ? '❌ bounced to login' : '✓ authed');
  await p.screenshot({ path: `${OUT}/airy-01-home.png`, fullPage: true });

  // 2. Theme page, default look
  await p.goto(`${BASE}/theme`, { waitUntil: 'networkidle', timeout: 30000 });
  await p.waitForTimeout(600);
  const before = await readAccent(p);
  console.log('2. accent-hue before upload:', JSON.stringify(before), before === '' ? '✓ (default)' : '(already themed)');
  await p.screenshot({ path: `${OUT}/airy-02-theme-default.png`, fullPage: true });

  // 3. Upload image → deriveThemeFromImage + applyTheme should re-theme the page
  await p.locator('input[type=file]').first().setInputFiles(IMG);
  await p.waitForTimeout(1800);
  const after = await readAccent(p);
  console.log('3. accent-hue after upload:', JSON.stringify(after), after && after !== before ? '✓ RE-THEMED' : '❌ no change');
  await p.screenshot({ path: `${OUT}/airy-03-theme-applied.png`, fullPage: true });

  // 4. Save → persist per-bot (Save is disabled when no bot is selected)
  const saveBtn = p.getByRole('button', { name: /^Save/ }).first();
  if (await saveBtn.count() && await saveBtn.isEnabled().catch(() => false)) {
    await saveBtn.click(); await p.waitForTimeout(1500);
    console.log('4a. clicked Save');
  } else {
    console.log('4a. Save disabled (no bot selected) — per-bot persistence not exercisable');
  }
  const keys = await p.evaluate(() => Object.keys(localStorage).filter(k => k.startsWith('airy.imageTheme')));
  const perBot = keys.some(k => /airy\.imageTheme\.v1\..+/.test(k));
  console.log('4. localStorage theme keys:', keys, perBot ? '✓ per-bot key present' : '❌ no per-bot key');

  // 5. Avatar shows the uploaded image (image = profile pic)
  const avatarImg = await p.locator('.avatar img').count();
  console.log('5. switcher avatar <img> count:', avatarImg, avatarImg >= 1 ? '✓ image is the bot avatar' : '❌ none');

  // 6. Reload → applyStartupTheme should re-apply (persistence across reload)
  await p.goto(`${BASE}/`, { waitUntil: 'networkidle', timeout: 30000 });
  await p.waitForTimeout(900);
  const reload = await readAccent(p);
  console.log('6. accent-hue after reload:', JSON.stringify(reload), reload && reload === after ? '✓ PERSISTED' : '❌ lost on reload');
  await p.screenshot({ path: `${OUT}/airy-04-after-reload.png`, fullPage: true });

  // 7. Migrated Databases view renders
  await p.goto(`${BASE}/database`, { waitUntil: 'networkidle', timeout: 30000 });
  await p.waitForTimeout(900);
  const dbHeading = await p.locator('h1').first().innerText().catch(() => '(none)');
  console.log('7. Databases page H1:', JSON.stringify(dbHeading));
  await p.screenshot({ path: `${OUT}/airy-05-databases.png`, fullPage: true });

  await b.close();
  console.log('DONE');
})().catch(e => { console.log('FATAL', e.message); process.exit(1); });
