// This file is part of the TA.ArduinoPowerController project
//
// Copyright © 2016-2019 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
//
// File: GlobalAssemblyInfo.cs  Last modified: 2017-04-07@17:48 by Tim Long

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Global assembly attributes shared by all projects in the solution.

[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Tigra Astronomy")]
[assembly: AssemblyProduct("ASCOM Switch driver for Arduino Power Controller")]
[assembly: AssemblyCopyright("Copyright © 2017-19 Tigra Astronomy")]
[assembly: AssemblyTrademark("Tigra Astronomy")]
[assembly: AssemblyCulture("")]

/*
 * Only classes that implement an ASCOM device interface need to be COM-Visible.
 * The majority of types in the solution should NOT be COM-visible, so we disable
 * COM visibility globally here. It is recommended to place [ComVisible(true)]
 * attributes explicitly on each driver class.
 */
[assembly: ComVisible(false)]

/*
 Version information will be injected during the build by GitVersion.
 If you do not wish to use GitVersion, then uninstall the GitVersionTask
 NuGet package and add your own version information here, as follows:
[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyFileVersion("1.0.3")]
[assembly: AssemblyInformationalVersion("1.0.3")]
*/

// Make internals visible to the unit test assembly.
[assembly:InternalsVisibleTo("TA.ArduinoPowerController.Test")]