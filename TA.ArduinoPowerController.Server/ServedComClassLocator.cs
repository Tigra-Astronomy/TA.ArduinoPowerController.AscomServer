// This file is part of the ASCOM.K8056.Switch project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: ServedComClassLocator.cs  Last modified: 2017-03-08@15:42 by Tim Long

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ASCOM;
using NLog;

namespace TA.VellemanK8056.Server
    {
    internal class ServedComClassLocator : MarshalByRefObject
        {
        private readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public ServedComClassLocator()
            {
            DiscoveredAssemblyNames = new List<string>();
            DiscoveredTypes = new List<Type>();
            }

        /// <summary>
        ///     Gets the list of the names of assemblies that contain ASCOM drivers to be served by
        ///     the LocalServer.
        /// </summary>
        /// <value>The discovered assembly full names.</value>
        public List<string> DiscoveredAssemblyNames { get; }

        /// <summary>
        ///     Gets the discovered types that were found to be decorated with the <see cref="ServedClassNameAttribute" />
        ///     attribute.
        /// </summary>
        /// <value>A list of the discovered types.</value>
        public List<Type> DiscoveredTypes { get; }

        /// <summary>
        ///     Discovers types (and their declaring assemblies) that are decorated with the
        ///     <see cref="ServedClassNameAttribute" />, which identifies an ASCOM driver that should be served
        ///     by the LocalServer.
        /// </summary>
        /// <remarks>
        ///     Assemblies are loaded using <see cref="Assembly.ReflectionOnlyLoad(string)" /> to prevent any code execution.
        ///     This would otherwise be a potential malware attack vector, since untrusted assemblies would be loaded and
        ///     potentially into
        ///     a privileged user context. If any of the loaded assemblies must later execute, then this operation should be
        ///     performed
        ///     in an isolated application domain so that the domain and all the loaded assemblies can later be unloaded. Unloaing
        ///     an entire
        ///     AppDomain is the only way to remove assemblies from memory.
        ///     The class inherits from <see cref="MarshalByRefObject" /> specifically so that it can be proxied across an
        ///     App Domain boundary.
        /// </remarks>
        /// <seealso cref="AppDomainIsolated{TWorker}" />
        public void DiscoverServedClasses()
            {
            Log.Info("Loading served COM classes");

            // put everything into one folder, the same as the server.
            var assyPath = Assembly.GetExecutingAssembly().Location;
            assyPath = Path.GetDirectoryName(assyPath);
            Log.Debug($"Assembly load path is {assyPath}");

            var d = new DirectoryInfo(assyPath);
            var fileInfos = d.GetFiles("*.dll");
            Log.Debug($"Discovered {fileInfos.Length} candidate assemblies");
            try
                {
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += HandleReflectionOnlyAssemblyResolve;
                foreach (var fi in fileInfos)
                    {
                    Log.Trace($"Examining types in {fi.Name}");
                    var aPath = fi.FullName;
                    try
                        {
                        Log.Trace($"Attempting reflection only load for {aPath}");
                        var so = Assembly.ReflectionOnlyLoad(Path.GetFileNameWithoutExtension(fi.Name));
                        var soShortName = so.GetName().Name;
                        var types = so.GetTypes();
                        Log.Trace($"Reflection found {types.Length} types in assembly {so.FullName}");
                        var servedClasses = from type in types.AsParallel()
                                            let memberInfo = (MemberInfo) type
                                            let safeAttributes = CustomAttributeData.GetCustomAttributes(memberInfo)
                                            where safeAttributes.Any(
                                                p => p.AttributeType.Name == nameof(ServedClassNameAttribute))
                                            select type;
                        var discoveredTypes = servedClasses.ToList();
                        if (discoveredTypes.Any())
                            {
                            DiscoveredTypes.AddRange(discoveredTypes);
                            DiscoveredAssemblyNames.Add(so.FullName);
                            Log.Warn($"Discovered {discoveredTypes.Count} served clases in assembly {soShortName}");
                            }
                        }
                    catch (BadImageFormatException ex)
                        {
                        Log.Warn(ex, $"BadImageFormat: {fi.Name}.{fi.Extension} continuing");
                        // Probably an attempt to load a Win32 DLL (i.e. not a .net assembly)
                        // Just swallow the exception and continue to the next item.
                        }
                    catch (Exception ex)
                        {
                        Log.Error(ex, $"Unexpected error processing {fi.Name}: {ex.Message}");
                        //return false;
                        }
                    }
                }
            finally
                {
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= HandleReflectionOnlyAssemblyResolve;
                }
            }


        /// <summary>
        ///     Handles the reflection only assembly resolve.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ResolveEventArgs" /> instance containing the event data.</param>
        /// <returns>Assembly.</returns>
        private Assembly HandleReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
            {
            try
                {
                Log.Info(
                    $"Event: ReflectionOnlyResolveAssembly for {args.Name} requested by {args.RequestingAssembly.GetName().Name}",
                    args.Name);
                var resolved = Assembly.ReflectionOnlyLoad(args.Name);
                Log.Info($"Successfully resolved assembly with {resolved.FullName}");
                return resolved;
                }
            catch (FileLoadException ex)
                {
                Log.Error(ex, $"Failed to resolve assembly: {args.Name}");
                return null; // Let the app raise its own error.
                }
            }
        }
    }