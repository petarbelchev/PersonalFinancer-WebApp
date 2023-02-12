// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
	$('#datepicker').datepicker({
		format: "dd.mm.yyyy",
		todayBtn: "linked",
		todayHighlight: true
	});
});