<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Pagination.ascx.cs" Inherits="SearchEngineExample.Pagination" %>
<asp:PlaceHolder ID="plhPagination" runat="server">
	<ul class="pagination">
		<li><asp:LinkButton ID="lnkPrevious" runat="server" CssClass="hyperlink_left" Visible="false" OnClick="lnkPrevious_Click">&laquo;</asp:LinkButton></li>
		<asp:Repeater ID="repPage" runat="server" OnItemCommand="repPage_ItemCommand" OnItemDataBound="repPage_ItemDataBound">
			<ItemTemplate>
				<li id="li" runat="server"><asp:LinkButton ID="lnkPage" runat="server" CommandArgument='<%# Container.DataItem %>' Text='<%# Container.DataItem %>'></asp:LinkButton></li>
			</ItemTemplate>
		</asp:Repeater>
		<li><asp:LinkButton ID="lnkNext" runat="server" CssClass="hyperlink_right" Visible="true" OnClick="lnkNext_Click">&raquo;</asp:LinkButton></li>
	</ul>
</asp:PlaceHolder>