using System.Diagnostics.CodeAnalysis;
using Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application;

[SuppressMessage("ReSharper", "ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator")]
public class Deleter
{
    private readonly DeleteOptions options;
    private readonly List<TargetDirectory> directories = [];
    private readonly ILogger logger;
    private readonly List<Failure> failures = [];

    public Deleter(IOptions<DeleteOptions> options, ILogger<Deleter> logger)
    {
        this.logger = logger;
        this.options = options.Value;
    }

    public List<Failure> StartDeleting()
    {
        foreach (var option in options.Directories)
            directories.Add(new TargetDirectory(option.Path, option.DeleteTimeSpan));

        foreach (var directory in directories)
        {
            DateTime deleteBefore = DateTime.UtcNow - directory.DeleteTimeSpan;
            
            var files = directory.Directory.GetFiles()
                .Where(file => file.CreationTimeUtc <= deleteBefore).ToArray();
            
            DeleteFiles(files);
        }

        return failures;
    }

    private void DeleteFiles(FileInfo[] files)
    {
        foreach (var file in files)
            if (file.Exists) DeleteFile(file);
        
        return;
        
        void DeleteFile(FileInfo file)
        {
            try
            {
                file.Delete();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Problem occured deleting file at {path}", file.FullName);
                
                failures.Add(new Failure
                {
                    Message = "Unable to delete file.",
                    Path = file.FullName,
                    Reason = ex.GetType().ToString()
                });
            }
        }
    }
}