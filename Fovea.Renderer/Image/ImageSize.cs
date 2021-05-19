namespace Fovea.Renderer.Image
{
    public readonly struct ImageSize
    {
        public ImageSize(int imageWidth, int imageHeight, float aspectRatio)
        {
            ImageWidth = imageWidth;
            ImageHeight = imageHeight;
            AspectRatio = aspectRatio;
        }

        public readonly int ImageWidth;
        public readonly int ImageHeight;
        public readonly float AspectRatio;
    }
}