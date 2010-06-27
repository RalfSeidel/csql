using System.Collections.Generic;
using System;
using System.Linq;

namespace Sqt.DbcProvider.Gui
{
	/// <summary>
	/// Adapter to fill the view model with the data saved in the connection
	/// most recently used history and to save the view model data once
	/// the input is finished.
	/// </summary>
	public class DbConnectionViewModelAdapter
	{
		private const int MaxMruEntries = 10;

		public DbConnectionViewModel Load( MruConnections history )
		{
			DbConnectionViewModel vm = new DbConnectionViewModel();
			vm.Load( history );
			return vm;
		}

		public MruConnections Save( DbConnectionViewModel vm )
		{
			if ( vm.MruConnections == null )
				return null;
			MruDbConnectionParameterAdapter.SetMruDbConnectionParameter( vm.MruConnections, vm.ConnectionParameter );
			return vm.MruConnections;
		}

	}

}