﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;

// https://gist.github.com/thomaslevesque/10023516

namespace Steam_Library_Manager.Framework
{
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

        private void ExecuteOnSyncContext(Action action)
        {
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                action();
            }
            else
            {
                _synchronizationContext.Send(_ => action(), null);
            }
        }

        protected override void InsertItem(int index, T item) => ExecuteOnSyncContext(() => base.InsertItem(index, item));

        protected override void RemoveItem(int index)
        {
            try
            {
                ExecuteOnSyncContext(() => base.RemoveItem(index));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show(ex.ToString());
                Functions.Logger.LogToFile(Functions.Logger.LogType.SLM, ex.ToString());
            }
        }

        protected override void SetItem(int index, T item) => ExecuteOnSyncContext(() => base.SetItem(index, item));

        protected override void MoveItem(int oldIndex, int newIndex) => ExecuteOnSyncContext(() => base.MoveItem(oldIndex, newIndex));

        protected override void ClearItems() => ExecuteOnSyncContext(() => base.ClearItems());
    }
}
