using JetBrains.Annotations;
using Machine.Specifications;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TA.Utils.Core.Diagnostics;

namespace TA.ArduinoPowerController.Test
    {
    [UsedImplicitly]
    public class LogSetup : IAssemblyContext
        {
        static ILog log;

        public void OnAssemblyStart()
            {
            var configuration = new LoggingConfiguration();
            var unitTestRunnerTarget = new TraceTarget();
            unitTestRunnerTarget.RawWrite = true;
            configuration.AddTarget("Unit test runner", unitTestRunnerTarget);
            var logEverything = new LoggingRule("*", LogLevel.Debug, unitTestRunnerTarget);
            configuration.LoggingRules.Add(logEverything);
            LogManager.Configuration = configuration;
            log = new TA.Utils.Logging.NLog.LoggingService();
            log.Info().Message("Logging initialized").Write();
            }

        public void OnAssemblyComplete() { }
        }
    }
