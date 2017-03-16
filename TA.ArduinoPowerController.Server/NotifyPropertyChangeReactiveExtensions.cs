// This file is part of the ASCOM.K8056.Switch project
// 
// Copyright © 2016-2017 Tigra Astronomy, all rights reserved.
// Licensed under the MIT license, see http://tigra.mit-license.org/
// 
// File: NotifyPropertyChangeReactiveExtensions.cs  Last modified: 2017-03-07@20:09 by Tim Long

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;

namespace TA.VellemanK8056.Server
    {
    public static class NotifyPropertyChangeReactiveExtensions
        {
        // Returns the values of property (an Expression) as they
        // change, starting with the current value
        /// <summary>
        ///     Gets an observable sequence that produces a new value whenever the value of a
        ///     property (member expression) changes, starting with the current value.
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="property">The property.</param>
        /// <returns>IObservable&lt;TValue&gt;.</returns>
        /// <exception cref="System.ArgumentException">
        ///     property must directly access " + "a property of the source
        /// </exception>
        public static IObservable<TValue> GetObservableValueFor<TSource, TValue>(
            this TSource source, Expression<Func<TSource, TValue>> property)
            where TSource : INotifyPropertyChanged
            {
            var memberExpression =
                property.Body as MemberExpression;

            if (memberExpression == null)
                {
                throw new ArgumentException(
                    "property must directly access " +
                    "a property of the source");
                }

            var propertyName = memberExpression.Member.Name;

            var accessor = property.Compile();

            return source.GetPropertyChangedEvents()
                .Where(x => x.EventArgs.PropertyName == propertyName)
                .Select(x => accessor(source))
                .StartWith(accessor(source));
            }

        // This is a wrapper around FromEvent(PropertyChanged)
        public static IObservable<EventPattern<PropertyChangedEventArgs>> GetPropertyChangedEvents(
            this INotifyPropertyChanged source)
            {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler => handler.Invoke,
                handler => source.PropertyChanged += handler,
                handler => source.PropertyChanged -= handler
            );
            }

        public static IDisposable
            Subscribe<TSource, TValue>(
                this TSource source,
                Expression<Func<TSource, TValue>> property,
                Action<TValue> observer)
            where TSource : INotifyPropertyChanged
            {
            return source
                .GetObservableValueFor(property)
                .Subscribe(observer);
            }

        public static IDisposable
            Subscribe<TSource, TValue>(
                this TSource source,
                Expression<Func<TSource, TValue>> property,
                Action<TValue> observer,
                Action onCompleted)
            where TSource : INotifyPropertyChanged
            {
            return source
                .GetObservableValueFor(property)
                .Subscribe(observer, onCompleted);
            }

        public static IDisposable
            Subscribe<TSource, TValue>(
                this TSource source,
                Expression<Func<TSource, TValue>> property,
                Action<TValue> observer,
                Action<Exception> onException)
            where TSource : INotifyPropertyChanged
            {
            return source
                .GetObservableValueFor(property)
                .Subscribe(observer, onException);
            }

        public static IDisposable
            Subscribe<TSource, TValue>(
                this TSource source,
                Expression<Func<TSource, TValue>> property,
                Action<TValue> observer,
                Action<Exception> onException,
                Action onCompleted)
            where TSource : INotifyPropertyChanged
            {
            return source
                .GetObservableValueFor(property)
                .Subscribe(observer, onException, onCompleted);
            }
        }
    }