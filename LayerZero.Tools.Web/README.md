# 📦 Dynamic Bundle Loader for ASP.NET Core

A convention-based asset bundling system for .NET 8+ using WebOptimizer. Automatically discovers and injects CSS/JS bundles per controller and action using Razor TagHelpers.

## 🔍 Purpose

Eliminates manual asset management in Razor views by scanning controller/action folder structures and auto-generating optimized bundles at runtime.

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

## ⚙️ Installation

1. Create and reference the library project

```
dotnet new classlib -n DynamicBundleLoader
```

Add the reference in your web app:

```
dotnet add reference ../DynamicBundleLoader/DynamicBundleLoader.csproj
```

2. NuGet Dependencies

```xml
<PackageReference Include="LigerShark.WebOptimizer.Core" Version="3.0.456" />
<FrameworkReference Include="Microsoft.AspNetCore.App" />
```

## 🚀 Usage

__Register bundles in `Program.cs`__

```csharp
builder.Services.AddSingleton(DynamicBundleMapper.Bundles);
builder.Services.AddWebOptimizer(pipeline =>
{
    DynamicBundleMapper.Register(pipeline);
});
```

__Middleware setup__

Ensure the WebOptimizer middleware is added:

```
app.UseWebOptimizer();
```

__Custom asset folder paths__

If you use a custom structure:

```csharp
builder.Services.AddWebOptimizer(pipeline =>
{
    DynamicBundleMapper.Register(pipeline, JsRoot: "assets/js", CssRoot: "assets/styles");
});
```

Make sure the paths are relative to `wwwroot/`. Absolute paths or incorrect base folders will result in missing bundles.

## ⚠️ CSS Path Warning

When using relative URLs in CSS (e.g. `url('../images/icon.svg')`), remember:

Bundles are served from `/bundles/...`

Asset references like images or fonts must resolve correctly from the bundle's URL, not the source folder.

Fix: Use root-relative paths (`/images/icon.svg`) or ensure your build pipeline rewrites paths.

## 🧠 TagHelpers

Add to `_ViewImports.cshtml`

```cshtml
@addTagHelper *, DynamicBundleLoader
```

In `_Layout.cshtml`

```cshtml
<head>
    <style-bundle-loader />
</head>
<body>
    @RenderBody()
    <script-bundle-loader />
</body>
```

Action-specific bundles override controller-wide ones (loaded after).

## 📦 Features

✅ Convention-over-configuration

✅ Minification via WebOptimizer

✅ Controller & action bundle granularity

✅ TagHelpers for clean layout injection

✅ Auto-registers bundles at startup

✅ Supports custom asset folder paths

## ✨ Example

Requesting `/Home/Index` loads:

```html
<link rel="stylesheet" href="/bundles/home.min.css" />
<link rel="stylesheet" href="/bundles/home/index.min.css" />
<script src="/bundles/home.min.js"></script>
<script src="/bundles/home/index.min.js"></script>
```

## 🔮 Roadmap (optional additions)

Asset versioning (?v=hash)

Inline critical CSS

TagHelper preload hints

Razor directives for explicit override

## 👤 Author

LayerZero Team – Built for clean architecture and developer clarity.