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

					SetFileStatus(path, AssetStatusConverter.StringToEnum(status));
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
					SetStatus(path, status);
					return;
				}

				FileAttributes attr = File.GetAttributes(path);
				if (attr.HasFlag(FileAttributes.Directory) == false)
				{
					return;
				}

				// If this directory status is NEW,
				// change child directories and files status to NEW also.
				var absolutePath = GetAbsolutePath(path);
				string[] filePaths = Directory.GetFiles(absolutePath, "*", SearchOption.AllDirectories);
				string[] folderPaths = Directory.GetDirectories(absolutePath, "*", SearchOption.AllDirectories);
				var paths = new List<string>();
				paths.AddRange(filePaths);
				paths.AddRange(folderPaths);
				paths.RemoveAll((x) => x.Contains(".meta"));
				for (int i = 0; i < paths.Count; i++)
				{
					paths[i] = GetRelativePath(paths[i]);
					SetStatus(paths[i], status);
				}
			}
			else
			{
				if (path.Contains(".meta"))
				{
					path = path.Replace(".meta", string.Empty);
				}

				// Parent folders status follow file's status
				// if it's not in a normal status.
				var parentDirectoryInfo = Directory.GetParent(GetAbsolutePath(path));
				do
				{
					string relativePath = GetRelativePath(parentDirectoryInfo.FullName);
					SetStatus(relativePath, status);
					parentDirectoryInfo = parentDirectoryInfo.Parent;
				} while (parentDirectoryInfo.FullName.Contains("Assets"));

				SetStatus(path, status);
			}
		}

		private static string GetRelativePath(string path)
		{
			return path.Replace("\\", "/").Replace(Application.dataPath, "Assets");
		}

		private static string GetAbsolutePath(string path)
		{
			return Application.dataPath + path.Replace("Assets", string.Empty);
		}

		private static void SetStatus(string path, AssetStatus status)
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

		public static AssetStatus GetStatus(string path)
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