// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: ReferenceCountedObject.cs  Last modified: 2017-03-16@23:34 by Tim Long

using System.Runtime.InteropServices;

namespace TA.ArduinoPowerController.Server
    {
    [ComVisible(false)]
    public class ReferenceCountedObjectBase
        {
        public ReferenceCountedObjectBase()
            {
            // We increment the global count of objects.
            Server.CountObject();
            }

        ~ReferenceCountedObjectBase()
            {
            // We decrement the global count of objects.
            Server.UncountObject();
            // We then immediately test to see if we the conditions
            // are right to attempt to terminate this server application.
            Server.ExitIf();
            }
        }
    }