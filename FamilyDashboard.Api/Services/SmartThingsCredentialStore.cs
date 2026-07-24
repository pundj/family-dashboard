using FamilyDashboard.Api.Data;
using FamilyDashboard.Api.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace FamilyDashboard.Api.Services;

public class SmartThingsCredentialStore : ISmartThingsCredentialStore
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IDataProtector _protector;

    public SmartThingsCredentialStore(ApplicationDbContext dbContext, IDataProtectionProvider dataProtectionProvider)
    {
        _dbContext = dbContext;
        _protector = dataProtectionProvider.CreateProtector("SmartThingsCredentialStore.v1");
    }

    public async Task<bool> HasTokenAsync(string userId)
    {
        return await _dbContext.SmartThingsCredentials.AnyAsync(x => x.UserId == userId);
    }

    public async Task<string?> GetTokenAsync(string userId)
    {
        var credential = await _dbContext.SmartThingsCredentials.SingleOrDefaultAsync(x => x.UserId == userId);
        if (credential is null)
            return null;

        return _protector.Unprotect(credential.ProtectedToken);
    }

    public async Task SaveTokenAsync(string userId, string token)
    {
        var credential = await _dbContext.SmartThingsCredentials.SingleOrDefaultAsync(x => x.UserId == userId);
        var protectedToken = _protector.Protect(token.Trim());

        if (credential is null)
        {
            credential = new SmartThingsCredential
            {
                UserId = userId,
                ProtectedToken = protectedToken,
                UpdatedUtc = DateTimeOffset.UtcNow
            };
            _dbContext.SmartThingsCredentials.Add(credential);
        }
        else
        {
            credential.ProtectedToken = protectedToken;
            credential.UpdatedUtc = DateTimeOffset.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveTokenAsync(string userId)
    {
        var credential = await _dbContext.SmartThingsCredentials.SingleOrDefaultAsync(x => x.UserId == userId);
        if (credential is null)
            return;

        _dbContext.SmartThingsCredentials.Remove(credential);
        await _dbContext.SaveChangesAsync();
    }
}
