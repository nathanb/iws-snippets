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
		serializeCustomData: function () {
			var data;
			data = $(this).uploader.settings.customData;
			var s = '';
			var ix = 0;
			for (property in data) {
				if (ix > 0)
					s += ';';
				s += property + ':' + data[property].toString();
				ix++;
			}
			return s;
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
