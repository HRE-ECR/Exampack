using ProformaCombiner.Wpf.Services;
using ProformaCombiner.Wpf.ViewModels;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;

namespace ProformaCombiner.Wpf.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm = new();
    private readonly AppConfig _cfg;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;
        _cfg = ConfigService.Load();

        var hints = new List<string>
        {
            File.Exists(_cfg.AT300ExcelPath) ? "AT300 Excel found" : "AT300 Excel missing",
            File.Exists(_cfg.AT200ExcelPath) ? "AT200 Excel found" : "AT200 Excel missing"
        };

        _vm.Status = "Status: " + string.Join(" | ", hints);
    }

    private void AT300_Click(object sender, RoutedEventArgs e)
    {
        if (!File.Exists(_cfg.AT300ExcelPath))
        {
            MessageBox.Show($"AT300 Excel not found:\n{_cfg.AT300ExcelPath}", "Missing file", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        LoadExcel(_cfg.AT300ExcelPath);
    }

    private void AT200_Click(object sender, RoutedEventArgs e)
    {
        if (!File.Exists(_cfg.AT200ExcelPath))
        {
            MessageBox.Show($"AT200 Excel not found:\n{_cfg.AT200ExcelPath}", "Missing file", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        LoadExcel(_cfg.AT200ExcelPath);
    }

    private void LoadExcel(string excelPath)
    {
        try
        {
            var records = ExcelService.LoadRecords(excelPath, _cfg.SheetName);
            _vm.SetItems(records);
            _vm.Status = $"Loaded {records.Count} item(s) from: {excelPath}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to read Excel:\n{ex.Message}", "Excel error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void Export_Click(object sender, RoutedEventArgs e)
    {
        var selected = _vm.GetSelectedRecords();
        if (selected.Count == 0)
        {
            MessageBox.Show("Please select one or more tiles before exporting.", "No selection", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        int totalPages = PdfExportService.CountTotalPages(selected);
        if (totalPages == 0)
        {
            MessageBox.Show("No valid pages found to export. Check your 'Page' values.", "Nothing to export", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var sfd = new SaveFileDialog
        {
            Title = "Save combined PDF",
            Filter = "PDF files (*.pdf)|*.pdf",
            FileName = $"Combined_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
        };

        if (sfd.ShowDialog(this) != true)
            return;

        var dlg = new ProgressDialog(totalPages) { Owner = this };
        dlg.Log("Starting export...");
        dlg.Show();

        int addedPages = 0;
        try
        {
            addedPages = await Task.Run(() =>
                PdfExportService.ExportCombinedPdf(
                    selected,
                    sfd.FileName,
                    msg => dlg.Dispatcher.Invoke(() => dlg.Log(msg)),
                    () => dlg.Dispatcher.Invoke(dlg.StepOne))
            );
        }
        catch (Exception ex)
        {
            dlg.Log("FATAL: " + ex.Message);
            MessageBox.Show($"Export failed:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            dlg.MarkComplete();
        }

        if (addedPages == 0)
        {
            MessageBox.Show("No valid pages found to export after processing.", "Nothing exported", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _vm.Status = $"Exported {addedPages} page(s) -> {sfd.FileName}";

        try { Process.Start(new ProcessStartInfo { FileName = sfd.FileName, UseShellExecute = true }); }
        catch { }
    }
}
