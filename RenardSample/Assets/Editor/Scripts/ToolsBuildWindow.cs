using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace SignageHADO
{
    public class ToolsBuildWindow : EditorWindow
    {
        private static ToolsBuildWindow window = null;

        private const string outputFolder = "Tools";
        private const string createLicenseApp = "CreateLicenseApp";
        private const string downloaderApp = "DownloaderApp";

        private static string defaultPath => Path.GetFullPath($"{Application.dataPath}/../..");

        private bool isOpenWindow = false;
        private bool buildCreateLicenseApp = false;
        private bool buildDownloaderApp = false;
        private bool nowBuild = false;
        private string outputPath = string.Empty;
        private string outputTargetName = string.Empty;
        private string outputTargetScene = string.Empty;
        private string selectPath = string.Empty;
        private BuildTarget buildTarget = default;
        private string launcherScenePath = string.Empty;
        private string targetScenePath = string.Empty;
        private BuildReport buildResult = null;

        [MenuItem("RenardSample/Tools Build")]
        public static void ShowWindow()
        {
            if (window != null && window.isOpenWindow)
                return;

            window = GetWindow<ToolsBuildWindow>();
            window.titleContent = new GUIContent("ビルドツール");
            window.maxSize = new Vector2(400, 350);
            window.minSize = new Vector2(400, 350);
            window.isOpenWindow = true;

            window.selectPath = defaultPath;

            if (Application.platform == RuntimePlatform.WindowsEditor)
                window.buildTarget = BuildTarget.StandaloneWindows64;

            if (Application.platform == RuntimePlatform.OSXEditor)
                window.buildTarget = BuildTarget.StandaloneOSX;

            GetTargetScenePath("LauncherTools.unity", out window.launcherScenePath);

            window.Show();
        }

        public static void CloseWindow()
        {
            if (window != null)
            {
                window.isOpenWindow = false;
                window.Close();
            }

            window = null;
        }

        private void OnLostFocus()
        {
            // フォーカスを失ったら閉じる ※ビルド時は除く
            if (!nowBuild)
                CloseWindow();
        }

        private void OnGUI()
        {
            try
            {
                DrawUI();
            }
            catch
            {
                // 何もしない
            }
        }

        private void DrawUI()
        {
            GUILayout.Label($"ビルドツール [{buildTarget}]");

            EditorGUILayout.Space();

            // ビルド出力先
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("ビルド出力先", GUILayout.ExpandWidth(true));

                if (GUILayout.Button("選択", GUILayout.Height(20), GUILayout.Width(100)))
                {
                    selectPath = EditorUtility.OpenFolderPanel("ビルド出力先", selectPath, outputFolder);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel = 1;

                var path = EditorGUILayout.TextField(selectPath, GUILayout.Height(40), GUILayout.ExpandWidth(true));
                if (selectPath != path)
                {
                    if (string.IsNullOrEmpty(path))
                        selectPath = path;

                    UpdateBuildSettings();
                }
            }

            EditorGUILayout.Space();
            EditorGUI.indentLevel = 0;

            // ビルド種別
            {
                EditorGUILayout.LabelField("ビルド種別", GUILayout.ExpandWidth(true));

                EditorGUI.indentLevel = 1;

                EditorGUILayout.BeginHorizontal();

                var toggleCreateLicenseApp = EditorGUILayout.Toggle("ライセンス発行ツール", buildCreateLicenseApp);
                var toggleDownloaderApp = EditorGUILayout.Toggle("ダウンロードツール", buildDownloaderApp);

                EditorGUILayout.EndHorizontal();

                if ((toggleCreateLicenseApp != buildCreateLicenseApp) ||
                    (toggleDownloaderApp != buildDownloaderApp))
                {
                    if (toggleCreateLicenseApp != buildCreateLicenseApp)
                    {
                        if (toggleCreateLicenseApp)
                            toggleDownloaderApp = false;
                    }
                    else if (toggleDownloaderApp != buildDownloaderApp)
                    {
                        if (toggleDownloaderApp)
                            toggleCreateLicenseApp = false;
                    }

                    buildCreateLicenseApp = toggleCreateLicenseApp;
                    buildDownloaderApp = toggleDownloaderApp;
                    UpdateBuildSettings();
                }
            }

            EditorGUILayout.Space();
            EditorGUI.indentLevel = 0;

            // 設定情報
            {
                EditorGUILayout.LabelField("設定情報", GUILayout.ExpandWidth(true));

                EditorGUI.indentLevel = 1;

                var style = new GUIStyle(GUI.skin.label);
                style.wordWrap = true;

                if (buildCreateLicenseApp || buildDownloaderApp)
                {
                    if (!string.IsNullOrEmpty(targetScenePath))
                    {
                        EditorGUILayout.LabelField($"ビルド出力先: {outputPath}", style, GUILayout.ExpandWidth(true));
                    }
                    else
                    {
                        EditorGUILayout.LabelField($"シーンリストに対象が存在しません。 シーン名: {outputTargetScene}", style, GUILayout.ExpandWidth(true));
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("ビルドするものを選んでください。", style, GUILayout.ExpandWidth(true));
                }
            }

            EditorGUILayout.Space();
            EditorGUI.indentLevel = 0;

            // 機能ボタン
            {
                if (!nowBuild)
                {
                    if (!string.IsNullOrEmpty(outputPath))
                    {
                        if (GUILayout.Button("ビルド", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                        {
                            Build(outputPath, outputTargetName, targetScenePath);
                        }
                    }

                    EditorGUILayout.Space();

                    if (GUILayout.Button("閉じる", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                    {
                        CloseWindow();
                    }
                }
            }

            if (buildResult != null)
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel = 0;

                var style = new GUIStyle(GUI.skin.label);
                style.wordWrap = true;

                EditorGUILayout.LabelField($"{(buildResult.summary.result == BuildResult.Succeeded ? "作成完了" : "作成失敗")}", style, GUILayout.ExpandWidth(true));
            }
        }

        private void UpdateBuildSettings()
        {
            outputTargetName = buildCreateLicenseApp ? createLicenseApp : buildDownloaderApp ? downloaderApp : string.Empty;
            outputTargetScene = buildCreateLicenseApp ? LauncherTools.TargetSceneNameCreateLicense : buildDownloaderApp ? LauncherTools.TargetSceneNameDownloader : string.Empty;

            if (GetTargetScenePath(outputTargetScene, out targetScenePath))
            {
                outputPath = $"{selectPath}/{outputFolder}/{outputTargetName}";
            }
            else
            {
                outputPath = string.Empty;
            }
        }

        private static bool GetTargetScenePath(string sceneName, out string targetPath)
        {
            targetPath = string.Empty;

            if (!string.IsNullOrEmpty(sceneName))
            {
                foreach (var scene in EditorBuildSettings.scenes)
                {
                    if (Path.GetFileName(scene.path) == sceneName)
                    {
                        targetPath = scene.path;
                        return true;
                    }
                }
            }
            return false;
        }

        private string GetAppExtension()
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return ".app";
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return ".exe";
            }
            return string.Empty;
        }

        private void Build(string outputPath, string createName, params string[] targetScenes)
        {
            nowBuild = true;

            try
            {
                if (!Directory.Exists(outputPath))
                    Directory.CreateDirectory(outputPath);

                var scenes = new List<string>();
                scenes.Add(launcherScenePath); // 必ず先頭に入れる
                scenes.AddRange(targetScenes);

                var extension = GetAppExtension();

                var buildOptions = new BuildPlayerOptions
                                        {
                                            scenes = scenes.ToArray(),
                                            locationPathName = $"{outputPath}/{createName}{extension}",
                                            target = buildTarget,
                                            options = BuildOptions.None
                                        };

                buildResult = BuildPipeline.BuildPlayer(buildOptions);
            }
            catch (Exception ex)
            {
                Debug.Log($"{typeof(ToolsBuildWindow).Name}::Build - {ex.Message}");
            }
            finally
            {
                nowBuild = false;
            }
        }
    }
}