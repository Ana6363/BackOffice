namespace BackOffice.Application.Specialization
{
    public class SpecializationDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public SpecializationDto(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
