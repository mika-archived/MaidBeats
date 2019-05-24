using MaidBeats.Mvvm;

using Reactive.Bindings;

namespace MaidBeats.ViewModels.Tabs
{
    public abstract class TabBaseViewModel : ViewModel
    {
        public abstract ReactiveProperty<string> Title { get; }
    }
}