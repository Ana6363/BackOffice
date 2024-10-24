using BackOffice.Domain.Shared;
using System.Text.RegularExpressions;

namespace BackOffice.Domain.Patients
{
    public class Name
    {
        public string NameValue { get; set; }

        public Name(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleValidationException("Name cannot be empty.");

            if (!Regex.IsMatch(name, @"^[a-zA-Z\s]+$"))
                throw new BusinessRuleValidationException("Names must only contain alphabetic characters and spaces.");

            NameValue = name;
        }
    }
}
