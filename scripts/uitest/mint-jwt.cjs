// Mint a panel JWT (HS256) signed with the API's Jwt:Secret — which equals
// The-Hive's AIRY_JWT_SECRET (one login covers both). Prints the token to stdout.
//   AIRY_JWT_SECRET=... node mint-jwt.cjs
const crypto = require('node:crypto');
const SECRET = process.env.AIRY_JWT_SECRET;
if (!SECRET) { console.error('AIRY_JWT_SECRET not set'); process.exit(1); }
const USER = 'uitest', ISS = 'AiryBotCode', AUD = 'AiryBotCode';
const b64 = o => Buffer.from(JSON.stringify(o)).toString('base64url');
const now = Math.floor(Date.now() / 1000);
const pl = {
  sub: USER, nameid: USER,
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': USER,
  iss: ISS, aud: AUD, iat: now, exp: now + 3600,
};
const data = `${b64({ alg: 'HS256', typ: 'JWT' })}.${b64(pl)}`;
const sig = crypto.createHmac('sha256', SECRET).update(data).digest('base64url');
process.stdout.write(`${data}.${sig}`);
