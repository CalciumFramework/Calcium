# Codon
Codon is a zero-dependency cross-platform MVVM framework 
for creating UWP, WPF, and Xamarin applications. 
It provides much of what you need to rapidly create sophisticated 
yet maintainable applications. 

Most of Codon's code-base is located in .NET Standard 1.4 libraries, 
allowing them to referenced from just about anywhere.

Codon includes two main libraries: a minimal core library and an extras library.
Codon's core includes an easy to use ICommand implementation, frictionless INPC, a cross-platform settings service, IoC and DI, and a weak referencing pub/sub messenger. 
Codon Extras includes a User Options system, Form Input Validation, an Undo/Redo Task system, application State Preservation, and various cross-platform launchers for sharing links, sending emails and so forth.

Where features require platform specific implementation, 
platform specific libraries complement the .NET Standard libraries.

The minimal core and extras are developed on independent timelines, 
with the core experiencing fewer API changes.

In addition to the Codon.Extras library, 
Codon includes a data-binding library, Codon.UI.Data, 
for use with non-XAML based technologies such as Xamarin.Android and Xamarin.iOS;
and an Undo-Redo system located in the Codon.UndoModel assembly.
Codon.UndoModel is downloadable as an independent package.

# Support
- Join our [Slack Channel](https://codonfx.slack.com)
- For general questions and support, post your questions on [StackOverflow](http://stackoverflow.com/questions/tagged/codon)
