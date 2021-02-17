﻿using System;
using System.Globalization;
using Tailviewer.Core.Columns;
using Tailviewer.Core.Properties;
using Tailviewer.Settings;

namespace Tailviewer.Ui.Controls.LogView.ElapsedTime
{
	public sealed class ElapsedTimeColumnPresenter
		: AbstractLogColumnPresenter<TimeSpan?>
	{
		public ElapsedTimeColumnPresenter(TextSettings textSettings)
			: base(LogColumns.ElapsedTime, textSettings)
		{
		}

		protected override void UpdateWidth(ILogSource logSource, TextSettings textSettings)
		{
			if (logSource != null)
			{
				var culture = CultureInfo.CurrentCulture;
				var maximum = ElapsedTimeFormatter.ToString(logSource.GetProperty(GeneralProperties.EndTimestamp) - logSource.GetProperty(GeneralProperties.StartTimestamp), culture);
				var maximumWidth = textSettings.EstimateWidthUpperLimit(maximum);
				Width = maximumWidth;
			}
			else
			{
				Width = 0;
			}
		}

		protected override AbstractLogEntryValueFormatter CreateFormatter(TimeSpan? value)
		{
			return new ElapsedTimeFormatter(value, TextSettings);
		}
	}
}