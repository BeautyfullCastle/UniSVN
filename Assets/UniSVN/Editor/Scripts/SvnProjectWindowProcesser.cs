using UnityEditor;
using UnityEngine;

namespace UniSVN
{
	[InitializeOnLoad]
	public class SvnProjectWindowProcesser
	{
		private static Texture modifiedTexture;
		private static Texture commitedTexture;
		private static Texture newTexture;
		private static Texture addTexture;
		private static Texture conflictTexture;
		private static Texture externalTexture;

		private static readonly string iconsFolderPath = Path.rootPath + "/Icons";

		static SvnProjectWindowProcesser()
		{
			commitedTexture = AssetDatabase.LoadAssetAtPath<Texture>(iconsFolderPath + "/icon-commited.png");
			modifiedTexture = AssetDatabase.LoadAssetAtPath<Texture>(iconsFolderPath + "/icon-modify.png");
			newTexture = AssetDatabase.LoadAssetAtPath<Texture>(iconsFolderPath + "/icon-new.png");
			addTexture = AssetDatabase.LoadAssetAtPath<Texture>(iconsFolderPath + "/icon-add.png");
			conflictTexture = AssetDatabase.LoadAssetAtPath<Texture>(iconsFolderPath + "/icon-conficted.png");
			externalTexture = AssetDatabase.LoadAssetAtPath<Texture>(iconsFolderPath + "/icon-external-link.png");
			EditorApplication.projectWindowItemOnGUI += OnProjectItemGUI;
		}

		private static void OnProjectItemGUI(string guid, Rect selectionRect)
		{
			if (SettingsUpdater.ShowIcon == false)
			{
				return;
			}

			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			if (string.IsNullOrEmpty(assetPath))
			{
				return;
			}

			AssetStatus type = SVNFileStatusCache.GetFileStatus(assetPath);

			Texture icon = null;
			switch (type)
			{
				case AssetStatus.None:
					icon = commitedTexture;
					break;

				case AssetStatus.Added:
					icon = addTexture;
					break;

				case AssetStatus.Modify:
					icon = modifiedTexture;
					break;

				case AssetStatus.Delete:
					break;

				case AssetStatus.New:
					icon = newTexture;
					break;

				case AssetStatus.Conflict:
					icon = conflictTexture;
					break;

				case AssetStatus.External:
					icon = externalTexture;
					break;
			}

			if (icon == null)
			{
				return;
			}

			Debug.Log(icon.name);
			GUI.DrawTexture(new Rect(selectionRect.xMin - 8, selectionRect.yMin - 2, 16, 16), icon);
		}
	}
}