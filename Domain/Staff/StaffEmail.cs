using BackOffice.Domain.Shared;
using System;

public class StaffEmail
{
    public string Value { get; private set; }

    public StaffEmail(string staffId, IConfiguration configuration)
    {
        // Get the domain from configuration settings
        string domain = configuration["EmailSettings:MyDns"];

        // Ensure the domain is configured properly
        if (string.IsNullOrWhiteSpace(domain))
            throw new BusinessRuleValidationException("Domain is not configured properly.");

        // Generate the email based on the StaffId and domain
        Value = $"{staffId}@{domain}".ToLower();
    }

    public override bool Equals(object obj)
    {
        if (obj is not StaffEmail other)
            return false;

        return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return Value;
    }
}
