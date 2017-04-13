using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace SearchEngineExample
{
	public partial class Pagination : System.Web.UI.UserControl
	{
		public event EventHandler PageNumberChanged;

		protected void Page_Init(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				this.PageNumber = 1;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Page.IsPostBack)
			{
				this.PageNumber = ViewState["PageNumber"] == null ? 1 : (int)ViewState["PageNumber"];
				this.RecordCount = ViewState["RecordCount"] == null ? 1 : (int)ViewState["RecordCount"];
				this.PageSize = ViewState["PageSize"] == null ? 1 : (int)ViewState["PageSize"];
			}
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			int pageCount = (int)(Math.Ceiling((double)this.RecordCount / (double)this.PageSize));

			if (pageCount < 2)
			{
				plhPagination.Visible = false;
			}
			else
			{
				plhPagination.Visible = true;

				if (this.PageNumber > 1)
					lnkPrevious.Visible = true;
				else
					lnkPrevious.Visible = false;

				List<int> pageDataSource = new List<int>();

				int start = this.PageNumber > 11 ? this.PageNumber - 10 : 1;
				int end = pageCount < this.PageNumber + 9 ? pageCount : this.PageNumber + 9;


				for (int i = start; i <= end; i++)
					pageDataSource.Add(i);

				if (this.PageNumber > pageCount)
					this.PageNumber = pageCount;

				repPage.DataSource = pageDataSource;
				repPage.DataBind();

				if (this.PageNumber < pageCount)
					lnkNext.Visible = true;
				else
					lnkNext.Visible = false;
			}

			ViewState["PageNumber"] = this.PageNumber;
			ViewState["RecordCount"] = this.RecordCount;
			ViewState["PageSize"] = this.PageSize;
		}

		public int PageNumber { get; set; }

		public int RecordCount { get; set; }

		public int PageSize { get; set; }

		protected void lnkPrevious_Click(object sender, EventArgs e)
		{
			if (this.PageNumber > 1)
				SetPageNumber(this.PageNumber - 1);
		}

		protected void lnkNext_Click(object sender, EventArgs e)
		{
			SetPageNumber(this.PageNumber + 1);
		}

		protected void repPage_ItemCommand(object sender, RepeaterCommandEventArgs e)
		{
			SetPageNumber(int.Parse(e.CommandArgument.ToString()));
		}

		protected void repPage_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			LinkButton lnkPage = (LinkButton)e.Item.FindControl("lnkPage");

			int page = int.Parse(lnkPage.CommandArgument.ToString());

			if (page == this.PageNumber)
			{
				((HtmlControl)e.Item.FindControl("li")).Attributes.Add("class", "active");
			}
		}

		protected void SetPageNumber(int pageNumber)
		{
			this.PageNumber = pageNumber;
			if (PageNumberChanged != null)
				PageNumberChanged(this, EventArgs.Empty);
		}

	}
}
