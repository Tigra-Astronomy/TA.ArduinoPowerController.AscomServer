// This file is part of the TI.DigitalDomeWorks project
// 
// Copyright © 2015-2016 Tigra Networks., all rights reserved.
// 
// File: NLogTraceWithArgumentsAttribute.cs  Last modified: 2016-09-13@00:23 by Tim Long

using System;
using System.Reflection;
using System.Text;
using System.Threading;
using NLog;
using PostSharp.Aspects;
using PostSharp.Aspects.Dependencies;

namespace TA.PostSharp.Aspects
    {
    /// <summary>
    ///     Class NLogTraceWithArgumentsAttribute. This class cannot be inherited. Traces member entry and exit, with
    ///     argument values and return values.
    /// </summary>
    [Serializable]
    [AttributeUsage(
        AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method |
        AttributeTargets.Property | AttributeTargets.Struct)]
    [ProvideAspectRole("Trace")]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.Before, "ASCOM")]
    public sealed class NLogTraceWithArgumentsAttribute : OnMethodBoundaryAspect
        {
        private static readonly Type MyType = typeof(NLogTraceWithArgumentsAttribute);
        [NonSerialized] private static int indent;
        private readonly int logAtLevelOrdinal;
        [NonSerialized] private string enteringMessage;
        [NonSerialized] private string exitingMessage;
        [NonSerialized] private Logger log;
        [NonSerialized] private string loggerName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NLogTraceWithArgumentsAttribute" /> class.
        /// </summary>
        /// <param name="logAtLevel">The log at level.</param>
        public NLogTraceWithArgumentsAttribute(int logAtLevel = 0)
            {
            logAtLevelOrdinal = logAtLevel;
            }

        private LogLevel LogAtLevel
            {
            get
                {
                if (logAtLevelOrdinal == LogLevel.Trace.Ordinal)
                    return LogLevel.Trace;
                if (logAtLevelOrdinal == LogLevel.Debug.Ordinal)
                    return LogLevel.Debug;
                if (logAtLevelOrdinal == LogLevel.Info.Ordinal)
                    return LogLevel.Info;
                if (logAtLevelOrdinal == LogLevel.Warn.Ordinal)
                    return LogLevel.Warn;
                if (logAtLevelOrdinal == LogLevel.Error.Ordinal)
                    return LogLevel.Error;
                if (logAtLevelOrdinal == LogLevel.Fatal.Ordinal)
                    return LogLevel.Fatal;
                if (logAtLevelOrdinal == LogLevel.Off.Ordinal)
                    return LogLevel.Off;
                throw new ArgumentException("Invalid trace level specified");
                }
            }

        /// <summary>
        ///     Initializes the current aspect. Invoked only once at runtime from the static constructor of type declaring
        ///     the target method.
        /// </summary>
        /// <param name="method">Method to which the current aspect is applied.</param>
        public override void RuntimeInitialize(MethodBase method)
            {
            if (method.DeclaringType != null) loggerName = method.DeclaringType.FullName;
            var methodName = method.Name;
            enteringMessage = "Enter " + methodName + '(';
            exitingMessage = "Exit " + methodName + "()";
            log = LogManager.GetLogger(loggerName);
            }

        /// <summary>
        ///     Method executed <b>before</b> the body of methods to which this aspect is applied.
        /// </summary>
        /// <param name="args">
        ///     Event arguments specifying which method
        ///     is being executed, which are its arguments, and how should the execution continue
        ///     after the execution of
        ///     <see cref="M:PostSharp.Aspects.IOnMethodBoundaryAspect.OnEntry(PostSharp.Aspects.MethodExecutionArgs)" />.
        /// </param>
        public override void OnEntry(MethodExecutionArgs args)
            {
            base.OnEntry(args);
            var useIndent = Interlocked.Increment(ref indent);
            LogMethodEntryWithParameters(args, useIndent);
            }

        private void LogMethodEntryWithParameters(MethodExecutionArgs args, int indent = 0)
            {
            var builder = new StringBuilder();
            if (indent < 0) indent = 0;
            builder.Append(' ', indent);
            builder.Append(enteringMessage);
            foreach (var argument in args.Arguments)
                {
                if (argument == null)
                    builder.Append("null");
                else
                    {
                    builder.Append(argument.GetType().Name);
                    builder.Append('=');
                    builder.Append(argument);
                    }
                builder.Append(", ");
                }
            if (args.Arguments.Count > 0)
                builder.Length -= 2; // Remove the last comma
            builder.Append(')');
            LogWithUnwoundStack(builder.ToString());
            }

        /// <summary>
        ///     Method executed <b>after</b> the body of methods to which this aspect is applied,
        ///     even when the method exists with an exception (this method is invoked from
        ///     the <c>finally</c> block).
        /// </summary>
        /// <param name="args">
        ///     Event arguments specifying which method
        ///     is being executed and which are its arguments.
        /// </param>
        public override void OnExit(MethodExecutionArgs args)
            {
            base.OnExit(args);
            LogMethodExit(args, indent);
            Interlocked.Decrement(ref indent);
            }

        /// <summary>
        ///     Sends output to NLog while correctly preserving the call site of the original logging event.
        /// </summary>
        /// <param name="message">The verbatim message to be logged.</param>
        private void LogWithUnwoundStack(string message)
            {
            var logEvent = new LogEventInfo(LogAtLevel, loggerName, message);
            log.Log(MyType, logEvent);
            }

        private void LogMethodExit(MethodExecutionArgs args, int indent = 0)
            {
            var builder = new StringBuilder();
            if (indent < 0) indent = 0;
            builder.Append(' ', indent);
            builder.Append(exitingMessage);
            if (args.ReturnValue != null)
                {
                builder.Append(" == ");
                builder.Append(args.ReturnValue);
                }
            LogWithUnwoundStack(builder.ToString());
            }
        }
    }