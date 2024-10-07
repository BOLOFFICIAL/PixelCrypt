using PixelCrypt.Commands.Base;
using PixelCrypt.ProgramData;
using PixelCrypt.View;
using PixelCrypt.View.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PixelCrypt.ViewModel.Page
{
    internal class TextInPicturePageViewModel : Base.ViewModel
    {
        private string _actionButtonName = "";

        private bool _isSplit = false;
        private bool _isImport = false;

        public ICommand ClosePageCommand { get; }
        public ICommand ActionCommand { get; }
        public ICommand SplitCommand { get; }

        public TextInPicturePageViewModel() 
        {
            ClosePageCommand = new LambdaCommand(OnClosePageCommandExecuted);
            ActionCommand = new LambdaCommand(OnActionCommandExecuted);
            SplitCommand = new LambdaCommand(OnSplitCommandExecuted);

            ImportAction();
        }

        public string ActionButtonName 
        {
            get => _actionButtonName;
            set => Set(ref _actionButtonName, value);
        }

        private void OnClosePageCommandExecuted(object p)
        {
            Context.MainWindowViewModel.CurrentPage = new MainPage();
        }

        private void OnSplitCommandExecuted(object p)
        {
            
        }

        private void OnActionCommandExecuted(object p) 
        {
            if (p is not string actionName) return;

            switch (actionName)
            {
                case "Import": ImportAction();break;
                case "Export": ExportAction(); break;
            }
        }

        private void ImportAction() 
        {
            ActionButtonName = "Импортировать";
            _isImport = true;
        }

        private void ExportAction()
        {
            ActionButtonName = "Экспортировать";
            _isImport = false;
        }
    }
}
