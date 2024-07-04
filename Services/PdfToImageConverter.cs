using System.Collections.Generic;
using System.IO;
using SkiaSharp;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

public class PdfToImageConverter
{
    private const int DPI = 300; // Set a higher DPI for better quality

    public List<SKBitmap> ConvertPdfToImages(Stream pdfStream)
    {
        var images = new List<SKBitmap>();

        using var document = PdfDocument.Open(pdfStream);
        foreach (var page in document.GetPages())
        {
            var width = (int)(page.Width * DPI / 72);
            var height = (int)(page.Height * DPI / 72);
            var bitmap = new SKBitmap(width, height);

            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);

            var paint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 12
            };

            // Render the PDF page text onto the canvas
            var text = page.Text;
            var lines = text.Split('\n');
            float y = paint.TextSize;
            foreach (var line in lines)
            {
                canvas.DrawText(line, 10, y, paint);
                y += paint.TextSize + 2;
            }

            images.Add(bitmap);
        }

        return images;
    }

    public byte[] BitmapToByteArray(SKBitmap bitmap)
    {
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
}
