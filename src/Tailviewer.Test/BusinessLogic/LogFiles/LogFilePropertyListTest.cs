﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Tailviewer.BusinessLogic;
using Tailviewer.Core.Properties;

namespace Tailviewer.Test.BusinessLogic.LogFiles
{
	[TestFixture]
	public sealed class LogFilePropertyListTest
		: AbstractLogFilePropertiesTest
	{
		[Test]
		public void TestConstruction()
		{
			var properties = new PropertiesBufferList(GeneralProperties.Created);
			properties.GetValue(GeneralProperties.Created).Should().Be(GeneralProperties.Created.DefaultValue);
		}

		[Test]
		public void TestAdd()
		{
			var properties = new PropertiesBufferList();
			properties.Properties.Should().BeEmpty();

			properties.Add(GeneralProperties.Created);
			properties.Properties.Should().Equal(new object[]{GeneralProperties.Created});

			properties.SetValue(GeneralProperties.Created, new DateTime(2021, 02, 14, 12, 13, 01));
			new Action(()=> properties.Add(GeneralProperties.Created)).Should().NotThrow("because adding properties again should be tolerate and just not do anything");
			properties.Properties.Should().Equal(new object[] {GeneralProperties.Created});
			properties.GetValue(GeneralProperties.Created).Should().Be(new DateTime(2021, 02, 14, 12, 13, 01));
		}

		[Test]
		[Description("Verifies that values can be overwritten")]
		public void TestSetValue5()
		{
			var properties = new PropertiesBufferList();
			properties.SetValue(GeneralProperties.EmptyReason, ErrorFlags.SourceDoesNotExist);
			properties.GetValue(GeneralProperties.EmptyReason).Should().Be(ErrorFlags.SourceDoesNotExist);

			properties.SetValue(GeneralProperties.EmptyReason, ErrorFlags.SourceCannotBeAccessed);
			properties.GetValue(GeneralProperties.EmptyReason).Should().Be(ErrorFlags.SourceCannotBeAccessed);
		}

		[Test]
		[Description("Verifies that values can be overwritten")]
		public void TestSetValue6()
		{
			var properties = new PropertiesBufferList();
			properties.SetValue((IReadOnlyPropertyDescriptor)GeneralProperties.EmptyReason, ErrorFlags.SourceDoesNotExist);
			properties.GetValue(GeneralProperties.EmptyReason).Should().Be(ErrorFlags.SourceDoesNotExist);

			properties.SetValue((IReadOnlyPropertyDescriptor)GeneralProperties.EmptyReason, ErrorFlags.SourceCannotBeAccessed);
			properties.GetValue(GeneralProperties.EmptyReason).Should().Be(ErrorFlags.SourceCannotBeAccessed);
		}

		[Test]
		[Description("Verifies that values are reset to the default value as specified by their property")]
		public void TestReset()
		{
			var properties = new PropertiesBufferList();
			properties.SetValue(GeneralProperties.EmptyReason, ErrorFlags.SourceCannotBeAccessed);
			properties.SetValue(GeneralProperties.PercentageProcessed, Percentage.Of(50, 100));

			properties.Reset();
			properties.GetValue(GeneralProperties.EmptyReason).Should().Be(GeneralProperties.EmptyReason.DefaultValue);
			properties.GetValue(GeneralProperties.PercentageProcessed).Should().Be(Percentage.Zero);
		}

		[Test]
		[Description("Verifies that all properties are removed from the list")]
		public void TestClear()
		{
			var properties = new PropertiesBufferList();
			properties.SetValue(GeneralProperties.EmptyReason, ErrorFlags.SourceCannotBeAccessed);
			properties.SetValue(GeneralProperties.PercentageProcessed, Percentage.Of(50, 100));

			properties.Clear();
			properties.Properties.Should().BeEmpty();
			properties.GetValue(GeneralProperties.EmptyReason).Should().Be(GeneralProperties.EmptyReason.DefaultValue);
			properties.GetValue(GeneralProperties.PercentageProcessed).Should().Be(GeneralProperties.PercentageProcessed.DefaultValue);

			properties.TryGetValue(GeneralProperties.EmptyReason, out _).Should().BeFalse();
			properties.TryGetValue(GeneralProperties.PercentageProcessed, out _).Should().BeFalse();
		}

		protected override IPropertiesBuffer Create(params KeyValuePair<IReadOnlyPropertyDescriptor, object>[] properties)
		{
			var list = new PropertiesBufferList(properties.Select(x => x.Key).ToArray());
			foreach (var pair in properties)
			{
				list.SetValue(pair.Key, pair.Value);
			}

			return list;
		}
	}
}