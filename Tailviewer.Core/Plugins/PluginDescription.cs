using System;
using System.Collections.Generic;

namespace Tailviewer.Core.Plugins
{
	/// <summary>
	///     Describes a plugin that is located inside a .NET assembly.
	/// </summary>
	public sealed class PluginDescription
		: IPluginDescription
	{
		/// <inheritdoc />
		public string FilePath { get; set; }

		/// <inheritdoc />
		public string Author { get; set; }

		/// <inheritdoc />
		public string Description { get; set; }

		/// <inheritdoc />
		public Uri Website { get; set; }

		/// <inheritdoc />
		public IReadOnlyDictionary<Type, string> Plugins { get; set; }
	}
}