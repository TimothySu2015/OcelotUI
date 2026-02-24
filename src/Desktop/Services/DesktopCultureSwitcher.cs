using OcelotUI.UI.Services;

namespace OcelotUI.Desktop.Services;

/// <summary>
/// Desktop 實作：將語系偏好寫入 %AppData%/OcelotUI/culture.txt，然後重啟應用程式。
/// </summary>
public class DesktopCultureSwitcher : ICultureSwitcher
{
    public void SetCulture(string cultureName)
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "OcelotUI");
        Directory.CreateDirectory(dir);

        File.WriteAllText(Path.Combine(dir, "culture.txt"), cultureName);

        System.Windows.Forms.Application.Restart();
    }
}
