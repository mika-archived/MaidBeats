using MaidBeats.Extensions;
using MaidBeats.Mvvm;
using MaidBeats.ViewModels.Tabs;

using Reactive.Bindings;

namespace MaidBeats.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        public ReactiveProperty<string> Title { get; }
        public ReactiveCollection<TabBaseViewModel> TabItems { get; }

        public MainWindowViewModel()
        {
            Title = new ReactiveProperty<string>("MaidBeats - Mod Installer / Manager for Beat Saber").AddTo(this);
            TabItems = new ReactiveCollection<TabBaseViewModel>
            {
                new AboutTabViewModel().AddTo(this)
            }.AddTo(this);
        }
    }
}