namespace PixelCrypt2026.Model
{
    public class ControlAction
    {
        public Func<bool> CanExecute = () => true;
        public Action ExecuteRequested;
        public Func<bool> ConfirmationRequired = () => true;
    }
}
