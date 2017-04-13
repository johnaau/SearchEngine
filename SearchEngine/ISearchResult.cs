using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchEngine
{
	public interface ISearchResult
	{
		double SearchScore { get; set; }
		string SearchIndex { get; set; }
	}
}
