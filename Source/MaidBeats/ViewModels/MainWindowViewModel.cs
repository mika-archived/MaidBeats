using MaidBeats.Extensions;
using MaidBeats.Mvvm;

using Reactive.Bindings;

namespace MaidBeats.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        public ReactiveProperty<string> Title { get; }

        public MainWindowViewModel()
        {
            Title = new ReactiveProperty<string>("MaidBeats - Mod Installer / Manager for Beat Saber").AddTo(this);
        }
    }
}