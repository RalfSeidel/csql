using System.Collections.Generic;
using System;

namespace csql.addin.Settings
{
	[Serializable]
	internal class SettingsCollection
	{
		public SettingsCollection()
		{
			this.SettingsObjects = new List<CSqlParameter>();
		}

		public List<CSqlParameter> SettingsObjects { get; set; }
	}
}
