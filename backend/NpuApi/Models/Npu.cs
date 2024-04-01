public class ScoreDto
{
    public double Score { get; set; }
    public int Votes { get; set; }
}

public class NpuDto
{
    public string? Id { get; set; }
    public ScoreDto? Creativity { get; set; }
    public ScoreDto? Uniqueness { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
}