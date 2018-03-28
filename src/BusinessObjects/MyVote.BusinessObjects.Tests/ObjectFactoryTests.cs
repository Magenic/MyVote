using Csla;
using FluentAssertions;
using MyVote.BusinessObjects.Core.Contracts;
using MyVote.BusinessObjects.Tests.Contracts;
using Spackle;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class ObjectFactoryTests
	{
		[Fact]
		public void BeginCreate()
		{
			new Action(() => new ObjectFactory<ObjectFactoryTest>().BeginCreate()).ShouldThrow<NotImplementedException>();
		}

		[Fact]
		public void BeginCreateWithCriteria()
		{
			new Action(() => new ObjectFactory<ObjectFactoryTest>().BeginCreate(null)).ShouldThrow<NotImplementedException>();
		}

		[Fact]
		public void BeginCreateWithCriteriaAndUserState()
		{
			new Action(() => new ObjectFactory<ObjectFactoryTest>().BeginCreate(null, null)).ShouldThrow<NotImplementedException>();
		}

		[Fact]
		public void BeginDeleteWithCriteria()
		{
			new Action(() => new ObjectFactory<ObjectFactoryTest>().BeginDelete(null)).ShouldThrow<NotImplementedException>();
		}

		[Fact]
		public void BeginDeleteWithCriteriaAndUserState()
		{
			new Action(() => new ObjectFactory<ObjectFactoryTest>().BeginDelete(null, null)).ShouldThrow<NotImplementedException>();
		}

		[Fact]
		public void BeginExecuteWithObject()
		{
			new Action(() => new ObjectFactory<ObjectFactoryTest>().BeginExecute(null)).ShouldThrow<NotImplementedException>();
		}

		[Fact]
		public void BeginExecuteWithObjectAndUserState()
		{
			new Action(() => new ObjectFactory<ObjectFactoryTest>().BeginExecute(null, null)).ShouldThrow<NotImplementedException>();
		}

		[Fact]
		public void BeginFetch()
		{
			new Action(() => new ObjectFactory<ObjectFactoryTest>().BeginFetch()).ShouldThrow<NotImplementedException>();
		}

		[Fact]
		public void BeginFetchWithCriteria()
		{
			new Action(() => new ObjectFactory<ObjectFactoryTest>().BeginFetch(null)).ShouldThrow<NotImplementedException>();
		}

		[Fact]
		public void BeginFetchWithCriteriaAndUserState()
		{
			new Action(() => new ObjectFactory<ObjectFactoryTest>().BeginFetch(null, null)).ShouldThrow<NotImplementedException>();
		}

		[Fact]
		public void BeginFetchWithObject()
		{
			new Action(() => new ObjectFactory<ObjectFactoryTest>().BeginUpdate(null)).ShouldThrow<NotImplementedException>();
		}

		[Fact]
		public void BeginFetchWithObjectAndUserState()
		{
			new Action(() => new ObjectFactory<ObjectFactoryTest>().BeginUpdate(null, null)).ShouldThrow<NotImplementedException>();
		}

		[Fact]
		public void BeginUpdateWithObject()
		{
			new Action(() => new ObjectFactory<ObjectFactoryTest>().BeginUpdate(null)).ShouldThrow<NotImplementedException>();
		}

		[Fact]
		public void BeginUpdateWithObjectAndUserState()
		{
			new Action(() => new ObjectFactory<ObjectFactoryTest>().BeginUpdate(null, null)).ShouldThrow<NotImplementedException>();
		}

		[Fact]
		public void Create()
		{
			var test = new ObjectFactory<ObjectFactoryTest>().Create();
			test.Data.Should().BeEmpty();
		}

		[Fact]
		public void CreateWithCriteria()
		{
			var data = new RandomObjectGenerator().Generate<string>();
			var test = new ObjectFactory<ObjectFactoryTest>().Create(data);
			test.Data.Should().Be(data);
		}

		[Fact]
		public async Task CreateAsync()
		{
			var test = await new ObjectFactory<ObjectFactoryTest>().CreateAsync();
			test.Data.Should().BeEmpty();
		}

		[Fact]
		public async Task CreateAsyncWithCriteria()
		{
			var data = new RandomObjectGenerator().Generate<string>();
			var test = await new ObjectFactory<ObjectFactoryTest>().CreateAsync(data);
			test.Data.Should().Be(data);
		}

		[Fact]
		public void Delete()
		{
			var data = new RandomObjectGenerator().Generate<string>();
			new Action(() => new ObjectFactory<ObjectFactoryTest>().Delete(data)).ShouldNotThrow();
		}

		[Fact]
		public void DeleteAsync()
		{
			var data = new RandomObjectGenerator().Generate<string>();
			var func = new Func<Task>(async () => await new ObjectFactory<ObjectFactoryTest>().DeleteAsync(data));
			func.ShouldNotThrow();
		}

		[Fact]
		public void Fetch()
		{
			var test = new ObjectFactory<ObjectFactoryTest>().Fetch();
			test.Data.Should().BeEmpty();
		}

		[Fact]
		public void FetchWithCriteria()
		{
			var data = new RandomObjectGenerator().Generate<string>();
			var test = new ObjectFactory<ObjectFactoryTest>().Fetch(data);
			test.Data.Should().Be(data);
		}

		[Fact]
		public async Task FetchAsync()
		{
			var test = await new ObjectFactory<ObjectFactoryTest>().FetchAsync();
			test.Data.Should().BeEmpty();
		}

		[Fact]
		public async Task FetchAsyncWithCriteria()
		{
			var data = new RandomObjectGenerator().Generate<string>();
			var test = await new ObjectFactory<ObjectFactoryTest>().FetchAsync(data);
			test.Data.Should().Be(data);
		}

		[Fact]
		public void Execute()
		{
			var factory = new ObjectFactory<ObjectFactoryTestCommand>();
			var test = factory.Execute(factory.Create());
			test.Data.Should().Be("done");
		}

		[Fact]
		public async Task ExecuteAsync()
		{
			var factory = new ObjectFactory<ObjectFactoryTestCommand>();
			var test = await factory.ExecuteAsync(factory.Create());
			test.Data.Should().Be("done");
		}

		[Fact]
		public void GetGlobalContext()
		{
			var factory = new ObjectFactory<ObjectFactoryTestCommand>();
			factory.GlobalContext.Should().BeSameAs(ApplicationContext.GlobalContext);
		}

		[Fact]
		public void Update()
		{
			new Action(() =>
			{
				var factory = new ObjectFactory<ObjectFactoryTest>();
				var test = factory.Fetch();
				factory.Update(test);
			}).ShouldNotThrow();
		}

		[Fact]
		public void UpdateAsync()
		{
			var func = new Func<Task>(async () =>
			{
				var factory = new ObjectFactory<ObjectFactoryTest>();
				var test = factory.Fetch();
				await factory.UpdateAsync(test);
			});

			func.ShouldNotThrow();
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

		public static readonly PropertyInfo<string> DataProperty =
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

		public static readonly PropertyInfo<string> DataProperty =
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