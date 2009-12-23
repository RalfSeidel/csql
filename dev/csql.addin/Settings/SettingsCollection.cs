using System.Collections.Generic;
using System;

namespace csql.addin.Settings
{
	[Serializable]
	public class SettingsCollection
	{
		public List<SettingsItem> SettingsObjects { get; set; }

		public SettingsCollection()
		{
			this.SettingsObjects = new List<SettingsItem>();
		}
	}
}
