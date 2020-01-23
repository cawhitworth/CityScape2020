using System;
using System.Collections.Generic;

namespace CityScape2
{
    internal class Component : IDisposable
    {
        List<IDisposable> disposables = new List<IDisposable>();

        public void Dispose()
        {
            foreach(var disposable in disposables)
            {
                disposable.Dispose();
            }
            disposables.Clear();
        }

        protected T ToDispose<T>(T disposable) where T : class, IDisposable
        {
            this.disposables.Add(disposable);

            return disposable;
        }

    }
}