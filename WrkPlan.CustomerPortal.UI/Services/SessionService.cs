using Blazored.LocalStorage;
using WrkPlan.CustomerPortal.Shared.Dtos;

namespace WrkPlan.CustomerPortal.UI.Services;

public record SessionSnapshot(AuthResponseDto Auth, string TenantKey);

public class SessionService(ILocalStorageService localStorage)
{
    private const string SessionKey = "wrkplan.session";

    public AuthResponseDto? Auth { get; private set; }
    public string? TenantKey { get; private set; }
    public bool IsAuthenticated => Auth is not null;

    public event Action? Changed;

    public async Task SetSessionAsync(AuthResponseDto auth, string tenantKey)
    {
        Auth = auth;
        TenantKey = tenantKey;
        await localStorage.SetItemAsync(SessionKey, new SessionSnapshot(auth, tenantKey));
        Changed?.Invoke();
    }

    // Sync fallback — in-memory only
    public void SetSession(AuthResponseDto auth, string tenantKey)
    {
        Auth = auth;
        TenantKey = tenantKey;
        Changed?.Invoke();
    }

    public async Task TryRestoreAsync()
    {
        if (IsAuthenticated) return;
        try
        {
            var snap = await localStorage.GetItemAsync<SessionSnapshot>(SessionKey);
            if (snap?.Auth is not null)
            {
                Auth = snap.Auth;
                TenantKey = snap.TenantKey;
                Changed?.Invoke();
            }
        }
        catch { /* localStorage unavailable during prerender */ }
    }

    public async Task ClearAsync()
    {
        Auth = null;
        TenantKey = null;
        try { await localStorage.RemoveItemAsync(SessionKey); } catch { }
        Changed?.Invoke();
    }

    public void Clear()
    {
        Auth = null;
        TenantKey = null;
        Changed?.Invoke();
    }
}
