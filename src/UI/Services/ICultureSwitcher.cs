namespace OcelotUI.UI.Services;

/// <summary>
/// 語系切換抽象介面。Web 實作走 HTTP Cookie，Desktop 實作走本地檔案 + 重啟。
/// </summary>
public interface ICultureSwitcher
{
    void SetCulture(string cultureName);
}
