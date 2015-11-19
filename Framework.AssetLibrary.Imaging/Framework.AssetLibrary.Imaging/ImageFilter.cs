using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Framework.AssetLibrary.Imaging
{
    /// <summary>
    /// Class Filters.
    /// </summary>
    internal class ImageFilter
    {
        /// <summary>
        /// Brightness
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="percent">The percent.</param>
        /// <returns>Bitmap.</returns>
        public Bitmap Brightness(Bitmap image, int percent)
        {
            try
            {
                BitmapData fileData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int stride = fileData.Stride;

                IntPtr scan0 = fileData.Scan0;

                unsafe
                {
                    byte* p = (byte*)(void*)scan0;

                    int nOffset = stride - image.Width * 3;

                    int nWidth = image.Width * 3;

                    for (int y = 0; y < image.Height; y++)
                    {
                        for (int x = 0; x < nWidth; x++)
                        {
                            var nVal = (int)(p[0] + percent);
                            if (nVal < 0) nVal = 0;
                            if (nVal > 255) nVal = 255;
                            p[0] = (byte)nVal;
                            p++;
                        }

                        p = p + nOffset;
                    }
                }

                image.UnlockBits(fileData);

                return image;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }

        /// <summary>
        /// Grayscales the specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Bitmap.</returns>
        public Bitmap Grayscale(Bitmap image)
        {
            try
            {
                ColorMatrix cm = new ColorMatrix(new float[][]{ new float[]{0.3f,0.3f,0.3f,0,0},
                                                            new float[]{0.59f,0.59f,0.59f,0,0},
                                                            new float[]{0.11f,0.11f,0.11f,0,0},
                                                            new float[]{0,0,0,1,0,0},
                                                            new float[]{0,0,0,0,1,0},
                                                            new float[]{0,0,0,0,0,1}});

                Bitmap bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);

                Graphics graphics = Graphics.FromImage(bitmap);

                ImageAttributes imageAttributes = new ImageAttributes();

                imageAttributes.SetColorMatrix(cm);

                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);

                return bitmap;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }

        /// <summary>
        /// Difference filter for 2 images
        /// </summary>
        /// <param name="image1">The image1.</param>
        /// <param name="image2">The image2.</param>
        /// <returns>Bitmap.</returns>
        public Bitmap Difference(Bitmap image1, Bitmap image2)
        {
            try
            {
                int startThreshold = 15;
                int endThreshold = 255;

                BitmapData file1Data = image1.LockBits(new Rectangle(0, 0, image1.Width, image1.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                BitmapData file2Data = image2.LockBits(new Rectangle(0, 0, image2.Width, image2.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int strideForFile1 = file1Data.Stride;
                int strideForFile2 = file2Data.Stride;

                IntPtr scan0File1 = file1Data.Scan0;
                IntPtr scan0File2 = file2Data.Scan0;

                unsafe
                {
                    byte* p1 = (byte*)(void*)scan0File1;
                    byte* p2 = (byte*)(void*)scan0File2;

                    int nOffsetfile1 = strideForFile1 - image1.Width * 3;
                    int nOffsetfile2 = strideForFile2 - image2.Width * 3;

                    int nWidth = image1.Width * 3;

                    for (int y = 0; y < image1.Height; y++)
                    {
                        for (int x = 0; x < nWidth; x++)
                        {
                            var nVal = (int)p2[0] - (int)p1[0];

                            if ((nVal <= startThreshold) || (nVal >= endThreshold - 1))
                            {
                                p2[0] = (byte)0;
                            }

                            p1++;
                            p2++;
                        }

                        p1 = p1 + nOffsetfile1;
                        p2 = p2 + nOffsetfile2;
                    }
                }

                image1.UnlockBits(file1Data);
                image2.UnlockBits(file2Data);

                return image2;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }

        /// <summary>
        /// Dilate the and erode filter.
        /// </summary>
        /// <param name="sourceBitmap">The source bitmap.</param>
        /// <param name="matrixSize">Size of the matrix.</param>
        /// <param name="morphType">Type of the morph.</param>
        /// <param name="applyBlue">if set to <c>true</c> [apply blue].</param>
        /// <param name="applyGreen">if set to <c>true</c> [apply green].</param>
        /// <param name="applyRed">if set to <c>true</c> [apply red].</param>
        /// <param name="edgeType">Type of the edge.</param>
        /// <returns>Bitmap.</returns>
        public Bitmap DilateAndErodeFilter(Bitmap sourceBitmap, int matrixSize, MorphologyType morphType, bool applyBlue = true, bool applyGreen = true, bool applyRed = true, MorphologyEdgeType edgeType = MorphologyEdgeType.None)
        {
            try
            {
                BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

                byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

                Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

                sourceBitmap.UnlockBits(sourceData);

                int filterOffset = (matrixSize - 1) / 2;

                int calcOffset = 0;
                int byteOffset = 0;

                int blue = 0;
                int green = 0;
                int red = 0;

                byte morphResetValue = 0;

                if (morphType == MorphologyType.Erosion)
                {
                    morphResetValue = 255;
                }

                for (int offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
                {
                    for (int offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++)
                    {
                        byteOffset = offsetY * sourceData.Stride + offsetX * 4;

                        blue = morphResetValue;
                        green = morphResetValue;
                        red = morphResetValue;

                        if (morphType == MorphologyType.Dilation)
                        {
                            for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                            {
                                for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                                {
                                    calcOffset = byteOffset + (filterX * 4) + (filterY * sourceData.Stride);

                                    if (pixelBuffer[calcOffset] > blue)
                                    {
                                        blue = pixelBuffer[calcOffset];
                                    }

                                    if (pixelBuffer[calcOffset + 1] > green)
                                    {
                                        green = pixelBuffer[calcOffset + 1];
                                    }

                                    if (pixelBuffer[calcOffset + 2] > red)
                                    {
                                        red = pixelBuffer[calcOffset + 2];
                                    }
                                }
                            }
                        }
                        else if (morphType == MorphologyType.Erosion)
                        {
                            for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                            {
                                for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                                {
                                    calcOffset = byteOffset + (filterX * 4) + (filterY * sourceData.Stride);

                                    if (pixelBuffer[calcOffset] < blue)
                                    {
                                        blue = pixelBuffer[calcOffset];
                                    }

                                    if (pixelBuffer[calcOffset + 1] < green)
                                    {
                                        green = pixelBuffer[calcOffset + 1];
                                    }

                                    if (pixelBuffer[calcOffset + 2] < red)
                                    {
                                        red = pixelBuffer[calcOffset + 2];
                                    }
                                }
                            }
                        }

                        if (applyBlue == false)
                        {
                            blue = pixelBuffer[byteOffset];
                        }

                        if (applyGreen == false)
                        {
                            green = pixelBuffer[byteOffset + 1];
                        }

                        if (applyRed == false)
                        {
                            red = pixelBuffer[byteOffset + 2];
                        }

                        if (edgeType == MorphologyEdgeType.Edge || edgeType == MorphologyEdgeType.EdgeSharpen)
                        {
                            if (morphType == MorphologyType.Dilation)
                            {
                                blue = blue - pixelBuffer[byteOffset];
                                green = green - pixelBuffer[byteOffset + 1];
                                red = red - pixelBuffer[byteOffset + 2];
                            }
                            else if (morphType == MorphologyType.Erosion)
                            {
                                blue = pixelBuffer[byteOffset] - blue;
                                green = pixelBuffer[byteOffset + 1] - green;
                                red = pixelBuffer[byteOffset + 2] - red;
                            }


                            if (edgeType == MorphologyEdgeType.EdgeSharpen)
                            {
                                blue += pixelBuffer[byteOffset];
                                green += pixelBuffer[byteOffset + 1];
                                red += pixelBuffer[byteOffset + 2];
                            }
                        }


                        blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));
                        green = (green > 255 ? 255 : (green < 0 ? 0 : green));
                        red = (red > 255 ? 255 : (red < 0 ? 0 : red));

                        resultBuffer[byteOffset] = (byte)blue;
                        resultBuffer[byteOffset + 1] = (byte)green;
                        resultBuffer[byteOffset + 2] = (byte)red;
                        resultBuffer[byteOffset + 3] = 255;
                    }
                }

                Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

                BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);

                resultBitmap.UnlockBits(resultData);

                return resultBitmap;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }

        /// <summary>
        /// Balance filter based on RGB
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="red">The red.</param>
        /// <param name="blue">The blue.</param>
        /// <param name="green">The green.</param>
        /// <returns>Bitmap.</returns>
        public Bitmap Balance(Bitmap image, int red, int blue, int green)
        {
            try
            {
                BitmapData fileData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int stride = fileData.Stride;

                unsafe
                {
                    byte* ptr = (byte*)fileData.Scan0;
                    int bytesPerPixel = 3;

                    int remain = stride - fileData.Width * bytesPerPixel;

                    for (int y = 0; y < fileData.Height; y++)
                    {
                        for (int x = 0; x < fileData.Width; x++)
                        {
                            int b = (int)ptr[0] + blue;
                            int g = (int)ptr[1] + green;
                            int r = (int)ptr[2] + red;

                            if (b > 255)
                                b = 255;
                            if (b < 0)
                                b = 0;

                            if (g > 255)
                                g = 255;
                            if (g < 0)
                                g = 0;

                            if (r > 255)
                                r = 255;
                            if (r < 0)
                                r = 0;

                            ptr[0] = (byte)b;
                            ptr[1] = (byte)g;
                            ptr[2] = (byte)r;

                            ptr = ptr + bytesPerPixel;
                        }

                        ptr = ptr + remain;
                    }
                }

                image.UnlockBits(fileData);
                return image;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }

        /// <summary>
        /// Channel filter with RGB
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>Bitmap.</returns>
        public Bitmap Channel(Bitmap file, Rgb filter)
        {
            try
            {
                int red = 0;
                int blue = 0;
                int green = 0;

                switch (filter)
                {
                    case Rgb.Blue:
                        red = -255;
                        green = -255;
                        break;

                    case Rgb.Green:
                        blue = -255;
                        red = -255;
                        break;

                    case Rgb.Red:
                        blue = -255;
                        green = -255;
                        break;
                }

                return Balance(file, red, blue, green);
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }

        /// <summary>
        /// Channel filter based on 2 images with RGB
        /// </summary>
        /// <param name="image1">The image1.</param>
        /// <param name="image2">The image2.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>Bitmap.</returns>
        public Bitmap Channel(Bitmap image1, Bitmap image2, Rgb filter)
        {
            try
            {
                int r = 0;
                int b = 0;
                int g = 0;

                BitmapData fileData = image1.LockBits(new Rectangle(0, 0, image1.Width, image1.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int stride = fileData.Stride;

                BitmapData fileData1 = image2.LockBits(new Rectangle(0, 0, image2.Width, image2.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* ptr = (byte*)fileData.Scan0;
                    byte* ptr1 = (byte*)fileData1.Scan0;

                    int bytesPerPixel = 3;

                    int remain = stride - fileData.Width * bytesPerPixel;

                    for (int y = 0; y < fileData.Height; y++)
                    {
                        for (int x = 0; x < fileData.Width; x++)
                        {
                            switch (filter)
                            {
                                case Rgb.Blue:
                                    b = (int)ptr[1];
                                    if (b == 0)
                                    {
                                        b = (int)ptr1[0];
                                    }

                                    g = (int)ptr1[1];
                                    r = (int)ptr1[2];
                                    break;

                                case Rgb.Green:
                                    b = (int)ptr1[0];
                                    g = (int)ptr[0];
                                    if (g == 0)
                                    {
                                        g = (int)ptr1[1];
                                    }
                                    r = (int)ptr1[2];
                                    break;

                                case Rgb.Red:
                                    b = (int)ptr1[0];
                                    g = (int)ptr1[1];
                                    r = (int)ptr[0];
                                    if (r == 0)
                                    {
                                        r = (int)ptr1[2];
                                    }
                                    break;
                            }

                            ptr1[0] = (byte)b;
                            ptr1[1] = (byte)g;
                            ptr1[2] = (byte)r;

                            ptr = ptr + bytesPerPixel;
                            ptr1 = ptr1 + bytesPerPixel;
                        }

                        ptr = ptr + remain;
                        ptr1 = ptr1 + remain;
                    }
                }

                image1.UnlockBits(fileData);
                image2.UnlockBits(fileData1);

                return image2;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }

        /// <summary>
        /// Overlays image 1 on image 2
        /// </summary>
        /// <param name="image1">The image1.</param>
        /// <param name="image2">The image2.</param>
        /// <param name="alpha">The alpha.</param>
        /// <returns>Bitmap.</returns>
        public Bitmap Overlay(Bitmap image1, Bitmap image2, double alpha)
        {
            try
            {
                BitmapData bmpData = image1.LockBits(new Rectangle(0, 0, image1.Width, image1.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                BitmapData bmpData2 = image2.LockBits(new Rectangle(0, 0, image2.Width, image2.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                int width = bmpData.Width;
                int height = bmpData.Height;

                if (bmpData2.Width > width)
                {
                    width = bmpData2.Width;
                }

                if (bmpData2.Height > height)
                {
                    height = bmpData2.Height;
                }

                image1.UnlockBits(bmpData);
                image2.UnlockBits(bmpData2);

                Bitmap bit1 = new Bitmap(image1, width, height);
                Bitmap bit2 = new Bitmap(image2, width, height);

                Bitmap bmpResult = new Bitmap(width, height);

                BitmapData data1 = bit1.LockBits(new Rectangle(0, 0, bit1.Width, bit1.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                BitmapData data2 = bit2.LockBits(new Rectangle(0, 0, bit2.Width, bit2.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                BitmapData data3 = bmpResult.LockBits(new Rectangle(0, 0, bmpResult.Width, bmpResult.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                unsafe
                {
                    int remain1 = data1.Stride - data1.Width * 3;
                    int remain2 = data2.Stride - data2.Width * 3;
                    int remain3 = data3.Stride - data3.Width * 3;

                    byte* ptr1 = (byte*)data1.Scan0;
                    byte* ptr2 = (byte*)data2.Scan0;
                    byte* ptr3 = (byte*)data3.Scan0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width * 3; j++)
                        {
                            ptr3[0] = (byte)(alpha * ptr1[0] + (1 - alpha) * ptr2[0]);
                            ptr1++;
                            ptr2++;
                            ptr3++;
                        }

                        ptr1 += remain1;
                        ptr2 += remain2;
                        ptr3 += remain3;
                    }
                }

                bit1.UnlockBits(data1);
                bit2.UnlockBits(data2);
                bmpResult.UnlockBits(data3);

                return bmpResult;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }

        /// <summary>
        /// Sobel filter
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>Bitmap.</returns>
        public Bitmap Sobel(Bitmap image)
        {
            try
            {
                int[,] gx = { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
                int[,] gy = { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };

                Bitmap b = image;
                Bitmap b1 = new Bitmap(image);

                for (int i = 1; i < b.Height - 1; i++)
                {
                    for (int j = 1; j < b.Width - 1; j++)
                    {
                        float new_x = 0;
                        float new_y = 0;
                        float c;

                        for (int hw = -1; hw < 2; hw++)
                        {
                            for (int wi = -1; wi < 2; wi++)
                            {
                                c = (b.GetPixel(j + wi, i + hw).B + b.GetPixel(j + wi, i + hw).R + b.GetPixel(j + wi, i + hw).G) / 3;
                                new_x += gx[hw + 1, wi + 1] * c;
                                new_y += gy[hw + 1, wi + 1] * c;
                            }
                        }
                        if (new_x * new_x + new_y * new_y > 128 * 128)
                        {
                            b1.SetPixel(j, i, Color.Black);
                        }
                        else
                        {
                            b1.SetPixel(j, i, Color.White);
                        }
                    }
                }

                return b1;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }

        /// <summary>
        /// Blur filter
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="blur">The blur.</param>
        /// <returns>Bitmap.</returns>
        public Bitmap Blur(Bitmap image, Rectangle rectangle, int blur)
        {
            try
            {
                Bitmap bitmap = new Bitmap(image.Width, image.Height);

                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                }

                for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
                {
                    for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                    {
                        int avgR = 0;
                        int avgG = 0;
                        int avgB = 0;
                        int blurPixelCount = 0;

                        for (int x = xx; (x < xx + blur && x < image.Width); x++)
                        {
                            for (int y = yy; (y < yy + blur && y < image.Height); y++)
                            {
                                Color pixel = bitmap.GetPixel(x, y);

                                avgR += pixel.R;
                                avgG += pixel.G;
                                avgB += pixel.B;

                                blurPixelCount++;
                            }
                        }

                        avgR = avgR / blurPixelCount;
                        avgG = avgG / blurPixelCount;
                        avgB = avgB / blurPixelCount;

                        for (int x = xx; x < xx + blur && x < image.Width && x < rectangle.Width; x++)
                        {
                            for (int y = yy; y < yy + blur && y < image.Height && y < rectangle.Height; y++)
                            {
                                bitmap.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                            }
                        }
                    }
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                return new Bitmap(ImageFactory.Draw(ex.Message));
            }
        }
    }
}