namespace OcelotUI.Web.Services;

public enum PreviewSection
{
    Routes,
    GlobalConfiguration,
    Aggregates,
    DynamicRoutes,
}

/// <summary>
/// Scoped service that tracks whether the global JSON preview panel is open.
/// </summary>
public class JsonPreviewState
{
    public bool IsOpen { get; private set; }

    public bool IsLocked { get; private set; }

    public PreviewSection? FocusSection { get; private set; }

    public int? FocusIndex { get; private set; }

    public event Action? OnChange;

    public event Action? OnFocusChange;

    public void Toggle()
    {
        if (IsLocked) return;
        IsOpen = !IsOpen;
        OnChange?.Invoke();
    }

    public void Lock()
    {
        IsLocked = true;
        OnChange?.Invoke();
    }

    public void Unlock()
    {
        IsLocked = false;
        OnChange?.Invoke();
    }

    public void NotifyChanged() => OnChange?.Invoke();

    public void SetFocus(PreviewSection section, int? index)
    {
        FocusSection = section;
        FocusIndex = index;
        OnFocusChange?.Invoke();
    }

    public void ClearFocus()
    {
        FocusSection = null;
        FocusIndex = null;
        OnFocusChange?.Invoke();
    }
}
