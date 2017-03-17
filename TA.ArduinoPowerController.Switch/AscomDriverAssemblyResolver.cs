// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: AscomDriverAssemblyResolver.cs  Last modified: 2017-03-17@17:36 by Tim Long

using System;
using System.IO;
using System.Reflection;
using NLog;

namespace TA.ArduinoPowerController.AscomSwitch
    {
    internal static class AscomDriverAssemblyResolver
        {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Resolves support assemblies for an ASCOM driver by searching in the same directory
        ///     as the driver assembly.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">
        ///     The <see cref="ResolveEventArgs" /> instance containing the event data.
        /// </param>
        /// <returns>The loaded resolved assembly.</returns>
        /// <remarks>
        ///     <para>
        ///         The CLR's default assembly resolution strategy is to look in the Global Assembly Cache,
        ///         then in the application directory and subfolders. However, an ASCOM driver is loaded
        ///         via COM Interop and is not normally located in the application directory, so the runtime
        ///         will need help locating an referenced assemblies.
        ///     </para>
        ///     <para>
        ///         By default, ASCOM solves this by including the CodeBase registry value in the driver's
        ///         COM Interop registration. This requires that the driver is strong named, which means that
        ///         any referenced assemblies also have to be strong named. This can be problematic, especially
        ///         if the driver uses third party code that is not signed.
        ///     </para>
        ///     <para>
        ///         Tigra Astronomy prefers to use a custom assembly resolver, which simply looks in the same
        ///         directory as the driver assembly. This avoids the need to sign the driver with a strong-name,
        ///         which gives us a bit more flexibility in using third party libraries and tends to simplify
        ///         build, debug and deployment.
        ///     </para>
        /// </remarks>
        public static Assembly ResolveSupportAssemblies(object sender, ResolveEventArgs args)
            {
            try
                {
                Log.Info($"Handling AppDomain.ResolveAssembly event for {args.Name}");
                Log.Info("Attempting to resolve the assembly");
                var me = Assembly.GetExecutingAssembly();
                var here = me.Location;
                var myDirectory = Path.GetDirectoryName(here);
                var commaPosition = args.Name.IndexOf(',');
                var targetName = args.Name.Head(commaPosition);
                var targetDll = targetName + ".dll";
                var target = Path.Combine(myDirectory, targetDll);
                Log.Info($"Target assembly is {target}");
                var resolvedAssembly = Assembly.LoadFrom(target);
                Log.Info($"Successfully resolved assembly load with {resolvedAssembly.GetName()}");
                return resolvedAssembly;
                }
            catch (Exception ex)
                {
                Log.Error(ex, $"Failed to resolve assembly: {args.Name}");
                return null; // Let the app raise its own error.
                }
            }
        }
    }