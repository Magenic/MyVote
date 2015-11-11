using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyVote.BusinessObjects.Core.Contracts;
using MyVote.BusinessObjects.Tests.Contracts;
using MyVote.Core.Extensions;
using Spackle;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class ObjectFactoryTests
	{
		[TestMethod]
		[ExpectedException(typeof(NotImplementedException))]
		public void BeginCreate()
		{
			new ObjectFactory<ObjectFactoryTest>().BeginCreate();
		}

		[TestMethod, ExpectedException(typeof(NotImplementedException))]
		public void BeginCreateWithCriteria()
		{
			new ObjectFactory<ObjectFactoryTest>().BeginCreate(null);
		}

		[TestMethod, ExpectedException(typeof(NotImplementedException))]
		public void BeginCreateWithCriteriaAndUserState()
		{
			new ObjectFactory<ObjectFactoryTest>().BeginCreate(null, null);
		}

		[TestMethod, ExpectedException(typeof(NotImplementedException))]
		public void BeginDeleteWithCriteria()
		{
			new ObjectFactory<ObjectFactoryTest>().BeginDelete(null);
		}

		[TestMethod, ExpectedException(typeof(NotImplementedException))]
		public void BeginDeleteWithCriteriaAndUserState()
		{
			new ObjectFactory<ObjectFactoryTest>().BeginDelete(null, null);
		}

		[TestMethod, ExpectedException(typeof(NotImplementedException))]
		public void BeginExecuteWithObject()
		{
			new ObjectFactory<ObjectFactoryTest>().BeginExecute(null);
		}

		[TestMethod, ExpectedException(typeof(NotImplementedException))]
		public void BeginExecuteWithObjectAndUserState()
		{
			new ObjectFactory<ObjectFactoryTest>().BeginExecute(null, null);
		}

		[TestMethod, ExpectedException(typeof(NotImplementedException))]
		public void BeginFetch()
		{
			new ObjectFactory<ObjectFactoryTest>().BeginFetch();
		}

		[TestMethod, ExpectedException(typeof(NotImplementedException))]
		public void BeginFetchWithCriteria()
		{
			new ObjectFactory<ObjectFactoryTest>().BeginFetch(null);
		}

		[TestMethod, ExpectedException(typeof(NotImplementedException))]
		public void BeginFetchWithCriteriaAndUserState()
		{
			new ObjectFactory<ObjectFactoryTest>().BeginFetch(null, null);
		}

		[TestMethod, ExpectedException(typeof(NotImplementedException))]
		public void BeginFetchWithObject()
		{
			new ObjectFactory<ObjectFactoryTest>().BeginUpdate(null);
		}

		[TestMethod, ExpectedException(typeof(NotImplementedException))]
		public void BeginFetchWithObjectAndUserState()
		{
			new ObjectFactory<ObjectFactoryTest>().BeginUpdate(null, null);
		}

		[TestMethod, ExpectedException(typeof(NotImplementedException))]
		public void BeginUpdateWithObject()
		{
			new ObjectFactory<ObjectFactoryTest>().BeginUpdate(null);
		}

		[TestMethod, ExpectedException(typeof(NotImplementedException))]
		public void BeginUpdateWithObjectAndUserState()
		{
			new ObjectFactory<ObjectFactoryTest>().BeginUpdate(null, null);
		}

		[TestMethod]
		public void Create()
		{
			var test = new ObjectFactory<ObjectFactoryTest>().Create();
			Assert.AreEqual(string.Empty, test.Data, test.GetPropertyName(_ => _.Data));
		}

		[TestMethod]
		public void CreateWithCriteria()
		{
			var data = new RandomObjectGenerator().Generate<string>();
			var test = new ObjectFactory<ObjectFactoryTest>().Create(data);
			Assert.AreEqual(data, test.Data, test.GetPropertyName(_ => _.Data));
		}

		[TestMethod]
		public async Task CreateAsync()
		{
			var test = await new ObjectFactory<ObjectFactoryTest>().CreateAsync();
			Assert.AreEqual(string.Empty, test.Data, test.GetPropertyName(_ => _.Data));
		}

		[TestMethod]
		public async Task CreateAsyncWithCriteria()
		{
			var data = new RandomObjectGenerator().Generate<string>();
			var test = await new ObjectFactory<ObjectFactoryTest>().CreateAsync(data);
			Assert.AreEqual(data, test.Data, test.GetPropertyName(_ => _.Data));
		}

		[TestMethod]
		public void Delete()
		{
			var data = new RandomObjectGenerator().Generate<string>();
			new ObjectFactory<ObjectFactoryTest>().Delete(data);
		}

		[TestMethod]
		public async Task DeleteAsync()
		{
			var data = new RandomObjectGenerator().Generate<string>();
			await new ObjectFactory<ObjectFactoryTest>().DeleteAsync(data);
		}

		[TestMethod]
		public void Fetch()
		{
			var test = new ObjectFactory<ObjectFactoryTest>().Fetch();
			Assert.AreEqual(string.Empty, test.Data, test.GetPropertyName(_ => _.Data));
		}

		[TestMethod]
		public void FetchWithCriteria()
		{
			var data = new RandomObjectGenerator().Generate<string>();
			var test = new ObjectFactory<ObjectFactoryTest>().Fetch(data);
			Assert.AreEqual(data, test.Data, test.GetPropertyName(_ => _.Data));
		}

		[TestMethod]
		public async Task FetchAsync()
		{
			var test = await new ObjectFactory<ObjectFactoryTest>().FetchAsync();
			Assert.AreEqual(string.Empty, test.Data, test.GetPropertyName(_ => _.Data));
		}

		[TestMethod]
		public async Task FetchAsyncWithCriteria()
		{
			var data = new RandomObjectGenerator().Generate<string>();
			var test = await new ObjectFactory<ObjectFactoryTest>().FetchAsync(data);
			Assert.AreEqual(data, test.Data, test.GetPropertyName(_ => _.Data));
		}

		[TestMethod]
		public void Execute()
		{
			var factory = new ObjectFactory<ObjectFactoryTestCommand>();
			var test = factory.Execute(factory.Create());
			Assert.AreEqual("done", test.Data, test.GetPropertyName(_ => _.Data));
		}

		[TestMethod]
		public async Task ExecuteAsync()
		{
			var factory = new ObjectFactory<ObjectFactoryTestCommand>();
			var test = await factory.ExecuteAsync(factory.Create());
			Assert.AreEqual("done", test.Data, test.GetPropertyName(_ => _.Data));
		}

		[TestMethod]
		public void GetGlobalContext()
		{
			var factory = new ObjectFactory<ObjectFactoryTestCommand>();
			Assert.AreSame(ApplicationContext.GlobalContext, factory.GlobalContext, factory.GetPropertyName(_ => _.GlobalContext));
		}

		[TestMethod]
		public void Update()
		{
			var factory = new ObjectFactory<ObjectFactoryTest>();
			var test = factory.Fetch();
			factory.Update(test);
		}

		[TestMethod]
		public async Task UpdateAsync()
		{
			var factory = new ObjectFactory<ObjectFactoryTest>();
			var test = factory.Fetch();
			await factory.UpdateAsync(test);
		}
	}

	[Serializable]
	public sealed class ObjectFactoryTestCommand
		: CommandBase<ObjectFactoryTestCommand>, IObjectFactoryTestCommand
	{
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		private void DataPortal_Create() { }

		protected override void DataPortal_Execute()
		{
			this.Data = "done";
		}

		public static PropertyInfo<string> DataProperty =
			ObjectFactoryTestCommand.RegisterProperty<string>(_ => _.Data);
		public string Data
		{
			get { return this.ReadProperty(ObjectFactoryTestCommand.DataProperty); }
			set { this.LoadProperty(ObjectFactoryTestCommand.DataProperty, value); }
		}
	}

	[Serializable]
	public sealed class ObjectFactoryTest
		: BusinessBase<ObjectFactoryTest>, IObjectFactoryTest
	{
		private void Child_Create(string data)
		{
			this.Data = data;
		}

		private void Child_Fetch(string data)
		{
			this.Data = data;
		}

		private void DataPortal_Create(string data)
		{
			this.Data = data;
		}

		private void DataPortal_Delete(string data)
		{
			this.Data = data;
		}

		private void DataPortal_Fetch(string data)
		{
			this.Data = data;
		}

		public static PropertyInfo<string> DataProperty =
			ObjectFactoryTest.RegisterProperty<string>(_ => _.Data);
		public string Data
		{
			get { return this.GetProperty(ObjectFactoryTest.DataProperty); }
			set { this.SetProperty(ObjectFactoryTest.DataProperty, value); }
		}
	}
}

namespace MyVote.BusinessObjects.Tests.Contracts
{
	public interface IObjectFactoryTest
		: IBusinessBase
	{
		string Data { get; set; }
	}

	public interface IObjectFactoryTestCommand
		: ICommandBaseCore
	{
		string Data { get; set; }
	}
}