# Changelog

## [1.0.1] - 2025-06-13
### Added
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
### Added
- Initial release of `LayerZero.Tools.Web`.
- Automatic JS/CSS bundle generation based on controller/action folder structure.
- Integration with WebOptimizer pipeline.