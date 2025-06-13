# Changelog

## [1.0.0] - 2025-06-08
### Added
- Initial release of LayerZero.Tools.Web
- Auto-bundling logic for layout-specific CSS/JS based on controller/action

### Fixed
- N/A
## [1.0.1] - Unreleased

### ✨ Added
- Guard checks using `SpindleTreeGuard`:
  - Skips empty directories based on file patterns (e.g., `.js`, `.css`).
  - Ensures clean and minimal bundle generation.
- Bundling behavior now respects environment:
  - **Unminified output in Development mode** for easier debugging.
  - **Minified output in Production** for optimized delivery.

### 🔧 Changed
- Refactored `BundleBuilder.Register()` for improved clarity and conditional execution.
- Improved path normalization (cross-platform compatibility with `/` separators).

### 🛠️ Technical
- Introduced stricter directory checks to avoid unnecessary minification or empty bundles.


## [Unreleased]
### Added
- Placeholder for upcoming features
