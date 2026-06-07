// src/lib/auth.ts
//
// Discord-OAuth login state for the control panel.
//
// Flow:
//   1. login()  -> sends the browser to Discord's authorize page.
//   2. Discord  -> redirects to the API (/api/auth/discord/redirect?code=...).
//   3. The API  -> exchanges the code, issues a JWT, and redirects back here
//                  with the token in the URL fragment: #token=<jwt>.
//   4. captureTokenFromHash() picks it up on load and stores it.
//
// The JWT is sent as a Bearer header on every API call (see api.ts).

import { writable } from "svelte/store";

const TOKEN_KEY = "airy.jwt";

// Discord application (client) id == the bot id. Override per-env if needed.
const CLIENT_ID: string =
  (import.meta as any).env?.VITE_DISCORD_CLIENT_ID ?? "1318870826862379018";

// Must exactly match the redirect URL registered in the Discord portal AND the
// API's Discord:RedirectUri.
const REDIRECT_URI: string =
  (import.meta as any).env?.VITE_DISCORD_REDIRECT_URI ??
  "https://thatevilserver.tail2a87af.ts.net/api/auth/discord/redirect";

export const token = writable<string | null>(localStorage.getItem(TOKEN_KEY));
export const isAuthenticated = writable<boolean>(!!localStorage.getItem(TOKEN_KEY));

function setToken(value: string | null) {
  if (value) {
    localStorage.setItem(TOKEN_KEY, value);
  } else {
    localStorage.removeItem(TOKEN_KEY);
  }
  token.set(value);
  isAuthenticated.set(!!value);
}

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

/** Pull a freshly-issued token out of the URL fragment after the OAuth round-trip. */
export function captureTokenFromHash(): void {
  const hash = window.location.hash;
  if (!hash || !hash.includes("token=")) return;

  const params = new URLSearchParams(hash.replace(/^#/, ""));
  const t = params.get("token");
  if (t) {
    setToken(t);
    // Scrub the token out of the address bar / history.
    history.replaceState(null, "", window.location.pathname + window.location.search);
  }
}

/** Send the browser to Discord to start the OAuth login. */
export function login(): void {
  const url = new URL("https://discord.com/api/oauth2/authorize");
  url.searchParams.set("client_id", CLIENT_ID);
  url.searchParams.set("redirect_uri", REDIRECT_URI);
  url.searchParams.set("response_type", "code");
  url.searchParams.set("scope", "identify");
  window.location.href = url.toString();
}

export function logout(): void {
  setToken(null);
}
