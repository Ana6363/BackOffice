namespace BackOffice.Domain.Specialization
{
    public class Description
    {

        public string Value { get; private set; }

        public Description(string description)
        {
            Value = description;
        }

    }
}
