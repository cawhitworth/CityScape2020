// <copyright file="Component.cs" company="Chris Whitworth">
// Copyright (c) Chris Whitworth. All rights reserved.
// </copyright>

namespace CityScape2020
{
    using System;
    using System.Collections.Generic;

    internal class Component : IDisposable
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        public void Dispose()
        {
            foreach (var disposable in this.disposables)
            {
                disposable.Dispose();
            }

            this.disposables.Clear();
        }

        protected T ToDispose<T>(T disposable)
            where T : class, IDisposable
        {
            this.disposables.Add(disposable);

            return disposable;
        }
    }
}