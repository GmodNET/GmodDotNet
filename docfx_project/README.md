# How to build docs

Docs must be build with [DocFX Version 2.58](https://github.com/dotnet/docfx) or greater. DocFX version 3.x is not (yet) supported.

Build instructions (given that DocFX is in your `PATH`):

To build docs (compiled docs artifacts will be in the `_site` folder)
```
[Path to GmodDotNet repo]\docfx_project> docfx .\docfx.json
```

To host a local http server and preview docs locally:
```
[Path to GmodDotNet repo]\docfx_project> docfx .\docfx.json --serve
```
