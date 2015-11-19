using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Framework.AssetLibrary.Imaging
{
    public static class ImageManipulationExtensions
    {
        /// <summary>
        /// Image format to image codec
        /// </summary>
        /// <param name="imageFormat">The image format.</param>
        /// <returns>ImageCodecInfo.</returns>
        public static ImageCodecInfo ToImageCodecInfo(this ImageFormat imageFormat)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            return encoders.First(codec => codec.FormatID == imageFormat.Guid);
        }

        /// <summary>
        /// To the base64.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="imageFormat">The image format.</param>
        /// <returns>System.String.</returns>
        public static string ToBase64(this Image image, ImageFormat imageFormat)
        {
            MemoryStream memoryStream = new MemoryStream();

            image.Save(memoryStream, imageFormat);

            byte[] imageBytes = memoryStream.ToArray();

            memoryStream.Close();

            memoryStream.Dispose();

            return System.Convert.ToBase64String(imageBytes);
        }

        /// <summary>
        /// To the data URI.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="imageFormat">The image format.</param>
        /// <returns>System.String.</returns>
        public static string ToDataUri(this Image image, ImageFormat imageFormat)
        {
            return $"data:{imageFormat.ToImageCodecInfo().MimeType};base64,{image.ToBase64(imageFormat)}";
        }

        /// <summary>
        /// Resizes the specified image to a new size. (Fit it to the window of the new size and crops over extending edges)
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="newSize">The new size.</param>
        /// <returns>Image.</returns>
        public static Image Resize(this Image image, Size newSize)
        {
            Image imageByHeight = image.ResizeByHeight(newSize.Height, Color.Transparent);
            Image imageByWidth = image.ResizeByWidth(newSize.Width, Color.Transparent);

            if((imageByHeight.Height >= newSize.Height) && (imageByHeight.Width >= newSize.Width))
                image = imageByHeight;
            else if ((imageByWidth.Height >= newSize.Height) && (imageByWidth.Width >= newSize.Width))
                image = imageByWidth;

            int offSetX = 0;
            int offSetY = 0;

            if (image.Width > newSize.Width)
            {
                offSetX = (image.Width / 2) - (newSize.Width / 2);
            }

            if (image.Height > newSize.Height)
            {
                offSetY = (image.Height/2) - (newSize.Height/2);
            }

            return image.Crop(new Point((int)offSetX, (int)offSetY), newSize);
        }

        /// <summary>
        /// Resizes the specified image to a new size. (Maintains aspect ratio if aspect ratio is set to true and fills the background of the window with the specified color) 
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="newSize">The new size.</param>
        /// <param name="backgroundFillColor">Color of the background fill.</param>
        /// <param name="maintainAspectRatio">if set to <c>true</c> [maintain aspect ratio].</param>
        /// <returns>Image.</returns>
        public static Image Resize(this Image image, Size newSize, Color backgroundFillColor, bool maintainAspectRatio = true)
        {
            ImageManipulation imageManipulation = new ImageManipulation();
            return imageManipulation.Resize(image, newSize, backgroundFillColor, maintainAspectRatio);
        }

        /// <summary>
        /// Resizes an image by width
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="width">The width.</param>
        /// <param name="backgroundFillColor">Color of the background fill.</param>
        /// <returns>Image.</returns>
        public static Image ResizeByWidth(this Image image, int width, Color backgroundFillColor)
        {
            ImageManipulation imageManipulation = new ImageManipulation();
            return imageManipulation.ResizeByWidth(image, width, backgroundFillColor);
        }

        /// <summary>
        /// Resizes and image by height
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="height">The height.</param>
        /// <param name="backgroundFillColor">Color of the background fill.</param>
        /// <returns>Image.</returns>
        public static Image ResizeByHeight(this Image image, int height, Color backgroundFillColor)
        {
            ImageManipulation imageManipulation = new ImageManipulation();
            return imageManipulation.ResizeByHeight(image, height, backgroundFillColor);
        }

        /// <summary>
        /// Converts the specified image format.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="imageFormat">The image format.</param>
        /// <returns>Image.</returns>
        public static Image Convert(this Image image, ImageFormat imageFormat)
        {
            ImageManipulation imageManipulation = new ImageManipulation();
            return imageManipulation.Convert(image, imageFormat);
        }

        /// <summary>
        /// Crops the specified image to new size from the specified top left points.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="topLeft">The top left.</param>
        /// <param name="newSize">The new size.</param>
        /// <returns>Image.</returns>
        public static Image Crop(this Image image, Point topLeft, Size newSize)
        {
            ImageManipulation imageManipulation = new ImageManipulation();
            return imageManipulation.Crop(image, topLeft, newSize);
        }

        /// <summary>
        /// Crops the background. (Removes unwanted backgrounds)
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="pass">The pass.</param>
        /// <returns>Image.</returns>
        public static Image CropBackground(this Image image, double  pass = 1)
        {
            ImageManipulation imageManipulation = new ImageManipulation();

            for (int count = 0; count < pass; count ++)
            {
                image = imageManipulation.CropBackground(image);
            }

            return image;
        }

        /// <summary>
        /// Optimizes the specified image format based on the specified quality
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="imageFormat">The image format.</param>
        /// <param name="quality">The quality.</param>
        /// <returns>Image.</returns>
        public static Image Optimize(this Image image, ImageFormat imageFormat, ImageQuality quality)
        {
            ImageManipulation imageManipulation = new ImageManipulation();
            return imageManipulation.Optimize(image, imageFormat, quality);
        }

        /// <summary>
        /// Grayscale filter
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Image.</returns>
        public static Image Grayscale(this Image image)
        {
            ImageFilter imageFilter = new ImageFilter();
            return imageFilter.Grayscale(new Bitmap(image));
        }

        /// <summary>
        /// Difference filter
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="comparingImage">The comparing image.</param>
        /// <returns>Image.</returns>
        public static Image Difference(this Image image, Image comparingImage)
        {
            ImageFilter imageFilter = new ImageFilter();
            return imageFilter.Difference(new Bitmap(image), new Bitmap(comparingImage));
        }

        /// <summary>
        /// Brightnesses
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="percent">The percent.</param>
        /// <returns>Image.</returns>
        public static Image Brightness(this Image image, int percent)
        {
            ImageFilter imageFilter = new ImageFilter();
            return imageFilter.Brightness(new Bitmap(image), percent);
        }

        /// <summary>
        /// Erosion filter
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="matrixSize">Size of the matrix.</param>
        /// <param name="applyBlue">if set to <c>true</c> [apply blue].</param>
        /// <param name="applyGreen">if set to <c>true</c> [apply green].</param>
        /// <param name="applyRed">if set to <c>true</c> [apply red].</param>
        /// <param name="edgeType">Type of the edge.</param>
        /// <returns>Image.</returns>
        public static Image Erosion(this Image image, int matrixSize, bool applyBlue = true, bool applyGreen = true, bool applyRed = true, MorphologyEdgeType edgeType = MorphologyEdgeType.None)
        {
            ImageFilter imageFilter = new ImageFilter();

            return imageFilter.DilateAndErodeFilter(new Bitmap(image), matrixSize, MorphologyType.Erosion, applyBlue, applyGreen, applyRed, edgeType);
        }

        /// <summary>
        /// Dilation filter
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="matrixSize">Size of the matrix.</param>
        /// <param name="applyBlue">if set to <c>true</c> [apply blue].</param>
        /// <param name="applyGreen">if set to <c>true</c> [apply green].</param>
        /// <param name="applyRed">if set to <c>true</c> [apply red].</param>
        /// <param name="edgeType">Type of the edge.</param>
        /// <returns>Image.</returns>
        public static Image Dilation(this Image image, int matrixSize, bool applyBlue = true, bool applyGreen = true, bool applyRed = true, MorphologyEdgeType edgeType = MorphologyEdgeType.None)
        {
            ImageFilter imageFilter = new ImageFilter();

            return imageFilter.DilateAndErodeFilter(new Bitmap(image), matrixSize, MorphologyType.Dilation, applyBlue, applyGreen, applyRed, edgeType);
        }

        /// <summary>
        /// Balance filter
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="red">The red.</param>
        /// <param name="blue">The blue.</param>
        /// <param name="green">The green.</param>
        /// <returns>Image.</returns>
        public static Image Balance(this Image image, int red, int blue, int green)
        {
            ImageFilter imageFilter = new ImageFilter();
            return imageFilter.Balance(new Bitmap(image), red, blue, green);
        }

        /// <summary>
        /// Channel filter
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>Image.</returns>
        public static Image Channel(this Image image, Rgb filter)
        {
            ImageFilter imageFilter = new ImageFilter();
            return imageFilter.Channel(new Bitmap(image), filter);
        }

        /// <summary>
        /// Channel filter
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="comparingImage">The comparing image.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>Image.</returns>
        public static Image Channel(this Image image, Image comparingImage,  Rgb filter)
        {
            ImageFilter imageFilter = new ImageFilter();
            return imageFilter.Channel(new Bitmap(image), new Bitmap(comparingImage), filter);
        }

        /// <summary>
        /// Overlay two images
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="overlayingImage">The overlaying image.</param>
        /// <param name="alpha">The alpha.</param>
        /// <returns>Image.</returns>
        public static Image Overlay(this Image image, Image overlayingImage, double alpha = 0.5)
        {
            ImageFilter imageFilter = new ImageFilter();
            return imageFilter.Overlay(new Bitmap(image),  new Bitmap(overlayingImage), alpha);
        }

        /// <summary>
        /// Overlay a color on a image
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="alpha">The alpha.</param>
        /// <returns>Image.</returns>
        public static Image Overlay(this Image image, Brush brush, double alpha = 0.5)
        {
            Bitmap colorBitmap = new Bitmap(image.Width, image.Height);

            using (Graphics graph = Graphics.FromImage(colorBitmap))
            {
                Rectangle rectangle = new Rectangle(0, 0, colorBitmap.Width, colorBitmap.Height);
                graph.FillRectangle(brush, rectangle);
            }

            ImageFilter imageFilter = new ImageFilter();

            return imageFilter.Overlay(new Bitmap(image), colorBitmap, alpha);
        }

        /// <summary>
        /// Sobel filter
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Image.</returns>
        public static Image Sobel(this Image image)
        {
            ImageFilter imageFilter = new ImageFilter();
            return imageFilter.Sobel(new Bitmap(image));
        }

        /// <summary>
        /// Blur filter
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="blur">The blur.</param>
        /// <returns>Image.</returns>
        public static Image Blur(this Image image, int blur = 5)
        {
            ImageFilter imageFilter = new ImageFilter();
            return imageFilter.Blur(new Bitmap(image), new Rectangle(0, 0, image.Width, image.Height), blur);
        }

        /// <summary>
        /// Blur filter
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="blur">The blur.</param>
        /// <returns>Image.</returns>
        public static Image Blur(this Image image, Rectangle rectangle, int blur = 5)
        {
            ImageFilter imageFilter = new ImageFilter();
            return imageFilter.Blur(new Bitmap(image), rectangle, blur);
        }
    }
}
