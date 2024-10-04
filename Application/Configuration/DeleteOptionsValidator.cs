using System.ComponentModel.DataAnnotations;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using MiniValidation;

namespace Application.Configuration;

[SupportedOSPlatform("Windows")]
public class DeleteOptionsValidator
{
    private readonly List<string> badPaths = [];
    
    public bool Validate(DeleteOptions[] options)
    {
        UInt16 annotationErrors = 0;
        foreach (var option in options)
            if (!ValidateAnnotations(option)) annotationErrors++;

        if (annotationErrors != 0)
            throw new ValidationException("One or more validation errors occured.");
        
        ValidatePaths(options);

        if (badPaths.Count == 0) return true;
        
        Console.WriteLine("One or more directories do not exist:");
        foreach (string path in badPaths)
            Console.WriteLine($"\"{path}\"");

        return false;
    }

    private void ValidatePaths(DeleteOptions[] options)
    {
        foreach (var option in options)
        {
            bool exists = Directory.Exists(option.Path);
            if (exists)
            {
                bool write = option.Path.HasWriteAccess();
                badPaths.Add($"No write access to \"{option.Path}\"");
            }
            else
            {
                badPaths.Add($"Directory does not exist at \"{option.Path}\"");
            }
        }
    }

    private static bool ValidateAnnotations(DeleteOptions options)
    {
        bool valid = MiniValidator.TryValidate(options, out var errors);
        
        foreach (var entry in errors)
        {
             Console.WriteLine($"  {entry.Key}:");
             foreach (var error in entry.Value)
                 Console.WriteLine($"  - {error}");
        }

        return valid;
    }
    
}

[SupportedOSPlatform("Windows")]
internal static class Utilities
{
    internal static bool HasWriteAccess(this DirectoryInfo directory)
    {
        try
        {
            _ = directory.GetAccessControl();
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }
    
    internal static bool HasWriteAccess(this string directoryName)
    {
        try
        {
            var directory = new DirectoryInfo(directoryName);
            _ = directory.GetAccessControl();
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }
}
