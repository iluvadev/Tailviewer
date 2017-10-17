﻿using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using Tailviewer.BusinessLogic.DataSources;
using Tailviewer.Core.LogFiles;
using Tailviewer.Settings;
using Tailviewer.Ui.ViewModels;
using DataSources = Tailviewer.BusinessLogic.DataSources.DataSources;

namespace Tailviewer.Test.Ui
{
	[TestFixture]
	public sealed class MergedDataSourceViewModelTest
	{
		[SetUp]
		public void SetUp()
		{
			_settings = new Tailviewer.Settings.DataSources();
			_scheduler = new ManualTaskScheduler();
			_logFileFactory = new PluginLogFileFactory(_scheduler);
			_dataSources = new DataSources(_logFileFactory, _scheduler, _settings);
			
		}

		private DataSources _dataSources;
		private Tailviewer.Settings.DataSources _settings;
		private ManualTaskScheduler _scheduler;
		private ILogFileFactory _logFileFactory;

		[Test]
		public void TestConstruction1()
		{
			var model = new MergedDataSourceViewModel(_dataSources.AddGroup());
			model.IsSelected.Should().BeFalse();
			model.IsExpanded.Should().BeTrue();
		}

		[Test]
		public void TestConstruction2([Values(DataSourceDisplayMode.Filename, DataSourceDisplayMode.CharacterCode)] DataSourceDisplayMode displayMode)
		{
			var dataSource = _dataSources.AddGroup();
			dataSource.DisplayMode = displayMode;

			var model = new MergedDataSourceViewModel(dataSource);
			model.DisplayMode.Should().Be(displayMode);
		}

		[Test]
		public void TestExpand()
		{
			var dataSource = _dataSources.AddGroup();
			var model = new MergedDataSourceViewModel(dataSource);
			model.IsExpanded = false;
			model.IsExpanded.Should().BeFalse();
			dataSource.IsExpanded.Should().BeFalse();

			model.IsExpanded = true;
			model.IsExpanded.Should().BeTrue();
			dataSource.IsExpanded.Should().BeTrue();
		}

		[Test]
		public void TestAddChild1()
		{
			var model = new MergedDataSourceViewModel(_dataSources.AddGroup());
			SingleDataSource source = _dataSources.AddDataSource("foo");
			var sourceViewModel = new SingleDataSourceViewModel(source);
			model.AddChild(sourceViewModel);
			model.Observable.Should().Equal(sourceViewModel);
			sourceViewModel.Parent.Should().BeSameAs(model);
		}

		[Test]
		public void TestAddChild2()
		{
			var dataSource = _dataSources.AddGroup();
			var model = new MergedDataSourceViewModel(dataSource);

			SingleDataSource source = _dataSources.AddDataSource("foo");
			var sourceViewModel = new SingleDataSourceViewModel(source);

			model.AddChild(sourceViewModel);
			sourceViewModel.CharacterCode.Should().Be("A", "because the merged data source is responsible for providing unique character codes");
		}

		[Test]
		public void TestInsertChild1()
		{
			var dataSource = _dataSources.AddGroup();
			var model = new MergedDataSourceViewModel(dataSource);

			SingleDataSource source = _dataSources.AddDataSource("foo");
			var sourceViewModel = new SingleDataSourceViewModel(source);

			model.Insert(0, sourceViewModel);
			sourceViewModel.CharacterCode.Should().Be("A", "because the merged data source is responsible for providing unique character codes");
		}

		[Test]
		public void TestInsertChild2()
		{
			var dataSource = _dataSources.AddGroup();
			var model = new MergedDataSourceViewModel(dataSource);

			var child1 = new SingleDataSourceViewModel(_dataSources.AddDataSource("foo"));
			model.AddChild(child1);
			child1.CharacterCode.Should().Be("A");

			var child2 = new SingleDataSourceViewModel(_dataSources.AddDataSource("bar"));
			model.Insert(0, child2);
			model.Observable.Should().Equal(new object[]
			{
				child2, child1
			});

			const string reason = "because the merged data source is responsible for providing unique character codes";
			child2.CharacterCode.Should().Be("A", reason);
			child1.CharacterCode.Should().Be("B", reason);
		}

		[Test]
		public void TestRemoveChild1()
		{
			var dataSource = _dataSources.AddGroup();
			var model = new MergedDataSourceViewModel(dataSource);

			var child1 = new SingleDataSourceViewModel(_dataSources.AddDataSource("foo"));
			model.AddChild(child1);

			var child2 = new SingleDataSourceViewModel(_dataSources.AddDataSource("bar"));
			model.AddChild(child2);
			model.Observable.Should().Equal(new object[]
			{
				child1, child2
			});

			child1.CharacterCode.Should().Be("A");
			child2.CharacterCode.Should().Be("B");

			model.RemoveChild(child1);
			model.Observable.Should().Equal(new object[] {child2});
			child2.CharacterCode.Should().Be("A");
		}

		[Test]
		public void TestChangeDisplayMode()
		{
			var dataSource = _dataSources.AddGroup();
			var model = new MergedDataSourceViewModel(dataSource);

			model.DisplayMode = DataSourceDisplayMode.CharacterCode;
			dataSource.DisplayMode.Should().Be(DataSourceDisplayMode.CharacterCode);

			model.DisplayMode = DataSourceDisplayMode.Filename;
			dataSource.DisplayMode.Should().Be(DataSourceDisplayMode.Filename);
		}
	}
}