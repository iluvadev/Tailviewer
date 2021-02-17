﻿using Tailviewer.Core.Columns;
using Tailviewer.Settings;

namespace Tailviewer.Ui.Controls.LogView.Messages
{
	public sealed class MessageColumnPresenter
		: AbstractLogColumnPresenter<string>
	{
		public MessageColumnPresenter(TextSettings textSettings)
			: base(LogColumns.Message, textSettings)
		{}

		#region Overrides of AbstractLogColumnPresenter<string>

		protected override void UpdateWidth(ILogSource logSource, TextSettings textSettings)
		{
			// TODO: What do we do here? This column shall simply take all the space necessary...
		}

		protected override AbstractLogEntryValueFormatter CreateFormatter(string value)
		{
			return new MessageFormatter(value, TextSettings);
		}

		#endregion
	}
}
