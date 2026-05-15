# 📦 Dynamic Bundle Loader for ASP.NET Core
[![NuGet](https://img.shields.io/nuget/v/LayerZero.Tools.Web.svg)](https://www.nuget.org/packages/LayerZero.Tools.Web)
[![NuGet Downloads](https://img.shields.io/nuget/dt/LayerZero.Tools.Web.svg)](https://www.nuget.org/packages/LayerZero.Tools.Web)
![.NET](https://img.shields.io/badge/.NET-8.0-blue)

---

A convention-based asset bundling system for .NET 8+ with zero external framework dependencies. Automatically discovers and injects CSS/JS bundles per controller and action using Razor TagHelpers.

> 🧩 No WebOptimizer required.\
> ✅ Self-contained bundle serving with built-in middleware.\
> ✅ NUglify-powered minification and CSS/JS validation.

---

## 🔍 Purpose

Eliminates manual asset management in Razor views by scanning controller/action folder structures and auto-generating optimized bundles at runtime.

---

## 🗂 Folder Convention

```
wwwroot/
|   
+---css
|   |   site.css
|   |   
|   +---Controller
|   |   \---Home
|   |       |   Home.css
|   |       |   
|   |       +---Index
|   |       |       StyleSheet.css
|   |       |       
|   |       \---Privacy
|   |               StyleSheet.css
|   |               
|   \---critical
|           StyleSheet-cr1.css
|           StyleSheet-cr2.css
|           
+---js
|   |   site.js
|   |   
|   +---Controller
|   |   \---Home
|   |       |   Script.js
|   |       |   
|   |       +---Index
|   |       |       Script.js
|   |       |       
|   |       \---Privacy
|   |               flickity.pkgd.min.js
|   |               JavaScript.js
|   |               
|   \---critical
|           JavaScript-cr1.js
|           JavaScript-cr2.js
|           
\---lib

```

 - `Controller/Action` structure drives bundle discovery. 
 - A special folder for `critical CSS` (`wwwroot/css/critical`)
 - A special folder for `critical JS` (`wwwroot/js/critical`)

---

## ⚙️ Installation

### 1. Add the NuGet Package

```
dotnet add package LayerZero.Tools.Web
```

Or reference the project directly:

```bash
dotnet add reference ../LayerZero.Tools.Web/LayerZero.Tools.Web.csproj
```

### 2. Dependencies

No external NuGet packages are required in your application. `LayerZero.Tools.Web` uses only:

- `NUglify` (bundled — CSS/JS minification and validation)
- `Microsoft.AspNetCore.App` framework reference

---

## 🚀 Usage

### Register in `Program.cs`

```csharp
builder.Services.AddDynamicBundle();
```

Or if you want to enable cache-busting on dev environment:

```csharp
builder.Services.AddDynamicBundle(builder.Environment);
```

### Enable Middleware

```csharp
app.UseBundleServing();
```

---

## 🧠 TagHelpers

### Register in `_ViewImports.cshtml`

```cshtml
@addTagHelper *, LayerZero.Tools.Web
```

### Use in `_Layout.cshtml`

```cshtml
<head>
    <critical-style-bundle-loader/>
    <style-bundle-loader />
</head>
<body>
    @RenderBody()
    <critical-script-bundle-loader/>
    <script-bundle-loader />
</body>
```

> Controller-wide assets load by default and are overridden by action-specific bundles if found.

---

## 💡 Features

✅ Convention-over-configuration\
✅ Minification only in production\
✅ Controller & action bundle granularity\
✅ TagHelpers for clean layout injection\
✅ Auto-registers bundles at startup\
✅ Inline critical CSS (NUglify-minified)\
✅ Inline critical JS (NUglify-validated)\
✅ Cache-busting in development mode\
✅ Zero external framework dependencies

---

## ⚠️ Known Limitations

- ❌ **Custom asset folder paths** are *not* configurable via `AddDynamicBundle()`.
- ❌ **Dynamic runtime configuration** of asset logic is not exposed yet.
- ✅ A static convention-based pathing system is in place (e.g., `wwwroot/css/Controller/Action/...`).

---

## 🔥 Critical CSS (v1.1.0+)

- Combines all `.css` files under `wwwroot/css/critical/` into a single `<style>` tag.
- Injected above all other stylesheets.
- CSS is minified via NUglify. If a file cannot be parsed at all, raw content is used as fallback.

> **Note:** Critical CSS is also accessible as a plain HTTP route at `/bundles/z-Critical.css` (or `/bundles/z-Critical.min.css` when minification is enabled). This endpoint is served with full caching headers and can be useful for debugging or inspecting the inlined content.

---

## 🔥 Critical JS (v1.2.0+)

- Combines all `.js` files under `wwwroot/js/critical/` into one `<script>` tag.
- Injected **before** all other scripts for optimal early execution.
- JS is validated via NUglify. Files with syntax errors are skipped with a `/* File X Skipped: reason */` comment in their place.

> **Note:** Critical JS is also accessible as a plain HTTP route at `/bundles/z-Critical.js` (or `/bundles/z-Critical.min.js`). Same as for Critical CSS — useful for inspection, not intended as the primary delivery mechanism.

---

## 🆕 What's New in v2.1.0

### Removed external parser dependencies

`LayerZero.Tools.Web` is now fully self-contained:

- **Removed `LigerShark.WebOptimizer.Core`** — replaced with a built-in `BundleStore` (lazy `ConcurrentDictionary` cache) and `BundleServingMiddleware`. Update `app.UseWebOptimizer()` → `app.UseBundleServing()`.
- **Removed `AngleSharp.Css`** — CSS parsing and formatting replaced with `NUglify`. Critical CSS is now minified inline rather than pretty-printed.
- **Removed `Esprima`** — JS syntax validation replaced with `NUglify`. Behaviour is identical: valid JS is inlined as-is; invalid JS produces a skip comment.
- **Removed `CssFileParser` / `JsFileParser`** (`LayerZero.Tools.Web.Parser`) — internal helpers superseded by `BundleStore.Build()`. If you referenced these classes directly, remove those usages.

> **Migration note:** Replace `app.UseWebOptimizer()` with `app.UseBundleServing()` in your `Program.cs`.

---

## 🆕 What's New in v1.3.0

LayerZero.Tools.Web added **Critical JavaScript** support:

- Place scripts in `wwwroot/js/critical/`
- Files are parsed and rendered inline, **before all standard JS bundles**
- Useful for early execution logic such as feature flags, layout adjustments, or performance-critical bootstraps

> Critical JS handling mirrors Critical CSS introduced in `v1.2.0`, forming a complete early asset delivery strategy.

---

## 🚫 Cache-Busting in Development

To prevent browser caching during local testing, development mode appends `?v=<random>` to asset URLs.

```html
<link rel="stylesheet" href="/bundles/home.min.css?v=46174bc4-f61a-4382-a733-81ffe8c73074" />
```

In production, clean URLs are used for optimal caching.

---

## ✨ Example

Requesting `/Home/Index` loads:

```html
<style>/* critical CSS injected here */</style>
<link rel="stylesheet" href="/bundles/home.min.css" />
<link rel="stylesheet" href="/bundles/home/index.min.css" />
<script>/* critical JS injected here */</script>
<script src="/bundles/home.min.js"></script>
<script src="/bundles/home/index.min.js"></script>
```

---

## 🛣 Upcoming

- ✅ Custom `JsRoot`, `CssRoot`, `CriticalCssRoot` directories.
- ✅ Optional feature toggles for minification, cache-busting, critical asset control.
- ✅ Fluent configuration syntax.

```csharp
builder.Services.AddDynamicBundle(new DynamicBundleConfig
{
    JsRoot = "wwwroot/assets/js",
    CssRoot = "wwwroot/assets/css",
    CriticalCssRoot = "wwwroot/assets/critical",
    EnableCacheBusting = true
});
```

---

## 👤 Author

**LayerZero Team** — Built for clean architecture and developer clarity.
