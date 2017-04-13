using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SearchEngine;

namespace SearchEngineExample
{
	public partial class Default : System.Web.UI.Page
	{
		protected Searching _search;

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			ctlPagination.PageNumber = 1;

			LoadResults();
		}

		protected void LoadResults()
		{
			List<MyData> myDataList = new List<MyData>();

			myDataList.Add(new MyData(1, "The quick brown fox jumps over the lazy dog", "An interesting story.", "foxes,dogs"));
			myDataList.Add(new MyData(1, "The quick brown dog jumps over the lazy fox", "An interesting story.", "foxes,dogs"));
			myDataList.Add(new MyData(1, "An interesting title", "The quick brown fox jumps over the lazy dog", "colors,green,dogs"));
			myDataList.Add(new MyData(1, "An interesting title", "An interesting story.", "brown,fox"));
			myDataList.Add(new MyData(1, "An interesting title", "An interesting story.", "keywords"));
			myDataList.Add(new MyData(1, "Another quick brown fox jumps over a lazy dog", "A very interesting story.", "keywords"));

			List<string> userDictionary;

			foreach (MyData myData in myDataList)
			{
				myData.SearchIndex = Indexing.GenerateIndex(out userDictionary, myData.Title, myData.Content, myData.Keywords);
			}

			_search = new Searching(txtQuery.Text);

			var q = from a in myDataList
					select a;

			foreach (string searchWord in _search.SearchWords)
			{
				string thisWord = searchWord;

				q = from a in q
					where a.SearchIndex.Contains("|" + thisWord + "|")
					select a;
			}

			List<MyData> results = q.ToList();

			_search.Scorer(q.ToList());

			ctlPagination.PageSize = 2;
			ctlPagination.RecordCount = results.Count();

			repResults.DataSource = (from a in results
									 orderby a.SearchScore descending
									 select a).Skip((ctlPagination.PageNumber - 1) * ctlPagination.PageSize).Take(ctlPagination.PageSize);
			repResults.DataBind();
		}

		protected void repResults_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
			{
				MyData myData = (MyData)e.Item.DataItem;

				((Literal)e.Item.FindControl("litTitle")).Text = _search.HitHighlight(myData.Title, "<span style=\"color:red\">", "</span>", false);
				((Literal)e.Item.FindControl("litContent")).Text = _search.HitHighlight(myData.Content, "<span style=\"color:red\">", "</span>", false);
				((Literal)e.Item.FindControl("litKeywords")).Text = _search.HitHighlight(myData.Keywords, "<span style=\"color:red\">", "</span>", false);
			}
		}

		protected void ctlPagination_PageNumberChanged(object sender, EventArgs e)
		{
			LoadResults();
		}

		public class MyData : SearchResult
		{
			public int ID { get; set; }
			public string Title { get; set; }
			public string Content { get; set; }
			public string Keywords { get; set; }
			public override string SearchIndex { get; set; }

			public MyData(int id, string title, string content, string keywords)
			{
				this.ID = id;
				this.Title = title;
				this.Content = content;
				this.Keywords = keywords;
			}
		}

	}
}