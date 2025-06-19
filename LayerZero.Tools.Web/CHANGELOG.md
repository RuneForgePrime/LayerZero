# 📦 LayerZero.Tools.Web – Changelog

## [1.1.0] - 2025-06-XX

### ✨ Added

- **Critical CSS Support**
  - Inline injection of CSS required for the first paint, improving perceived performance.
  - Critical CSS files are parsed and validated:
    - Invalid rules are rejected.
    - Entirely invalid or empty files are automatically skipped.
  - Stored by default in the `css/critical/` folder (can be customized).
  - Zero-cost if unused — works cleanly within the bundling pipeline.

- **Development Cache-Busting**
  - Minimal cache-busting logic for local development scenarios.
  - Activated by setting the `isDevelopment` flag in `DynamicBundleMapper`.
  - Ensures fresh asset delivery during development without affecting production behavior.

## [1.0.1] - 2025-06-13

### ✨ Added
- Support for **conditional minification**:
  - Bundles are served unminified in `Development` environment (`IWebHostEnvironment.IsDevelopment()`).
  - Minification remains enabled in `Production`.
- Guard checks using `SpindleTreeGuard`:
  - Skips empty directories based on file patterns (e.g., `.js`, `.css`).
  - Ensures clean and minimal bundle generation.

### 🔧 Changed
- Refactored `BundleBuilder.Register()` for improved clarity and conditional execution.
- Improved path normalization (cross-platform compatibility with `/` separators).

### 🛠️ Technical
- Introduced stricter directory checks to avoid unnecessary minification or empty bundles.

---

## [1.0.0] - 2025-06-08

### ✨ Added
- Initial release of `LayerZero.Tools.Web`.
- Automatic JS/CSS bundle generation based on controller/action folder structure.
- Integration with WebOptimizer pipeline.