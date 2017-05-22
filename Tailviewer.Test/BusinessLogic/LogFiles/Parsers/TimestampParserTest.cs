﻿using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Tailviewer.BusinessLogic;
using Tailviewer.BusinessLogic.LogFiles.Parsers;

namespace Tailviewer.Test.BusinessLogic.LogFiles.Parsers
{
	[TestFixture]
	public sealed class TimestampParserTest
	{
		[Test]
		public void TestTryParse1()
		{
			var parser = new TimestampParser();
			DateTime unused;
			parser.TryParse(
					"2015-10-07 19:50:58,982 [8092, 1] INFO  SharpRemote.Hosting.OutOfProcessSiloServer (null) - Silo Server starting, args (1): \"14056\", without custom type resolver",
					out unused)
				.Should()
				.BeTrue();

			parser.DateTimeColumn.Should().Be(0);
			parser.DateTimeLength.Should().Be(23);
			// This is not ideal yet because we don't detect that the next 4 characters is the amount of MS, but it's a start...
		}

		[Test]
		public void TestTryParse2()
		{
			var parser = new TimestampParser();
			DateTime dateTime;
			parser.TryParse("2015-10-07 19:50:58,998",
					out dateTime)
				.Should()
				.BeTrue();

			dateTime.Year.Should().Be(2015);
			dateTime.Month.Should().Be(10);
			dateTime.Day.Should().Be(7);
			dateTime.Hour.Should().Be(19);
			dateTime.Minute.Should().Be(50);
			dateTime.Second.Should().Be(58);
			dateTime.Millisecond.Should().Be(998);
		}

		[Test]
		public void TestTryParse3()
		{
			var parser = new TimestampParser();
			DateTime dateTime;
			parser.TryParse("2016 Feb 17 12:38:59.060754850",
					out dateTime)
				.Should()
				.BeTrue();
			parser.DateTimeColumn.Should().Be(0);
			parser.DateTimeLength.Should().Be(24);

			dateTime.Year.Should().Be(2016);
			dateTime.Month.Should().Be(2);
			dateTime.Day.Should().Be(17);
			dateTime.Hour.Should().Be(12);
			dateTime.Minute.Should().Be(38);
			dateTime.Second.Should().Be(59);
			dateTime.Millisecond.Should().Be(60);
		}

		[Test]
		public void TestTryParse4()
		{
			var parser = new TimestampParser();
			DateTime dateTime;
			parser.TryParse("07/Mar/2004:16:31:48",
				out dateTime);

			parser.DateTimeColumn.Should().Be(0);
			parser.DateTimeLength.Should().Be(20);

			dateTime.Year.Should().Be(2004);
			dateTime.Month.Should().Be(3);
			dateTime.Day.Should().Be(7);
			dateTime.Hour.Should().Be(16);
			dateTime.Minute.Should().Be(31);
			dateTime.Second.Should().Be(48);
			dateTime.Millisecond.Should().Be(0);
		}

		[Test]
		public void TestTryParse5()
		{
			var parser = new TimestampParser();
			DateTime timestamp;
			parser.TryParse(
					"06:51:57 ;      0.135345; Foo size               0; Th  6252(0x186c); Start;MC   14; Id  169= 169[ 0]; Bar; ",
					out timestamp)
				.Should()
				.BeTrue();

			var today = DateTime.Today;
			timestamp.Year.Should().Be(today.Year);
			timestamp.Month.Should().Be(today.Month);
			timestamp.Day.Should().Be(today.Day);
			timestamp.Hour.Should().Be(6);
			timestamp.Minute.Should().Be(51);
			timestamp.Second.Should().Be(57);
			timestamp.Millisecond.Should().Be(135);
		}

		[Test]
		public void TestTryParse6()
		{
			var parser = new TimestampParser();
			DateTime timestamp;
			parser.TryParse(
					"Fri May 05 11:28:24.662 2017; Created.",
					out timestamp)
				.Should()
				.BeTrue();

			timestamp.Year.Should().Be(2017);
			timestamp.Month.Should().Be(5);
			timestamp.Day.Should().Be(5);
			timestamp.Hour.Should().Be(11);
			timestamp.Minute.Should().Be(28);
			timestamp.Second.Should().Be(24);
			timestamp.Millisecond.Should().Be(662);
		}

		[Test]
		[Description("Verifies that exceptions thrown by sub-parsers are caught and handled")]
		public void TestTryParse7()
		{
			var subParser = new Mock<ITimestampParser>();
			var parser = new TimestampParser(subParser.Object);

			DateTime unused;
			subParser.Setup(x => x.TryParse(It.IsAny<string>(), out unused)).Throws<NullReferenceException>();

			new Action(() => parser.TryParse("dawwadw", out unused)).ShouldNotThrow("because the parser is supposed to catch *all* exceptions thrown by buggy sub-parsers");
			subParser.Verify(x => x.TryParse(It.IsAny<string>(), out unused), Times.AtLeastOnce);
		}

		[Test]
		public void TestTryParse8()
		{
			var parser = new TimestampParser();
			DateTime timestamp;
			parser
				.TryParse(
					"2017-03-24 11-45-22.182783; 0; 0;  0; 109;  0; 125;   1;PB_CONTINUE; ; ; 109; 2;   2.30; 0; S.N. 100564: 0.3 sec for:",
					out timestamp)
				.Should()
				.BeTrue();
			timestamp.Should().Be(new DateTime(2017, 3, 24, 11, 45, 22, 182));
		}

		[Test]
		public void TestTryParse9()
		{
			var parser = new TimestampParser();
			DateTime timestamp;
			parser
				.TryParse(
					"Foobar 2017-05-22 18-36-51",
					out timestamp)
				.Should()
				.BeTrue();
			timestamp.Should().Be(new DateTime(2017, 5, 22, 18, 36, 51, 0));
		}

		[Test]
		public void TestTryParse10()
		{
			var parser = new TimestampParser();
			DateTime timestamp;
			parser
				.TryParse(
					"Foobar 2017-05-22 18-36-51.541",
					out timestamp)
				.Should()
				.BeTrue();
			timestamp.Should().Be(new DateTime(2017, 5, 22, 18, 36, 51, 541));
		}
	}
}