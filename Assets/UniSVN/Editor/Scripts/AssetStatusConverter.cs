using System.Linq;

namespace UniSVN
{
	public static class AssetStatusConverter
	{
		public static AssetStatus StringToEnum(string status)
		{
			char cStatus = status.First();
			switch (cStatus)
			{
				case 'A':
					return AssetStatus.Added;

				case '?':
					return AssetStatus.New;

				case 'M':
					return AssetStatus.Modify;

				case 'D':
					return AssetStatus.Delete;

				case 'C':
					return AssetStatus.Conflict;

				case '!':
					return AssetStatus.Missing;

				case 'X':
					return AssetStatus.External;

				default:
					return AssetStatus.None;
			}
		}
	}
}