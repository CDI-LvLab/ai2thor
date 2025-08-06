using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Linq;
using UnityEngine;

public class RenameWebGLFiles : IPostprocessBuildWithReport {
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report) {
        if (report.summary.platform != BuildTarget.WebGL)
            return;

        string fullPath = report.summary.outputPath;
        string buildDir = Path.Combine(fullPath, "Build");

        // Get all files in Build folder
        string[] files = Directory.GetFiles(buildDir);

        // Extract the last folder name. That's our build name.
        string buildName = Path.GetFileName(fullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        string targetName = "Unity";

        foreach (var file in files) {
            string fileName = Path.GetFileName(file);

            if (fileName.StartsWith(buildName)) {
                string newName = targetName + fileName.Substring(buildName.Length);
                string newPath = Path.Combine(buildDir, newName);

                File.Move(file, newPath);
                Debug.Log($"Renamed {fileName} → {newName}");
            }
        }
    }
}
