# 📦 Dynamic Bundle Loader for ASP.NET Core

A convention-based asset bundling system for .NET 8+ using WebOptimizer. Automatically discovers and injects CSS/JS bundles per controller and action using Razor TagHelpers.

---

## 🔍 Purpose

Eliminates manual asset management in Razor views by scanning controller/action folder structures and auto-generating optimized bundles at runtime.

---

## 🗂 Folder Convention

```
wwwroot/
├── js/
│   └── controllers/
│       ├── Home/
│       │   ├── index.js
│       │   └── details.js
│       └── Dashboard/
│           └── overview.js
└── css/
    └── controllers/
        ├── Home/
        │   ├── index.css
        │   └── shared.css
        └── Dashboard/
            └── overview.css
```

`Controller/Action` structure drives bundle discovery.

---

## ⚙️ Installation

### 1. Add the NuGet Package

```
dotnet add package LayerZero.Tools.Web
```

Or reference the project directly:

```bash
dotnet new classlib -n DynamicBundleLoader
dotnet add reference ../DynamicBundleLoader/DynamicBundleLoader.csproj
```

### 2. NuGet Dependencies

```xml
<PackageReference Include="LigerShark.WebOptimizer.Core" Version="3.0.456" />
<FrameworkReference Include="Microsoft.AspNetCore.App" />
```

---

## 🚀 Usage

### Register in `Program.cs`

```csharp
builder.Services.AddSingleton(DynamicBundleMapper.Bundles);
builder.Services.AddWebOptimizer(pipeline =>
{
    DynamicBundleMapper.Register(pipeline, isDevelopment: app.Environment.IsDevelopment());
});
```

### Enable Middleware

```csharp
app.UseWebOptimizer();
```

### Use Custom Asset Folder Paths

```csharp
builder.Services.AddWebOptimizer(pipeline =>
{
    DynamicBundleMapper.Register(pipeline, JsRoot: "assets/js", CssRoot: "assets/styles", isDevelopment: app.Environment.IsDevelopment());
});
```

> Asset paths are relative to `wwwroot/`.

---

## 🧠 TagHelpers

### Register in `_ViewImports.cshtml`

```cshtml
@addTagHelper *, LayerZero.Tools.Web
```

### Use in `_Layout.cshtml`

```cshtml
<head>
    <style-bundle-loader />
</head>
<body>
    @RenderBody()
    <script-bundle-loader />
</body>
```

> Action-specific bundles override controller-wide ones.

---

## 💡 Features

✅ Convention-over-configuration  
✅ Minification only in production  
✅ Controller & action bundle granularity  
✅ TagHelpers for clean layout injection  
✅ Auto-registers bundles at startup  
✅ Supports custom asset folder paths  
✅ Inline critical CSS (with skip support)  
✅ Cache-busting in development mode

---

## 🔒 Minification Mode

Minification is automatically applied **only in production**. When `isDevelopment` is true, JS/CSS are included unminified for easier debugging.

---

## 🔥 Critical CSS (v1.1.0+)

- Combines all `.css` files under `wwwroot/css/critical/` into a single `<style>` tag.
- Injected above all other stylesheets.
- Skippable per-action using:

```csharp
[DisableCriticalCss]
public IActionResult MyView() => View();
```

Or globally via the `Register` toggle (v1.1.1+).

---

## 🚫 Cache-Busting in Development

To prevent browser caching during local testing, development mode appends `?v=<random>` to asset URLs.

```html
<link rel="stylesheet" href="/bundles/home.min.css?v=202406160915" />
```

In production, clean URLs are used for optimal caching.

---

## ✨ Example

Requesting `/Home/Index` loads:

```html
<style>/* critical CSS injected here */</style>
<link rel="stylesheet" href="/bundles/home.min.css" />
<link rel="stylesheet" href="/bundles/home/index.min.css" />
<script src="/bundles/home.min.js"></script>
<script src="/bundles/home/index.min.js"></script>
```

---

## 🧭 Roadmap

- Asset versioning mode selection (assembly version, timestamp, none) → v2.0.0  
- Deduplication of critical/main CSS  
- Inline critical JS  
- DevTools warning system for library dependencies  
- CLI asset validator  
- Razor directives for explicit override

---

## 👤 Author

**LayerZero Team** — Built for clean architecture and developer clarity.
