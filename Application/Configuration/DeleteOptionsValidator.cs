using System.ComponentModel.DataAnnotations;
using MiniValidation;

namespace Application.Configuration;

public class DeleteOptionsValidator
{
    private readonly List<string> badPaths = [];
    
    public bool Validate(DeleteOptions options)
    {
        UInt16 annotationErrors = 0;

        if (!ValidateAnnotations(options)) 
            annotationErrors++;

        if (annotationErrors != 0)
            throw new ValidationException("One or more validation errors occured.");
        
        ValidatePaths(options);

        if (badPaths.Count == 0) return true;
        
        Console.WriteLine("One or more directories do not exist:");
        foreach (string path in badPaths)
            Console.WriteLine($"\"{path}\"");

        return false;
    }

    private void ValidatePaths(DeleteOptions options)
    {
        foreach (var directory in options.Directories)
        {
            if(!Directory.Exists(directory.Path))
                badPaths.Add($"Directory does not exist at \"{directory.Path}\".");
            
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


