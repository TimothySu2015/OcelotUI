using Microsoft.AspNetCore.Components;
using OcelotUI.UI.Services;

namespace OcelotUI.Web.Services;

/// <summary>
/// Web 實作：透過 NavigateTo 呼叫 /api/set-culture 端點，以 Cookie 儲存語系偏好。
/// </summary>
public class WebCultureSwitcher(NavigationManager navigation) : ICultureSwitcher
{
    public void SetCulture(string cultureName)
    {
        var uri = new Uri(navigation.Uri)
            .GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
        var encodedCulture = Uri.EscapeDataString(cultureName);
        var encodedRedirect = Uri.EscapeDataString("/" + uri.TrimStart('/'));

        navigation.NavigateTo(
            $"/api/set-culture?culture={encodedCulture}&redirectUri={encodedRedirect}",
            forceLoad: true);
    }
}
