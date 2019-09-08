# About PostShapr Aspects

This project contains a number of PostSharp "Aspects".

"Aspect" is a term used in Apect Oriented Programming (AOP) that refers to a specially constructed class that can be injected into user code after compilation has been performed. This allows "boiler plate" code and design patterns to be added declaratively, keeping user code cleaner and simpler.

`PostSharp` is an AOP framework and compiler for .NET projects. The `PostSharp` compiler runs after the .NET compiler and "weaves" aspects into the already compiled user code. Everything needed for this to work is contained in the `PostSharp.*` NuGet packages. There is a Visual Studio extension that adds some Intellisense and designer support, but the extension is optional and is not needed for PostSharp to work.

Common uses for AOP are:
- to declaratively introduce logging into classes that otherwise don't have it
- to implement standard patterns patterns such as `INotifyPropertyChanged`
- to enforce certain threading models and patterns, such as Reader-Writer Lock
- to add code contracts to method calls

It is also possible to produce user-written custom aspects. 

Aspects are implemented as .NET attributes, and applying an aspect to an assembly, class or class member is as easy as applying an attribute. For example, data models can be made to implement `INotifyPropertyChanged` simply by applying the `[NotifyPropertyChanged]` attribute to the class.

## Solution Aspect Usage

This solution uses the `[ReaderWriterSynchronized`] threading pattern to implement a Reader-Writer Lock within the `ClientConnectionmanager` class. It also defines some custom aspects that are useful in ASCOM drivers, such as `MustBeConnectedAttribute` and `NLogTraceWithArgumentsAttribute`. These are described in following paragraphs.

## PostSharp Licensing

`PostSharp Essentials` is free and provides unlimited logging and all other patterns on up to 10 classes per solution. This will often be enough for an ASCOM driver project if aspects are used judiciously, as is the case in this solution.

If you do not wish to use PostSharp for any reason, simply uninstall the NuGet packages. You will then have to add equivalent code to your projects for any PostSharp aspects that were in use. These are documented below.

## `ReaderWriterSynchronized` Threading Model

This is used to protect the `ClientConnectionmanager` class from race conditions.

If PostSharp is removed from the solution, then an alternative solution must be found. There are numberous articles on implementing the Reader-Writer Lock pattern, [including this one][ReaderWriterLock].

## `MustBeConnected` Aspect

This aspect attribute can be applied to any method or property of an ASCOM driver that implements the `IAscomDriver` marker interface. When applied to a property or method, that property or method will check the value of the `Connected` property and will throw a `NotConnectedException` if it is false.

The aspect is coded so that nested calls to properties and methods only check the `Connected` property once at the start. This was done partly for efficience and partly so that log output is more readable.

This aspect is used in the `Switch` class on the `Action`, `GetSwitch`, `GetSwitchValue`, `SetSwitch` and `SetSwitchValue` methods. If postSharp is removed, these methods should manually check the `Connected` property.

## `NLogTraceWithArguments` Aspect

This aspect can be applied to any class and will inject logging of method calls (with argument values) and returns (with return values) into every property and method of that class.

We use this to inject logging into driver classes so that all driver calls are logged, along with argument values and return values.

[ReaderWriterLock]: https://www.c-sharpcorner.com/UploadFile/1d42da/readerwriterlock-class-in-C-Sharp-threading/ "CSharp Corner: ReaderWriterLock Class in C# Threading"