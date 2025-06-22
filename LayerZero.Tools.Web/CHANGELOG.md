# 📦 Changelog

All notable changes to this project are documented in this file.

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

### 🛣 Roadmap

Planned for `v2.0.0`:

- Configuration object support (`DynamicBundleConfig`)
- Customizable root paths (`JsRoot`, `CssRoot`, `CriticalCssRoot`)
- Fine-grained control over cache-busting, minification, and diagnostics
- Razor directives for override support
- Critical JS injection
- DevTools diagnostics page

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
