// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: AppDomainIsolated.cs  Last modified: 2017-03-16@23:33 by Tim Long

using System;

namespace TA.ArduinoPowerController.Server
    {
    internal class AppDomainIsolated<TWorker> : IDisposable
        where TWorker : MarshalByRefObject
        {
        private AppDomain domain;

        public AppDomainIsolated()
            {
            domain = AppDomain.CreateDomain("Isolated:" + Guid.NewGuid(),
                null, AppDomain.CurrentDomain.SetupInformation);
            var type = typeof(TWorker);
            Worker = (TWorker) domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
            }

        /// <summary>
        ///     Gets the worker object that will run in the isolated app domain.
        /// </summary>
        /// <value>The worker.</value>
        public TWorker Worker { get; }

        public void Dispose()
            {
            if (domain != null)
                {
                AppDomain.Unload(domain);

                domain = null;
                }
            }
        }
    }