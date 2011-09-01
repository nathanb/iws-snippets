(function ($) {
	var methods = {
		init: function (options) {

			return this.each(function () {
				if (options) {
					$.extend($(this).uploader.settings, options);
				}
			});
		},
		progress: function (percent) {
			this.uploader.settings.progress(percent);
			return this;
		},
		complete: function (success, message) {
			this.uploader.settings.complete(success, message);
			return this;
		},
		starting: function () {
			this.uploader.settings.starting();
			return this;
		}
	};


	$.fn.uploader = function (method) {

		if (methods[method]) {
			return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
		} else if (typeof method === 'object' || !method) {
			return methods.init.apply(this, arguments);
		} else {
			$.error('Method ' + method + ' does not exist on jQuery.uploader');
		}

	};

	$.fn.uploader.settings = {
		'complete': function (success, message) { },
		'progress': function (percent) { },
		'starting': function () { },
		'url': 'http://localhost/Test/Upload',
		'maxSize': 30720,
		'uploadInividually': true,
		'callbackCompleted': 'uploadComplete',
		'callbackProgress': 'uploadProgress',
		'callbackStarting': 'uploadStarting',
		'customData': null,
		'buttonText': 'Select Files'
	};
})(jQuery);


//default callbacks for this silverlight control
function uploadProgress(percent) {
	$('#SilverlightControl').uploader('progress', percent);
}
function uploadComplete(success, message) {
	$('#SilverlightControl').uploader('complete', success, message);
}
function uploadStarting() {
	$('#SilverlightControl').uploader('starting');
}

function uploaderOnLoad() {
	var settings = $('#SilverlightControl').uploader.settings;
	var raw = document.getElementById("SilverlightControl");
	raw.content.page.Setup(settings.url, settings.maxSize, settings.uploadInividually, settings.callbackProgress, settings.callbackCompleted, settings.callbackStarting, serializeCustomData(settings.customData), settings.buttonText);
}

function serializeCustomData(data) {
	var s = '';
	var ix = 0;
	for (property in data) {
		if (ix > 0)
			s += ';';
		s += property + ':' + data[property].toString();
		ix++;
	}
	return s;
}

function onSilverlightError(sender, args) {

	var appSource = "";
	if (sender != null && sender != 0) {
		appSource = sender.getHost().Source;
	}
	var errorType = args.ErrorType;
	var iErrorCode = args.ErrorCode;

	var errMsg = "Unhandled Error in Silverlight 2 Application " + appSource + "\n";

	errMsg += "Code: " + iErrorCode + "    \n";
	errMsg += "Category: " + errorType + "       \n";
	errMsg += "Message: " + args.ErrorMessage + "     \n";

	if (errorType == "ParserError") {
		errMsg += "File: " + args.xamlFile + "     \n";
		errMsg += "Line: " + args.lineNumber + "     \n";
		errMsg += "Position: " + args.charPosition + "     \n";
	}
	else if (errorType == "RuntimeError") {
		if (args.lineNumber != 0) {
			errMsg += "Line: " + args.lineNumber + "     \n";
			errMsg += "Position: " + args.charPosition + "     \n";
		}
		errMsg += "MethodName: " + args.methodName + "     \n";
	}

	throw new Error(errMsg);
}
