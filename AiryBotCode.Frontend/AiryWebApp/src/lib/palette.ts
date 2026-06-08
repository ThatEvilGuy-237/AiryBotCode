// Client-side palette extraction from an image — no upload, runs on a <canvas>.
// Ranks colors by  center-weight × vibrancy (saturation·value) × area,
// then merges near-duplicates so the result is a punchy, usable theme.

export interface Swatch {
  hex: string
  weight: number
}

function loadImage(file: File): Promise<HTMLImageElement> {
  return new Promise((resolve, reject) => {
    const url = URL.createObjectURL(file)
    const img = new Image()
    img.onload = () => {
      URL.revokeObjectURL(url)
      resolve(img)
    }
    img.onerror = (e) => {
      URL.revokeObjectURL(url)
      reject(e)
    }
    img.src = url
  })
}

function rgbToSv(r: number, g: number, b: number): { s: number; v: number } {
  const mr = r / 255,
    mg = g / 255,
    mb = b / 255
  const max = Math.max(mr, mg, mb)
  const min = Math.min(mr, mg, mb)
  const v = max
  const s = max === 0 ? 0 : (max - min) / max
  return { s, v }
}

function toHex(n: number): string {
  return Math.max(0, Math.min(255, Math.round(n))).toString(16).padStart(2, '0')
}

export function rgbToHex(r: number, g: number, b: number): string {
  return `#${toHex(r)}${toHex(g)}${toHex(b)}`
}

function dist(a: { r: number; g: number; b: number }, b: { r: number; g: number; b: number }): number {
  return Math.hypot(a.r - b.r, a.g - b.g, a.b - b.b)
}

interface Accum {
  r: number
  g: number
  b: number
  weight: number
}

/** Extract up to `count` theme colors from an image, most prominent first. */
export async function extractPalette(file: File, count = 5): Promise<Swatch[]> {
  const img = await loadImage(file)

  // Downscale for speed; aspect-preserving, longest side <= 140px.
  const maxDim = 140
  const scale = Math.min(1, maxDim / Math.max(img.width, img.height))
  const w = Math.max(1, Math.round(img.width * scale))
  const h = Math.max(1, Math.round(img.height * scale))

  const canvas = document.createElement('canvas')
  canvas.width = w
  canvas.height = h
  const ctx = canvas.getContext('2d', { willReadFrequently: true })
  if (!ctx) return []
  ctx.drawImage(img, 0, 0, w, h)
  const { data } = ctx.getImageData(0, 0, w, h)

  const cx = w / 2
  const cy = h / 2
  const maxDist = Math.hypot(cx, cy) || 1
  const buckets = new Map<number, Accum>()

  for (let y = 0; y < h; y++) {
    for (let x = 0; x < w; x++) {
      const i = (y * w + x) * 4
      if (data[i + 3] < 128) continue // skip transparent
      const r = data[i]
      const g = data[i + 1]
      const b = data[i + 2]

      // Center weight: ~1 at center, falls off toward edges (gaussian).
      const d = Math.hypot(x - cx, y - cy) / maxDist
      const center = Math.exp(-(d * d) * 2.0)
      // Vibrancy, but dull colors still count a little.
      const { s, v } = rgbToSv(r, g, b)
      const vibrancy = 0.2 + 0.8 * (s * v)
      const weight = center * vibrancy

      // Quantize to 5 bits per channel for bucketing.
      const key = ((r >> 3) << 10) | ((g >> 3) << 5) | (b >> 3)
      let acc = buckets.get(key)
      if (!acc) {
        acc = { r: 0, g: 0, b: 0, weight: 0 }
        buckets.set(key, acc)
      }
      acc.r += r * weight
      acc.g += g * weight
      acc.b += b * weight
      acc.weight += weight
    }
  }

  const ranked = [...buckets.values()]
    .filter((a) => a.weight > 0)
    .map((a) => ({ r: a.r / a.weight, g: a.g / a.weight, b: a.b / a.weight, weight: a.weight }))
    .sort((a, b) => b.weight - a.weight)

  // Merge near-duplicate colors (keep the heavier one, which comes first).
  const merged: typeof ranked = []
  for (const c of ranked) {
    if (merged.some((m) => dist(m, c) < 42)) continue
    merged.push(c)
    if (merged.length >= count) break
  }

  return merged.map((c) => ({ hex: rgbToHex(c.r, c.g, c.b), weight: c.weight }))
}

function hexToRgb(hex: string): { r: number; g: number; b: number } {
  const h = hex.replace('#', '')
  return { r: parseInt(h.slice(0, 2), 16), g: parseInt(h.slice(2, 4), 16), b: parseInt(h.slice(4, 6), 16) }
}

function hue(hex: string): number {
  const { r, g, b } = hexToRgb(hex)
  const mr = r / 255,
    mg = g / 255,
    mb = b / 255
  const max = Math.max(mr, mg, mb)
  const min = Math.min(mr, mg, mb)
  const delta = max - min
  if (delta === 0) return 0
  let hh: number
  if (max === mr) hh = ((mg - mb) / delta) % 6
  else if (max === mg) hh = (mb - mr) / delta + 2
  else hh = (mr - mg) / delta + 4
  return (hh * 60 + 360) % 360
}

/** Choose a primary (most prominent) and an accent (most distinct hue). */
export function pickPrimaryAccent(swatches: Swatch[]): { primary: string; accent: string } {
  if (swatches.length === 0) return { primary: '#e8467a', accent: '#e06699' }
  const primary = swatches[0].hex
  const ph = hue(primary)
  let accent = primary
  let best = -1
  for (const s of swatches.slice(1)) {
    const diff = Math.min(Math.abs(hue(s.hex) - ph), 360 - Math.abs(hue(s.hex) - ph))
    if (diff > best) {
      best = diff
      accent = s.hex
    }
  }
  return { primary, accent }
}
