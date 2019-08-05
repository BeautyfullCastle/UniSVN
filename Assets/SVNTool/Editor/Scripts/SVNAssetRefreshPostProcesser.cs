using UnityEditor;

namespace CalmIsland.Util.SVN
{
	public class SVNAssetRefreshPostProcesser : AssetPostprocessor
	{
		static SVNAssetRefreshPostProcesser()
		{
			SvnCommands.OnCommitSuccess += OnRefreshFileStatus;
			SvnCommands.OnUpdateCompleted += OnRefreshFileStatus;
			SvnCommands.OnRevertSuccess += OnRefreshFileStatus;
			SettingsUpdater.OnUpdateCompleted += OnRefreshFileStatus;
			SettingsUpdater.UpdateSettingsAsset();
		}

		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnRefreshFileStatus()
		{
			SVNFileStatusCache.Refresh(EditorApplication.RepaintProjectWindow);
		}

		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
		{
			SVNFileStatusCache.Refresh(EditorApplication.RepaintProjectWindow);
		}
	}
}