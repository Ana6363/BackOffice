using System.Text.Json.Serialization;

public class DeleteAllergyDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

}
