using System.Collections.Generic;
using System;

namespace csql.addin.Settings
{
	[Serializable]
	internal class SettingsCollection
	{
		public List<CSqlParameter> SettingsObjects { get; set; }

		public SettingsCollection()
		{
			this.SettingsObjects = new List<CSqlParameter>();
		}
	}
}
