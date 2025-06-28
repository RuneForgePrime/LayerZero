# 📦 Changelog

All notable changes to this project are documented in this file.

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
