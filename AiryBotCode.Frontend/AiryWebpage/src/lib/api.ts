// src/lib/api.ts
//
// Thin client for the command-settings API. The page stays usable even when the
// API is down: reads fall back to the static registry, and saves report failure
// so the UI can surface it.

import type { CommandConfig, CommandSetting, CommandSettingUiHint } from "./types/database";
import { mockCommandConfigs } from "./commands";

// Override with VITE_API_BASE when the API runs somewhere other than :5000.
const API_BASE: string =
    (import.meta as any).env?.VITE_API_BASE ?? "http://localhost:5000";

interface ApiSetting {
    key: string;
    value: string;
    description: string;
    category: string;
    uiHint: string;
    isReloadable: boolean;
}

interface ApiCommand {
    commandName: string;
    settings: ApiSetting[];
}

export interface LoadResult {
    commands: CommandConfig[];
    live: boolean; // true when data came from the API, false when from the registry
}

export async function loadCommands(): Promise<LoadResult> {
    try {
        const res = await fetch(`${API_BASE}/api/commands`);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        const data: ApiCommand[] = await res.json();
        return { commands: mergeWithRegistry(data), live: true };
    } catch (e) {
        console.warn("[commands] API unavailable, using local registry:", e);
        return { commands: structuredClone(mockCommandConfigs), live: false };
    }
}

export async function saveCommand(command: CommandConfig): Promise<boolean> {
    try {
        const res = await fetch(`${API_BASE}/api/commands/${encodeURIComponent(command.commandName)}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                settings: command.settings.map((s) => ({ key: s.key, value: s.value })),
            }),
        });
        return res.ok;
    } catch (e) {
        console.warn("[commands] save failed (API unavailable):", e);
        return false;
    }
}

// The API only stores per-setting rows, so the static registry supplies the
// display metadata (displayName / slug / description) while persisted values and
// setting metadata are overlaid from the API response.
function mergeWithRegistry(api: ApiCommand[]): CommandConfig[] {
    const byName = new Map(api.map((c) => [c.commandName, c]));

    return mockCommandConfigs.map((reg) => {
        const live = byName.get(reg.commandName);
        if (!live) return structuredClone(reg);

        const liveByKey = new Map(live.settings.map((s) => [s.key, s]));
        const settings: CommandSetting[] = reg.settings.map((s) => {
            const l = liveByKey.get(s.key);
            if (!l) return { ...s };
            return {
                ...s,
                value: l.value,
                description: l.description || s.description,
                category: l.category || s.category,
                uiHint: (l.uiHint as CommandSettingUiHint) || s.uiHint,
                isReloadable: l.isReloadable,
            };
        });

        return { ...reg, settings };
    });
}
