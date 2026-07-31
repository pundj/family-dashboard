namespace FamilyDashboard.Blazor.Services;

public class DashboardSettingsState
{
    public event Action? OpenRequested;
    public event Func<Task>? ScreensaverShown;

    public void RequestOpen()
    {
        OpenRequested?.Invoke();
    }

    public Task NotifyScreensaverShownAsync()
    {
        return ScreensaverShown is null
            ? Task.CompletedTask
            : Task.WhenAll(ScreensaverShown.GetInvocationList()
                .Cast<Func<Task>>()
                .Select(handler => handler()));
    }
}
