using BackOffice.Domain.Shared;
using System;

public class StaffEmail
{
    public string Value { get; private set; }

    public StaffEmail(string licenseNumber, string value, IConfiguration configuration)
    {
        string domain = configuration["EmailSettings:MyDns"];

        if (string.IsNullOrWhiteSpace(domain))
            throw new BusinessRuleValidationException("Domain is not configured properly.");

        if (string.IsNullOrWhiteSpace(value) || !value.Equals($"{licenseNumber}@{domain}", StringComparison.OrdinalIgnoreCase))
            throw new BusinessRuleValidationException($"Invalid email address. It must be in the format: {licenseNumber}@{domain}.");
        
        Value = value;
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
