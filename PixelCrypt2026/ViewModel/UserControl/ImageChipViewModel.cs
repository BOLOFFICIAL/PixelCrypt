using PixelCrypt2026.Commands.Base;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace PixelCrypt2026.ViewModel.UserControl
{
    class ImageChipViewModel
    {
        private readonly ImageListViewModel _parent;

        public string ImagePath { get; }

        public string FileName { get; }

        public ICommand MoveUpCommand { get; }

        public ICommand MoveDownCommand { get; }

        public ICommand RemoveCommand { get; }

        public ICommand OpenOriginalCommand { get; }

        public ImageChipViewModel(string filePath, ImageListViewModel parent)
        {
            _parent = parent;

            FileName = Path.GetFileName(filePath);
            ImagePath = filePath;

            MoveUpCommand = new LambdaCommand(OnMoveUp, CanMoveUp);
            MoveDownCommand = new LambdaCommand(OnMoveDown, CanMoveDown);
            RemoveCommand = new LambdaCommand(OnRemove);
            OpenOriginalCommand = new LambdaCommand(OnOpenOriginal);
        }

        private void OnMoveUp(object p)
        {
            int index = _parent.Images.IndexOf(this);

            if (index <= 0)
                return;

            _parent.Images.Move(index, index - 1);
        }

        private bool CanMoveUp(object p)
            => _parent.Images.IndexOf(this) > 0;

        private void OnMoveDown(object p)
        {
            int index = _parent.Images.IndexOf(this);

            if (index < 0 || index >= _parent.Images.Count - 1)
                return;

            _parent.Images.Move(index, index + 1);
        }

        private bool CanMoveDown(object p)
            => _parent.Images.IndexOf(this) < _parent.Images.Count - 1;

        private void OnRemove(object p)
        {
            _parent.Images.Remove(this);
        }

        private void OnOpenOriginal(object p)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = ImagePath,
                UseShellExecute = true
            });
        }
    }
}
