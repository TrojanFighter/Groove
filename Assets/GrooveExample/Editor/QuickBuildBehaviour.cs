using UnityEditor;

namespace Spectrum
{
	public class GrooveExample
	{
		public static string ExampleRoot = "Assets/GrooveExample";

		public static BuildTarget TargetPlatform;

		//public static BuildOptions BuildOptions = BuildOptions.Il2CPP;
		public static BuildOptions BuildOptions;

		public static string PrevPath = null;

		public static void BuildServer(string path)
		{
			var masterScenes = new[]
			{
				ExampleRoot+ "/Scenes/GrooveExample.unity"
			};
			TargetPlatform = BuildTarget.StandaloneWindows64;
			BuildPipeline.BuildPlayer(masterScenes, path + "/Win64/Groove.exe", TargetPlatform, BuildOptions);
		}

		public static void BuildClient(string path)
		{
			var masterScenes = new[]
			{
				ExampleRoot+ "/Scenes/GrooveExample.unity"
			};
			TargetPlatform = BuildTarget.WebGL;
			BuildPipeline.BuildPlayer(masterScenes, path + "/WebGL", TargetPlatform, BuildOptions);
		}


		#region Editor Menu

		[MenuItem("Tools/GrooveExamples/Build Server", false, 11)]
		public static void BuildServerMenu()
		{
			var path = GetPath();
			if (!string.IsNullOrEmpty(path))
			{
				BuildServer(path);
			}
		}

		[MenuItem("Tools/GrooveExamples/Build Client", false, 11)]
		public static void BuildClientMenu()
		{
			var path = GetPath();
			if (!string.IsNullOrEmpty(path))
			{
				BuildClient(path);
			}
		}

		#endregion

		public static string GetPath()
		{
			var prevPath = EditorPrefs.GetString("groove.buildPath", "");
			string path = EditorUtility.SaveFolderPanel("Choose Location for binaries", prevPath, "");

			if (!string.IsNullOrEmpty(path))
			{
				EditorPrefs.SetString("groove.buildPath", path);
			}
			return path;
		}
	}
}