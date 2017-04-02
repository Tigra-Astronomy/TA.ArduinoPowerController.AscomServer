// This file is part of the TA.ArduinoPowerController project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: LocalServer.cs  Last modified: 2017-04-02@18:43 by Tim Long

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using ASCOM;
using ASCOM.Utilities;
using Microsoft.Win32;
using NLog;
using TA.PostSharp.Aspects;

namespace TA.ArduinoPowerController.Server
    {
    public static class Server
        {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        // This property returns the main thread's id.
        public static uint MainThreadId { get; private set; } // Stores the main thread's thread id.

        // Used to tell if started by COM or manually
        public static bool StartedByCOM { get; private set; } // True if server started by COM (-embedding)

        #region SERVER ENTRY POINT (main)
        //
        // ==================
        // SERVER ENTRY POINT
        // ==================
        //
        [STAThread]
        [NLogTraceWithArguments]
        private static void Main(string[] args)
            {
            // Manage unhandled exceptions
            Application.ThreadException += UnhandledThreadException;
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            var foundTypes = LoadComObjectAssemblies();
            if (foundTypes < 1)
                return; // There is no point continuing if we found nothing to serve.

            if (!ProcessArguments(args)) return; // Register/Unregister

            // Initialize critical member variables.
            objsInUse = 0;
            serverLocks = 0;
            MainThreadId = GetCurrentThreadId();
            Thread.CurrentThread.Name = "Main Thread";

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            s_MainForm = new ServerStatusDisplay();
//#if !DEBUG
//// Only show the application main window if it was started manually by the user.
//            if (StartedByCOM) s_MainForm.WindowState = FormWindowState.Minimized;
//#endif

            // Register the class factories of the served objects
            RegisterClassFactories();

            // Start up the garbage collection thread.
            var GarbageCollector = new GarbageCollection(1000);
            var GCThread = new Thread(GarbageCollector.GCWatch);
            GCThread.Name = "Garbage Collection Thread";
            GCThread.Start();

            //
            // Start the message loop. This serializes incoming calls to our
            // served COM objects, making this act like the VB6 equivalent!
            //
            try
                {
                Application.Run(s_MainForm);
                }
            finally
                {
                // Revoke the class factories immediately.
                // Don't wait until the thread has stopped before
                // we perform revocation!!!
                RevokeClassFactories();

                // Now stop the Garbage Collector thread.
                GarbageCollector.StopThread();
                GarbageCollector.WaitForThreadToStop();
                Application.ThreadException -= UnhandledThreadException;
                AppDomain.CurrentDomain.UnhandledException -= UnhandledException;
                }
            }
        #endregion

        #region Command Line Arguments
        //
        // ProcessArguments() will process the command-line arguments
        // If the return value is true, we carry on and start this application.
        // If the return value is false, we terminate this application immediately.
        //
        private static bool ProcessArguments(string[] args)
            {
            var bRet = true;

            //
            //**TODO** -Embedding is "ActiveX start". Prohibit non_AX starting?
            //
            if (args.Length > 0)
                {
                switch (args[0].ToLower())
                    {
                        case "-embedding":
                            StartedByCOM = true; // Indicate COM started us
                            break;

                        case "-register":
                        case @"/register":
                        case "-regserver": // Emulate VB6
                        case @"/regserver":
                            RegisterObjects(); // Register each served object
                            bRet = false;
                            break;

                        case "-unregister":
                        case @"/unregister":
                        case "-unregserver": // Emulate VB6
                        case @"/unregserver":
                            UnregisterObjects(); //Unregister each served object
                            bRet = false;
                            break;

                        default:
                            MessageBox.Show(
                                "Unknown argument: " + args[0] + "\nValid are : -register, -unregister and -embedding",
                                "ASCOM driver for Arduino Power Controller", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                            break;
                    }
                }
            else
                StartedByCOM = false;

            return bRet;
            }
        #endregion

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs ea)
            {
            Log.Error((Exception) ea.ExceptionObject, "Unhandled exception");
            }

        private static void UnhandledThreadException(object sender, ThreadExceptionEventArgs ea)
            {
            Log.Error(ea.Exception, "Unhandled thread exception");
            }

        // -----------------
        // PRIVATE FUNCTIONS
        // -----------------

        #region Dynamic Driver Assembly Loader
        /// <summary>
        ///     Load the assemblies that we will serve via COM. These will be located in the same
        ///     folder as out executable and will have at least one type decorated with a
        ///     <see cref="ServedClassNameAttribute" />
        /// </summary>
        /// <returns>The count of types found.</returns>
        private static int LoadComObjectAssemblies()
            {
            Log.Info("Loading served COM classes");
            try
                {
                // Discover assemblies and types to be served in Reflection Only context in an isolated AppDomain
                using (var reflectionContext = new AppDomainIsolated<ServedComClassLocator>())
                    {
                    reflectionContext.Worker.DiscoverServedClasses();
                    s_ComObjectAssys = new List<string>(reflectionContext.Worker.DiscoveredAssemblyNames);
                    s_ComObjectTypes = new List<Type>(reflectionContext.Worker.DiscoveredTypes);
                    }
                // Now load the discovered assemblies into the current domain's execution context
                foreach (var assemblyName in s_ComObjectAssys)
                    {
                    Assembly.Load(assemblyName);
                    }
                return s_ComObjectTypes.Count;
                }
            finally
                {
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomainOnReflectionOnlyAssemblyResolve;
                }
            }

        private static Assembly CurrentDomainOnReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
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
        #endregion

        #region Access to kernel32.dll, user32.dll, and ole32.dll functions
        [Flags]
        private enum CLSCTX : uint
            {
            CLSCTX_INPROC_SERVER = 0x1,
            CLSCTX_INPROC_HANDLER = 0x2,
            CLSCTX_LOCAL_SERVER = 0x4,
            CLSCTX_INPROC_SERVER16 = 0x8,
            CLSCTX_REMOTE_SERVER = 0x10,
            CLSCTX_INPROC_HANDLER16 = 0x20,
            CLSCTX_RESERVED1 = 0x40,
            CLSCTX_RESERVED2 = 0x80,
            CLSCTX_RESERVED3 = 0x100,
            CLSCTX_RESERVED4 = 0x200,
            CLSCTX_NO_CODE_DOWNLOAD = 0x400,
            CLSCTX_RESERVED5 = 0x800,
            CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
            CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
            CLSCTX_NO_FAILURE_LOG = 0x4000,
            CLSCTX_DISABLE_AAA = 0x8000,
            CLSCTX_ENABLE_AAA = 0x10000,
            CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
            CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
            CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
            CLSCTX_ALL = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER
            }

        [Flags]
        private enum COINIT : uint
            {
            /// Initializes the thread for multi-threaded object concurrency.
            COINIT_MULTITHREADED = 0x0,
            /// Initializes the thread for apartment-threaded object concurrency.
            COINIT_APARTMENTTHREADED = 0x2,
            /// Disables DDE for Ole1 support.
            COINIT_DISABLE_OLE1DDE = 0x4,
            /// Trades memory for speed.
            COINIT_SPEED_OVER_MEMORY = 0x8
            }

        [Flags]
        private enum REGCLS : uint
            {
            REGCLS_SINGLEUSE = 0,
            REGCLS_MULTIPLEUSE = 1,
            REGCLS_MULTI_SEPARATE = 2,
            REGCLS_SUSPENDED = 4,
            REGCLS_SURROGATE = 8
            }


        // CoInitializeEx() can be used to set the apartment model
        // of individual threads.
        [DllImport("ole32.dll")]
        private static extern int CoInitializeEx(IntPtr pvReserved, uint dwCoInit);

        // CoUninitialize() is used to uninitialize a COM thread.
        [DllImport("ole32.dll")]
        private static extern void CoUninitialize();

        // PostThreadMessage() allows us to post a Windows Message to
        // a specific thread (identified by its thread id).
        // We will need this API to post a WM_QUIT message to the main 
        // thread in order to terminate this application.
        [DllImport("user32.dll")]
        private static extern bool PostThreadMessage(uint idThread, uint Msg, UIntPtr wParam,
            IntPtr lParam);

        // GetCurrentThreadId() allows us to obtain the thread id of the
        // calling thread. This allows us to post the WM_QUIT message to
        // the main thread.
        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();
        #endregion

        #region Private Data
        private static int objsInUse; // Keeps a count on the total number of objects alive.
        private static int serverLocks; // Keeps a lock count on this application.
        private static ServerStatusDisplay s_MainForm; // Reference to our main form
        private static List<string> s_ComObjectAssys; // Dynamically loaded assemblies containing served COM objects
        private static List<Type> s_ComObjectTypes; // Served COM object types
        private static ArrayList s_ClassFactories; // Served COM object class factories
        private static readonly string s_appId = "{50e4c255-0a0e-4276-bd33-aae742da21b1}"; // Our AppId
        private static readonly object lockObject = new object();
        #endregion

        #region Server Lock, Object Counting, and AutoQuit on COM startup
        // Returns the total number of objects alive currently.
        public static int ObjectsCount
            {
            get
                {
                lock (lockObject)
                    {
                    return objsInUse;
                    }
                }
            }

        // This method performs a thread-safe incrementation of the objects count.
        public static int CountObject()
            {
            // Increment the global count of objects.
            return Interlocked.Increment(ref objsInUse);
            }

        // This method performs a thread-safe decrementation the objects count.
        public static int UncountObject()
            {
            // Decrement the global count of objects.
            return Interlocked.Decrement(ref objsInUse);
            }

        // Returns the current server lock count.
        public static int ServerLockCount
            {
            get
                {
                lock (lockObject)
                    {
                    return serverLocks;
                    }
                }
            }

        // This method performs a thread-safe incrementation the 
        // server lock count.
        public static int CountLock()
            {
            // Increment the global lock count of this server.
            return Interlocked.Increment(ref serverLocks);
            }

        // This method performs a thread-safe decrementation the 
        // server lock count.
        public static int UncountLock()
            {
            // Decrement the global lock count of this server.
            return Interlocked.Decrement(ref serverLocks);
            }

        // AttemptToTerminateServer() will check to see if the objects count and the server 
        // lock count have both dropped to zero.
        //
        // If so, and if we were started by COM, we post a WM_QUIT message to the main thread's
        // message loop. This will cause the message loop to exit and hence the termination 
        // of this application. If hand-started, then just trace that it WOULD exit now.
        //
        public static void ExitIf()
            {
            lock (lockObject)
                {
                if (ObjectsCount <= 0 && ServerLockCount <= 0)
                    {
                    if (StartedByCOM)
                        {
                        var wParam = new UIntPtr(0);
                        var lParam = new IntPtr(0);
                        PostThreadMessage(MainThreadId, 0x0012, wParam, lParam);
                        }
                    }
                }
            }

        public static void TerminateLocalServer()
            {
            if (StartedByCOM)
                Application.Exit();
            }
        #endregion

        #region COM Registration and Unregistration
        //
        // Test if running elevated
        //
        private static bool IsAdministrator
            {
            get
                {
                var i = WindowsIdentity.GetCurrent();
                var p = new WindowsPrincipal(i);
                return p.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }

        //
        // Elevate by re-running ourselves with elevation dialog
        //
        private static void ElevateSelf(string arg)
            {
            var si = new ProcessStartInfo();
            si.Arguments = arg;
            si.WorkingDirectory = Environment.CurrentDirectory;
            si.FileName = Application.ExecutablePath;
            si.Verb = "runas";
            try
                {
                Process.Start(si);
                }
            catch (Win32Exception)
                {
                MessageBox.Show(
                    "The LocalServer was not " + (arg == "/register" ? "registered" : "unregistered") +
                    " because you did not allow it.", SharedResources.SwitchDriverId, MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                }
            catch (Exception ex)
                {
                MessageBox.Show(ex.ToString(), SharedResources.SwitchDriverId, MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);
                }
            }

        //
        // Do everything to register this for COM. Never use REGASM on
        // this exe assembly! It would create InProcServer32 entries 
        // which would prevent proper activation!
        //
        // Using the list of COM object types generated during dynamic
        // assembly loading, it registers each one for COM as served by our
        // exe/local server, as well as registering it for ASCOM. It also
        // adds DCOM info for the local server itself, so it can be activated
        // via an outboiud connection from TheSky.
        //
        private static void RegisterObjects()
            {
            if (!IsAdministrator)
                {
                ElevateSelf("/register");
                return;
                }
            //
            // If reached here, we're running elevated
            //

            var assy = Assembly.GetExecutingAssembly();
            var attr = Attribute.GetCustomAttribute(assy, typeof(AssemblyTitleAttribute));
            var assyTitle = ((AssemblyTitleAttribute) attr).Title;
            attr = Attribute.GetCustomAttribute(assy, typeof(AssemblyDescriptionAttribute));
            var assyDescription = ((AssemblyDescriptionAttribute) attr).Description;

            //
            // Local server's DCOM/AppID information
            //
            try
                {
                //
                // HKCR\APPID\appid
                //
                using (var key = Registry.ClassesRoot.CreateSubKey("APPID\\" + s_appId))
                    {
                    key.SetValue(null, assyDescription);
                    key.SetValue("AppID", s_appId);
                    key.SetValue("AuthenticationLevel", 1, RegistryValueKind.DWord);
                    }
                //
                // HKCR\APPID\exename.ext
                //
                using (var key = Registry.ClassesRoot.CreateSubKey(string.Format("APPID\\{0}",
                    Application.ExecutablePath.Substring(Application.ExecutablePath.LastIndexOf('\\') + 1))))
                    {
                    key.SetValue("AppID", s_appId);
                    }
                }
            catch (Exception ex)
                {
                MessageBox.Show("Error while registering the server:\n" + ex,
                    SharedResources.SwitchDriverId, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
                }
            finally {}

            //
            // For each of the driver assemblies
            //
            foreach (var type in s_ComObjectTypes)
                {
                var bFail = false;
                try
                    {
                    //
                    // HKCR\CLSID\clsid
                    //
                    var clsid = Marshal.GenerateGuidForType(type).ToString("B");
                    var progid = Marshal.GenerateProgIdForType(type);
                    //PWGS Generate device type from the Class name
                    var deviceType = type.Name;

                    using (var key = Registry.ClassesRoot.CreateSubKey(string.Format("CLSID\\{0}", clsid)))
                        {
                        key.SetValue(null, progid); // Could be assyTitle/Desc??, but .NET components show ProgId here
                        key.SetValue("AppId", s_appId);
                        using (var key2 = key.CreateSubKey("Implemented Categories"))
                            {
                            key2.CreateSubKey("{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}");
                            }
                        using (var key2 = key.CreateSubKey("ProgId"))
                            {
                            key2.SetValue(null, progid);
                            }
                        key.CreateSubKey("Programmable");
                        using (var key2 = key.CreateSubKey("LocalServer32"))
                            {
                            key2.SetValue(null, Application.ExecutablePath);
                            }
                        }
                    //
                    // HKCR\progid
                    //
                    using (var key = Registry.ClassesRoot.CreateSubKey(progid))
                        {
                        key.SetValue(null, assyTitle);
                        using (var key2 = key.CreateSubKey("CLSID"))
                            {
                            key2.SetValue(null, clsid);
                            }
                        }
                    //
                    // ASCOM 
                    //
                    assy = type.Assembly;

                    // Pull the display name from the ServedClassName attribute.
                    attr = Attribute.GetCustomAttribute(type, typeof(ServedClassNameAttribute));
                    //PWGS Changed to search type for attribute rather than assembly
                    var chooserName = ((ServedClassNameAttribute) attr).DisplayName ?? "MultiServer";
                    using (var P = new Profile())
                        {
                        P.DeviceType = deviceType;
                        P.Register(progid, chooserName);
                        }
                    }
                catch (Exception ex)
                    {
                    MessageBox.Show("Error while registering the server:\n" + ex,
                        SharedResources.SwitchDriverId, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    bFail = true;
                    }
                finally {}
                if (bFail) break;
                }
            }

        //
        // Remove all traces of this from the registry. 
        //
        // **TODO** If the above does AppID/DCOM stuff, this would have
        // to remove that stuff too.
        //
        private static void UnregisterObjects()
            {
            if (!IsAdministrator)
                {
                ElevateSelf("/unregister");
                return;
                }

            //
            // Local server's DCOM/AppID information
            //
            Registry.ClassesRoot.DeleteSubKey(string.Format("APPID\\{0}", s_appId), false);
            Registry.ClassesRoot.DeleteSubKey(string.Format("APPID\\{0}",
                Application.ExecutablePath.Substring(Application.ExecutablePath.LastIndexOf('\\') + 1)), false);

            //
            // For each of the driver assemblies
            //
            foreach (var type in s_ComObjectTypes)
                {
                var clsid = Marshal.GenerateGuidForType(type).ToString("B");
                var progid = Marshal.GenerateProgIdForType(type);
                var deviceType = type.Name;
                //
                // Best efforts
                //
                //
                // HKCR\progid
                //
                Registry.ClassesRoot.DeleteSubKey(string.Format("{0}\\CLSID", progid), false);
                Registry.ClassesRoot.DeleteSubKey(progid, false);
                //
                // HKCR\CLSID\clsid
                //
                Registry.ClassesRoot.DeleteSubKey(
                    string.Format("CLSID\\{0}\\Implemented Categories\\{{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}}",
                        clsid),
                    false);
                Registry.ClassesRoot.DeleteSubKey(string.Format("CLSID\\{0}\\Implemented Categories", clsid), false);
                Registry.ClassesRoot.DeleteSubKey(string.Format("CLSID\\{0}\\ProgId", clsid), false);
                Registry.ClassesRoot.DeleteSubKey(string.Format("CLSID\\{0}\\LocalServer32", clsid), false);
                Registry.ClassesRoot.DeleteSubKey(string.Format("CLSID\\{0}\\Programmable", clsid), false);
                Registry.ClassesRoot.DeleteSubKey(string.Format("CLSID\\{0}", clsid), false);
                try
                    {
                    //
                    // ASCOM
                    //
                    using (var P = new Profile())
                        {
                        P.DeviceType = deviceType;
                        P.Unregister(progid);
                        }
                    }
                catch (Exception) {}
                }
            }
        #endregion

        #region Class Factory Support
        //
        // On startup, we register the class factories of the COM objects
        // that we serve. This requires the class facgtory name to be
        // equal to the served class name + "ClassFactory".
        //
        private static bool RegisterClassFactories()
            {
            s_ClassFactories = new ArrayList();
            foreach (var type in s_ComObjectTypes)
                {
                var factory = new ClassFactory(type); // Use default context & flags
                s_ClassFactories.Add(factory);
                if (!factory.RegisterClassObject())
                    {
                    MessageBox.Show("Failed to register class factory for " + type.Name,
                        "TA.ArduinoPowerController.Server", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                    }
                }
            ClassFactory.ResumeClassObjects(); // Served objects now go live
            return true;
            }

        private static void RevokeClassFactories()
            {
            ClassFactory.SuspendClassObjects(); // Prevent race conditions
            foreach (ClassFactory factory in s_ClassFactories)
                factory.RevokeClassObject();
            }
        #endregion
        }
    }