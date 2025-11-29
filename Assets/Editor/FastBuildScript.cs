using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

public class FastBuildScript
{
    // Windows build with one click
    [MenuItem("Fast Build/Build Windows")]
    public static void BuildWindows()
    {
        string buildPath = "Builds/Windows/" + Application.productName + ".exe";
        BuildGame(BuildTarget.StandaloneWindows64, buildPath, BuildOptions.None);
    }

    // Build and Play - Windows
    [MenuItem("Fast Build/Build and Play Windows")]
    public static void BuildAndPlayWindows()
    {
        string buildPath = "Builds/Windows/" + Application.productName + ".exe";
        BuildGame(BuildTarget.StandaloneWindows64, buildPath, 
            BuildOptions.AutoRunPlayer | BuildOptions.Development);
    }

    // Development build with debug symbols
    [MenuItem("Fast Build/Build Windows (Development)")]
    public static void BuildWindowsDev()
    {
        string buildPath = "Builds/WindowsDev/" + Application.productName + ".exe";
        BuildGame(BuildTarget.StandaloneWindows64, buildPath, 
            BuildOptions.Development | BuildOptions.AllowDebugging);
    }

    // Build and Play Development - Windows
    [MenuItem("Fast Build/Build and Play Windows (Development)")]
    public static void BuildAndPlayWindowsDev()
    {
        string buildPath = "Builds/WindowsDev/" + Application.productName + ".exe";
        BuildGame(BuildTarget.StandaloneWindows64, buildPath, 
            BuildOptions.AutoRunPlayer | BuildOptions.Development | BuildOptions.AllowDebugging);
    }

    // Linux build
    [MenuItem("Fast Build/Build Linux")]
    public static void BuildLinux()
    {
        string buildPath = "Builds/Linux/" + Application.productName + ".x86_64";
        BuildGame(BuildTarget.StandaloneLinux64, buildPath, BuildOptions.None);
    }

    // Build and Play - Linux
    [MenuItem("Fast Build/Build and Play Linux")]
    public static void BuildAndPlayLinux()
    {
        string buildPath = "Builds/Linux/" + Application.productName + ".x86_64";
        BuildGame(BuildTarget.StandaloneLinux64, buildPath, 
            BuildOptions.AutoRunPlayer | BuildOptions.Development);
    }

    // Core build function
    private static void BuildGame(BuildTarget target, string path, BuildOptions options)
    {
        // Get all scenes from build settings
        string[] scenes = GetScenePaths();
        
        // Create directory if it doesn't exist
        string directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Configure build
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = path,
            target = target,
            options = options
        };

        // Execute build
        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        BuildSummary summary = report.summary;

        // Log results
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded: {path} ({summary.totalSize / 1024 / 1024} MB)");
            
            // Only reveal in finder if not auto-running
            if ((options & BuildOptions.AutoRunPlayer) == 0)
            {
                EditorUtility.RevealInFinder(path);
            }
        }
        else
        {
            Debug.LogError($"Build failed: {summary.result}");
        }
    }

    // Get all enabled scenes from build settings
    private static string[] GetScenePaths()
    {
        var scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }
}
