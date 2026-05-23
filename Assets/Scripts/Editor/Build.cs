// 役割: Unity -batchmode から WebGL ビルドを実行するための Editor スクリプト。
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

namespace Ironbound.EditorTools
{
    public static class Build
    {
        public static void WebGL()
        {
            string output = Path.Combine(Directory.GetCurrentDirectory(), "Builds/WebGL");
            Directory.CreateDirectory(output);
            string[] scenes = { "Assets/Scenes/Title.unity", "Assets/Scenes/ClassSelect.unity", "Assets/Scenes/AshenPlain.unity", "Assets/Scenes/Result.unity" };
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.template = "PROJECT:Default";
            PlayerSettings.WebGL.memorySize = 512;
            PlayerSettings.runInBackground = false;
            var opts = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = output,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };
            BuildReport report = BuildPipeline.BuildPlayer(opts);
            if (report.summary.result != BuildResult.Succeeded)
                throw new System.Exception("WebGL Build failed: " + report.summary.result);
            Debug.Log("WebGL Build OK: " + output);
        }
    }
}
#endif
