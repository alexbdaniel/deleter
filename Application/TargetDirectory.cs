namespace Application;

internal class TargetDirectory
{
    internal DirectoryInfo Directory { get; private set; }
    
    internal TimeSpan DeleteTimeSpan { get; private set; }


    public TargetDirectory(string path, TimeSpan deleteTimeSpan)
    {
        this.Directory = new DirectoryInfo(path);
        this.DeleteTimeSpan = deleteTimeSpan;
    }
}