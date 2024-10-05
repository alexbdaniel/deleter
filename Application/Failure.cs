namespace Application;

public class Failure
{
    public required string Message { get; init; }
    
    public required string Reason { get; init; }
    
    public required string Path { get; init; }
}