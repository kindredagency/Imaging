using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;


namespace Framework.AssetLibrary.Imaging
{
    internal class ImageManipulation
    {
        /// <summary>
        /// Converts the specified raw image.
        /// </summary>
        /// <param name="rawImage">The raw image.</param>
        /// <param name="imageFormat">The image format.</param>
        /// <returns>Image.</returns>
        public Image Convert(Image rawImage, ImageFormat imageFormat)
        {
            try
            {
                MemoryStream memStream = new MemoryStream();

                rawImage.Save(memStream, imageFormat);

                Bitmap bitmap = new Bitmap(memStream);

                return bitmap;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }

        /// <summary>
        /// Resizes the specified raw image.
        /// </summary>
        /// <param name="rawImage">The raw image.</param>
        /// <param name="newSize">The new size.</param>
        /// <param name="backgroundFillColor">Color of the background fill.</param>
        /// <returns>Image.</returns>
        public Image Resize(Image rawImage, Size newSize, Color backgroundFillColor)
        {
            try
            {
                double ratio = 0d;
                double myThumbWidth = 0d;
                double myThumbHeight = 0d;

                int x = 0;
                int y = 0;

                if ((rawImage.Width / System.Convert.ToDouble(newSize.Width)) >
                    (rawImage.Height / System.Convert.ToDouble(newSize.Height)))
                {
                    ratio = System.Convert.ToDouble(rawImage.Width) / System.Convert.ToDouble(newSize.Width);
                }
                else
                {
                    ratio = System.Convert.ToDouble(rawImage.Height) / System.Convert.ToDouble(newSize.Height);
                }

                myThumbHeight = Math.Ceiling(rawImage.Height / ratio);
                myThumbWidth = Math.Ceiling(rawImage.Width / ratio);

                Size thumbSize = new Size((int)myThumbWidth, (int)myThumbHeight);

                Image bitmap = backgroundFillColor == Color.Transparent
                    ? new Bitmap(newSize.Width, newSize.Height, PixelFormat.Format32bppArgb)
                    : new Bitmap(newSize.Width, newSize.Height);

                x = (newSize.Width - thumbSize.Width) / 2;
                y = (newSize.Height - thumbSize.Height) / 2;

                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                SolidBrush brush = new SolidBrush(backgroundFillColor);
                graphics.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);

                Rectangle rect = new Rectangle(x, y, thumbSize.Width, thumbSize.Height);
                graphics.DrawImage(rawImage, rect, 0, 0, rawImage.Width, rawImage.Height, GraphicsUnit.Pixel);

                return bitmap;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }

        /// <summary>
        /// Resizes the specified raw image.
        /// </summary>
        /// <param name="rawImage">The raw image.</param>
        /// <param name="newSize">The new size.</param>
        /// <param name="backgroundFillColor">Color of the background fill.</param>
        /// <param name="maintainAspectRatio">if set to <c>true</c> [maintain aspect ratio].</param>
        /// <returns>Image.</returns>
        public Image Resize(Image rawImage, Size newSize, Color backgroundFillColor, bool maintainAspectRatio)
        {
            try
            {
                if (maintainAspectRatio)
                {
                    return Resize(rawImage, newSize, backgroundFillColor);
                }

                int x = 0;
                int y = 0;

                Size thumbSize = newSize;

                Image bitmap = backgroundFillColor == Color.Transparent
                    ? new Bitmap(newSize.Width, newSize.Height, PixelFormat.Format32bppArgb)
                    : new Bitmap(newSize.Width, newSize.Height);

                x = (newSize.Width - thumbSize.Width) / 2;
                y = (newSize.Height - thumbSize.Height) / 2;

                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                SolidBrush brush = new SolidBrush(backgroundFillColor);
                graphics.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);

                Rectangle rect = new Rectangle(x, y, thumbSize.Width, thumbSize.Height);
                graphics.DrawImage(rawImage, rect, 0, 0, rawImage.Width, rawImage.Height, GraphicsUnit.Pixel);

                return bitmap;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }

        /// <summary>
        /// Resizes the raw image by width
        /// </summary>
        /// <param name="rawImage">The raw image.</param>
        /// <param name="width">The width.</param>
        /// <param name="backgroundFillColor">Color of the background fill.</param>
        /// <returns>Image.</returns>
        public Image ResizeByWidth(Image rawImage, int width, Color backgroundFillColor)
        {
            try
            {
                double ratio = 0d;
                double myThumbWidth = 0d;
                double myThumbHeight = 0d;

                int x = 0;
                int y = 0;

                ratio = System.Convert.ToDouble(rawImage.Width) / System.Convert.ToDouble(width);

                myThumbHeight = Math.Ceiling(rawImage.Height / ratio);
                myThumbWidth = Math.Ceiling(rawImage.Width / ratio);

                Size thumbSize = new Size((int)myThumbWidth, (int)myThumbHeight);

                Image bitmap = new Bitmap((int)myThumbWidth, (int)myThumbHeight);

                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                SolidBrush brush = new SolidBrush(backgroundFillColor);
                graphics.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);

                Rectangle rect = new Rectangle(x, y, thumbSize.Width, thumbSize.Height);
                graphics.DrawImage(rawImage, rect, 0, 0, rawImage.Width, rawImage.Height, GraphicsUnit.Pixel);

                return bitmap;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }

        /// <summary>
        /// Resizes the raw image by height
        /// </summary>
        /// <param name="rawImage">The raw image.</param>
        /// <param name="height">The height.</param>
        /// <param name="backgroundFillColor">Color of the background fill.</param>
        /// <returns>Image.</returns>
        public Image ResizeByHeight(Image rawImage, int height, Color backgroundFillColor)
        {
            try
            {
                double ratio = 0d;
                double myThumbWidth = 0d;
                double myThumbHeight = 0d;

                int x = 0;
                int y = 0;

                ratio = System.Convert.ToDouble(rawImage.Height) / System.Convert.ToDouble(height);

                myThumbHeight = Math.Ceiling(rawImage.Height / ratio);
                myThumbWidth = Math.Ceiling(rawImage.Width / ratio);

                Size thumbSize = new Size((int)myThumbWidth, (int)myThumbHeight);

                Bitmap bitmap = new Bitmap((int)myThumbWidth, (int)myThumbHeight);

                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                SolidBrush brush = new SolidBrush(backgroundFillColor);
                graphics.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);

                Rectangle rect = new Rectangle(x, y, thumbSize.Width, thumbSize.Height);
                graphics.DrawImage(rawImage, rect, 0, 0, rawImage.Width, rawImage.Height, GraphicsUnit.Pixel);

                return bitmap;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }

        /// <summary>
        /// Crops the specified raw image.
        /// </summary>
        /// <param name="rawImage">The raw image.</param>
        /// <param name="topLeft">The top left.</param>
        /// <param name="newSize">The new size.</param>
        /// <returns>Image.</returns>
        public Image Crop(Image rawImage, Point topLeft, Size newSize)
        {
            try
            {
                if (rawImage.Width < (topLeft.X + newSize.Width))
                {
                    newSize.Width = rawImage.Width - topLeft.X;
                }

                if (rawImage.Height < (topLeft.Y + newSize.Height))
                {
                    newSize.Height = rawImage.Height - topLeft.Y;
                }

                Bitmap clippedImage = new Bitmap(rawImage);

                clippedImage = clippedImage.Clone(new Rectangle(topLeft.X, topLeft.Y, newSize.Width, newSize.Height),
                    rawImage.PixelFormat);

                return clippedImage;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }

        /// <summary>
        /// Crops the background.
        /// </summary>
        /// <param name="rawImage">The raw image.</param>
        /// <returns>Image.</returns>
        public Image CropBackground(Image rawImage)
        {
            try
            {
                Bitmap bitmap = new Bitmap(rawImage);

                Color backColor = Color.Transparent;

                var corners = new[]
                {
                new Point(0, 0),
                new Point(0, bitmap.Height - 1),
                new Point(bitmap.Width - 1, 0),
                new Point(bitmap.Width - 1, bitmap.Height - 1)
            };

                for (var i = 0; i < 4; i++)
                {
                    var cornerMatched = 0;

                    Color tempBackColor = bitmap.GetPixel(corners[i].X, corners[i].Y);

                    for (var j = 0; j < 4; j++)
                    {
                        var cornerColor = bitmap.GetPixel(corners[j].X, corners[j].Y);
                        if ((cornerColor.R <= tempBackColor.R * 1.1 && cornerColor.R >= tempBackColor.R * 0.9) &&
                            (cornerColor.G <= tempBackColor.G * 1.1 && cornerColor.G >= tempBackColor.G * 0.9) &&
                            (cornerColor.B <= tempBackColor.B * 1.1 && cornerColor.B >= tempBackColor.B * 0.9))
                        {
                            cornerMatched++;
                        }
                    }

                    if (cornerMatched > 2)
                    {
                        backColor = tempBackColor;
                    }
                }

                Point[] bounds = new Point[2];

                int width = bitmap.Width, height = bitmap.Height;

                bool upperLeftPointFounded = false;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var processingColor = bitmap.GetPixel(x, y);

                        bool sameAsBackColor = ((processingColor.R <= backColor.R * 1.1 &&
                                                 processingColor.R >= backColor.R * 0.9) &&
                                                (processingColor.G <= backColor.G * 1.1 &&
                                                 processingColor.G >= backColor.G * 0.9) &&
                                                (processingColor.B <= backColor.B * 1.1 &&
                                                 processingColor.B >= backColor.B * 0.9));
                        if (!sameAsBackColor)
                        {
                            if (!upperLeftPointFounded)
                            {
                                bounds[0] = new Point(x, y);
                                bounds[1] = new Point(x, y);
                                upperLeftPointFounded = true;
                            }
                            else
                            {
                                if (x > bounds[1].X)
                                {
                                    bounds[1].X = x;
                                }
                                else if (x < bounds[0].X)
                                {
                                    bounds[0].X = x;
                                }
                                if (y >= bounds[1].Y)
                                {
                                    bounds[1].Y = y;
                                }
                            }
                        }
                    }
                }

                int differenceX = bounds[1].X - bounds[0].X + 1;

                int differenceY = bounds[1].Y - bounds[0].Y + 1;

                Bitmap croppedBitmap = new Bitmap(differenceX, differenceY);

                Graphics graphics = Graphics.FromImage(croppedBitmap);

                Rectangle destinationRectangle = new Rectangle(0, 0, croppedBitmap.Width, croppedBitmap.Height);

                Rectangle sourceRectangle = new Rectangle(bounds[0].X, bounds[0].Y, differenceX, differenceY);

                graphics.DrawImage(bitmap, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);

                return croppedBitmap;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }

        /// <summary>
        /// Optimizes the specified raw image. (Compresses an image based on quality)
        /// </summary>
        /// <param name="rawImage">The raw image.</param>
        /// <param name="imageFormat">The image format.</param>
        /// <param name="quality">The quality.</param>
        /// <returns>Image.</returns>
        public Image Optimize(Image rawImage, ImageFormat imageFormat, ImageQuality quality)
        {
            try
            {
                Encoder myEncoder = Encoder.Quality;

                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, (long)quality);

                myEncoderParameters.Param[0] = myEncoderParameter;

                MemoryStream stream = new MemoryStream();

                rawImage.Save(stream, imageFormat.ToImageCodecInfo(), myEncoderParameters);

                Bitmap compressedBitmap = new Bitmap(stream);

                return compressedBitmap;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }
    }
}