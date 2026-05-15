# 📦 Changelog

All notable changes to this project are documented in this file.

---

## [2.1.0] - 2026-05-15

### 🗑 Removed

- **`LigerShark.WebOptimizer.Core`** — eliminated external WebOptimizer dependency.
  - Replaced with a built-in `BundleStore` (lazy `ConcurrentDictionary` cache) and `BundleServingMiddleware`.
  - `app.UseWebOptimizer()` → `app.UseBundleServing()`.
  - `AddDynamicBundle()` signature is unchanged.

- **`AngleSharp.Css`** — removed CSS parser dependency.
  - Critical CSS is now minified via `NUglify`. Output changes from pretty-printed to minified.
  - Fallback to raw file content if NUglify produces no output.

- **`Esprima`** — removed JS syntax validator dependency.
  - JS validation now handled by `NUglify`. Behaviour is identical: valid JS is inlined as-is; files with syntax errors produce a `/* File X Skipped: reason */` comment.

- **`CssFileParser` / `JsFileParser`** — removed internal parser utility classes (`LayerZero.Tools.Web.Parser` namespace).
  - These were internal helpers for critical asset loading, now superseded by `BundleStore.Build()` which handles all asset types uniformly.

### 🛠 Improvements

- `LayerZero.Tools.Web` is now fully self-contained — only `NUglify` and the ASP.NET Core framework reference are required.
- Bundle serving no longer depends on any third-party middleware pipeline.
- Critical CSS and JS now flow through the same `BundleStore` pipeline as all other bundles — same build, same caching, same file-watcher eviction, same ETag fingerprinting.
- Critical assets are also accessible as HTTP routes (`/bundles/z-Critical.css`, `/bundles/z-Critical.js`) for inspection and debugging.

### ⚠️ Breaking Changes

- `app.UseWebOptimizer()` must be replaced with `app.UseBundleServing()`.
- Critical CSS output format changed from indented/pretty-printed to NUglify-minified.
- `LayerZero.Tools.Web.Parser.CssFileParser` and `LayerZero.Tools.Web.Parser.JsFileParser` are removed. If referenced directly, remove those usages.

---

## [1.3.0] - 2025-07-06

### ✨ Added

- **Critical JS Support**:
  - Introduced support for `wwwroot/js/critical/` folder.
  - JS files placed in this path are parsed, merged, and injected **before regular bundles**.
  - Designed for inline or early-execution scripts such as feature detection, cookie banners, or user agent fixes.

### 🧭 Notes

- Like its CSS counterpart, the critical JS logic is **scoped globally** for now.
- Per-controller/action scoping is under consideration for future versions, alongside dynamic loading and async asset management.
- This update finalizes the `v1.x` critical asset foundation before introducing partial view support in `2.0.0`.

---

## [1.2.0] - 2025-06-24 *(planned)*

### ✨ Added

- **Critical JS Support**:
  - New support for inline critical JavaScript under `wwwroot/js/critical/`.
  - All `.js` files are merged and injected into a single `<script>` tag above other bundles.
  - No validation or dependency resolution is performed in this version—scripts are injected as-is.

- **New TagHelpers**:
  - `<critical-script-bundle-loader />` and `<critical-style-bundle-loader />` added for clean injection of critical assets.
  - TagHelpers are automatically scoped and resolved in layout files.

### 🛠 Improvements

- Enhanced example and documentation clarity in the README.
- Updated development guidance and inline injection examples.

### 📌 Known Limitations

- ❌ Critical JS is injected without validation; malformed scripts may still execute.
- ❌ Custom asset folder paths are still not configurable (`AddDynamicBundle()` remains convention-bound).
- ❌ No runtime configuration object yet (static behavior only).

---

## [1.1.0] - 2025-06-21

### ✨ Added

- **Critical CSS Support**:
  - New support for inline critical CSS under `wwwroot/css/critical/`.
  - Multiple files are merged into a single `<style>` tag injected before all other bundles.
  - Graceful fallback for broken or malformed CSS files (logged, but not injected).

- **Improved Development Mode Behavior**:
  - Cache-busting enabled for local development via version query strings (e.g., `?v=timestamp`).
  - Minification disabled automatically when `isDevelopment` is true.

### 🛠 Fixed

- ⚠️ **Environment Detection Bug**:
  - `AddDynamicBundle()` now accepts optional `IWebHostEnvironment` to correctly detect development/production modes.
  - If not supplied, it defaults to `isDevelopment = false` for backward compatibility.

### 📌 Known Limitations

- ❌ **Custom asset folder paths** are not yet configurable.
- ❌ **No configuration object or runtime overrides**—behavior is static and convention-bound.

---

## [1.0.1] - 2025-06-10

### ✅ Fixed

- TagHelper bug where controller/action resolution failed in certain nested route structures.
- Bundle generation skipped when asset directories were empty (edge case regression).

---

## [1.0.0] - 2025-06-08

### 🎉 Initial Release

- Convention-based bundle generation using WebOptimizer
- Razor TagHelpers for automatic injection
- Supports controller/action folder structure
- Works with default WebOptimizer pipeline

---
