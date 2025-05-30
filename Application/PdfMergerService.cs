using ContractPdfMerger.Domain;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using Serilog;

namespace ContractPdfMerger.Application;

public interface IPdfMergerService
{
    Task<byte[]> MergeAsync(byte[] mainPdfContent, List<MergeTargetItem> supplementalFiles);
    Task<System.Drawing.Image?> GenerateThumbnailAsync(byte[] pdfContent);
    Task<System.Drawing.Image?> GeneratePageThumbnailAsync(byte[] pdfContent, int pageIndex, int targetHeight);
    bool ValidatePdfFile(string filePath, out string errorMessage);
}

public class PdfMergerService : IPdfMergerService
{
    private const int MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB

    public async Task<byte[]> MergeAsync(byte[] mainPdfContent, List<MergeTargetItem> supplementalFiles)
    {
        try
        {
            using var outputDocument = new PdfDocument();

            // Add main PDF
            await Task.Run(() => AddPdfToDocument(outputDocument, mainPdfContent, "メイン契約書"));

            // Add supplemental PDFs
            foreach (var item in supplementalFiles)
            {
                await Task.Run(() => AddPdfToDocument(outputDocument, item.FileContent, item.FileName));
            }

            // Save to memory stream
            using var stream = new MemoryStream();
            outputDocument.Save(stream);
            var result = stream.ToArray();

            // Check final size
            if (result.Length > MaxFileSizeBytes)
            {
                throw new InvalidOperationException($"結合後のファイルサイズが上限({MaxFileSizeBytes / 1024 / 1024}MB)を超えています。");
            }

            Log.Information("PDF merge completed successfully. Output size: {Size} bytes", result.Length);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "PDF merge failed");
            throw;
        }
    }

    private void AddPdfToDocument(PdfDocument outputDocument, byte[] pdfContent, string fileName)
    {
        try
        {
            using var stream = new MemoryStream(pdfContent);
            using var inputDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import);

            for (int i = 0; i < inputDocument.PageCount; i++)
            {
                var page = inputDocument.Pages[i];
                outputDocument.AddPage(page);
            }

            Log.Debug("Added {PageCount} pages from {FileName}", inputDocument.PageCount, fileName);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to add PDF {FileName} to output document", fileName);
            throw new InvalidOperationException($"ファイル '{fileName}' の結合に失敗しました: {ex.Message}");
        }
    }

    public async Task<System.Drawing.Image?> GenerateThumbnailAsync(byte[] pdfContent)
    {
        try
        {
            // Validate PDF content first
            using var testStream = new MemoryStream(pdfContent);
            using var testDocument = PdfReader.Open(testStream, PdfDocumentOpenMode.Import);

            if (testDocument.PageCount == 0)
            {
                return null;
            }

            return await Task.Run(() =>
            {
                // Create professional-looking thumbnail
                return CreateProfessionalThumbnail(testDocument.PageCount, pdfContent.Length);
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to generate PDF thumbnail");
            return CreateErrorThumbnail();
        }
    }

    public async Task<System.Drawing.Image?> GeneratePageThumbnailAsync(byte[] pdfContent, int pageIndex, int targetHeight)
    {
        try
        {
            // Validate PDF content first
            using var testStream = new MemoryStream(pdfContent);
            using var testDocument = PdfReader.Open(testStream, PdfDocumentOpenMode.Import);

            if (testDocument.PageCount == 0 || pageIndex >= testDocument.PageCount || pageIndex < 0)
            {
                return null;
            }

            return await Task.Run(() =>
            {
                // Calculate width based on typical A4 aspect ratio (1:1.414)
                int targetWidth = (int)(targetHeight / 1.414);

                // Create professional-looking page thumbnail
                return CreatePageThumbnail(pageIndex + 1, testDocument.PageCount, pdfContent.Length, targetWidth, targetHeight);
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to generate page thumbnail for page {PageIndex}", pageIndex);
            return CreateErrorThumbnail();
        }
    }

    private System.Drawing.Image CreateProfessionalThumbnail(int pageCount, int fileSize)
    {
        var bitmap = new System.Drawing.Bitmap(200, 250);
        using (var g = System.Drawing.Graphics.FromImage(bitmap))
        {
            // Enable anti-aliasing for smooth graphics
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Background with subtle gradient
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Color.FromArgb(252, 252, 252),
                System.Drawing.Color.FromArgb(245, 245, 245),
                System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
            }

            // Main border with shadow effect
            using (var shadowBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(50, 0, 0, 0)))
            {
                g.FillRectangle(shadowBrush, 3, 3, bitmap.Width - 3, bitmap.Height - 3);
            }
            g.FillRectangle(System.Drawing.Brushes.White, 0, 0, bitmap.Width - 3, bitmap.Height - 3);
            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.FromArgb(180, 180, 180), 2), 0, 0, bitmap.Width - 3, bitmap.Height - 3);

            // PDF Header with gradient
            var headerRect = new System.Drawing.Rectangle(8, 8, bitmap.Width - 19, 35);
            using (var headerBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                headerRect,
                System.Drawing.Color.FromArgb(220, 53, 69),    // PDF red
                System.Drawing.Color.FromArgb(183, 28, 28),    // Darker red
                System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                g.FillRectangle(headerBrush, headerRect);
            }

            // PDF icon and title
            using (var titleFont = new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold))
            {
                g.DrawString("PDF", titleFont, System.Drawing.Brushes.White, 15, 18);
            }

            // Document icon
            var iconRect = new System.Drawing.Rectangle(headerRect.Right - 30, headerRect.Y + 8, 20, 20);
            g.FillRectangle(System.Drawing.Brushes.White, iconRect);
            g.DrawRectangle(System.Drawing.Pens.DarkRed, iconRect);
            using (var iconFont = new System.Drawing.Font("Arial", 6, System.Drawing.FontStyle.Bold))
            {
                g.DrawString("PDF", iconFont, System.Drawing.Brushes.Red, iconRect.X + 2, iconRect.Y + 6);
            }

            // Document information section
            var infoY = 55;
            using (var labelFont = new System.Drawing.Font("MS UI Gothic", 9, System.Drawing.FontStyle.Bold))
            using (var valueFont = new System.Drawing.Font("MS UI Gothic", 9))
            using (var grayBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(70, 70, 70)))
            using (var darkBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(50, 50, 50)))
            {
                // Page count
                g.DrawString("ページ数:", labelFont, grayBrush, 15, infoY);
                g.DrawString($"{pageCount} ページ", valueFont, darkBrush, 85, infoY);

                // File size
                infoY += 20;
                g.DrawString("ファイルサイズ:", labelFont, grayBrush, 15, infoY);
                g.DrawString(FormatFileSize(fileSize), valueFont, darkBrush, 100, infoY);
            }

            // Separator line
            infoY += 25;
            using (var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(200, 200, 200), 1))
            {
                g.DrawLine(pen, 15, infoY, bitmap.Width - 25, infoY);
            }

            // Document preview area
            var previewRect = new System.Drawing.Rectangle(15, infoY + 10, bitmap.Width - 35, bitmap.Height - infoY - 25);
            using (var previewBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(252, 252, 252)))
            {
                g.FillRectangle(previewBrush, previewRect);
            }
            g.DrawRectangle(System.Drawing.Pens.LightGray, previewRect);

            // Simulate document content with varying line lengths
            using (var contentPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(150, 150, 150), 1))
            {
                var random = new Random(42); // Fixed seed for consistent appearance
                for (int i = 0; i < 12; i++)
                {
                    int y = previewRect.Y + 8 + (i * 12);
                    if (y > previewRect.Bottom - 10) break;

                    // Vary line lengths to simulate real text
                    int lineLength = random.Next(30, previewRect.Width - 20);
                    if (i % 4 == 3) lineLength = random.Next(20, previewRect.Width / 2); // Shorter paragraph endings

                    g.DrawLine(contentPen, previewRect.X + 8, y, previewRect.X + lineLength, y);
                }
            }

            // Footer with PDF branding
            using (var footerFont = new System.Drawing.Font("Arial", 7, System.Drawing.FontStyle.Italic))
            {
                var footerText = "Adobe PDF Document";
                var footerSize = g.MeasureString(footerText, footerFont);
                g.DrawString(footerText, footerFont, System.Drawing.Brushes.Gray,
                    bitmap.Width - footerSize.Width - 10, bitmap.Height - 20);
            }
        }
        return bitmap;
    }

    private System.Drawing.Image CreatePageThumbnail(int currentPage, int totalPages, int fileSize, int width, int height)
    {
        var bitmap = new System.Drawing.Bitmap(width, height);
        using (var g = System.Drawing.Graphics.FromImage(bitmap))
        {
            // Enable anti-aliasing for smooth graphics
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Background with subtle gradient
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Color.FromArgb(252, 252, 252),
                System.Drawing.Color.FromArgb(245, 245, 245),
                System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
            }

            // Main border with shadow effect
            using (var shadowBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(30, 0, 0, 0)))
            {
                g.FillRectangle(shadowBrush, 3, 3, bitmap.Width - 3, bitmap.Height - 3);
            }
            g.FillRectangle(System.Drawing.Brushes.White, 0, 0, bitmap.Width - 3, bitmap.Height - 3);
            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.FromArgb(180, 180, 180), 2), 0, 0, bitmap.Width - 3, bitmap.Height - 3);

            // PDF Header with gradient
            var headerHeight = Math.Max(35, height / 15);
            var headerRect = new System.Drawing.Rectangle(8, 8, bitmap.Width - 19, headerHeight);
            using (var headerBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                headerRect,
                System.Drawing.Color.FromArgb(220, 53, 69),    // PDF red
                System.Drawing.Color.FromArgb(183, 28, 28),    // Darker red
                System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                g.FillRectangle(headerBrush, headerRect);
            }

            // PDF title and page info
            using (var titleFont = new System.Drawing.Font("Arial", Math.Max(8, height / 50), System.Drawing.FontStyle.Bold))
            {
                g.DrawString("PDF", titleFont, System.Drawing.Brushes.White, 15, headerRect.Y + 8);

                // Page number
                var pageText = $"ページ {currentPage}/{totalPages}";
                var pageTextSize = g.MeasureString(pageText, titleFont);
                g.DrawString(pageText, titleFont, System.Drawing.Brushes.White,
                    headerRect.Right - pageTextSize.Width - 10, headerRect.Y + 8);
            }

            // Document content area - full height simulation
            var contentY = headerRect.Bottom + 10;
            var contentHeight = bitmap.Height - contentY - 20;
            var contentRect = new System.Drawing.Rectangle(15, contentY, bitmap.Width - 35, contentHeight);

            using (var contentBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(254, 254, 254)))
            {
                g.FillRectangle(contentBrush, contentRect);
            }
            g.DrawRectangle(System.Drawing.Pens.LightGray, contentRect);

            // Simulate document content with varying line lengths (more realistic for full page)
            using (var contentPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(120, 120, 120), 1))
            {
                var random = new Random(42 + currentPage); // Page-specific seed for variation
                var lineHeight = Math.Max(8, contentHeight / 40); // Adaptive line height
                var linesCount = Math.Max(10, contentHeight / lineHeight - 2);

                for (int i = 0; i < linesCount; i++)
                {
                    int y = contentRect.Y + 8 + (i * lineHeight);
                    if (y > contentRect.Bottom - 10) break;

                    // Vary line lengths to simulate real text with paragraphs
                    int lineLength;
                    if (i % 8 == 7) // Paragraph break
                        lineLength = random.Next(contentRect.Width / 4, contentRect.Width / 2);
                    else if (i % 8 == 0) // New paragraph - indented
                        lineLength = random.Next(contentRect.Width - 40, contentRect.Width - 20);
                    else
                        lineLength = random.Next(contentRect.Width - 30, contentRect.Width - 10);

                    var startX = (i % 8 == 0) ? contentRect.X + 20 : contentRect.X + 8; // Indent first line
                    g.DrawLine(contentPen, startX, y, Math.Min(startX + lineLength, contentRect.Right - 8), y);
                }
            }

            // Add some "bold" headers occasionally
            using (var boldPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(80, 80, 80), 2))
            {
                var headerPositions = new[] { contentRect.Y + 30, contentRect.Y + contentRect.Height / 3, contentRect.Y + 2 * contentRect.Height / 3 };
                foreach (var headerY in headerPositions)
                {
                    if (headerY < contentRect.Bottom - 20)
                    {
                        g.DrawLine(boldPen, contentRect.X + 8, headerY, contentRect.X + contentRect.Width / 2, headerY);
                    }
                }
            }

            // Footer with file info
            using (var footerFont = new System.Drawing.Font("Arial", Math.Max(6, height / 80), System.Drawing.FontStyle.Italic))
            {
                var footerText = $"Adobe PDF Document - {FormatFileSize(fileSize)}";
                var footerSize = g.MeasureString(footerText, footerFont);
                g.DrawString(footerText, footerFont, System.Drawing.Brushes.Gray,
                    bitmap.Width - footerSize.Width - 10, bitmap.Height - 20);
            }
        }
        return bitmap;
    }

    private System.Drawing.Image CreateErrorThumbnail()
    {
        var bitmap = new System.Drawing.Bitmap(200, 250);
        using (var g = System.Drawing.Graphics.FromImage(bitmap))
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Error background
            g.Clear(System.Drawing.Color.FromArgb(245, 245, 245));
            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.FromArgb(220, 53, 69), 2), 0, 0, bitmap.Width - 1, bitmap.Height - 1);

            // Error icon area
            var iconRect = new System.Drawing.Rectangle(bitmap.Width / 2 - 25, 80, 50, 50);
            using (var iconBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(220, 53, 69)))
            {
                g.FillEllipse(iconBrush, iconRect);
            }

            // X mark
            using (var pen = new System.Drawing.Pen(System.Drawing.Color.White, 4))
            {
                g.DrawLine(pen, iconRect.X + 15, iconRect.Y + 15, iconRect.X + 35, iconRect.Y + 35);
                g.DrawLine(pen, iconRect.X + 35, iconRect.Y + 15, iconRect.X + 15, iconRect.Y + 35);
            }

            // Error message
            using (var font = new System.Drawing.Font("MS UI Gothic", 10, System.Drawing.FontStyle.Bold))
            using (var errorBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(220, 53, 69)))
            {
                var text = "プレビュー読込エラー";
                var textSize = g.MeasureString(text, font);
                g.DrawString(text, font, errorBrush,
                    (bitmap.Width - textSize.Width) / 2, 150);
            }
        }
        return bitmap;
    }

    private string FormatFileSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024} KB";
        return $"{bytes / (1024 * 1024)} MB";
    }

    public bool ValidatePdfFile(string filePath, out string errorMessage)
    {
        errorMessage = string.Empty;

        try
        {
            // Check file extension
            if (!Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "PDFファイルを選択してください。";
                return false;
            }

            // Check file size
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > MaxFileSizeBytes)
            {
                errorMessage = $"ファイルサイズが上限({MaxFileSizeBytes / 1024 / 1024}MB)を超えています。";
                return false;
            }

            // Try to open as PDF
            var content = File.ReadAllBytes(filePath);
            using var stream = new MemoryStream(content);
            using var document = PdfReader.Open(stream, PdfDocumentOpenMode.Import);

            return true;
        }
        catch (Exception ex)
        {
            errorMessage = $"PDFファイルが無効です: {ex.Message}";
            return false;
        }
    }
}