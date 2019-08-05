using System;
using UnityEditor;
using UnityEngine;

namespace UniSVN
{
	public static class SettingsUpdater
	{
		private static readonly string folderName = "Settings";
		private static readonly string assetName = "SvnSettings.asset";
		private static readonly string assetPath = string.Format("{0}/{1}/{2}", Path.rootPath, folderName, assetName);

		private static Settings settings = null;

		public static Action OnUpdateCompleted { get; set; }

		public static string TortoiseSvnPath
		{
			get
			{
				if (settings == null)
				{
					UpdateSettingsAsset();
				}
				return settings.TortoiseSvnPath;
			}
		}

		public static bool ShowIcon
		{
			get
			{
				if (settings == null)
				{
					UpdateSettingsAsset();
				}
				return settings.ShowIcon;
			}
		}

		public static void UpdateSettingsAsset()
		{
			settings = AssetDatabase.LoadAssetAtPath<Settings>(assetPath);
			if (settings == null)
			{
				if (AssetDatabase.IsValidFolder(Path.rootPath + "/" + folderName) == false)
				{
					AssetDatabase.CreateFolder(Path.rootPath, folderName);
				}

				settings = ScriptableObject.CreateInstance<Settings>();
				AssetDatabase.CreateAsset(settings, assetPath);
				AssetDatabase.SaveAssets();
			}

			OnUpdateCompleted?.Invoke();
		}

		[MenuItem("Assets/SVN/Settings #%&t")]
		private static void SelectSettings()
		{
			if (settings == null)
			{
				UpdateSettingsAsset();
			}
			EditorGUIUtility.PingObject(settings);
		}
	}
}