namespace PixelCrypt2026.Program
{
    class OperationStatusService
    {
        private static OperationStatusService _instance;
        public static OperationStatusService Instance => _instance ??= new OperationStatusService();

        public event Action<Type, string> StatusChanged;

        public void UpdateStatus(Type type, string status)
        {
            StatusChanged?.Invoke(type, status);
        }
    }
}
