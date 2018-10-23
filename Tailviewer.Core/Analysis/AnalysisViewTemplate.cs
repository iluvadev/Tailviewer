using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Tailviewer.Core.Analysis
{
	/// <summary>
	///     Represents view-configuration of an analysis.
	/// </summary>
	/// <remarks>
	///     Contains:
	///     - Pages and their layouts
	///     - Widgets per page
	///     - View configuration of every widget
	/// </remarks>
	public sealed class AnalysisViewTemplate
		: IAnalysisViewTemplate
	{
		private readonly List<PageTemplate> _pages;
		private string _name;

		/// <summary>
		///     Initializes this template.
		/// </summary>
		public AnalysisViewTemplate()
		{
			_name = "<Unnamed>";
			_pages = new List<PageTemplate>();
		}

		private AnalysisViewTemplate(IEnumerable<PageTemplate> pages)
		{
			_pages = new List<PageTemplate>(pages);
		}

		/// <inheritdoc />
		public string Name
		{
			get => _name;
			set => _name = value;
		}

		/// <inheritdoc />
		public IEnumerable<IPageTemplate> Pages => _pages;

		/// <inheritdoc />
		public void Serialize(IWriter writer)
		{
			writer.WriteAttribute("Name", _name);
			writer.WriteAttribute("Pages", _pages);
		}

		/// <inheritdoc />
		public void Deserialize(IReader reader)
		{
			reader.TryReadAttribute("Name", out _name);
			reader.TryReadAttribute("Pages", _pages);
		}

		/// <summary>
		///     Adds the given page to this template.
		/// </summary>
		/// <param name="template"></param>
		public void Add(PageTemplate template)
		{
			_pages.Add(template);
		}

		/// <summary>
		///     Removes the given page from this template.
		/// </summary>
		/// <param name="template"></param>
		public void Remove(PageTemplate template)
		{
			_pages.Remove(template);
		}

		/// <summary>
		/// Returns a deep clone of this object.
		/// </summary>
		/// <returns></returns>
		[Pure]
		public AnalysisViewTemplate Clone()
		{
			return new AnalysisViewTemplate(_pages.Select(x => x.Clone()));
		}

		object ICloneable.Clone()
		{
			return Clone();
		}
	}
}