using System.Text.Json.Serialization;

public class Score
{
    [JsonPropertyName("score")]
    public double ScoreValue { get; set; }

    [JsonPropertyName("votes")]
    public int Votes { get; set; }
}

public class Npu
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("creativity")]
    public Score? Creativity { get; set; }

    [JsonPropertyName("uniqueness")]
    public Score? Uniqueness { get; set; }
}
