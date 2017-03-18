Arduino Power Controller with ASCOM Switch Driver
=================================================

![Arduino Power Controller][image]

This driver and its source code and the accompanying Arduino Sketch are licensed under the [Tigra MIT license][license]. Essentially, anyone can do anything at all with this software, including commercially, without restriction. Anything you do is your responsibility and not mine.

For further information, please visit the [Project Home Page][project] or the [BitBucket Source Repository][source].

Project Description
-------------------

This project is designed to enable easy control of power to astronomical instruments and ancillary
equipment in a robotic observatory. The module can be controlled via the ASCOM Switch interface
from any planetarium or observatory automation software that supports it. Alternatively, the driver
can be used directly from a PowerShell script or any Windows scripting engine that can load a COM object.  

We have tested the unit while switching 12 volt power supplies. The limitations on voltage and
current will depend on the relay module used. Commonly available modules have relays rated for
up to 10 Amps and up to 250 volts AC. However, switching mains voltages and/or high current loads
can be hazardous. A simple mistake can result in destruction of equipment, fire, electrocution 
and death. ***Therefore we cannot endorse the use of this technology for switching high voltage or high
current loads***. We suggest using the module only to switch low voltage DC power where the consequences
of a mistake are more manageable.

The project uses cheap, commonly available modules to create a computer controlled power switch.
We used an Arduino UNO R3, available for as little as £5, and an 8-port relay module obtained
from eBay, also costing around £5. The only other item required is a 10-way Dupont style Male-to-Female
jumper cable to connect up the relay module. A 5 volt Arduino UNO should be capable of driving
the relays directly from the USB power.

Note that the Arduino is not capable of powering your equipment directly and it should never be
directly connected to the power supply being switched. The output side of the relays must remain completely isolated from the Arduino and the USB connection. The observatory equipment itself should be powered using a separate power supply designed for the purpose.

Software Architecture
---------------------

For developers, this project demonstrates some advanced techniques for developing ASCOM drivers. At Tigra Astronomy, we use techniques that differ significantly from the methods described in the ASCOM documentation. We have our own reasons for this and this is not to say that our techniques are better or more correct, but they do demonstrate some alternative practices.

In particular:

- **Reactive ASCOM** - the project demonstrates our Reactive Communications library in use in a real driver.
- **WiX Installer** - demonstrates how to perform a declarative no-code install of an ASCOM LocalServer driver, without running any of the code being installed during the installation. Normally, registration must be performed by running the LocalServer application with the /register option. Our install does this declaratively by directly creating the required registry keys. Our installers use WiX - Windows Installer XML, which is free and open source (it was actually Microsoft's very first open source product). The pros and cons of our technique vs. the ASCOM standard technique are complex and this is not the place to have that discussion, but we've used our techniques successfully in at least 6 production drivers.
- **Aspect Oriented Programming** We use a product called PostSharp, which is an Aspect Oriented Programming framework and tool chain. PostSharp is not free but it will work in unlicensed mode for a project with a small number of properties and methods, and most ASCOM drivers fit that description nicely. PostSharp allows us to factor out a lot of 'boilerplate' code and replace it with attributes. Some of the attributes you'll notice in this project are:
	- `[MustBeConnected]` used in the ASCOM Switch driver on methods that require that the comms channel is active. This is done in such a way that the check is only performed once in the case of nested calls.
	- `[NLogTraceWithArguments]` emits diagnostics output on entry and exit from driver methods, including information about passed parameter values and return values, all without writing any code.
	- `[NotifyPropertyChanged]` used in the device layer to implement the `INotifyPropertyChanged` interface automatically. 
	- `[ReaderWriterSynchronized]` used in the `ClientConnectionManager` class within the LocalServer project to create a thread safe Reader/Writer lock around the list of connected clients. 
	- 
- **Modified LocalServer** Our LocalServer implementation has a few interesting customizations.
	- **Reduced Attack Surface Area** that uses the Reflection-only load context when searching for assemblies with ASCOM drivers. The original LocalServer loads every assembly it finds into the execution context whilst running with elevated permissions. That comes with a high risk since malicious code could very easily be dropped into the folder. We minimize this attack vector by examining all assemblies in the Reflection-only context, so that no malicious code could execute while the server has elevated permissions.At other times we only load the assemblies containing ASCOM drivers. Note: there is no known incident of a LocalServer being used as an attack vector, but once we had identified the possibility we felt that we had to address it.
	- **Status Display GUI** that shows the status of each client connection and also updates annunciators to show the state of the hardware. This also provides easy access to the Setup Dialog.


Installation
------------

<a href="https://bitbucket.org/tigra-astronomy/ta.arduinopowercontroller-ascom-switch-driver/downloads/" class="btn btn-info" target="new">Download &raquo;</a>

The installer can be downloaded from [BitBucket][download]. The installer has a minimal user interface but is fully functional. Upgrades can be installed 'on top of' existing versions and we expect that settings will be preserved in that situation.

The installer has two versions, one for 32-bit (x86) computers and another for 64-bit (x64) computers. The installer checks that it is running on the right type of system and will not allow installation to proceed if it finds a mismatch. This allows our ASCOM drivers to function correctly in all situations and they are compatible with operating systems and applications of all varieties.

<span class="text-danger">End users should install the `Release` version of the driver. `Debug` versions are for diagnostic use and will have much worse performance than the release build.</span>

Driver Configuration
--------------------

The driver currently allows configuration only of the Comm Port Name (typically "COM1") via the Setup Dialog. Other parameters may be configurable but there is no user interface. They can be edited using the *ASCOM Profile Explorer* utility.

Diagnostics
-----------

The driver emits diagnostic information using NLog. It is configured by default to emit informational messages to the Trace channel and errors & warnings to the log file. Live output can be viewed with a utility such as SysInternals' DebugView, or Binary Fortress' Log Fusion.

Alternatively, the NLog configuration file `NLog.dll.nlog` and `NLog.config` can be edited to emit logging information to any NLog target, including a file or database. See the [NLog wiki][nlog] for information on configuring NLog.

Feeback and Bugs
----------------

There is a bug tracker at the [BitBucket source repository][source]. Please submit any bug reports and feature requests there.

It would be most helpful if you could quote the full version number of the driver when reporting any issues.

Get Involved
------------

There are probably many ways in which this driver could be improved. If you would like to contribute, then we would be delighted to accept your pull request at our [BitBucket source repository][source]. Any source code that you contribute will be covered by the original [Tigra MIT License][license] and once merged, contributions are irrevocable.

If you want to get involved but are not sure what to do, please check the [bug tracker][source] to see if there are any outstanding issues or feature requests that you could work on.

Buy Me a Cup of Coffee
----------------------

Software development is powered by coffee! If you've found this driver useful, or you're just feeling benevolent, then you might consider [buying me a cup of coffee][coffee] (or several cups) using the link at the bottom of my web site. Thank you!

August 2016, Tim Long <Tim@tigra-astronomy.com>

[license]: https://tigra.mit-license.org/		                   "Tigra Astronomy Open Source License"
[project]: http://tigra-astronomy.com/oss/arduino-power-controller      "Project Home Page at Tigra Astronomy"
[source]:  https://bitbucket.org/account/user/tigra-astronomy/projects/OPC  "BitBucket Git Source Control"
[download]: https://bitbucket.org/tigra-astronomy/ta.arduinopowercontroller-ascom-switch-driver/downloads/ "Download the installer"
[tigra]:   http://tigra-astronomy.com                              "Tigra Astronomy Web Site"
[nlog]:    https://github.com/nlog/nlog/wiki/Configuration-file#targets "NLog Targets"
[coffee]:  http://tigra-astronomy.com/#coffee                      "Buy me a cup of coffee"
[image]:   http://tigra-astronomy.com/Media/TigraAstronomy/site-images/arduino-power-controller/hardware.png  "Arduino Power Controller"