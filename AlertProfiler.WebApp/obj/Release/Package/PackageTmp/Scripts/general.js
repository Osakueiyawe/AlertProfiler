/*=============================================================
    Author : Gbolahan Alli
    Author URI : www.verspace.com
    Author Email : gbolahan.alli@verspace.com
    Date : 10/05/2017    
    ========================================================  */
function general() { };
general.prototype.dangerColor = "#8A0707";
general.prototype.successColor = "#3c763d";
general.spinner = {};
general.prototype.urlToSendTo = window.location.href
// Spinner Configuration settings
general.prototype.opts = {
	lines: 13 // The number of lines to draw
	, length: 28 // The length of each line
	, width: 14 // The line thickness
	, radius: 84 // The radius of the inner circle
	, scale: 1 // Scales overall size of the spinner
	, corners: 1 // Corner roundness (0..1)
	, color: '#000' // #rgb or #rrggbb or array of colors
	, opacity: 0.05 // Opacity of the lines
	, rotate: 0 // The rotation offset
	, direction: 1 // 1: clockwise, -1: counterclockwise
	, speed: 1 // Rounds per second
	, trail: 60 // Afterglow percentage
	, fps: 20 // Frames per second when using setTimeout() as a fallback for CSS
	, zIndex: 2e9 // The z-index (defaults to 2000000000)
	, className: 'spinner' // The CSS class to assign to the spinner
	, top: '50%' // Top position relative to parent
	, left: '50%' // Left position relative to parent
	, shadow: true // Whether to render a shadow
	, hwaccel: false // Whether to use hardware acceleration
	, position: 'absolute' // Element positioning
}

// TOAST METHOD
general.prototype.Toast = function Toast(message, color) {
	var snackBarDiv = document.createElement("div");
	snackBarDiv.setAttribute("id", "snackbar");
	document.body.appendChild(snackBarDiv);
	snackBarDiv.innerHTML = message;
	snackBarDiv.style.backgroundColor = color;
	snackBarDiv.style.backgroundColor = color;
	snackBarDiv.className = "show";
	setTimeout(function () { snackBarDiv.className = snackBarDiv.className.replace("show", ""); }, 4000);
}

// Start spinner to show activity happening in the background
general.prototype.beginSpinner = function () {
	var target = document.getElementById('spinner');
	$("#spinnerModal").modal({ backdrop: "static" });
	general.spinner = new Spinner(general.prototype.opts).spin(target);
}

// Stop spinner as activity in the background is done.
general.prototype.stopSpinner = function () {
	$("#spinnerModal").modal("hide");
	general.spinner.stop();
}

//Sending attached files to server using Ajax and leveraging on promises to ensure UI doesn't freeze and delivery reliability 
general.prototype.ajaxPromise = function (typeOfAjaxRequest, urlToUploadTo, booleanState, dataToSend) {
	var xmlhttp;
	return new Promise(function (resolve, reject) {
		if (window.XMLHttpRequest) {
			xmlhttp = new XMLHttpRequest;
		} else {
			xmlhttp = new ActiveXincomeDistribution("Microsoft.XMLHTTP");
		}
		xmlhttp.onload = function () {
			general.prototype.beginSpinner();
			if (xmlhttp.status === 200 & xmlhttp.readyState === 4) {
				var response = xmlhttp.responseText;
				resolve(response);
			} else {
				reject(xmlhttp.responseText);
			}
			general.prototype.stopSpinner();
		}
		xmlhttp.onerror = function (e) {
			reject(Error("Could not upload docuemt", e.message));
			console.log("Couldnt not upload file");
			general.prototype.stopSpinner();
		}
		xmlhttp.open(typeOfAjaxRequest, urlToUploadTo, booleanState);
		xmlhttp.send(dataToSend);
	});
}
general.prototype.datepicker = function () {
	$('#endDate').daterangepicker({
		singleDatePicker: true,
		locale: {
			format: 'DD-MMMM-YYYY'
		},
		showDropdowns: true,
		startDate: moment(),
		maxDate: new Date()
	})
}
general.prototype.ajaxJsonPromise = function (typeOfAjaxRequest, urlToUploadTo, booleanState, dataToSend) {
	var xmlhttp;
	return new Promise(function (resolve, reject) {
		if (window.XMLHttpRequest) {
			xmlhttp = new XMLHttpRequest;
		} else {
			xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
		}
		xmlhttp.onload = function () {
			if (xmlhttp.status === 200 & xmlhttp.readyState === 4) {
				resolve(xmlhttp.responseText);
			} else {
				reject(xmlhttp.responseText);
			}
		}
		xmlhttp.onerror = function (e) {
			reject(Error("Could not homepage docuemt", e.message));
			console.log("Couldnt not homepage file");
			$("#myModal").modal("hide");
		}
		xmlhttp.open(typeOfAjaxRequest, urlToUploadTo, booleanState);
		xmlhttp.setRequestHeader("Content-Type", "application/json");
		xmlhttp.send(dataToSend);
	});
}

general.prototype.inputFieldValidation = function (selectedFiles, expectedFileName, errorWhenEmpty, errorWhenMismatch) {
	let numberOfFiles = selectedFiles.length;
	if (numberOfFiles === 0) throw new Error(errorWhenEmpty);
	let fileName = selectedFiles[0].name;
	expectedFileName = expectedFileName.toUpperCase();
	console.log(expectedFileName + " " + fileName.toUpperCase());
	if (fileName.toUpperCase().indexOf(expectedFileName) === -1) throw new Error(errorWhenMismatch);
	return;
}

general.prototype.printExcel = function (currentPageActionMethod, actionMethodToPrint) {
	general.prototype.ajaxPromise("POST", general.prototype.urlToSendTo.replace(currentPageActionMethod, actionMethodToPrint), true, null)
		.then(function (result) {
			let response = JSON.parse(result);
			if (!response.status) {
				general.prototype.Toast(response.message, general.prototype.dangerColor);
				console.error(response.devMessage);
				throw Error(response.devMessage);
			}
			general.prototype.Toast(response.message, general.prototype.successColor);
		});

}

general.prototype.exportCurrentDataToExcel = function (dataFromDatatable, currentPageActionMethod, actionMethodToPrint) {
	var length = dataFromDatatable.length;
	var data = [];
	for (var i = 0; i < length; i++) {
		data.push(dataFromDatatable[i]);
	}
	general.prototype.ajaxJsonPromise("POST", general.prototype.urlToSendTo.replace(currentPageActionMethod, actionMethodToPrint), true, JSON.stringify(data))
		.then(function (response) {
			let result = JSON.parse(response);
			if (!result.status) {
				general.prototype.Toast(result.message, general.prototype.dangerColor);
				console.error(result.devMessage);
				throw Error(result.devMessage);
			}
			general.prototype.Toast(result.message, general.prototype.successColor);
		})
}

general.prototype.convertObjectToFormData = function (object) {
	var theFormData = new FormData();
	for (var key in object) {
		theFormData.append(key, object[key]);
	}
	return theFormData;
}

general.prototype.validateUploadedFileForConformity = function (selectedFiles, expectedExchangeRateFileName, noFileErrorMessage,fileMisMatchError) {
	let numberOfFiles = selectedFiles.length;
	if (numberOfFiles === 0) throw new Error(noFileErrorMessage);
	let fileFullName = selectedFiles[0].name.toUpperCase();
	let fileNameAndExtensionArray = fileFullName.split('.');
	let extension = fileNameAndExtensionArray[1].toString();
	let fileName = fileNameAndExtensionArray[0].toString();
	if (fileName.indexOf(expectedExchangeRateFileName.toUpperCase()) === -1) throw new Error(fileMisMatchError);
	return;
}