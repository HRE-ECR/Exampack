using System.Collections.ObjectModel;
using System.Windows;

namespace ProformaCombiner.Wpf.Views;

public partial class ProgressDialog : Window
{
    private readonly int _total;
    private int _current;
    private readonly ObservableCollection<string> _log = new();

    public ProgressDialog(int total)
    {
        InitializeComponent();
        _total = Math.Max(1, total);
        Bar.Minimum = 0;
        Bar.Maximum = _total;
        Bar.Value = 0;
        LogList.ItemsSource = _log;
    }

    public void Log(string message)
    {
        _log.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        if (_log.Count > 0)
            LogList.ScrollIntoView(_log[^1]);
    }

    public void StepOne()
    {
        _current++;
        if (_current > _total) _current = _total;
        Bar.Value = _current;
    }

    public void MarkComplete()
    {
        BtnClose.IsEnabled = true;
        if (_current < _total) Bar.Value = _total;
        Log("Export finished.");
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
