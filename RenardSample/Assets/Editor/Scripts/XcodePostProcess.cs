#if UNITY_IOS || UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

/*
 * Info.plistを設定するためのスクリプト 
 */

public class XcodePostProcess : IPreprocessBuildWithReport
{
    public int callbackOrder { get; }

    public void OnPreprocessBuild(BuildReport report)
    {
        PlayerSettings.iOS.appInBackgroundBehavior = iOSAppInBackgroundBehavior.Custom;
        PlayerSettings.iOS.backgroundModes = iOSBackgroundMode.BluetoothCentral;
    }

    [PostProcessBuild]
    private static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS) return;

        var projectPath = PBXProject.GetPBXProjectPath(path);
        var project = new PBXProject();
        project.ReadFromString(File.ReadAllText(projectPath));

        SetSwiftVersion(project);

        // 拡張ライブラリを加える際に使う
        //AddLibrary(project, "Plugins/iOS");

        project.WriteToFile(projectPath);

        // Plist設定を変更する際に使う
        AddInfoPlist(project, projectPath, path);
    }

    private static void SetSwiftVersion(PBXProject project)
    {
        var targetGuid = project.GetUnityFrameworkTargetGuid();
        project.SetBuildProperty(targetGuid, "SWIFT_VERSION", "5.0");
    }

    private static void AddLibrary(PBXProject project, string pluginsPath)
    {
        var mainTargetGuid = project.GetUnityMainTargetGuid();
        var dirPath = Path.Combine("Assets", pluginsPath);
        var dir = new DirectoryInfo(dirPath);
        var dirs = dir.GetDirectories();

        var frameworkPath = string.Empty;
        var fileGuid = string.Empty;

        foreach (DirectoryInfo subdir in dirs)
        {
            if (!subdir.FullName.EndsWith(".framework")) continue;

            frameworkPath = Path.Combine("Frameworks", pluginsPath, subdir.Name);
            fileGuid = project.FindFileGuidByProjectPath(frameworkPath);
            PBXProjectExtensions.AddFileToEmbedFrameworks(project, mainTargetGuid, fileGuid);
        }
    }

    private static void AddInfoPlist(PBXProject project, string projectPath, string path)
    {
        project.ReadFromString(File.ReadAllText(projectPath));
        project.WriteToFile(projectPath);

        var plistPath = Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();

        plist.ReadFromFile(plistPath);

        // ファイルアプリにドキュメントを表示してアクセスするための設定を追加する
        plist.root.SetBoolean("UIFileSharingEnabled", true);
        plist.root.SetBoolean("LSSupportsOpeningDocumentsInPlace", true);

        plist.WriteToFile(plistPath);
    }
}

#endif // UNITY_IOS || UNITY_EDITOR
