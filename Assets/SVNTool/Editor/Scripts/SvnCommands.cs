using System;
using System.Diagnostics;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CalmIsland.Util.SVN
{
	public static class SvnCommands
	{
		public static Action OnCommitSuccess { get; set; }
		public static Action OnUpdateCompleted { get; set; }
		public static Action OnRevertSuccess { get; set; }

		[MenuItem("Assets/SVN/Update #%&u")]
		private static void Update()
		{
			Call("update", GetPathsFromSelectedObject());
			OnUpdateCompleted?.Invoke();
		}

		[MenuItem("Assets/SVN/Commit #%&c")]
		private static void Commit()
		{
			Call("commit", GetPathsFromSelectedObject());
			OnCommitSuccess?.Invoke();
		}

		[MenuItem("Assets/SVN/Show Log #%&l")]
		private static void ShowLog()
		{
			Call("log", GetPathsFromSelectedObject());
		}

		[MenuItem("Assets/SVN/Checkout #%&h")]
		private static void Checkout()
		{
			Call("checkout", GetPathsFromSelectedObject());
		}

		[MenuItem("Assets/SVN/Clean up #%&e")]
		private static void Cleanup()
		{
			Call("cleanup", GetPathsFromSelectedObject());
		}

		[MenuItem("Assets/SVN/Revert #%&r")]
		private static void Revert()
		{
			Call("revert", GetPathsFromSelectedObject());
			OnRevertSuccess?.Invoke();
		}

		[MenuItem("Assets/SVN/Resolve #%&s")]
		private static void Resolve()
		{
			Call("resolve", GetPathsFromSelectedObject());
		}

		[MenuItem("Assets/SVN/Merge #%&m")]
		private static void Merge()
		{
			Call("merge", GetPathsFromSelectedObject());
		}

		[MenuItem("Assets/SVN/Properties #%&p")]
		private static void Properties()
		{
			Call("properties", GetPathsFromSelectedObject());
		}

		[MenuItem("Assets/SVN/Repo-browser #%&b")]
		private static void RepoBrowser()
		{
			Call("repobrowser", GetPathsFromSelectedObject());
		}

		private static string[] GetPathsFromSelectedObject()
		{
			var selectedObjs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
			if (selectedObjs == null || selectedObjs.Length <= 0)
			{
				return new string[] { Application.dataPath + "/../" };
			}

			var paths = new string[selectedObjs.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				var path = AssetDatabase.GetAssetPath(selectedObjs[i].GetInstanceID());
				path = path.Replace("Assets", string.Empty);
				path = Application.dataPath + path;
				paths[i] = path;
			}
			return paths;
		}

		public static void Call(string command, params string[] assetPaths)
		{
			string output = null;
			try
			{
				output = RunProcess(SettingsUpdater.TortoiseSvnPath, command, assetPaths);
			}
			catch (System.Exception ex)
			{
				Debug.LogErrorFormat("[SvnCommand] svn {0} : STDERR\n{1}", command, ex.Message);
			}

			if (output != null && output.Length > 0)
			{
				Debug.LogFormat("[SvnCommand] svn {0} : STDOUT\n{1}", command, output);
			}
		}

		public static string RunProcess(string exePath, string command, params string[] assetPaths)
		{
			var paths = new StringBuilder();
			for (int i = 0; i < assetPaths.Length; i++)
			{
				paths.Append(assetPaths[i]);
				paths.Append('*');
				paths.Append(assetPaths[i]);
				paths.Append(".meta");
				if (i < assetPaths.Length - 1)
				{
					paths.Append('*');
				}
			}

			var proc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = exePath,
					Arguments = string.Format("/command:{0} /path:\"{1}\" /closeonend:0", command, paths.ToString()),
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true
				}
			};
			proc.Start();
			var stdError = proc.StandardError.ReadToEnd();
			if (stdError != null && stdError.Equals(string.Empty) == false)
			{
				throw new Exception(stdError);
			}
			return proc.StandardOutput.ReadToEnd();
		}
	}
}