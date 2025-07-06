using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.IO;

namespace TourPlanner.Utilities
{
    public static class PdfReportGenerator
    {
        public static void CreateSingleTourReport(Tour tour, string filePath)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = $"Tour Report: {tour.Name}";

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont titleFont = new XFont("Verdana", 16, XFontStyle.Bold);
            XFont font = new XFont("Verdana", 12, XFontStyle.Regular);
            double y = 40;

            gfx.DrawString($"Tour Report: {tour.Name}", titleFont, XBrushes.Black, new XPoint(40, y));
            y += 40;

            gfx.DrawString($"Description: {tour.Description}", font, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"From: {tour.From}", font, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"To: {tour.To}", font, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Transport: {tour.TransportType}", font, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Distance: {tour.Distance} km", font, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Estimated Time: {tour.EstimatedTime} h", font, XBrushes.Black, new XPoint(40, y)); y += 30;

            if (File.Exists(tour.Image))
            {
                try
                {
                    using var imageStream = File.OpenRead(tour.Image);
                    var skBitmap = SKBitmap.Decode(imageStream);

                    if (skBitmap != null)
                    {
                        using var ms = new MemoryStream();
                        skBitmap.Encode(ms, SKEncodedImageFormat.Png, 100);
                        ms.Seek(0, SeekOrigin.Begin);

                        var xImage = XImage.FromStream(() => ms);
                        gfx.DrawImage(xImage, 40, y, 200, 150);
                        y += 160;
                    }
                }
                catch
                {
                    gfx.DrawString("Bild konnte nicht geladen werden.", font, XBrushes.Red, new XPoint(40, y));
                    y += 20;
                }
            }

            gfx.DrawString("Tour Logs:", titleFont, XBrushes.Black, new XPoint(40, y));
            y += 30;

            foreach (var log in tour.TourLogs)
            {
                gfx.DrawString($"Date: {log.Date:d} | Time: {log.TotalTime} h | Distance: {log.TotalDistance} km | Rating: {log.Rating} | Difficulty: {log.Difficulty}",
                    font, XBrushes.Black, new XPoint(40, y));
                y += 20;
                gfx.DrawString($"Comment: {log.Comment}", font, XBrushes.Gray, new XPoint(60, y));
                y += 30;

                if (y > page.Height - 100)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = 40;
                }
            }

            document.Save(filePath);
            Process.Start("explorer.exe", $"/select,\"{filePath}\"");
        }

        public static void CreateSummaryReport(IEnumerable<Tour> tours, string filePath)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Tour Summary Report";

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont titleFont = new XFont("Verdana", 16, XFontStyle.Bold);
            XFont font = new XFont("Verdana", 12, XFontStyle.Regular);
            double y = 40;

            gfx.DrawString("Summary Report", titleFont, XBrushes.Black, new XPoint(40, y));
            y += 40;

            foreach (var tour in tours)
            {
                if (tour.TourLogs.Count == 0) continue;

                double avgTime = tour.TourLogs.Average(l => l.TotalTime);
                double avgDist = tour.TourLogs.Average(l => l.TotalDistance);
                double avgRating = tour.TourLogs.Average(l => l.Rating);

                gfx.DrawString($"Tour: {tour.Name}", font, XBrushes.Black, new XPoint(40, y)); y += 20;
                gfx.DrawString($"Avg Time: {avgTime:F2} h | Avg Distance: {avgDist:F2} km | Avg Rating: {avgRating:F1}/5", font, XBrushes.Black, new XPoint(60, y));
                y += 30;

                if (y > page.Height - 100)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = 40;
                }
            }

            document.Save(filePath);
            Process.Start("explorer.exe", $"/select,\"{filePath}\"");
        }

    }
}
