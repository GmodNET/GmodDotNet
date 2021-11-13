---
uid: net-sdk-cli
title: "Using .NET SDK CLI and Templates"
---

# Using .NET SDK CLI and Templates

.NET SDK ships with a convenient [Command-Line Interface (or simply CLI)](https://docs.microsoft.com/en-us/dotnet/core/tools/) which allows one to create, manage,
and build .NET projects on any operation system without Visual Studio at all.
Since Gmod.NET modules are full-featured .NET class libraries, you can use command line to work with them as well.
This article will describe how to create and build Gmod.NET modules with .NET SDK CLI.

## Using Gmod.NET templates with .NET SDK CLI

.NET SDK features a powerful [templating engine](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new), which allows developers to create common types of .NET projects without need to write boilerplate code.
We, as GmodNET org, maintain our own [templates](https://github.com/GmodNET/Templates). You can use them to create new Gmod.NET modules without too much of a hassle.
Here how you can obtain and use our templates.

1. GmodNET templates are shiped as a NuGet package and can be obtained from NuGet.org.
To install templates on you local machine, run the following command:
```bash
dotnet new --install GmodNET.Templates
```
2. `GmodNET.Templates` will install several Gmod.NET templates, such as `gmodnet-module` and `gmodnet-module-web`.
You can then create new Gmod.NET module projects by running the following command in the project folder:
```bash
dotnet new gmodnet-module
```
3. Resulting C# projects can be opened by Visual Studio, Visual Studio Code, or any other editor of your choice, or simply managed by .NET SDK CLI.
4. We will update our templates from time to time.
In order to get latest version of `GmodNET.Templates` you should use `dotnet new --update-check` and `dotnet new --update-apply` commands.
5. Templates can also be uninstalled by `dotnet new --uninstall GmodNET.Templates` command.

## Building Gmod.NET modules with .NET SDK CLI

You can use .NET SDK CLI to build Gmod.NET modules which can be safely moved across computers.

1. Navigate to the root folder of your Gmod.NET module project (the one where `*.csproj` file locates).
2. Run the command
```bash
dotnet publish -c Release
```
to build your module in `Release` configuration or run
```bash
dotnet publish -c Debug
```
to compile it as a Debug build.

3. The resulting build is usually located in `[your_module_project_folder]/bin/Release/net6.0/publish/` or `[your_module_project_folder]/bin/Debug/net6.0/publish/` folders.
The content of the `publish` directory is a build of your Gmod.NET module together with all dependencies (both managed and native) which could be safely moved
to `garrysmod/lua/bin/Modules/` or even other computer without fear of missing some dependency or configuration file.

>[!NOTE]
>.NET projects can also build with `dotnet build` command, but unlike `dotnet publish`,
`dotnet build` will not copy dependencies and miscellaneous files to the output folder, storing them in the build cache instead.
Thus, the output of the `dotnet build` can considered incomplete and may not work if moved between different machines.
Therefore we urge you to always use `dotnet publish` instead of `dotnet build`, unless you really know what are you doing.

Check out the official Microsoft docs on `dotnet publish`: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish
