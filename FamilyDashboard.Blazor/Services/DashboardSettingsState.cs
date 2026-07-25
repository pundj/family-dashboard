namespace FamilyDashboard.Blazor.Services;

public class DashboardSettingsState
{
    public event Action? OpenRequested;

    public void RequestOpen()
    {
        OpenRequested?.Invoke();
    }
}
