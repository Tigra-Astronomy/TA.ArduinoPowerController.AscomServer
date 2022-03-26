# Arduino Power Controller with ASCOM Switch Driver

![Arduino Power Controller][image]

This driver and its source code and the accompanying Arduino Sketch are licensed under the [Tigra MIT license][license]. Essentially, anyone can do anything at all with this software, including commercially, without restriction. Anything you do is your responsibility and not the author's.

For further information, please visit the [Project Home Page][project] or the [Git Source Repository][source].

## Project Description

This project serves a dual purpose. It is a fully functional turnkey solution designed to enable easy control of power to astronomical instruments and ancillary equipment in an observatory. At the same time, it can form a learning platform and a jumping-off point for other new drivers. The Switch code can be gradually replaced with new code and if done with care, it is possible to keep a working build at all times. In the past, driver development began with a mostly empty template project which didn't offer much guidance on how to get things done or demonstrate good architectural choices. The author discovered, after developing over a dozen ASCOM drivers, that it was actually easier to start from an already fully functional project and add new code from there.

To begin work on your own driver, we recommend forking this project on GitHub, building it on your own system and then striking out towards your new driver.

The project consists of a an ASCOM Switch driver, plus an Arduino sketch. The two parts of the project communicate via the USB interface, which acts as a virtual COM port. The module can be controlled via the ASCOM Switch interface from any planetarium or observatory automation software that supports the ASCOM `ISwitch` interface. Alternatively, the driver can be used directly from a PowerShell script or any Windows scripting engine that can load a COM object. 

The serves as a base implementation that can be extended to build other drivers and to understand the operation of in-process (COM LocalServer) drivers.

### Cautions and Limitations

We have tested the unit while switching 12 volt power supplies. The limitations on voltage and current will depend on the relay module used. Commonly available modules have relays rated for up to 10 Amps and up to 250 volts AC. However, switching mains voltages and/or high current loads can be hazardous. A simple mistake can result in destruction of equipment, fire, electrocution and death. **_Therefore we cannot endorse the use of this technology for switching high voltage or high current loads_**. We suggest using the module only to switch low voltage DC power where the consequences of a mistake are more manageable. The author has used this technique quite successfully to control 12v accessories for over 2 years at the time of writing.

The project uses cheap, commonly available modules to create a computer controlled power switch. We used an Arduino UNO R3, available for as little as £5, and an 8-port relay module obtained from eBay, also costing around £5. The only other item required is a 10-way Dupont style Male-to-Female jumper cable to connect up the relay module. A 5 volt Arduino UNO should be capable of driving the relays directly from the USB power, but a 3.3 volt board would probably need additional level shifting buffers.

Note that the Arduino is not capable of powering your equipment directly and it should never be directly connected to the power supply being switched. The output side of the relays must remain completely isolated from the Arduino and the USB connection. The observatory equipment itself should be powered using a separate power supply designed for the purpose.

You must accept personal responsibility if you choose to use this project. This is a non-negotiable condition of the license agreement and the author(s) cannot be held liable for anything that happens as a result of you using the project.

## Software Architecture

For developers, this project demonstrates some best practices for developing ASCOM in-process (COM LocalServer) drivers and can be used as a template project to create new drivers. The driver has been developed using modern object-oriented techniques. While this may take a little effort to understand initially, the techniques used can be very powerful and save you a lot of time in the long run, so it is worth persevering. You can always post questions to the [ASCOM-Talk Developers Group][ascom-dev] if you are stuck on any of the implementation details. People there are only too willing to help.

In particular:

- **Reactive ASCOM** - the project demonstrates a transactional thread-safe approach to handling device communications, using the [Reactive Communications for ASCOM][rx-ascom] library. Command/Response sequencing and thread safety can be a real challenge to get right and there are many subtle pitfalls to be avoided. Using a transactional approach helps to ensure correct sequencing of commands and responses, while the Reactive Extensions for .NET ensure thread safety and provide an event-driven programming model. ASCOM drivers are essentially real-time systems and this model is a good fit for that. For information and learning resources on the [Reactive Communications for ASCOM][rx-ascom] library, please visit the [project home page][rx-ascom].
- **WiX Installer** - demonstrates how to perform a declarative no-code install of an ASCOM LocalServer driver, without running any of the code being installed during the installation. Normally, registration must be performed by running the LocalServer application with the /register option. Our install does this declaratively by directly creating the required registry keys. Our installers use WiX - Windows Installer XML, which is free and open source (it was actually Microsoft's very first open source project, written by people on the Windows Installer team).
- **Aspect Oriented Programming** We use a product called PostSharp, which is an Aspect Oriented Programming framework and tool chain. PostSharp Essentials is free for a project with a relatively small number of classes, and most ASCOM drivers fit that description nicely. PostSharp allows us to factor out a lot of 'boilerplate' code and replace it with attributes. Some of the attributes you'll notice in this project are:

  - `[MustBeConnected]` used in the ASCOM Switch driver on methods that require that the comms channel is active. This is done in such a way that the check is only performed once in the case of nested calls.
  - `[NLogTraceWithArguments]` emits diagnostics output on entry and exit from driver methods, including information about passed parameter values and return values, all without writing any code.
  - `[NotifyPropertyChanged]` used in the device layer to implement the `INotifyPropertyChanged` interface automatically.
  - `[ReaderWriterSynchronized]` used in the `ClientConnectionManager` class within the LocalServer project to create a thread safe Reader/Writer lock around the list of connected clients.

  The `Aspects` project contains a ReadMe file with more details.

- **LocalServer with Integral Driver Classes** A recent development with the LocalServer pattern was to incorporate driver classes directly into the LocalServer project. Previously, drivers were implemented as separate projects and dynamically loaded by the LocalServer process. However, it was realized that this had a number of disadvantages and promoted poor design. Driver classes are now incorporated directly into the LocalServer project and _should contain the bare minimum code necessary to compile and load_.

- **Status Display User Interface** that shows the status of each client connection and also updates annunciators to show the state of the hardware. This also provides easy access to the Setup Dialog. Thread safety is again ensured using the Reactive Extensions for .NET.

## Installation

[Download »](https://bitbucket.org/tigra-astronomy/ta.arduinopowercontroller-ascom-switch-driver/downloads/)

The installer can be downloaded from [BitBucket][download]. The installer has a minimal user interface but is fully functional. Upgrades can be installed 'on top of' existing versions and we expect that settings will be preserved in that situation.

The installer has two versions, one for 32-bit (x86) computers and another for 64-bit (x64) computers. The installer checks that it is running on the right type of system and will not allow installation to proceed if it finds a mismatch. This allows our ASCOM drivers to function correctly in all situations and they are compatible with operating systems and applications of all varieties.

<span class="text-danger">End users should install the <code>Release</code> version of the driver. <code>Debug</code> versions are for diagnostic use and will have much worse performance than the release build.</span>

## Driver Configuration

The driver currently allows configuration only of the Comm Port Name (typically "COM1") via the Setup Dialog. Other parameters may be configurable but there is no user interface. They can be edited using the _ASCOM Profile Explorer_ utility.

## Diagnostics

The driver emits diagnostic information using NLog. It is configured by default to emit informational messages to the Trace channel and errors & warnings to the log file. Live output can be viewed with a utility such as SysInternals' DebugView, or Binary Fortress' Log Fusion.

Alternatively, the NLog configuration file `NLog.dll.nlog` and `NLog.config` can be edited to emit logging information to any NLog target, including a file or database. See the [NLog wiki][nlog] for information on configuring NLog.

## Feeback and Bugs

There is a bug tracker at the [BitBucket source repository][source]. Please submit any bug reports and feature requests there.

It would be most helpful if you could quote the full version number of the driver when reporting any issues.

## Get Involved

There are probably many ways in which this driver could be improved. If you would like to contribute, then we would be delighted to accept your pull request at our [BitBucket source repository][source]. Any source code that you contribute will be covered by the original [Tigra MIT License][license] and once merged, contributions are irrevocable.

If you want to get involved but are not sure what to do, please check the [bug tracker][source] to see if there are any outstanding issues or feature requests that you could work on.

## Buy Me a Cup of Coffee

| | |

Software development is powered by coffee! If you've found this driver useful, or you're just feeling benevolent, then you might consider [buying me a cup of coffee][coffee] (or several cups) using the link at the bottom of my web site. Thank you!

August 2016, Tim Long <Tim@tigra-astronomy.com>

[coffee]: http://tigra-astronomy.com/#coffee "Buy me a cup of coffee"
[download]: https://bitbucket.org/tigra-astronomy/ta.arduinopowercontroller-ascom-switch-driver/downloads/ "Download the installer"
[image]: http://tigra-astronomy.com/Media/TigraAstronomy/site-images/arduino-power-controller/hardware.png "Arduino Power Controller"
[license]: https://tigra.mit-license.org/ "Tigra Astronomy Open Source License"
[nlog]: https://github.com/nlog/nlog/wiki/Configuration-file#targets "NLog Targets"
[project]: http://tigra-astronomy.com/oss/arduino-power-controller "Project Home Page at Tigra Astronomy"
[source]: https://github.com/Tigra-Astronomy/TA.ArduinoPowerController.AscomServer "Git Source Repository"
[tigra]: http://tigra-astronomy.com "Tigra Astronomy Web Site"
[ascom-dev]: https://ascomtalk.groups.io/g/Developer "ASCOM-Talk/Developer discussion group"
[rx-ascom]: http://tigra-astronomy.com/reactive-communications-for-ascom "Project home page: Reactive Communications for ASCOM"