namespace PixelCrypt2025.Interfaces
{
    internal interface IImagePage
    {
        public List<Model.Image> InputImage { get; }
        public Dictionary<Model.Image, Model.ResultImage> OutputImage { get; }
    }
}
