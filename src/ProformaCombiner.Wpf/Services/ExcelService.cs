using System.Data;
using ExcelDataReader;
using ProformaCombiner.Wpf.Models;

namespace ProformaCombiner.Wpf.Services;

public static class ExcelService
{
    public static List<ProformaRecord> LoadRecords(string excelPath, string sheetName)
    {
        using var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var ds = reader.AsDataSet(new ExcelDataSetConfiguration
        {
            ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true },
            FilterSheet = (tableReader, _) => string.IsNullOrWhiteSpace(sheetName) || tableReader.Name.Equals(sheetName, StringComparison.OrdinalIgnoreCase)
        });

        DataTable? table = null;
        if (!string.IsNullOrWhiteSpace(sheetName))
        {
            foreach (DataTable t in ds.Tables)
                if (t.TableName.Equals(sheetName, StringComparison.OrdinalIgnoreCase)) { table = t; break; }
            if (table == null && ds.Tables.Count > 0) table = ds.Tables[0];
        }
        else table = ds.Tables.Count > 0 ? ds.Tables[0] : null;

        if (table == null || table.Columns.Count == 0)
            throw new Exception("No worksheet data found.");

        int colTitle = TryGetColumn(table, "Title");
        int colPath = TryGetColumn(table, "Path");
        int colPage = TryGetColumn(table, "Page");
        if (colTitle < 0 || colPath < 0 || colPage < 0)
            throw new Exception("Excel must contain headers: Title, Path, Page");

        var records = new List<ProformaRecord>();
        foreach (DataRow row in table.Rows)
        {
            var title = SafeString(row[colTitle]);
            var path = SafeString(row[colPath]);
            var pageText = SafeString(row[colPage]);

            var pagesText = string.IsNullOrWhiteSpace(pageText) ? "1" : pageText;
            var rec = new ProformaRecord
            {
                Title = title,
                PdfPath = path,
                PagesText = pagesText,
                Pages = PageParser.ParsePages(pagesText)
            };

            if (string.IsNullOrWhiteSpace(rec.Title) && string.IsNullOrWhiteSpace(rec.PdfPath))
                continue;

            records.Add(rec);
        }

        return records;
    }

    private static int TryGetColumn(DataTable t, string name)
    {
        for (int i = 0; i < t.Columns.Count; i++)
            if (t.Columns[i].ColumnName.Trim().Equals(name, StringComparison.OrdinalIgnoreCase))
                return i;
        return -1;
    }

    private static string SafeString(object o)
    {
        if (o is null || o == DBNull.Value) return string.Empty;
        return Convert.ToString(o)?.Trim() ?? string.Empty;
    }
}
