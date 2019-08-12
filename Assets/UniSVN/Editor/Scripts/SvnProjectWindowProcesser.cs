using UnityEditor;
using UnityEngine;

namespace UniSVN
{
	[InitializeOnLoad]
	public class SvnProjectWindowProcesser
	{
		private static readonly Texture modifiedTexture;
		private static readonly Texture committedTexture;
		private static readonly Texture newTexture;
		private static readonly Texture addTexture;
		private static readonly Texture conflictTexture;
		private static readonly Texture externalTexture;

		private static readonly string iconsFolderPath = Path.rootPath + "/Icons";

		static SvnProjectWindowProcesser()
		{
			committedTexture = AssetDatabase.LoadAssetAtPath<Texture>(iconsFolderPath + "/icon-commited.png");
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

			AssetStatus type = SVNFileStatusCache.GetStatus(assetPath);

			Texture icon = null;
			switch (type)
			{
				case AssetStatus.None:
					icon = committedTexture;
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

			GUI.DrawTexture(new Rect(selectionRect.xMin - 8, selectionRect.yMin - 2, 16, 16), icon);
		}
	}
}