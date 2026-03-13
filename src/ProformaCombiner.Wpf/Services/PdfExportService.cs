using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using ProformaCombiner.Wpf.Models;

namespace ProformaCombiner.Wpf.Services;

public static class PdfExportService
{
    public static int CountTotalPages(IEnumerable<ProformaRecord> records)
        => records.Sum(r => r.Pages?.Count ?? 0);

    public static int ExportCombinedPdf(IEnumerable<ProformaRecord> records, string outputPath, Action<string> log, Action step)
    {
        var outDoc = new PdfDocument();
        int added = 0;

        foreach (var rec in records)
        {
            if (string.IsNullOrWhiteSpace(rec.PdfPath) || !File.Exists(rec.PdfPath))
            {
                log($"SKIP: Missing PDF -> {rec.PdfPath}");
                continue;
            }

            log($"OPEN: {rec.PdfPath}");

            PdfDocument? src = null;
            try
            {
                src = PdfReader.Open(rec.PdfPath, PdfDocumentOpenMode.Import);
                if (src.PageCount <= 0)
                {
                    log($"SKIP: No pages in PDF -> {rec.PdfPath}");
                    continue;
                }

                foreach (var pageNumber in rec.Pages)
                {
                    int index = Math.Max(0, Math.Min(pageNumber - 1, src.PageCount - 1));
                    outDoc.AddPage(src.Pages[index]);
                    added++;
                    log($"ADD: {Path.GetFileName(rec.PdfPath)} page {pageNumber} (used {index + 1})");
                    step();
                }
            }
            catch (Exception ex)
            {
                log($"ERROR: {Path.GetFileName(rec.PdfPath)} -> {ex.Message}");
            }
            finally
            {
                try { src?.Close(); } catch { }
            }
        }

        if (added > 0)
        {
            log("SAVE: Writing output PDF...");
            outDoc.Save(outputPath);
            log("DONE: Saved output PDF.");
        }

        return added;
    }
}
