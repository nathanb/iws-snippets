<%@ Page Language="C#" AutoEventWireup="true" %>

<%@ Import Namespace="System.Collections.Generic" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>

	<script type="text/C#" runat="server">

		protected void Button1_Click(object sender, EventArgs e)
		{
			Button b = sender as Button;
			if (b != null)
			{
				lblClicked.Text = "clicked!!";
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
$(document).ready(function() {
	$('#<%=this.Button1.ClientID %>').click(function(){
		$('#dialogContent').dialog('open'); return false;
	});
	$('#<%=this.Button2.ClientID %>').click(function(){
		$('#dialogContent').dialog('open'); return false;
	});

	$("#dialogContent").dialog(
		{ autoOpen: false,
			modal: true,
			bgiframe: true,
			width: 450,
			height: 350,
			buttons: {
			'You Bet!': function() {
			<%=this.Page.ClientScript.GetPostBackEventReference(new PostBackOptions(this.Button1))%>;
			},
			'No Thanks': function() {
			$(this).dialog('close');
			}
		}
	})
});
</script>

</head>
<body>
	<form id="form1" runat="server">
	<asp:Literal ID="uxTest" runat="server" />
	<asp:Label ID="lblClicked" runat="server" />
	<asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Test"/>
	<asp:LinkButton ID="Button2" runat="server" OnClick="Button1_Click" Text="Test2" />
	<div id="dialogContent" style="display: none;">
		<h2>Terms and Conditions</h2>
		<p>
			By accepting, you agree to the following:
			<ul>
				<li>Are you really, really sure???? </li>
			</ul>
		</p>
	</div>
	</form>
</body>
</html>
