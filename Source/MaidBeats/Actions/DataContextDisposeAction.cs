using System;
using System.Windows;

using Microsoft.Xaml.Behaviors;

namespace MaidBeats.Actions
{
    internal class DataContextDisposeAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            var disposable = AssociatedObject.DataContext as IDisposable;
            disposable?.Dispose();
        }
    }
}