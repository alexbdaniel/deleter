using System.ComponentModel.DataAnnotations;

namespace Application.Configuration;

public class DeleteOptions
{
    public const string Key = "Directories";
    
    [Required]
    public required string Path { get; init; }
    
    [Required]
    public required TimeSpan DeleteTimeSpan { get; init; }
}