using System.Globalization;

namespace OcelotUI.Desktop;

static class Program
{
    [STAThread]
    static void Main()
    {
        // 從 %AppData%/OcelotUI/culture.txt 載入語系設定
        var culturePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "OcelotUI", "culture.txt");

        if (File.Exists(culturePath))
        {
            var cultureName = File.ReadAllText(culturePath).Trim();
            if (!string.IsNullOrEmpty(cultureName))
            {
                var culture = new CultureInfo(cultureName);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }
        }

        System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware);
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
        System.Windows.Forms.Application.Run(new MainForm());
    }
}
