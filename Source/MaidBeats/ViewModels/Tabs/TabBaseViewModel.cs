using MaidBeats.Mvvm;

namespace MaidBeats.ViewModels.Tabs
{
    public abstract class TabBaseViewModel : ViewModel
    {
        public string Title { get; }

        protected TabBaseViewModel(string title)
        {
            Title = title;
        }
    }
}