# Calcium
Calcium is a zero-dependency cross-platform framework 
for creating UWP, WPF, and Xamarin based applications. 
It provides much of what you need to rapidly create sophisticated 
yet maintainable applications. 

Most of Calcium's code-base is located in .NET Standard libraries, 
allowing them to referenced from just about anywhere. Where features require platform specific implementation, 
platform specific libraries complement the .NET Standard libraries.

Calcium includes two main libraries: a minimal core library and an extras library.
Calcium's core includes an easy to use ICommand implementation, frictionless INPC, a cross-platform settings service, IoC and DI, and a weak referencing pub/sub messenger. 

Calcium Extras includes a user options system, form input validation, 
an undo/redo task system, application state preservation, and various cross-platform launchers for sharing links, sending emails and so forth.

The Essentials and Extras libraries are developed on independent timelines, 
with the Essentials experiencing fewer API changes.

In addition to the Calcium.Extras library, 
Calcium includes a data-binding library, Calcium.UI.Data, 
for use with non-XAML based technologies such as Xamarin.Android and Xamarin.iOS;
and an Undo-Redo system located in the Calcium.UndoModel assembly.
Calcium.UndoModel is downloadable as an independent package.

Read the [Getting Started Guide](http://CalciumFramework.com/Guides/001_GettingStarted/).

# Support
- You can post bugs and feature requests in our [Issues](https://github.com/CalciumFramework/Calcium/issues).
- Join our [Slack Channel](https://CalciumFramework.slack.com)
- For general questions and support, post your questions on [StackOverflow](http://stackoverflow.com/questions/tagged/calcium)

# NuGet Packages

Calcium packages may be installed invidually, or you may choose
to reference a dependency package. The two dependency packages
are [Calcium.Essentials][2], which references the .NET Standard core package
along with its platform specific support libraries; and [Calcium.Extras][7]
which references the .NET Standard Extras package 
along with its platform specific support libraries.

If you're just getting started and are unsure about what
package to install, choose [Calcium.Essentials][2].

### Packages

These are Calcium's .NET Standard packages, 
together with the platform specific support packages.

| Platform | Assembly | Package | Version |
| -------- | -------- | ------- | ------- |
| .NET Standard | Calcium.dll | [Calcium][1] | [![21]][1] |
| WPF | Calcium.Platform.dll | [Calcium.Wpf][6] | [![26]][6] |
| WPF .NET Core | Calcium.Platform.dll | [Calcium.WpfCore][15] | [![35]][15] |
| UWP | Calcium.Platform.dll | [Calcium.Uwp][5] | [![25]][5] |
| Xamarin.Android | Calcium.Platform.dll | [Calcium.Android][3] | [![23]][3] |
| Xamarin.iOS | Calcium.Platform.dll | [Calcium.Ios][4] | [![24]][4] |
| .NET Standard | Calcium.Extras.dll | [Calcium.Extras][7] | [![27]][7] |
| WPF | Calcium.Extras.Platform.dll | [Calcium.Extras.Wpf][12] | [![32]][12] |
| WPF .NET Core | Calcium.Extras.Platform.dll | [Calcium.Extras.Wpf][16] | [![36]][16] |
| UWP | Calcium.Extras.Platform.dll | [Calcium.Extras.Uwp][11] | [![25]][11] |
| Xamarin.Android | Calcium.Extras.Platform.dll | [Calcium.Extras.Android][9] | [![29]][9] |
| Xamarin.iOS | Calcium.Extras.Platform.dll | [Calcium.Extras.Ios][10] | [![30]][10] |
| .NET Standard | Calcium.UI.Data.dll | [Calcium.UI.Data][13] | [![33]][13] |
| .NET Standard | Calcium.UndoModel.dll | [Calcium.UndoModel][14] | [![34]][14] |

[1]: https://www.nuget.org/packages/Calcium/
[2]: https://www.nuget.org/packages/Calcium.Essentials/
[3]: https://www.nuget.org/packages/Calcium.Android/
[4]: https://www.nuget.org/packages/Calcium.Ios/
[5]: https://www.nuget.org/packages/Calcium.Uwp/
[6]: https://www.nuget.org/packages/Calcium.Wpf/
[7]: https://www.nuget.org/packages/Calcium.Extras/
[8]: https://www.nuget.org/packages/Calcium.Extras.Core/
[9]: https://www.nuget.org/packages/Calcium.Extras.Android/
[10]: https://www.nuget.org/packages/Calcium.Extras.Ios/
[11]: https://www.nuget.org/packages/Calcium.Extras.Uwp/
[12]: https://www.nuget.org/packages/Calcium.Extras.Wpf/
[13]: https://www.nuget.org/packages/Calcium.UI.Data/
[14]: https://www.nuget.org/packages/Calcium.UndoModel/
[15]: https://www.nuget.org/packages/Calcium.WpfCore/
[16]: https://www.nuget.org/packages/Calcium.Extras.WpfCore/

[21]: https://img.shields.io/nuget/vpre/Calcium.svg
[22]: https://img.shields.io/nuget/vpre/Calcium.Essentials.svg
[23]: https://img.shields.io/nuget/vpre/Calcium.Android.svg
[24]: https://img.shields.io/nuget/vpre/Calcium.Ios.svg
[25]: https://img.shields.io/nuget/vpre/Calcium.Uwp.svg
[26]: https://img.shields.io/nuget/vpre/Calcium.Wpf.svg
[27]: https://img.shields.io/nuget/vpre/Calcium.Extras.svg
[28]: https://img.shields.io/nuget/vpre/Calcium.Extras.Core.svg
[29]: https://img.shields.io/nuget/vpre/Calcium.Extras.Android.svg
[30]: https://img.shields.io/nuget/vpre/Calcium.Extras.Ios.svg
[31]: https://img.shields.io/nuget/vpre/Calcium.Extras.Uwp.svg
[32]: https://img.shields.io/nuget/vpre/Calcium.Extras.Wpf.svg
[33]: https://img.shields.io/nuget/vpre/Calcium.UI.Data.svg
[34]: https://img.shields.io/nuget/vpre/Calcium.UndoModel.svg
[35]: https://img.shields.io/nuget/vpre/Calcium.WpfCore.svg
[36]: https://img.shields.io/nuget/vpre/Calcium.Extras.WpfCore.svg
