using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProformaCombiner.Wpf.Models;

public class SelectableProforma : INotifyPropertyChanged
{
    public ProformaRecord Record { get; }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; OnPropertyChanged(); }
    }

    public SelectableProforma(ProformaRecord record) => Record = record;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
