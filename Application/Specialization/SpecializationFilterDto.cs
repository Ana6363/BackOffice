namespace BackOffice.Application.Specialization
{
    public class SpecializationFilterDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

        public SpecializationFilterDto(string? name = null, string? description = null)
        {
            Name = name;
            Description = description;
        }

        public SpecializationFilterDto() { }
    }
}
