namespace Fovea.Renderer.Image
{
    public readonly struct ImageSize
    {
        public ImageSize(int imageWidth, int imageHeight)
        {
            ImageWidth = imageWidth;
            ImageHeight = imageHeight;
        }

        public readonly int ImageWidth;
        public readonly int ImageHeight;
    }
}