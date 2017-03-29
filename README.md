# Codon
Codon is a zero-dependency cross-platform MVVM framework 
for creating UWP, WPF, and Xamarin based applications. 
It provides much of what you need to rapidly create sophisticated 
yet maintainable applications. 

Most of Codon's code-base is located in .NET Standard libraries, 
allowing them to referenced from just about anywhere. Where features require platform specific implementation, 
platform specific libraries complement the .NET Standard libraries.

Codon includes two main libraries: a minimal core library and an extras library.
Codon's core includes an easy to use ICommand implementation, frictionless INPC, a cross-platform settings service, IoC and DI, and a weak referencing pub/sub messenger. 

Codon Extras includes a user options system, form input validation, 
an undo/redo task system, application state preservation, and various cross-platform launchers for sharing links, sending emails and so forth.

The minimal core and extras libraries are developed on independent timelines, 
with the core experiencing fewer API changes.

In addition to the Codon.Extras library, 
Codon includes a data-binding library, Codon.UI.Data, 
for use with non-XAML based technologies such as Xamarin.Android and Xamarin.iOS;
and an Undo-Redo system located in the Codon.UndoModel assembly.
Codon.UndoModel is downloadable as an independent package.

# Support
- Join our [Slack Channel](https://codonfx.slack.com)
- For general questions and support, post your questions on [StackOverflow](http://stackoverflow.com/questions/tagged/codon)

# NuGet Packages

Codon packages may be installed invidually, or you may choose
to reference a dependency package. The two dependency packages
are [Codon.Essentials][2], which references the .NET Standard core package
along with its platform specific support libraries; and [Codon.Extras][7]
which references the .NET Standard Extras package 
along with its platform specific support libraries.

If you're just getting started and are unsure about what
package to install, choose [Codon.Essentials][2].

### Core Packages

These are Codon's .NET Standard core packages, 
together with the platform specific support packages.

| Platform | Assembly | Package | Version |
| -------- | -------- | ------- | ------- |
| .NET Standard | Codon.dll | [Codon][1] | [![21]][1] |
| WPF | Codon.Platform.dll | [Codon.Wpf][6] | [![26]][6] |
| UWP | Codon.Platform.dll | [Codon.Uwp][5] | [![25]][5] |
| Xamarin.Android | Codon.Platform.dll | [Codon.Android][3] | [![23]][3] |
| Xamarin.iOS | Codon.Platform.dll | [Codon.Ios][4] | [![24]][4] |
| .NET Standard | Codon.Extras.dll | [Codon.Extras][7] | [![27]][7] |
| WPF | Codon.Extras.Platform.dll | [Codon.Extras.Wpf][12] | [![32]][12] |
| UWP | Codon.Extras.Platform.dll | [Codon.Extras.Uwp][11] | [![25]][11] |
| Xamarin.Android | Codon.Extras.Platform.dll | [Codon.Extras.Android][9] | [![29]][9] |
| Xamarin.iOS | Codon.Extras.Platform.dll | [Codon.Extras.Ios][10] | [![30]][10] |
| .NET Standard | Codon.UI.Data.dll | [Codon.UI.Data][13] | [![33]][13] |
| .NET Standard | Codon.UndoModel.dll | [Codon.UndoModel][14] | [![34]][14] |

[1]: https://www.nuget.org/packages/Codon/
[2]: https://www.nuget.org/packages/Codon.Essentials/
[3]: https://www.nuget.org/packages/Codon.Android/
[4]: https://www.nuget.org/packages/Codon.Ios/
[5]: https://www.nuget.org/packages/Codon.Uwp/
[6]: https://www.nuget.org/packages/Codon.Wpf/
[7]: https://www.nuget.org/packages/Codon.Extras/
[8]: https://www.nuget.org/packages/Codon.Extras.Core/
[9]: https://www.nuget.org/packages/Codon.Extras.Android/
[10]: https://www.nuget.org/packages/Codon.Extras.Ios/
[11]: https://www.nuget.org/packages/Codon.Extras.Uwp/
[12]: https://www.nuget.org/packages/Codon.Extras.Wpf/
[13]: https://www.nuget.org/packages/Codon.UI.Data/
[14]: https://www.nuget.org/packages/Codon.UndoModel/

[21]: https://img.shields.io/nuget/vpre/Codon.svg
[22]: https://img.shields.io/nuget/vpre/Codon.Essentials.svg
[23]: https://img.shields.io/nuget/vpre/Codon.Android.svg
[24]: https://img.shields.io/nuget/vpre/Codon.Ios.svg
[25]: https://img.shields.io/nuget/vpre/Codon.Uwp.svg
[26]: https://img.shields.io/nuget/vpre/Codon.Wpf.svg
[27]: https://img.shields.io/nuget/vpre/Codon.Extras.svg
[28]: https://img.shields.io/nuget/vpre/Codon.Extras.Core.svg
[29]: https://img.shields.io/nuget/vpre/Codon.Extras.Android.svg
[30]: https://img.shields.io/nuget/vpre/Codon.Extras.Ios.svg
[31]: https://img.shields.io/nuget/vpre/Codon.Extras.Uwp.svg
[32]: https://img.shields.io/nuget/vpre/Codon.Extras.Wpf.svg
[33]: https://img.shields.io/nuget/vpre/Codon.UI.Data.svg
[34]: https://img.shields.io/nuget/vpre/Codon.UndoModel.svg