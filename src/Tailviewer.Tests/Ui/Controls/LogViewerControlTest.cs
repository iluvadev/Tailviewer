﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Threading;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Tailviewer.Api;
using Tailviewer.BusinessLogic.ActionCenter;
using Tailviewer.BusinessLogic.DataSources;
using Tailviewer.Core;
using Tailviewer.Settings;
using Tailviewer.Ui.DataSourceTree;
using Tailviewer.Ui.LogView;

namespace Tailviewer.Tests.Ui.Controls
{
	[TestFixture]
	[Apartment(ApartmentState.STA)]
	public sealed class LogViewerControlTest
	{
		private Mock<IActionCenter> _actionCenter;
		private LogViewerControl _control;
		private FileDataSourceViewModel _dataSource;
		private ILogSourceFactory _logSourceFactory;
		private ManualTaskScheduler _scheduler;
		private Mock<IApplicationSettings> _settings;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_scheduler = new ManualTaskScheduler();
			_logSourceFactory = new SimplePluginLogSourceFactory(_scheduler);
			_actionCenter = new Mock<IActionCenter>();
		}

		[SetUp]
		public void SetUp()
		{
			_settings = new Mock<IApplicationSettings>();
			_dataSource =
				CreateViewModel(
				                new FileDataSource(_logSourceFactory, _scheduler, new DataSource("Foobar") {Id = DataSourceId.CreateNew()}));
			_control = new LogViewerControl
			{
				DataSource = _dataSource,
				Width = 1024,
				Height = 768
			};

			DispatcherExtensions.ExecuteAllEvents();
		}

		[Pure]
		private FileDataSourceViewModel CreateViewModel(FileDataSource dataSource)
		{
			return new FileDataSourceViewModel(dataSource, _actionCenter.Object, _settings.Object);
		}

		private Mock<IDataSourceViewModel> CreateDataSourceViewModel()
		{
			var dataSource = new Mock<IDataSourceViewModel>();
			var search = new Mock<ISearchViewModel>();
			dataSource.Setup(x => x.Search).Returns(search.Object);
			return dataSource;
		}

		private Mock<IMergedDataSourceViewModel> CreateMergedDataSourceViewModel()
		{
			var dataSource = new Mock<IMergedDataSourceViewModel>();
			var search = new Mock<ISearchViewModel>();
			dataSource.Setup(x => x.Search).Returns(search.Object);
			return dataSource;
		}
		
		[Test]
		[Ignore("Doesn't work yet")]
		[NUnit.Framework.Description(
			                            "Verifies that upon setting the data source, the FollowTail property is forwarded to the LogEntryListView")]
		public void TestChangeDataSource1()
		{
			var dataSource = CreateDataSourceViewModel();
			dataSource.Setup(x => x.FollowTail).Returns(true);

			_control.DataSource = dataSource.Object;
			_control.PartListView.FollowTail.Should().BeTrue();
		}

		[Test]
		[NUnit.Framework.Description("Verifies that the ShowLineNumbers value on the new data source is used")]
		public void TestChangeDataSource2([Values(true, false)] bool showLineNumbers)
		{
			var dataSource1 = CreateDataSourceViewModel();
			dataSource1.Setup(x => x.ShowLineNumbers).Returns(showLineNumbers);

			_control.DataSource = dataSource1.Object;
			_control.PART_ListView.ShowLineNumbers.Should().Be(showLineNumbers);
			_control.PART_FindAllView.ShowLineNumbers.Should().Be(showLineNumbers);


			var dataSource2 = CreateDataSourceViewModel();
			dataSource2.Setup(x => x.ShowLineNumbers).Returns(!showLineNumbers);
			_control.DataSource = dataSource2.Object;
			_control.PART_ListView.ShowLineNumbers.Should().Be(!showLineNumbers);
			_control.PART_FindAllView.ShowLineNumbers.Should().Be(!showLineNumbers);
		}

		[Test]
		[NUnit.Framework.Description("Verifies that changes to the ShowLineNumbers property are propagated")]
		public void TestDataSourceChangeShowLineNumbers([Values(true, false)] bool showLineNumbers)
		{
			var dataSource = CreateDataSourceViewModel();
			dataSource.Setup(x => x.ShowLineNumbers).Returns(!showLineNumbers);

			_control.DataSource = dataSource.Object;

			dataSource.Setup(x => x.ShowLineNumbers).Returns(showLineNumbers);
			dataSource.Raise( x => x.PropertyChanged += null, new PropertyChangedEventArgs(nameof(IDataSourceViewModel.ShowLineNumbers)));
			_control.PART_ListView.ShowLineNumbers.Should().Be(showLineNumbers);
			_control.PART_FindAllView.ShowLineNumbers.Should().Be(showLineNumbers);
		}

		[Test]
		[NUnit.Framework.Description("Verifies that the ShowDeltaTime value on the new data source is used")]
		public void TestChangeDataSource3([Values(true, false)] bool showDeltaTimes)
		{
			var dataSource1 = CreateDataSourceViewModel();
			dataSource1.Setup(x => x.ShowDeltaTimes).Returns(showDeltaTimes);

			_control.DataSource = dataSource1.Object;
			_control.PART_ListView.ShowDeltaTimes.Should().Be(showDeltaTimes);
			_control.PART_FindAllView.ShowDeltaTimes.Should().Be(showDeltaTimes);


			var dataSource2 = CreateDataSourceViewModel();
			dataSource2.Setup(x => x.ShowDeltaTimes).Returns(!showDeltaTimes);
			_control.DataSource = dataSource2.Object;
			_control.PART_ListView.ShowDeltaTimes.Should().Be(!showDeltaTimes);
			_control.PART_FindAllView.ShowDeltaTimes.Should().Be(!showDeltaTimes);
		}

		[Test]
		[NUnit.Framework.Description("Verifies that changes to the ShowDeltaTime property are propagated")]
		public void TestDataSourceChangeShowDeltaTimes([Values(true, false)] bool showDeltaTimes)
		{
			var dataSource = CreateDataSourceViewModel();
			dataSource.Setup(x => x.ShowDeltaTimes).Returns(!showDeltaTimes);

			_control.DataSource = dataSource.Object;

			dataSource.Setup(x => x.ShowDeltaTimes).Returns(showDeltaTimes);
			dataSource.Raise( x => x.PropertyChanged += null, new PropertyChangedEventArgs(nameof(IDataSourceViewModel.ShowDeltaTimes)));
			_control.PART_ListView.ShowDeltaTimes.Should().Be(showDeltaTimes);
			_control.PART_FindAllView.ShowDeltaTimes.Should().Be(showDeltaTimes);
		}

		[Test]
		[NUnit.Framework.Description("Verifies that the ShowElapsedTime value on the new data source is used")]
		public void TestChangeDataSource4([Values(true, false)] bool showElapsedTime)
		{
			var dataSource1 = CreateDataSourceViewModel();
			dataSource1.Setup(x => x.ShowElapsedTime).Returns(showElapsedTime);

			_control.DataSource = dataSource1.Object;
			_control.PART_ListView.ShowElapsedTime.Should().Be(showElapsedTime);
			_control.PART_FindAllView.ShowElapsedTime.Should().Be(showElapsedTime);


			var dataSource2 = CreateDataSourceViewModel();
			dataSource2.Setup(x => x.ShowElapsedTime).Returns(!showElapsedTime);
			_control.DataSource = dataSource2.Object;
			_control.PART_ListView.ShowElapsedTime.Should().Be(!showElapsedTime);
			_control.PART_FindAllView.ShowElapsedTime.Should().Be(!showElapsedTime);
		}

		[Test]
		[NUnit.Framework.Description("Verifies that changes to the ShowElapsedTime property are propagated")]
		public void TestDataSourceChangeShowElapsedTimes([Values(true, false)] bool showElapsedTime)
		{
			var dataSource = CreateDataSourceViewModel();
			dataSource.Setup(x => x.ShowElapsedTime).Returns(!showElapsedTime);

			_control.DataSource = dataSource.Object;

			dataSource.Setup(x => x.ShowElapsedTime).Returns(showElapsedTime);
			dataSource.Raise( x => x.PropertyChanged += null, new PropertyChangedEventArgs(nameof(IDataSourceViewModel.ShowElapsedTime)));
			_control.PART_ListView.ShowElapsedTime.Should().Be(showElapsedTime);
			_control.PART_FindAllView.ShowElapsedTime.Should().Be(showElapsedTime);
		}

		[Test]
		[NUnit.Framework.Description("Verifies that the ColorByLevel value on the new data source is used")]
		public void TestChangeDataSource5([Values(true, false)] bool colorByLevel)
		{
			var dataSource1 = CreateDataSourceViewModel();
			dataSource1.Setup(x => x.ColorByLevel).Returns(colorByLevel);

			_control.DataSource = dataSource1.Object;
			_control.PART_ListView.ColorByLevel.Should().Be(colorByLevel);
			_control.PART_FindAllView.ColorByLevel.Should().Be(colorByLevel);


			var dataSource2 = CreateDataSourceViewModel();
			dataSource2.Setup(x => x.ColorByLevel).Returns(!colorByLevel);
			_control.DataSource = dataSource2.Object;
			_control.PART_ListView.ColorByLevel.Should().Be(!colorByLevel);
			_control.PART_FindAllView.ColorByLevel.Should().Be(!colorByLevel);
		}

		[Test]
		[NUnit.Framework.Description("Verifies that changes to the ColorByLevel property are propagated")]
		public void TestDataSourceChangeColorByLevel([Values(true, false)] bool colorByLevel)
		{
			var dataSource = CreateDataSourceViewModel();
			dataSource.Setup(x => x.ColorByLevel).Returns(!colorByLevel);

			_control.DataSource = dataSource.Object;

			dataSource.Setup(x => x.ColorByLevel).Returns(colorByLevel);
			dataSource.Raise( x => x.PropertyChanged += null, new PropertyChangedEventArgs(nameof(IDataSourceViewModel.ColorByLevel)));
			_control.PART_ListView.ColorByLevel.Should().Be(colorByLevel);
			_control.PART_FindAllView.ColorByLevel.Should().Be(colorByLevel);
		}

		[Test]
		public void TestChangeLevelAll()
		{
			_control.DataSource.LevelsFilter = LevelFlags.All;
			_control.ShowTrace.Should().BeTrue();
			_control.ShowDebug.Should().BeTrue();
			_control.ShowInfo.Should().BeTrue();
			_control.ShowWarning.Should().BeTrue();
			_control.ShowError.Should().BeTrue();
			_control.ShowFatal.Should().BeTrue();
		}

		[Test]
		public void TestChangeLevelDebug()
		{
			_control.DataSource.LevelsFilter = LevelFlags.Debug;
			_control.ShowTrace.Should().BeFalse();
			_control.ShowDebug.Should().BeTrue();
			_control.ShowInfo.Should().BeFalse();
			_control.ShowWarning.Should().BeFalse();
			_control.ShowError.Should().BeFalse();
			_control.ShowFatal.Should().BeFalse();
		}

		[Test]
		public void TestChangeLevelError()
		{
			_control.DataSource.LevelsFilter = LevelFlags.Error;
			_control.ShowTrace.Should().BeFalse();
			_control.ShowDebug.Should().BeFalse();
			_control.ShowInfo.Should().BeFalse();
			_control.ShowWarning.Should().BeFalse();
			_control.ShowError.Should().BeTrue();
			_control.ShowFatal.Should().BeFalse();
		}

		[Test]
		public void TestChangeLevelFatal()
		{
			_control.DataSource.LevelsFilter = LevelFlags.Fatal;
			_control.ShowTrace.Should().BeFalse();
			_control.ShowDebug.Should().BeFalse();
			_control.ShowInfo.Should().BeFalse();
			_control.ShowWarning.Should().BeFalse();
			_control.ShowError.Should().BeFalse();
			_control.ShowFatal.Should().BeTrue();
		}

		[Test]
		public void TestChangeLevelInfo()
		{
			_control.DataSource.LevelsFilter = LevelFlags.Info;
			_control.ShowTrace.Should().BeFalse();
			_control.ShowDebug.Should().BeFalse();
			_control.ShowInfo.Should().BeTrue();
			_control.ShowWarning.Should().BeFalse();
			_control.ShowError.Should().BeFalse();
			_control.ShowFatal.Should().BeFalse();
		}

		[Test]
		public void TestChangeLevelWarning()
		{
			_control.DataSource.LevelsFilter = LevelFlags.Warning;
			_control.ShowTrace.Should().BeFalse();
			_control.ShowDebug.Should().BeFalse();
			_control.ShowInfo.Should().BeFalse();
			_control.ShowWarning.Should().BeTrue();
			_control.ShowError.Should().BeFalse();
			_control.ShowFatal.Should().BeFalse();
		}

		[Test]
		public void TestChangeLevelTrace()
		{
			_control.DataSource.LevelsFilter = LevelFlags.Trace;
			_control.ShowTrace.Should().BeTrue();
			_control.ShowDebug.Should().BeFalse();
			_control.ShowInfo.Should().BeFalse();
			_control.ShowWarning.Should().BeFalse();
			_control.ShowError.Should().BeFalse();
			_control.ShowFatal.Should().BeFalse();
		}

		[Test]
		[NUnit.Framework.Description("Verifies that changing the LogView does NOT change the currently visible line of the old view")]
		public void TestChangeLogView1()
		{
			// TODO: This test requires that the template be fully loaded (or the control changed to a user control)

			var oldLog = CreateDataSourceViewModel();
			var oldDataSource = new Mock<IDataSource>();
			var oldLogFile = new Mock<ILogSource>();
			oldLogFile.Setup(x => x.GetProperty(Properties.LogEntryCount)).Returns(10000);
			oldDataSource.Setup(x => x.FilteredLogSource).Returns(oldLogFile.Object);
			oldDataSource.Setup(x => x.UnfilteredLogSource).Returns(new Mock<ILogSource>().Object);
			oldLog.Setup(x => x.DataSource).Returns(oldDataSource.Object);
			oldLog.SetupProperty(x => x.VisibleLogLine);
			oldLog.Object.VisibleLogLine = 42;
			var oldLogView = new LogViewerViewModel(oldLog.Object, _actionCenter.Object, _settings.Object);

			_control.LogView = oldLogView;


			var newLog = CreateDataSourceViewModel();
			var newDataSource = new Mock<IDataSource>();
			newDataSource.Setup(x => x.FilteredLogSource).Returns(new Mock<ILogSource>().Object);
			newDataSource.Setup(x => x.UnfilteredLogSource).Returns(new Mock<ILogSource>().Object);
			newLog.Setup(x => x.DataSource).Returns(newDataSource.Object);
			newLog.SetupProperty(x => x.VisibleLogLine);
			newLog.Object.VisibleLogLine = 1;
			var newLogView = new LogViewerViewModel(newLog.Object, _actionCenter.Object, _settings.Object);
			_control.LogView = newLogView;

			oldLog.Object.VisibleLogLine.Should()
				.Be(42, "Because the control shouldn't have changed the VisibleLogLine of the old logview");
			newLog.Object.VisibleLogLine.Should()
				.Be(1, "Because the control shouldn't have changed the VisibleLogLine of the old logview");
		}

		[Test]
		[NUnit.Framework.Description(
			                            "Verifies that the VisibleLogLine of a data source is properly propagated through all controls when the data source is changed"
		                            )]
		public void TestChangeLogView2()
		{
			var dataSource = new Mock<IDataSource>();
			var logFile = new Mock<ILogSource>();
			logFile.Setup(x => x.GetProperty(Properties.LogEntryCount)).Returns(100);
			dataSource.Setup(x => x.UnfilteredLogSource).Returns(logFile.Object);
			dataSource.Setup(x => x.FilteredLogSource).Returns(logFile.Object);
			var dataSourceViewModel = CreateDataSourceViewModel();
			dataSourceViewModel.Setup(x => x.DataSource).Returns(dataSource.Object);
			dataSourceViewModel.Setup(x => x.VisibleLogLine).Returns(new LogLineIndex(42));
			var logView = new LogViewerViewModel(dataSourceViewModel.Object, _actionCenter.Object, _settings.Object);

			_control.LogView = logView;
			_control.CurrentLogLine.Should().Be(42);
			_control.PartListView.CurrentLine.Should().Be(42);
			_control.PartListView.PartTextCanvas.CurrentLine.Should().Be(42);
		}

		[Test]
		[NUnit.Framework.Description("Verifies that when a new data source is attached, its Selection is used")]
		public void TestChangeLogView3()
		{
			_control.DataSource = null;

			var dataSource = new Mock<IDataSource>();
			var logFile = new Mock<ILogSource>();
			logFile.Setup(x => x.GetProperty(Properties.LogEntryCount)).Returns(100);
			dataSource.Setup(x => x.UnfilteredLogSource).Returns(logFile.Object);
			dataSource.Setup(x => x.FilteredLogSource).Returns(logFile.Object);
			var dataSourceViewModel = CreateDataSourceViewModel();
			dataSourceViewModel.Setup(x => x.DataSource).Returns(dataSource.Object);
			dataSourceViewModel.Setup(x => x.SelectedLogLines).Returns(new HashSet<LogLineIndex> {new LogLineIndex(42)});
			var logView = new LogViewerViewModel(dataSourceViewModel.Object, _actionCenter.Object, _settings.Object);

			_control.LogView = logView;
			_control.SelectedIndices.Should().Equal(new LogLineIndex(42));
		}

		[Test]
		[NUnit.Framework.Description(
			                            "Verifies that when a new data source is attached, the string filter of the new source is immediately used for highlighting"
		                            )]
		public void TestChangeLogView4()
		{
			_control.DataSource = null;

			var dataSource = new Mock<IDataSource>();
			var logFile = new Mock<ILogSource>();
			logFile.Setup(x => x.GetProperty(Properties.LogEntryCount)).Returns(100);
			dataSource.Setup(x => x.UnfilteredLogSource).Returns(logFile.Object);
			dataSource.Setup(x => x.FilteredLogSource).Returns(logFile.Object);
			var dataSourceViewModel = CreateDataSourceViewModel();
			dataSourceViewModel.Setup(x => x.DataSource).Returns(dataSource.Object);
			dataSourceViewModel.Setup(x => x.SelectedLogLines).Returns(new HashSet<LogLineIndex> {new LogLineIndex(42)});
			var searchViewModel = new Mock<ISearchViewModel>();
			searchViewModel.Setup(x => x.Term).Returns("Foobar");
			dataSourceViewModel.Setup(x => x.Search).Returns(searchViewModel.Object);
			var logView = new LogViewerViewModel(dataSourceViewModel.Object, _actionCenter.Object, _settings.Object);

			_control.LogView = logView;
			_control.SelectedIndices.Should().Equal(new LogLineIndex(42));
		}

		[Test]
		public void TestChangeSelection1()
		{
			_dataSource.SelectedLogLines.Should().BeEmpty();
			_control.Select(new LogLineIndex(1));
			_dataSource.SelectedLogLines.Should().Equal(new LogLineIndex(1));
		}

		[Test]
		public void TestChangeShowOther()
		{
			_control.DataSource.LevelsFilter = LevelFlags.None;
			_control.ShowOther.Should().BeFalse();

			_control.ShowOther = true;
			_control.DataSource.LevelsFilter.Should().Be(LevelFlags.Other);

			_control.ShowOther = false;
			_control.DataSource.LevelsFilter.Should().Be(LevelFlags.None);
		}

		[Test]
		public void TestChangeShowDebug()
		{
			_control.DataSource.LevelsFilter = LevelFlags.None;
			_control.ShowDebug.Should().BeFalse();

			_control.ShowDebug = true;
			_control.DataSource.LevelsFilter.Should().Be(LevelFlags.Debug);

			_control.ShowDebug = false;
			_control.DataSource.LevelsFilter.Should().Be(LevelFlags.None);
		}

		[Test]
		public void TestChangeShowError()
		{
			_control.DataSource.LevelsFilter = LevelFlags.None;
			_control.ShowError.Should().BeFalse();

			_control.ShowError = true;
			_control.DataSource.LevelsFilter.Should().Be(LevelFlags.Error);

			_control.ShowError = false;
			_control.DataSource.LevelsFilter.Should().Be(LevelFlags.None);
		}

		[Test]
		public void TestChangeShowFatal()
		{
			_control.DataSource.LevelsFilter = LevelFlags.None;
			_control.ShowFatal.Should().BeFalse();

			_control.ShowFatal = true;
			_control.DataSource.LevelsFilter.Should().Be(LevelFlags.Fatal);

			_control.ShowFatal = false;
			_control.DataSource.LevelsFilter.Should().Be(LevelFlags.None);
		}

		[Test]
		public void TestChangeShowInfo()
		{
			_control.DataSource.LevelsFilter = LevelFlags.None;
			_control.ShowInfo.Should().BeFalse();

			_control.ShowInfo = true;
			_control.DataSource.LevelsFilter.Should().Be(LevelFlags.Info);

			_control.ShowInfo = false;
			_control.DataSource.LevelsFilter.Should().Be(LevelFlags.None);
		}

		[Test]
		public void TestChangeShowWarning()
		{
			_control.DataSource.LevelsFilter = LevelFlags.None;
			_control.ShowWarning.Should().BeFalse();

			_control.ShowWarning = true;
			_control.DataSource.LevelsFilter.Should().Be(LevelFlags.Warning);

			_control.ShowWarning = false;
			_control.DataSource.LevelsFilter.Should().Be(LevelFlags.None);
		}

		[Test]
		public void TestCtor()
		{
			var source =
				CreateViewModel(
					new FileDataSource(_logSourceFactory, _scheduler, new DataSource("Foobar") {Id = DataSourceId.CreateNew()}));
			source.LevelsFilter = LevelFlags.All;

			var control = new LogViewerControl
			{
				DataSource = source
			};
			control.ShowOther.Should().BeTrue();
			control.ShowTrace.Should().BeTrue();
			control.ShowDebug.Should().BeTrue();
			control.ShowInfo.Should().BeTrue();
			control.ShowWarning.Should().BeTrue();
			control.ShowError.Should().BeTrue();
			control.ShowFatal.Should().BeTrue();
		}

		[Test]
		public void TestShowAll1()
		{
			_control.ShowAll = true;
			_control.ShowOther.Should().BeTrue();
			_control.ShowTrace.Should().BeTrue();
			_control.ShowDebug.Should().BeTrue();
			_control.ShowInfo.Should().BeTrue();
			_control.ShowWarning.Should().BeTrue();
			_control.ShowError.Should().BeTrue();
			_control.ShowFatal.Should().BeTrue();
		}

		[Test]
		public void TestShowAll2()
		{
			_control.ShowOther = true;
			_control.ShowTrace = true;
			_control.ShowDebug = true;
			_control.ShowInfo = true;
			_control.ShowWarning = true;
			_control.ShowError = true;
			_control.ShowFatal = true;
			_control.ShowAll = false;

			_control.ShowOther.Should().BeFalse();
			_control.ShowTrace.Should().BeFalse();
			_control.ShowDebug.Should().BeFalse();
			_control.ShowInfo.Should().BeFalse();
			_control.ShowWarning.Should().BeFalse();
			_control.ShowError.Should().BeFalse();
			_control.ShowFatal.Should().BeFalse();
		}

		[Test]
		public void TestShowAll3()
		{
			_control.ShowAll = true;
			_control.ShowDebug = false;
			_control.ShowAll.Should().NotHaveValue();

			_control.ShowOther.Should().BeTrue();
			_control.ShowTrace.Should().BeTrue();
			_control.ShowDebug.Should().BeFalse();
			_control.ShowInfo.Should().BeTrue();
			_control.ShowWarning.Should().BeTrue();
			_control.ShowError.Should().BeTrue();
			_control.ShowFatal.Should().BeTrue();
		}

		[Test]
		public void TestShowAll4()
		{
			_control.ShowAll = false;
			_control.ShowDebug = true;
			_control.ShowAll.Should().NotHaveValue();

			_control.ShowOther.Should().BeFalse();
			_control.ShowTrace.Should().BeFalse();
			_control.ShowDebug.Should().BeTrue();
			_control.ShowInfo.Should().BeFalse();
			_control.ShowWarning.Should().BeFalse();
			_control.ShowError.Should().BeFalse();
			_control.ShowFatal.Should().BeFalse();
		}

		[Test]
		public void TestShowAll5()
		{
			_control.ShowAll = false;
			_control.ShowAll.Should().BeFalse();

			_control.ShowOther = true;
			_control.ShowAll.Should().NotHaveValue();

			_control.ShowTrace = true;
			_control.ShowAll.Should().NotHaveValue();

			_control.ShowDebug = true;
			_control.ShowAll.Should().NotHaveValue();

			_control.ShowInfo = true;
			_control.ShowAll.Should().NotHaveValue();

			_control.ShowWarning = true;
			_control.ShowAll.Should().NotHaveValue();

			_control.ShowError = true;
			_control.ShowAll.Should().NotHaveValue();

			_control.ShowFatal = true;
			_control.ShowAll.Should().BeTrue();
		}

		[Test]
		[NUnit.Framework.Description(
			                            "Verifies that when the data source's selected log lines change, then the control synchronizes itself properly")]
		public void TestChangeSelectedLogLines()
		{
			_dataSource.SelectedLogLines = new HashSet<LogLineIndex>
			{
				new LogLineIndex(42),
				new LogLineIndex(108)
			};
			_control.SelectedIndices.Should().BeEquivalentTo(new[]
			{
				new LogLineIndex(42),
				new LogLineIndex(108)
			}, "because the control is expected to listen to changes of the data source");
		}

		[Test]
		[NUnit.Framework.Description(
			                            "Verifies that when the data source's visible log line changes, then the control synchronizes itself properly")]
		public void TestChangeVisibleLogLine()
		{
			_dataSource.VisibleLogLine = new LogLineIndex(9001);
			_control.CurrentLogLine.Should()
				.Be(new LogLineIndex(9001), "because the control is expected to listen to changes of the data source");
		}

		[Test]
		[NUnit.Framework.Description(
			                            "Verifies that changes to the MergedDataSourceDisplayMode property are forwarded to the data source view model")]
		public void TestChangeMergedDataSourceDisplayMode1()
		{
			var dataSource = CreateMergedDataSourceViewModel();
			dataSource.SetupProperty(x => x.DisplayMode);

			_control.DataSource = dataSource.Object;

			_control.MergedDataSourceDisplayMode = DataSourceDisplayMode.CharacterCode;
			dataSource.Object.DisplayMode.Should().Be(DataSourceDisplayMode.CharacterCode);

			_control.MergedDataSourceDisplayMode = DataSourceDisplayMode.Filename;
			dataSource.Object.DisplayMode.Should().Be(DataSourceDisplayMode.Filename);
		}

		[Test]
		[NUnit.Framework.Description(
			                            "Verifies that changes to the MergedDataSourceDisplayMode property are ignored if the view model isn't a merged one")
		]
		public void TestChangeMergedDataSourceDisplayMode2()
		{
			var dataSource = CreateMergedDataSourceViewModel();

			_control.DataSource = dataSource.Object;

			new Action(() => _control.MergedDataSourceDisplayMode = DataSourceDisplayMode.CharacterCode).Should().NotThrow();
			new Action(() => _control.MergedDataSourceDisplayMode = DataSourceDisplayMode.Filename).Should().NotThrow();
		}

		[Test]
		[NUnit.Framework.Description("Verifies that the display mode of the new data source is used")]
		public void TestChangeDataSource(
			[Values(DataSourceDisplayMode.Filename, DataSourceDisplayMode.CharacterCode)] DataSourceDisplayMode displayMode)
		{
			var dataSource = new Mock<IMergedDataSourceViewModel>();
			dataSource.SetupProperty(x => x.DisplayMode);
			var search = new Mock<ISearchViewModel>();
			dataSource.Setup(x => x.Search).Returns(search.Object);
			dataSource.Object.DisplayMode = displayMode;

			_control.DataSource = dataSource.Object;
			_control.MergedDataSourceDisplayMode.Should()
				.Be(displayMode,
					"because the view model determines the initial display mode and thus the control should just use that");
			dataSource.Object.DisplayMode.Should()
				.Be(displayMode, "because the display mode of the view model shouldn't have been changed in the process");
		}

		[Test]
		public void TestChangeMergedDisplayMode()
		{
			var dataSource = new Mock<IMergedDataSourceViewModel>();
			dataSource.Setup(x => x.Search).Returns(new Mock<ISearchViewModel>().Object);
			dataSource.SetupProperty(x => x.DisplayMode);
			dataSource.Object.DisplayMode = DataSourceDisplayMode.Filename;

			_control.DataSource = dataSource.Object;
			_control.MergedDataSourceDisplayMode.Should().Be(DataSourceDisplayMode.Filename);

			dataSource.Object.DisplayMode = DataSourceDisplayMode.CharacterCode;
			dataSource.Raise(x => x.PropertyChanged += null, dataSource.Object, new PropertyChangedEventArgs(nameof(IMergedDataSourceViewModel.DisplayMode)));
			_control.MergedDataSourceDisplayMode.Should().Be(DataSourceDisplayMode.CharacterCode);
		}

		[Test]
		[NUnit.Framework.Description("Verifies that the settings are simply forwarded to the LogEntryListView")]
		public void TestChangeSettings()
		{
			var settings = new LogViewerSettings();

			_control.Settings = settings;
			_control.PART_ListView.Settings.Should().BeSameAs(settings);
		}
	}
}