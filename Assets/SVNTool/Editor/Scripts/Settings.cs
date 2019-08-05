using UnityEngine;

namespace CalmIsland.Util.SVN
{
	public class Settings : ScriptableObject
	{
		[SerializeField]
		private bool showIcon = false;

		[SerializeField]
		private string tortoiseSvnPath = @"C:\Program Files\TortoiseSVN\bin\TortoiseProc.exe";

		public bool ShowIcon { get => showIcon; }
		public string TortoiseSvnPath { get => tortoiseSvnPath; }

		private void OnValidate()
		{
			SettingsUpdater.UpdateSettingsAsset();
		}
	}
}