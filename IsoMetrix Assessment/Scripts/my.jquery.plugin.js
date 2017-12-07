/* 
======================================================================== 

jQuery plugin to populate a drown with an object. 

Andre Steyn
========================================================================
*/

(function ( $ ) {

	$.fn.populateDropdown = function (options) {
	    var that = this;

	    $.each(options, function (key, value) {
	        that.append($("<option></option>").attr("value", value.value).text(value.text));
	    });

		return that; // Make this function chainable.
	};

}(jQuery));