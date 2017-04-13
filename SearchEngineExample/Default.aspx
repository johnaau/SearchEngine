<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SearchEngineExample.Default" %>
<%@ Register Src="~/Pagination.ascx" TagPrefix="uc1" TagName="Pagination" %>
<!DOCTYPE html>
<html lang="en">
<head runat="server">
	<meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
	<title>Search Engine Example</title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
      <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>
<body>
	<form id="form1" runat="server">
		<div class="container">
			<asp:Panel runat="server" DefaultButton="btnSearch">
				<div class="form-group">
					<asp:Label runat="server" AssociatedControlID="txtQuery" Text="Query"></asp:Label>
					<asp:TextBox ID="txtQuery" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
				</div>
				<asp:Button ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click" Text="Search" />
			</asp:Panel>
			<asp:Repeater ID="repResults" runat="server" OnItemDataBound="repResults_ItemDataBound">
				<ItemTemplate>
					<h2><asp:Literal ID="litTitle" runat="server"></asp:Literal></h2>
					<p><asp:Literal ID="litContent" runat="server"></asp:Literal></p>
					<p><asp:Literal ID="litKeywords" runat="server"></asp:Literal></p>
					<p>Score: <%# ((MyData)Container.DataItem).SearchScore %></p>
				</ItemTemplate>
			</asp:Repeater>
			<uc1:Pagination ID="ctlPagination" runat="server" OnPageNumberChanged="ctlPagination_PageNumberChanged" />
		</div>
	</form>
	<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
	<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js" integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa" crossorigin="anonymous"></script>
</body>
</html>