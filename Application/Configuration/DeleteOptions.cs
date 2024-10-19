using System.ComponentModel.DataAnnotations;

namespace Application.Configuration;

public class DeleteOptions
{
    public const string Key = "Setup";
    
    public required DirectoryToDelete[] Directories { get; init; }
}

public class DirectoryToDelete
{
    private readonly string? path;
    [Required]
    public required string Path
    {
        get => path ?? "";
        init => path = ReplaceTemplateWithUserName(value);
    }
    
    public TimeSpan DeleteTimeSpan => TimeSpan.FromMinutes(AgeToDelete);
    
    [Required]
    public double AgeToDelete { get; init; }

    private static string ReplaceTemplateWithUserName(string template) =>
        template.Replace("{username}", Environment.UserName);
}