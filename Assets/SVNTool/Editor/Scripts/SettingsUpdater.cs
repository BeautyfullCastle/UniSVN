using System;
using UnityEditor;
using UnityEngine;

namespace CalmIsland.Util.SVN
{
	public static class SettingsUpdater
	{
		private static string assetPath = "Assets/Common/SVNTool/Editor/Settings/SvnSettings.asset";

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
				if (AssetDatabase.IsValidFolder("Assets/Common/SVNTool/Editor/Settings") == false)
				{
					AssetDatabase.CreateFolder("Assets/Common/SVNTool/Editor", "Settings");
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