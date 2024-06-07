using System.Drawing;
using System.Drawing.Imaging;

namespace ProjektPaletaRGB
{
    public class PaletteSaving
    {
        public static void SavePaletteImage(string[] hexColors, string filePath)
        {
            int squareSize = 100;
            int numberOfSquares = hexColors.Length;
            int imageWidth = squareSize * numberOfSquares;
            int imageHeight = squareSize;

            using (Bitmap bitmap = new Bitmap(imageWidth, imageHeight))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.White);

                    for (int i = 0; i < hexColors.Length; i++)
                    {
                        Color color = ColorTranslator.FromHtml(hexColors[i]);

                        int x = i * squareSize;
                        int y = 0;
                        using (Brush brush = new SolidBrush(color))
                        {
                            // Draw the rectangle
                            graphics.FillRectangle(brush, x, y, squareSize, squareSize);
                        }
                    }
                }
                bitmap.Save(filePath, ImageFormat.Jpeg);
            }
        }
    }
}
