// This file is part of the GTD.Integra.FocusingRotator project
// 
// Copyright © 2016-2017 Tigra Astronomy., all rights reserved.
// 
// File: MustBeConnectedAttribute.cs  Last modified: 2017-02-13@01:17 by Tim Long

using System;
using System.Reflection;
using ASCOM;
using PostSharp.Aspects;
using PostSharp.Aspects.Dependencies;
using PostSharp.Extensibility;

namespace TA.PostSharp.Aspects
    {
    /// <summary>
    ///     MustBeConnected aspect. Verifies that the controlled device is connected and if not,
    ///     throws a
    ///     <see cref="NotConnectedException" />.
    /// </summary>
    [Serializable]
    [ProvideAspectRole("ASCOM")]
    public sealed class MustBeConnectedAttribute : OnMethodBoundaryAspect
        {
        static int nesting;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MustBeConnectedAttribute" /> class. Forces
        ///     the <see cref="ApplyToStateMachine" /> property to false, as this aspect should only
        ///     check advices upon first entry to any method.
        /// </summary>
        public MustBeConnectedAttribute()
            {
            ApplyToStateMachine = false;
            }

        public override bool CompileTimeValidate(MethodBase method)
            {
            var targetType = method.DeclaringType;
            if (!typeof(IAscomDriver).IsAssignableFrom(targetType))
                {
                throw new InvalidAnnotationException(
                    "This aspect can only be applied to members of types that implement IAscomDriver");
                }
            return base.CompileTimeValidate(method);
            }

        /// <summary>
        ///     Verifies that the  device is connected. Throws <see cref="NotConnectedException" />
        ///     if not.
        /// </summary>
        /// <param name="args">
        ///     Event arguments specifying which method is being executed, which are its arguments,
        ///     and how should the execution continue after the execution of
        ///     <see cref="M:PostSharp.Aspects.IOnMethodBoundaryAspect.OnEntry(PostSharp.Aspects.MethodExecutionArgs)" />
        /// </param>
        /// <exception cref="NotConnectedException">
        ///     Thrown if the device is not connected.
        /// </exception>
        public override void OnEntry(MethodExecutionArgs args)
            {
            base.OnEntry(args);
            var instance = args.Instance as IAscomDriver;
            if (nesting++ > 0) return; // Optimization - no need to check in nested calls.
            if (!instance.Connected)
                {
                var name = args.Method.Name;
                var message = $"{name} requires that Connected is true but it was false";
                throw new NotConnectedException(message);
                }
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
            nesting--;
            }
        }
    }