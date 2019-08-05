using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UniSVN
{
	public class SVNFileStatusCache
	{
		private static Dictionary<string, AssetStatus> statusCache = new Dictionary<string, AssetStatus>();

		public static void Refresh(System.Action onComplete = null)
		{
#if UNITY_EDITOR_WIN
			if (SettingsUpdater.ShowIcon)
			{
				statusCache.Clear();
				CallSvnStatus();
			}
#endif

			if (onComplete != null)
			{
				onComplete();
			}
		}

		private static void CallSvnStatus()
		{
			var proc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = @"cmd",
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
				}
			};
			proc.Start();

			proc.StandardInput.WriteLine("svn status");
			proc.StandardInput.Close();

			var stdError = proc.StandardError.ReadToEnd();
			proc.StandardError.Close();

			string stdOutput = @proc.StandardOutput.ReadToEnd().Replace("\\", "/");
			proc.StandardOutput.Close();

			if (stdError == null || stdError.Length <= 0)
			{
				var outputArr = stdOutput.Split("\n".ToCharArray());
				foreach (var output in outputArr)
				{
					if (output.Contains("Assets") == false)
					{
						continue;
					}

					var splitedOutput = output.Split(' ');
					if (splitedOutput.Length < 2)
					{
						continue;
					}

					var status = splitedOutput[0];
					var path = splitedOutput[splitedOutput.Length - 1];
					path = path.Replace("\r", string.Empty);

					SetFileStatus(path, AssetStatusConverter.Convert(status));
				}
			}
			else
			{
				Debug.LogError(stdError);
			}
			proc.WaitForExit();
			proc.Close();
		}

		private static void SetFileStatus(string path, AssetStatus status)
		{
			if (status == AssetStatus.New)
			{
				if (path.Contains(".meta"))
				{
					path = path.Replace(".meta", string.Empty);
					SetCache(path, status);
					return;
				}

				FileAttributes attr = File.GetAttributes(path);
				if (attr.HasFlag(FileAttributes.Directory) == false)
				{
					return;
				}

				var absolutePath = Application.dataPath + path.Replace("Assets", string.Empty);
				string[] filePaths = Directory.GetFiles(absolutePath, "*", SearchOption.AllDirectories);
				string[] folderPaths = Directory.GetDirectories(absolutePath, "*", SearchOption.AllDirectories);
				var paths = new List<string>();
				paths.AddRange(filePaths);
				paths.AddRange(folderPaths);
				paths.RemoveAll((x) => x.Contains(".meta"));
				for (int i = 0; i < paths.Count; i++)
				{
					paths[i] = paths[i].Replace(Application.dataPath, "Assets").Replace("\\", "/");
					SetCache(paths[i], status);
				}
			}
			else
			{
				if (path.Contains(".meta"))
				{
					path = path.Replace(".meta", string.Empty);
				}

				SetCache(path, status);
			}
		}

		private static void SetCache(string path, AssetStatus status)
		{
			if (statusCache.ContainsKey(path))
			{
				statusCache[path] = status;
			}
			else
			{
				statusCache.Add(path, status);
			}
		}

		public static AssetStatus GetFileStatus(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return AssetStatus.Missing;
			}

			if (statusCache.ContainsKey(path) == false)
			{
				return AssetStatus.None;
			}

			return statusCache[path];
		}

		public static Dictionary<string, AssetStatus> Filter(string workDir, string prefix)
		{
			Dictionary<string, AssetStatus> ret = new Dictionary<string, AssetStatus>();
			foreach (string key in statusCache.Keys)
			{
				if (key.StartsWith(System.IO.Path.Combine(workDir, prefix)))
				{
					string rPath = key.Substring(workDir.Length);
					if (rPath.StartsWith("/"))
					{
						rPath = rPath.Substring(1);
					}
					ret.Add(rPath, statusCache[key]);
				}
			}
			return ret;
		}
	}
}