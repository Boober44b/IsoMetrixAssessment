$(document).ready(function() {

    // Populate dropdown with object
    var dropdownObj = [
        {
            value: 1,
            text: "XML"
        },
        {
            value: 2,
            text: "JSON"
        }
    ];

    $("#FormatOption").populateDropdown(dropdownObj);

});

//function onLoginBegin() {
//    $('#logbtn').hide();
//}