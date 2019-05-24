using System;
using System.Diagnostics;
using System.Windows.Input;

using MaidBeats.Models;

using Microsoft.Xaml.Behaviors.Core;

using Reactive.Bindings;

namespace MaidBeats.ViewModels.Tabs
{
    internal class AboutTabViewModel : TabBaseViewModel
    {
        public override ReactiveProperty<string> Title { get; }
        public string Version => $"Version {MaidBeatsInfo.Version.Value}";

        public AboutTabViewModel()
        {
            Title = new ReactiveProperty<string>("About MaidBeats");
        }

        #region NavigateUrlCommand

        private ICommand _navigateUrlCommand;

        public ICommand NavigateUrlCommand => _navigateUrlCommand ?? (_navigateUrlCommand = new ActionCommand(NavigateUrl));

        private void NavigateUrl(object args)
        {
            Process.Start(args as string ?? throw new InvalidOperationException());
        }

        #endregion
    }
}