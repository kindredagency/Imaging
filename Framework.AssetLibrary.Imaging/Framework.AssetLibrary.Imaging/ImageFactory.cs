using System;
using System.Drawing;
using System.IO;

namespace Framework.AssetLibrary.Imaging
{
    public class ImageFactory
    {
        /// <summary>
        /// Reads the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Image.</returns>
        public static Image Read(string file)
        {
            try
            {
                Image image;

                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    image = Image.FromStream(fs);
                }

                return new Bitmap(image);
            }
            catch (Exception)
            {
                return Draw("Invalid image: Image is either not in a valid format or is too big to be opened.");
            }

        }

        /// <summary>
        /// Draws the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Image.</returns>
        public static Image Draw(string text)
        {
            return Draw(text, SystemFonts.DefaultFont, Color.Black, Color.White);
        }

        /// <summary>
        /// Draws the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="textColor">Color of the text.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <returns>Image.</returns>
        public static Image Draw(string text, Font font, Color textColor, Color backgroundColor)
        {
            SizeF fontSize = Graphics.FromImage(new Bitmap(1, 1)).MeasureString(text, font);

            Image image = new Bitmap((int)fontSize.Width, (int)fontSize.Height);

            Graphics graphics = Graphics.FromImage(image);
          
            graphics.Clear(backgroundColor);

            Brush textBrush = new SolidBrush(textColor);

            graphics.DrawString(text, font, textBrush, 0, 0);

            graphics.Save();

            textBrush.Dispose();

            graphics.Dispose();

            return image;
        }
    }
}
