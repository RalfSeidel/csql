using csql.addin.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csql.Commands
{
	/// <summary>
	/// Unit tests for <see cref="T:VariableSubstitutor"/>
	/// </summary>
	[TestClass]
	public class VariableSubstitutorTest
	{
		[TestMethod]
		public void ItemDirSubstitutionTest1()
		{
			var env = new MockDocumentEnvironment() { ItemDirectory = "c:\\test\\ItemDirectory" };
			var target = new VariableSubstitutor( env );
			string value = "$(ItemDir)";
			string result = target.Substitute( value );
			Assert.AreEqual( env.ItemDirectory, result );
		}


		[TestMethod]
		public void ItemDirSubstitutionTest2()
		{
			var env = new MockDocumentEnvironment() { ItemDirectory = "B" };
			var target = new VariableSubstitutor( env );
			string value = "A$(ItemDir)C";
			string result = target.Substitute( value );
			Assert.AreEqual( "ABC", result );
		}

		[TestMethod]
		public void ItemDirectorySubstitutionTest3()
		{
			var env = new MockDocumentEnvironment() { ItemDirectory = "B" };
			var target = new VariableSubstitutor( env );
			string value = "A$(ItemDirectory)C";
			string result = target.Substitute( value );
			Assert.AreEqual( "ABC", result );
		}

		[TestMethod]
		public void ItemDirectorySubstitutionTest4()
		{
			var env = new MockDocumentEnvironment() { ItemDirectory = "B" };
			var target = new VariableSubstitutor( env );
			string value = "A$(ItemDirectory)CC$(ItemDirectory)A";
			string result = target.Substitute( value );
			Assert.AreEqual( "ABCCBA", result );
		}



		[TestMethod]
		public void ProjectDirSubstitutionTest1()
		{
			var env = new MockDocumentEnvironment() { ProjectDirectory = "c:\\test\\ProjectDirectory" };
			var target = new VariableSubstitutor( env );
			string value = "$(ProjectDir)";
			string result = target.Substitute( value );
			Assert.AreEqual( env.ProjectDirectory, result );
		}


		[TestMethod]
		public void ProjectDirSubstitutionTest2()
		{
			var env = new MockDocumentEnvironment() { ProjectDirectory = "B" };
			var target = new VariableSubstitutor( env );
			string value = "A$(ProjectDir)C";
			string result = target.Substitute( value );
			Assert.AreEqual( "ABC", result );
		}

		[TestMethod]
		public void ProjectDirectorySubstitutionTest3()
		{
			var env = new MockDocumentEnvironment() { ProjectDirectory = "B" };
			var target = new VariableSubstitutor( env );
			string value = "A$(ProjectDirectory)C";
			string result = target.Substitute( value );
			Assert.AreEqual( "ABC", result );
		}

		[TestMethod]
		public void ProjectDirectorySubstitutionTest4()
		{
			var env = new MockDocumentEnvironment() { ProjectDirectory = "B" };
			var target = new VariableSubstitutor( env );
			string value = "A$(ProjectDirectory)CC$(ProjectDirectory)A";
			string result = target.Substitute( value );
			Assert.AreEqual( "ABCCBA", result );
		}


		[TestMethod]
		public void SolutionDirSubstitutionTest1()
		{
			var env = new MockDocumentEnvironment() { SolutionDirectory = "c:\\test\\SolutionDirectory" };
			var target = new VariableSubstitutor( env );
			string value = "$(SolutionDir)";
			string result = target.Substitute( value );
			Assert.AreEqual( env.SolutionDirectory, result );
		}


		[TestMethod]
		public void SolutionDirSubstitutionTest2()
		{
			var env = new MockDocumentEnvironment() { SolutionDirectory = "B" };
			var target = new VariableSubstitutor( env );
			string value = "A$(SolutionDir)C";
			string result = target.Substitute( value );
			Assert.AreEqual( "ABC", result );
		}

		[TestMethod]
		public void SolutionDirectorySubstitutionTest3()
		{
			var env = new MockDocumentEnvironment() { SolutionDirectory = "B" };
			var target = new VariableSubstitutor( env );
			string value = "A$(SolutionDirectory)C";
			string result = target.Substitute( value );
			Assert.AreEqual( "ABC", result );
		}

		[TestMethod]
		public void SolutionDirectorySubstitutionTest4()
		{
			var env = new MockDocumentEnvironment() { SolutionDirectory = "B" };
			var target = new VariableSubstitutor( env );
			string value = "A$(SolutionDirectory)CC$(SolutionDirectory)A";
			string result = target.Substitute( value );
			Assert.AreEqual( "ABCCBA", result );
		}

		[TestMethod]
		public void AllVariablesTest()
		{
			var env = new MockDocumentEnvironment() { ItemDirectory = "A", ProjectDirectory = "B", SolutionDirectory = "C" };
			var target = new VariableSubstitutor( env );
			string value = "$(ItemDirectory)$(ProjectDirectory)$(SolutionDirectory)";
			string result = target.Substitute( value );
			Assert.AreEqual( "ABC", result );
		}

		[TestMethod]
		public void UnsupportedVariablesTest()
		{
			var env = new MockDocumentEnvironment() { ItemDirectory = "A", ProjectDirectory = "B", SolutionDirectory = "C" };
			var target = new VariableSubstitutor( env );
			string value = "$(Unsupported)$";
			string result = target.Substitute( value );
			Assert.AreEqual( value, result );
		}


		private class MockDocumentEnvironment : IDocumentEnvironment
		{
			public string ItemDirectory
			{
				get;
				set;
			}

			public string ProjectDirectory
			{
				get;
				set;
			}

			public string SolutionDirectory
			{
				get;
				set;
			}

			public string TargetDirectory
			{
				get;
				set;
			}
		}
	}
}
