/*
range: function( value, element, param ) {
	return this.optional(element) || ( value >= param[0] && value <= param[1] );
}
*/

$.validator.methods.range = function (value, element, param) {
	var globalizedValue = value.replace(",", ".");
	return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
}

/*
number: function( value, element ) {
	return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?$/.test(value);
}
*/
 
$.validator.methods.number = function (value, element) {
    return this.optional(element) || /^-?(?:\d+|\d{1,3})?(?:,\d+)?$/.test(value);
}