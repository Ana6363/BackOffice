public class PhoneNumber
{
    public int Number { get; private set; }

    public PhoneNumber(int number)
    {
        if (number <= 0 && number > 9)
        {
            throw new ArgumentException("Phone number must be a positive integer or below 9 digits.", nameof(number));
        }

        Number = number;
    }

    public override bool Equals(object obj)
    {
        if (obj is PhoneNumber other)
        {
            return Number == other.Number;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Number.GetHashCode();
    }

    public override string ToString()
    {
        return Number.ToString();
    }
}
