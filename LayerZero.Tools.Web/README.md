# 📦 Dynamic Bundle Loader for ASP.NET Core
[![NuGet](https://img.shields.io/nuget/v/LayerZero.Tools.Web.svg)](https://www.nuget.org/packages/LayerZero.Tools.Web)
[![NuGet Downloads](https://img.shields.io/nuget/dt/LayerZero.Tools.Web.svg)](https://www.nuget.org/packages/LayerZero.Tools.Web)
![.NET](https://img.shields.io/badge/.NET-8.0-blue)

---

A convention-based asset bundling system for .NET 8+ using WebOptimizer. Automatically discovers and injects CSS/JS bundles per controller and action using Razor TagHelpers.

> 🧩 Not a full framework.\
> ❌ Doesn’t replace WebOptimizer.\
> ✅ Enhances it with dynamic discovery, layout scoping, and critical asset control.

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
dotnet new classlib -n LayerZero.Tools.Web
dotnet add reference ../LayerZero.Tools.Web/LayerZero.Tools.Web.csproj
```

### 2. NuGet Dependencies

```xml
<PackageReference Include="LigerShark.WebOptimizer.Core" Version="3.0.456" />
<PackageReference Include="LigerShark.WebOptimizer.Core" Version="3.0.456" />
<FrameworkReference Include="LayerZero.Tools" Version="1.0.1"/>
```

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
app.UseWebOptimizer();
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
✅ Inline critical CSS\
✅ Inline critical JS\
✅ Cache-busting in development mode

---

## ⚠️ Known Limitations

- ❌ **Custom asset folder paths** are *not* configurable via `AddDynamicBundle()` same as `v1.1.0`.
- ❌ **Dynamic runtime configuration** of asset logic is not exposed yet.
- ✅ A static convention-based pathing system is in place (e.g., `wwwroot/css/Controller/Action/...`).

These constraints persist in `v1.2.0` and will be addressed in `v2.0.0`.

---

## 🔥 Critical CSS (v1.1.0+)

- Combines all `.css` files under `wwwroot/css/critical/` into a single `<style>` tag.
- Injected above all other stylesheets.

---

## 🔥 Critical JS (v1.2.0+)

- Combines all `.js` files under `wwwroot/js/critical/` into one `<script>` tag.
- Injected **before** all other scripts for optimal early execution.
- In `v1.2.0`, scripts are injected as-is — no syntax validation or dependency analysis is performed yet.

---

## 🆕 What's New in v1.3.0

LayerZero.Tools.Web now includes **Critical JavaScript** support:

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

## 🛣 Planned for v2.0.0

A new configuration object will be introduced to allow:

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