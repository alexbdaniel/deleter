using System.ComponentModel.DataAnnotations;

namespace Application.Configuration;

public class DeleteOptions
{
    public const string Key = "Setup";
    
    public required DirectoryToDelete[] Directories { get; init; }
}

public class DirectoryToDelete
{
    [Required]
    public required string Path { get; init; }
    
    [Required]
    public required TimeSpan DeleteTimeSpan { get; init; }
    
}