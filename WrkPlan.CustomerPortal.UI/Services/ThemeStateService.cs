using Blazored.LocalStorage;
using WrkPlan.CustomerPortal.Shared.Dtos;

namespace WrkPlan.CustomerPortal.UI.Services;

public class ThemeStateService(ILocalStorageService localStorage)
{
    private const string ThemeKey = "wrkplan.theme";

    public ThemePreferenceDto Current { get; private set; } = new(false, 247, 249, 252);

    public event Action? Changed;

    public async Task InitializeAsync()
    {
        var value = await localStorage.GetItemAsync<ThemePreferenceDto>(ThemeKey);
        if (value is not null)
        {
            Current = value;
        }

        Changed?.Invoke();
    }

    public async Task SetAsync(ThemePreferenceDto request)
    {
        Current = request;
        await localStorage.SetItemAsync(ThemeKey, request);
        Changed?.Invoke();
    }

    public async Task ResetAsync()
    {
        Current = new ThemePreferenceDto(false, 247, 249, 252);
        await localStorage.RemoveItemAsync(ThemeKey);
        Changed?.Invoke();
    }
}
