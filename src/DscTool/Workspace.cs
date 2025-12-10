namespace DscTool;

public record Workspace
{
    public Workspace() {}

    public DirectoryInfo? Root {get;init;}
    public DirectoryInfo? Artifact {get;init;}
    public DirectoryInfo? Res {get;init;}
    public DirectoryInfo? Doc {get;init;}
    public DirectoryInfo? Script {get;init;}
    public DirectoryInfo? Src {get;init;}
    public FileInfo? GlobalJson {get;init;}
}
