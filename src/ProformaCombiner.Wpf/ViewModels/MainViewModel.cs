using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProformaCombiner.Wpf.Models;

namespace ProformaCombiner.Wpf.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public ObservableCollection<SelectableProforma> Items { get; } = new();

    private string _status = "Ready.";
    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    private string _modeLabel = "AT300";
    public string ModeLabel
    {
        get => _modeLabel;
        set { _modeLabel = value; OnPropertyChanged(); }
    }

    public void SetItems(IEnumerable<ProformaRecord> records)
    {
        Items.Clear();
        foreach (var r in records)
            Items.Add(new SelectableProforma(r));
    }

    public List<ProformaRecord> GetSelectedRecords()
        => Items.Where(i => i.IsSelected).Select(i => i.Record).ToList();

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
