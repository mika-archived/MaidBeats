using System;
using System.Diagnostics;
using System.Windows.Input;

using MaidBeats.Models;

using Microsoft.Xaml.Behaviors.Core;

namespace MaidBeats.ViewModels.Tabs
{
    internal class AboutTabViewModel : TabBaseViewModel
    {
        public string Version => $"Version {MaidBeatsInfo.Version.Value}";

        public AboutTabViewModel() : base("About MaidBeats") { }

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