using System.Windows.Input;

namespace jTech.Wpf.Mvvm
{
    public class DialogViewModel : NotifyPropertyChangedViewModel
    {
        private ICommand _closeWindow;
        public ICommand CloseWindowCmd => _closeWindow ?? (_closeWindow = new RelayCommand(DoCloseWindow));


        protected virtual void DoCloseWindow()
        {
            DialogResult = false;
        }

        private bool? _dialogResult;
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { OnChange(ref _dialogResult, value); }
        }

        private string _windowTitle;
        public string WindowTitle
        {
            get { return _windowTitle; }
            set { OnChange(ref _windowTitle, value); }
        }

    }
}
