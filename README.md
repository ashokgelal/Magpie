## Magpie

A modern software update framework for .net applications.

#### Master Branch Status:

[![Build status](https://ci.appveyor.com/api/projects/status/a5t0tq8i5y5q0ixi/branch/master?svg=true)](https://ci.appveyor.com/project/ashokgelal/magpie/branch/master)

![Update Available Screenshot](https://github.com/ashokgelal/Magpie/blob/master/screenshots/lp_screenshot.png)

![Download Screenshot](https://github.com/ashokgelal/Magpie/blob/master/screenshots/lp_download_screenshot.png)

### Features:

* **Based on WPF** - modern, beautiful and stylish
* **JSON based appcast** - no ugly xml
* **Markdown based release notes** - because there is no reason to use any other format
* **Looks great out of the box** - no need to write your own CSS
* **Very simple APIs** - needs 2 lines of extra code to get started
* **Very flexible APIs** - provides plenty of hooks and raises right events at the right time
* **Built-in logging for quick debugging** - plug-in a logger of your choice to see what's going on behind the scene
* **Built-in analytics** - measure the effectiveness of your updates by plugging-in your analytics logger to capture important events
* **Minimal dependencies** - one 1 external dependency (for Markdown parsing)

### Installing

Use [Magpie nuget package](https://www.nuget.org/packages/Magpie/1.0.7-beta):

`PM> Install-Package Magpie -Pre`

### Using Magpie:

To use Magpie in your project, you only need to interact with `MagpieService` class. Here are basic steps:

1) Create an instance of `AppInfo` class:

```csharp
var appInfo = new AppInfo("<url to appcast.json>");
```

2) Now, to run Magpie in the background:

```csharp
new MagpieService(appInfo).CheckInBackground();
```

3) To force check for updates (like via a 'Check for Updates' menu item):

```csharp
new MagpieService(appInfo).ForceCheckInBackground();
```

#### Publishing updates:

You add some basic information to a json file and publish it somewhere publicly accessible. The json appcast should contain atlas the following information:

```json
{
  "title": "App Title",
  "version": "x.y.z",
  "release_notes_url": "https://raw.githubusercontent.com/ashokgelal/Magpie/master/README.md",
  "artifact_url": "https://github.com/ashokgelal/Magpie/tree/master/installer.msi"
}
```

Obviously, `release_notes_url` and `artifact_url` should be somewhere publicly accessible as well. You can add extra information to this appcast file and can access those values later from `RemoteAppcast`'s `RawDictionary` property. 

#### Providing a logo:

1) Create an instance of `AppInfo` class:

```csharp
var appInfo = new AppInfo("<url to appcast.json>");
```

2) Call `SetAppIcon()` method with a namespace of your project that contains the logo, and the name of the logo itself:

```csharp
appInfo.SetAppIcon("<namespace of your project>", "<yourlogo.png>");
```

Look at `Magpie.Example` project for a demo application.

#### Hooking up analytics:

When instantiating MagpieService, you can also pass an instance `IAnalyticsLogger` if you want to log different user actions such as logging download, remind me later, skip etc. `Magpie` comes with an empty implementation of `IAnalyticsLogger` called `AnalyticsLogger` that you can extend and hook-in your own logic for actual logging of different events. We actually recommend that you extend `AnalyticsLogger` instead of extending `IAnalyticsLogger` interface. If we add more events to `IAnalyticsLogger`, and we will, it won't break your application.

### TODO

- [x] Build on AppVeyor
- [x] Markdown support for Release Notes
- [x] Download installer and allow to open it
- [x] Custom CSS Support (contributed by [mariannabudnikova](https://github.com/mariannabudnikova))
- [x] Force check
- [x] Notify "No Updates Available" (contributed by [antistrongguy](https://github.com/antistrongguy))
- [x] Create nuget package
- [x] Add XML docs
- [x] Analytics interface
- [ ] Validate signature of download files (see: [issue #19](https://github.com/ashokgelal/Magpie/issues/19))
- [ ] Implement a debugging window
- [ ] Add more tests
