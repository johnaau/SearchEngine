using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchEngine
{
	public class SearchResult : ISearchResult
	{
		public double SearchScore { get; set; }

		public virtual string SearchIndex { get; set; }
	}
}
