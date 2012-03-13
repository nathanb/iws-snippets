(function ($) {
	var methods = {
		init: function (options) {

			return this.each(function () {
				if (options) {
					$.extend($(this).uploader.settings, options);
				}
			});
		},
		sequenceProgress: function (percent, message) {
			this.uploader.settings.sequenceProgress(percent, message);
			return this;
		},
		contentProgress: function (percent, message) {
			this.uploader.settings.contentProgress(percent, message);
			return this;
		},
		complete: function (success, message) {
			this.uploader.settings.complete(success, message);
			return this;
		},
		starting: function () {
			this.uploader.settings.starting();
			return this;
		},
		option: function (options) {
			if (options)
				$.extend($(this).uploader.settings, options);
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
		'sequenceProgress': function (percent, message) { },
		'contentProgress': function (percent, message) { },
		'starting': function () { },
		'url': 'http://localhost/Test/Upload',
		'maxSize': 30720,
		'uploadInividually': true,
		'callbackCompleted': 'uploader_uploadComplete',
		'callbackSequenceProgress': 'uploader_uploadSequenceProgress',
		'callbackContentProgress': 'uploader_uploadContentProgress',
		'callbackStarting': 'uploader_uploadStarting',
		'customData': null,
		'buttonText': 'Select Files',
		'uploadChunked': false,
		'chunkSize': 0 //default of 200 KB
	};
})(jQuery);


function uploader_onSilverlightError (sender, args) {
	var appSource = "";
	if (sender != null && sender != 0) {
		appSource = sender.getHost().Source;
	}
	var errorType = args.ErrorType;
	var iErrorCode = args.ErrorCode;

	var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

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

//default callbacks for this silverlight control
function uploader_uploadSequenceProgress (percent, message) {
	$('#uploaderControl').uploader('sequenceProgress', percent, message);
}

function uploader_uploadContentProgress(percent, message) {
	$('#uploaderControl').uploader('contentProgress', percent, message);
}
function uploader_uploadComplete (success, message) {
	$('#uploaderControl').uploader('complete', success, message);
}
function uploader_uploadStarting () {
	$('#uploaderControl').uploader('starting');
}

function uploader_initSilverlight() {
	var settings = $('#uploaderControl').uploader.settings;
	var raw = document.getElementById("uploaderControl");
	raw.content.page.Setup(settings.url, settings.maxSize, settings.uploadInividually, settings.callbackSequenceProgress, settings.callbackContentProgress, settings.callbackCompleted, settings.callbackStarting, uploader_serializeCustomData(settings.customData), settings.buttonText, settings.uploadChunked, settings.chunkSize);
}

function uploader_serializeCustomData (data) {
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
