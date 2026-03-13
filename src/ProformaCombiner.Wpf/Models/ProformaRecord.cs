namespace ProformaCombiner.Wpf.Models;

public class ProformaRecord
{
    public string Title { get; set; } = string.Empty;
    public string PdfPath { get; set; } = string.Empty;
    public string PagesText { get; set; } = "1";
    public List<int> Pages { get; set; } = new();
}
