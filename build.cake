// Install modules
#module nuget:?package=Cake.DotNetTool.Module&version=0.3.0

// Install .NET Core tools.
#tool dotnet:?package=GitVersion.Tool&version=5.0.0
#tool dotnet:?package=NugetRepack.Tool&version=1.0.1

public class BuildData
{
    public BuildData(
        string configuration,
        bool noRestore,
        ConvertableDirectoryPath artifactsPath,
        ConvertableDirectoryPath testResultsPath,
        FilePath solution,
        FilePath[] projects,
        GitVersion gitVersion,
        string pushSource,
        string pushApiKey)
    {
        this.Configuration = configuration;
        this.NoRestore = noRestore;
        this.ArtifactsPath = artifactsPath;
        this.TestResultsPath = testResultsPath;
        this.Solution = solution;
        this.Projects = projects;
        this.GitVersion = gitVersion;
        this.PushSource = pushSource;
        this.PushApiKey = pushApiKey;
    }

    public ConvertableDirectoryPath ArtifactsPath { get; }

    public string Configuration { get; }

    public bool NoRestore { get; }

    public string PushSource { get; }

    public string PushApiKey { get; }

    public FilePath[] Projects { get; }

    public FilePath Solution { get; }

    public ConvertableDirectoryPath TestResultsPath { get; }

    public string BuildVersion => this.IsMaster ? this.GitVersion.MajorMinorPatch : this.GitVersion.SemVer;

    public string InformationalVersion => this.IsMaster ? $"{this.GitVersion.MajorMinorPatch}+{this.GitVersion.FullBuildMetaData}" : this.GitVersion.InformationalVersion;

    public string NugetVersion => this.GitVersion.SemVer;

    public bool IsMaster => this.GitVersion.BranchName == "master";

    private GitVersion GitVersion { get; }
}

Setup<BuildData>(context =>
{
    var version = GetVersion();

    var noRestore = HasArgument("no-restore") ? true : false;
    var configuration = Argument<string>("configuration", version.BranchName == "master" ? "Release" : "Debug");
    var artifactsPath = Argument<string>("artifacts", "artifacts");
    var testResultsPath = Argument<string>("testresults", "testresults");
    var solution = GetSolution();
    var projects = GetProjects();
    var pushSource = EnvironmentVariable("PUSH_SOURCE");
    var pushApiKey = EnvironmentVariable("PUSH_APIKEY");

    var buildData = new BuildData(
        configuration,
        noRestore,
        Directory(artifactsPath),
        Directory(testResultsPath),
        solution,
        projects,
        version,
        pushSource,
        pushApiKey);

    Verbose("Configuration: {0}", buildData.Configuration);
    Verbose("NoRestore: {0}", buildData.NoRestore);
    Verbose("ArtifactsPath: {0}", buildData.ArtifactsPath);
    Verbose("TestResultsPath: {0}", buildData.TestResultsPath);
    Verbose("Solution: {0}", buildData.Solution.GetFilename());
    Verbose("Projects: {0}", string.Join<FilePath>(',', buildData.Projects.Select(project => project.GetFilename())));
    Verbose("BuildVersion: {0}", buildData.BuildVersion);
    Verbose("InformationalVersion: {0}", buildData.InformationalVersion);
    Verbose("NugetVersion: {0}", buildData.NugetVersion);

    return buildData;
});

Teardown(context =>
{
    Information("Starting teardown");
});

Task("Clean")
    .WithCriteria<BuildData>((context, buildData) => buildData.NoRestore == false)
    .Does<BuildData>(buildData =>
{
    CleanDirectories("./**/bin/" + buildData.Configuration);
    CleanDirectories("./**/obj");
    CleanDirectory(buildData.ArtifactsPath);
    CleanDirectory(buildData.TestResultsPath);
});

Task("Build")
    .IsDependentOn("Clean")
    .Does<BuildData>(buildData =>
{
    var path = MakeAbsolute(buildData.Solution);
    var settings = new DotNetCoreBuildSettings()
    {
        Configuration = buildData.Configuration,
        NoRestore = buildData.NoRestore,
        ArgumentCustomization = args=>args
          .Append($"/property:Version=\"{buildData.BuildVersion}\"")
          .Append($"/property:InformationalVersion=\"{buildData.InformationalVersion}\"")
    };
    DotNetCoreBuild(path.FullPath, settings);
});

Task("Test")
    .IsDependentOn("Build")
    .Does<BuildData>(buildData =>
{
    var path = MakeAbsolute(buildData.Solution);
    var settings = new DotNetCoreTestSettings
    {
        Configuration = buildData.Configuration,
        NoBuild = true,
        NoRestore = true,
        Logger = "trx",
        ResultsDirectory = buildData.TestResultsPath,
    };
    DotNetCoreTest(path.FullPath, settings);
});

Task("Pack")
    .IsDependentOn("Test")
    .Does<BuildData>(buildData =>
{
    var settings = new DotNetCorePackSettings
    {
        Configuration = buildData.Configuration,
        NoBuild = true,
        NoRestore = true,
        OutputDirectory = buildData.ArtifactsPath,
        ArgumentCustomization = args=>args.Append($"/property:Version=\"{buildData.NugetVersion}\""),
    };
    foreach (var project in buildData.Projects)
    {
        DotNetCorePack(project.FullPath, settings);
    }
});

Task("Push")
    .IsDependentOn("Pack")
    .Does<BuildData>(buildData =>
{
    PushPackages(buildData);
});

Task("Repack")
    .Does<BuildData>(buildData =>
{
    var toolPath = Context.Tools.Resolve(IsRunningOnUnix() ? "nugetrepack" : "nugetrepack.exe");
    var settings = new ProcessSettings
    {
        RedirectStandardOutput = true,
    };
    var packages = GetFiles($"{buildData.ArtifactsPath}/*.nupkg");

    foreach (var package in packages)
    {
        settings.Arguments = $"repack {package.FullPath}";

        using (var process = Context.StartAndReturnProcess(toolPath, settings))
        {
            process.WaitForExit();
            var processResult = process.GetStandardOutput().Aggregate((result, next) => result + Environment.NewLine + next);
            Verbose(processResult);
        }
    }
});

Task("Repush")
    .IsDependentOn("Repack")
    .Does<BuildData>(buildData =>
{
    PushPackages(buildData);
});

FilePath GetSolution()
{
    FilePath solution;

    if (HasArgument("solution"))
    {
        solution = File(Argument<string>("solution"));
    }
    else
    {
        var solutions = GetFiles("./*.sln");

        if (solutions.Count == 0)
        {
            throw new Exception("No solution parameter was passed and no solutions were found");
        }
        else if (solutions.Count > 1)
        {
            throw new Exception("No solution parameter was passed and more than one solution was found");
        }

        solution = solutions.First();
    }

    return solution;
}

FilePath[] GetProjects()
{
    List<FilePath> projects = new List<FilePath>();

    if (HasArgument("projects"))
    {
        var projectsArgument = Argument<string>("projects").Split(new char[] { ',', ' ' });

        foreach (var project in projectsArgument)
        {
            var projectName = project.EndsWith(".csproj") ? project : $"{project}.csproj";
            var projectFile = GetFiles($"./**/{projectName}").FirstOrDefault();

            if (projectFile == null)
            {
                throw new Exception($"Could not find project {projectName}");
            }

            projects.Add(projectFile);
        }
    }
    else
    {
        var solution = GetSolution();
        var projectFile = GetFiles($"./**/{solution.GetFilenameWithoutExtension()}.csproj").FirstOrDefault();

        if (projectFile == null)
        {
            throw new Exception("No projects parameter was passed and no project with the same name as the solution was found");
        }

        projects.Add(projectFile);
    }

    return projects.ToArray();
}

GitVersion GetVersion()
{
    var version = GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.Json });

    return version;
}

void PushPackages(BuildData buildData)
{
    if (buildData.PushSource == null || buildData.PushApiKey == null)
    {
        Error("Missing required environment variable 'PUSH_SOURCE' and/or 'PUSH_APIKEY'!");
    }

    var settings = new DotNetCoreNuGetPushSettings
    {
        Source = buildData.PushSource,
        ApiKey = buildData.PushApiKey,
    };

    var packages = GetFiles($"{buildData.ArtifactsPath}/*.nupkg");

    foreach (var package in packages)
    {
        Verbose($"Pushing package: {package}");
        DotNetCoreNuGetPush(package.FullPath, settings);
    }
}

RunTarget(Argument("target", "Pack"));
