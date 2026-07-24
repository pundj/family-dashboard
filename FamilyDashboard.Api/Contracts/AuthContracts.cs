namespace FamilyDashboard.Api.Contracts;

public record AuthRequest(string UserName, string Password);
public record AuthStatusResponse(bool IsAuthenticated, string? UserName);
public record AuthActionResponse(bool Succeeded, string? Message = null);
