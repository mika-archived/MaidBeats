using Prism.Mvvm;

namespace MaidBeats.Models
{
    public class StatusService : BindableBase
    {
        public StatusService()
        {
            Text = "Ready";
        }

        #region Text

        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                    SetProperty(ref _text, value);
            }
        }

        #endregion
    }
}