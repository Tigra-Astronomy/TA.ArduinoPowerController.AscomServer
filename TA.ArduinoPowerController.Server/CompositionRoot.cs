// This file is part of the TA.WeatherListener project
// 
// Copyright © 2016-2018 Tigra Astronomy, all rights reserved.
// 
// File: CompositionRoot.cs  Last modified: 2018-04-04@01:29 by Tim Long

using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using Ninject.Planning.Bindings.Resolvers;
using Ninject.Syntax;
using System.IO;
using System.Reflection;
using TA.ArduinoPowerController.DeviceInterface;
using TA.ArduinoPowerController.Server.Properties;
using TA.Ascom.ReactiveCommunications;
using TA.Utils.Core.Diagnostics;
using TA.Utils.Logging.NLog;

namespace TA.ArduinoPowerController.Server
    {
    public static class CompositionRoot
        {
        static CompositionRoot()
            {
            var appAssembly = Assembly.GetExecutingAssembly();
            var appPath = appAssembly.Location;
            var appDirectory = Path.GetDirectoryName(appPath);
            var pluginLocation = Path.Combine(appDirectory, "Plugins");
            Kernel = new StandardKernel(new CoreModule(), new LoggingCompositionModule());
            Kernel.Components
                .Remove<IMissingBindingResolver, SelfBindingResolver>();
            //Kernel.Components.Add<IMissingBindingResolver, DefaultValueBindingResolver>();
            }

        public static IKernel Kernel { get; }

        public static ILog LogService => Kernel.Get<ILog>();
        private static ScopeObject CurrentScope { get; set; }


        public static void BeginSessionScope()
            {
            var log = LogService;
            var scope = new ScopeObject();
            log.Info()
                .Message("Beginning session scope id={ScopeId}", scope.ScopeId)
                .Write();
            CurrentScope = scope;
            }

        public static IBindingNamedWithOrOnSyntax<T> InSessionScope<T>(this IBindingInSyntax<T> binding)
            {
            return binding.InScope(ctx => CurrentScope);
            }
        }

    internal class ScopeObject
        {
        private static int scopeId;

        public ScopeObject()
            {
            ++scopeId;
            }

        public int ScopeId => scopeId;
        }

    internal class CoreModule : NinjectModule
        {
        public override void Load()
            {
            var settingsId = SharedResources.SwitchDriverId;
            var lastDot = settingsId.LastIndexOf('.');
            var deviceType = settingsId.Substring(lastDot + 1);
            Bind<DeviceController>().ToSelf().InSessionScope();
            Bind<ICommunicationChannel>()
                .ToMethod(BuildCommunicationsChannel)
                .InSessionScope();
            Bind<ChannelFactory>().ToSelf().InSessionScope();
            Bind<ServerStatusDisplay>().ToSelf().InSingletonScope();
            Bind<ITransactionProcessorFactory>().ToMethod(CreateTransactionProcessorFactory).InTransientScope();
            }

        private static ITransactionProcessorFactory CreateTransactionProcessorFactory(IContext _)
            {
            string connectionString = Settings.Default.ConnectionString;
            var log = CompositionRoot.Kernel.Get<ILog>();
            log.Info()
                .Message("Creating transaction processor factory with connection string {connectionString}", connectionString)
                .Write();
            return new ReactiveTransactionProcessorFactory(connectionString ?? "(not set)");
            }

        private ICommunicationChannel BuildCommunicationsChannel(IContext context)
            {
            var channelFactory = Kernel.Get<ChannelFactory>();
            var channel = channelFactory.FromConnectionString(Settings.Default.ConnectionString);
            return channel;
            }
        }

    internal class LoggingCompositionModule : NinjectModule
        {
        public override void Load()
            {
            Bind<ILog>().ToMethod(CreateLoggerForClass).InTransientScope();
            Bind<LoggingService>().ToSelf().InTransientScope();
            }

        private ILog CreateLoggerForClass(IContext context)
            {
            var callerName = context.Request.GetType().Name;
            return Kernel.Get<LoggingService>().WithAmbientProperty("LoggerName", callerName);
            }
        }

    internal class PowerControllerCompositionModule : NinjectModule
        {
        public override void Load()
            {
            Bind<DeviceController>().ToSelf().InSingletonScope();
            }
        }
    }