public class Gender
{
    public enum GenderType
    {
        MALE,
        FEMALE,
        RATHERNOTSAY
    }

    public GenderType Value { get; private set; }

    public Gender(GenderType gender)
    {
        Value = gender;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

}
