namespace OcelotUI.Web.Services;

/// <summary>
/// Scoped service that tracks whether the global JSON preview panel is open.
/// </summary>
public class JsonPreviewState
{
    public bool IsOpen { get; private set; }

    public event Action? OnChange;

    public void Toggle()
    {
        IsOpen = !IsOpen;
        OnChange?.Invoke();
    }

    public void NotifyChanged() => OnChange?.Invoke();
}
