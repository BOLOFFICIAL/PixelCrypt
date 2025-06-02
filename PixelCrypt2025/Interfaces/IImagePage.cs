namespace PixelCrypt2025.Interfaces
{
    internal interface IImagePage<T>
    {
        public List<Model.Image> ContextImage { get; }
        public void AddElement(string path);
        public void RemoveElement(Model.Image path);
        public T InputData { get; }
    }
}
