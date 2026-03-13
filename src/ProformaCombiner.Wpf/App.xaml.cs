using System.Text;
using System.Windows;

namespace ProformaCombiner.Wpf;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // Required for ExcelDataReader
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        base.OnStartup(e);
    }
}
