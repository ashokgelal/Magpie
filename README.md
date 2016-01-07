## Magpie

A modern software update framework for .net applications.

#### Master Branch Status:

[![Build status](https://ci.appveyor.com/api/projects/status/a5t0tq8i5y5q0ixi/branch/master?svg=true)](https://ci.appveyor.com/project/ashokgelal/magpie/branch/master)

![Update Available Screenshot](https://github.com/ashokgelal/Magpie/blob/master/screenshots/lp_screenshot.png)

![Download Screenshot](https://github.com/ashokgelal/Magpie/blob/master/screenshots/lp_download_screenshot.png)

### Installing

Use MetaGeek.Magpie nuget package

`PM> Install-Package Magpie -Pre`

### Using Magpie

To use Magpie in your project, you only need to interact with `MagpieService` class. Here are basic steps:

1. Create an instance of `AppInfo` class:
`var appInfo = new AppInfo("<url to appcast.json>");`
2. Now, to run Magpie in the background:
`new MagpieService(appInfo).CheckInBackground();`
3. To force check for updates (like via a 'Check for Updates' menu item):
`new MagpieService(appInfo).CheckInBackground();`

#### Using your own logo in the update window:

1. Create an instance of `AppInfo` class:
`var appInfo = new AppInfo("<url to appcast.json>");`
2. Call `SetAppIcon()` method with a namespace of your project that contains the logo, and the name of the logo itself:
`appInfo.SetAppIcon("<namespace of your project>", "<yourlogo.png>");`

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

