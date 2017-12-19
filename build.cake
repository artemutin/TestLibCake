using System.Xml.Linq;
using System.IO;

// https://github.com/cake-build/cake/issues/1522
VSTestSettings FixToolPath(VSTestSettings settings)
{
    #tool vswhere
    settings.ToolPath =
        VSWhereLatest(new VSWhereLatestSettings { Requires = "Microsoft.VisualStudio.PackageGroup.TestTools.Core" })
        .CombineWithFilePath(File(@"Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"));
    return settings;
}

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

var solution = GetFiles("./*.sln")
    .First()
    .GetFilenameWithoutExtension();

Task("Default")
    .Does(() =>
{
    Information("Cake bootstrap completed...");
});

Task("Clean")
    .Does(() =>
{
    CleanDirectories("./build");
});

Task("NuGet-Restore")
    .Does(() =>
{
    NuGetRestore("./" + solution + ".sln");
});

Task("Build")
    .Does(() =>
{
    MSBuild("./" + solution + ".sln", settings =>
        settings.SetConfiguration(configuration)
            .WithTarget("Rebuild"));
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    VSTest($"./UnitTestProject/bin/{configuration}/UnitTestProject.dll", FixToolPath(new VSTestSettings()));
    Information("Test completed...");
});

Task("NuGet-Pack")
    .Does(() =>
{
    var projects = GetFiles("./**/*.nuspec")
        .Select(file => file.GetFilenameWithoutExtension())
        .SelectMany(file => GetFiles("./**/" + file + ".csproj"));

    foreach (var project in projects)
    {
        var doc = XDocument.Load(project.FullPath);
        var version = Argument("Build-Version", "1.0.0");
        
        if(doc.Root?.Attribute("Sdk")?.Value == null)           //.NET FRAMEWORK
        {    
            var settings = new NuGetPackSettings 
            {
                Version = version,
                Copyright = "Copyright " + DateTime.Now.Year,
                OutputDirectory = "./build",
                Properties = new Dictionary<string, string>
                {
                    ["Configuration"] = configuration
                }
            }; 

            NuGetPack(project, settings);
        } 
        else                                                    //Microsoft.NET.Sdk (.NET STANDARD, CORE)
        {
            var settings = new DotNetCorePackSettings
            {
                OutputDirectory = new DirectoryPath("./build"),
                MSBuildSettings = new DotNetCoreMSBuildSettings()
                            .WithProperty("Version", version)
                            .WithProperty("AssemblyVersion", version)
                            .WithProperty("FileVersion", version)
                            .WithProperty("Copyright", "Copyright " + DateTime.Now.Year)
                            .WithProperty("Configuration", configuration)
            };

            DotNetCorePack(project.FullPath, settings);
        }
    }
});


RunTarget(target);