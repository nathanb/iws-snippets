<%@ Page Language="C#" AutoEventWireup="true"  %>
<%@ Import Namespace="System.Collections.Generic" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>

	<script type="text/C#" runat="server">
		protected void Page_Load(object sender, EventArgs e)
		{
			Page.GetPostBackEventReference(uxGrid);

			if (!Page.IsPostBack)
			{
				List<int> test = new List<int>();
				test.Add(2);
				test.Add(3);
				test.Add(32);
				test.Add(223);
				test.Add(5);
				test.Add(8);
				uxGrid.DataSource = test;
				uxGrid.DataBind();

			}
		}
		protected void uxRowAction_Click(object sender, EventArgs e)
		{
			Button b = sender as Button;
			if (b != null)
			{
				uxTest.Text = "clicked " + b.CommandArgument;
			}
		}

	</script>


	<link href="smoothness/jquery-ui-1.7.2.custom.css" rel="stylesheet" type="text/css" />
	<script src="https://www.google.com/jsapi?key=" type="text/javascript"></script>
	<script type="text/javascript">
		google.load("jquery", "1");
		google.load("jqueryui", "1");
	</script>
	<script type="text/javascript">
		$().ready(function() {
			$('#dialogContent').dialog({
				autoOpen: false,
				modal: true,
				bgiframe: true,
				title: "MySql Membership Config Tool",
				width: 800,
				height: 600
			});
		});

		function rowAction(uniqueID) {

			$('#dialogContent').dialog('option', 'buttons',
				{
					"OK": function() { __doPostBack(uniqueID, ''); $(this).dialog("close"); },
					"Cancel": function() { $(this).dialog("close"); }
				});

				$('#dialogContent').dialog('open');

			return false;
		}


	</script>
</head>
<body>
	<form id="form1" runat="server">
	<div id="dialogContent">
		<h3>confirm</h3>
		<p>Click ok to accept</p>
	</div>
	<asp:Literal ID="uxTest" runat="server" />
	<div>
		<asp:DataGrid ID="uxGrid" runat="server" AutoGenerateColumns="false">
			<Columns>
				<asp:TemplateColumn>
					<ItemTemplate>
						<asp:Button ID="uxRowAction" runat="server" CommandArgument='<%#Container.DataItem.ToString() %>' Text="Row Action" OnClick="uxRowAction_Click" OnClientClick="javascript:return rowAction(this.name);" />
						<asp:Literal ID="uxTest" runat="server" />
						</ItemTemplate>
				</asp:TemplateColumn>
			</Columns>
		</asp:DataGrid>
	</div>
	</form>
</body>
</html>
