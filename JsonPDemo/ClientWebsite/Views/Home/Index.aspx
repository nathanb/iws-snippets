<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Index</title>
	<script src="../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
	<script type="text/javascript">
		$(document).ready(function () {
			$('.result').hide();

			$('#test').click(function () {
				$('.result').fadeOut('fast');
				$('.result').html('');

				$.ajax({
					url: 'http://localhost:50211/Api/GetInformation',
					data: { key: $('input[name=key]').val() },
					type: "GET",
					dataType: "jsonp",
					jsonpCallback: "localJsonpCallback"
				});
			});
		});

		function localJsonpCallback(json) {
			//do stuff...
			if (json.Success) {
				$('.result').html(json.Data);
			}
			else {
				$('.result').html(json.Message);
			}

			$('.result').fadeIn('fast');
		}
	</script>
</head>
<body>
	<div>
		Enter key: <input type="text" name="key" value="some data key, this parameter is optional" />
		<br /><input type="button" value="Test JSONP" id="test" />
		<div class="result">
		</div>
	</div>
</body>
</html>
