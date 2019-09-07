// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2019 Tigra Astronomy, all rights reserved.
// Licensed under the Tigra MIT license, see http://tigra.mit-license.org/
// 
// File: DriverDiscovery.cs  Last modified: 2019-09-07@05:21 by Tim Long

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ASCOM;
using NLog.Fluent;

namespace TA.ArduinoPowerController.Server
{
    internal class DriverDiscovery : MarshalByRefObject
    {
        /// <summary>
        ///     Gets the full name of the LocalServer assembly
        /// </summary>
        public string DiscoveredAssemblyName { get; private set; } = string.Empty;

        /// <summary>
        ///     Gets the discovered types that were found to be decorated with the <see cref="ServedClassNameAttribute" />
        ///     attribute.
        /// </summary>
        /// <value>A list of the discovered types.</value>
        public List<Type> DiscoveredTypes { get; } = new List<Type>();

        /// <summary>
        ///     Discovers types within the LocalServer assembly that are decorated with the
        ///     <see cref="ServedClassNameAttribute" />, which identifies an ASCOM driver
        ///     that should be served by the LocalServer.
        /// </summary>
        public void DiscoverServedClasses()
        {
            Log.Info().Message("Loading served COM classes").Write();
            var thisAssembly = Assembly.GetExecutingAssembly();
            var assemblyName = thisAssembly.GetName();
            var assemblyFullName = assemblyName.FullName;
            var assemblyDisplayName = assemblyName.Name;
            var types = thisAssembly.GetTypes();
            Log.Trace()
                .Message("Reflection found {typeCount} types in assembly {assemblyName}",
                    types.Length, assemblyDisplayName)
                .Write();
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
                DiscoveredAssemblyName = assemblyFullName;
                Log.Info()
                    .Message("Discovered {servedClassCount} served classes in assembly {assembly}",
                        discoveredTypes.Count, assemblyName)
                    .Write();
            }
        }
    }
}