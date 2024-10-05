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
            bool exists = Directory.Exists(directory.Path);
            if (exists)
            {
                if(!directory.Path.HasWritePermission())
                    badPaths.Add($"No write access to \"{directory.Path}\"");
            }
            else
            {
                badPaths.Add($"Directory does not exist at \"{directory.Path}\"");
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

internal static class Utilities
{
    internal static bool HasWritePermission(this string directoryName)
    {
        try
        {
            var directory = new DirectoryInfo(directoryName);
            string fullPath = GetUniqueFileName(directory);
            File.Create(fullPath).Dispose();
            File.Delete(fullPath);
            return true;
        }
        catch (UnauthorizedAccessException ex)
        {
            return false;
        }
    }

    private static string GetUniqueFileName(DirectoryInfo directory)
    {
        string fullPath;
        
        UInt16 attempts = 0;
        bool exists;
        
        do
        {
            string name = Path.GetRandomFileName();
            fullPath = Path.Combine(directory.FullName, name);
            exists = File.Exists(fullPath);

        } while (!exists && ++attempts <= 10);

        if (!exists) return fullPath;

        throw new Exception($"Could not create unique file name for this directory " +
                            $"\"{directory.FullName}\"");
        
    }
}


// [SupportedOSPlatform("Windows")]
// internal static class Utilities
// {
//     
//     
//     
//     internal static bool HasWriteAccess(this DirectoryInfo directory)
//     {
//         try
//         {
//             _ = directory.GetAccessControl();
//             return true;
//         }
//         catch (UnauthorizedAccessException)
//         {
//             return false;
//         }
//     }
//     
//     internal static bool HasWriteAccess(this string directoryName)
//     {
//         try
//         {
//             var directory = new DirectoryInfo(directoryName);
//             _ = directory.GetAccessControl();
//             return true;
//         }
//         catch (UnauthorizedAccessException)
//         {
//             return false;
//         }
//     }
// }
