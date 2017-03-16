// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: GarbageCollection.cs  Last modified: 2017-03-16@23:33 by Tim Long

using System;
using System.Threading;

namespace TA.ArduinoPowerController.Server
    {
    /// <summary>
    ///     Summary description for GarbageCollection.
    /// </summary>
    internal class GarbageCollection
        {
        protected bool m_bContinueThread;
        protected ManualResetEvent m_EventThreadEnded;
        protected bool m_GCWatchStopped;
        protected int m_iInterval;

        public GarbageCollection(int iInterval)
            {
            m_bContinueThread = true;
            m_GCWatchStopped = false;
            m_iInterval = iInterval;
            m_EventThreadEnded = new ManualResetEvent(false);
            }

        protected bool ContinueThread()
            {
            lock (this)
                {
                return m_bContinueThread;
                }
            }

        public void GCWatch()
            {
            // Pause for a moment to provide a delay to make threads more apparent.
            while (ContinueThread())
                {
                GC.Collect();
                Thread.Sleep(m_iInterval);
                }
            m_EventThreadEnded.Set();
            }

        public void StopThread()
            {
            lock (this)
                {
                m_bContinueThread = false;
                }
            }

        public void WaitForThreadToStop()
            {
            m_EventThreadEnded.WaitOne();
            m_EventThreadEnded.Reset();
            }
        }
    }