function GetRadWindow() {
    var oWindow = null;
    if (window.radWindow) oWindow = window.radWindow;
    else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
    return oWindow;
}

function closeRadWindow() {
    GetRadWindow().close();
}

$(document).ready(function () {
    formValidator();
});

function formValidator() {
    $("#frmVersionPopup").validate({
        errorClass: "required",
        onsubmit: true,
        onKeyup: true,
        onChange: true,
        eachValidField: function () {
            checkMandatoryFields();
        },
        eachInvalidField: function () {
            checkMandatoryFields();
        }
    });
}

function checkMandatoryFields() {
    var rcbMeeting = $('input[name="meetingCtrl$rcbMeetings"]');

    var fieldsValid = true;
    if ($.trim($(rcbMeeting).val()) === "") {
        fieldsValid = false;
    }

    $('input[id$="btnSave"]').removeClass('btn3GPP-default');
    $('input[id$="btnSave"]').removeClass('btn3GPP-success');

    if (fieldsValid) {
        $('input[id$="btnSave"]').addClass('btn3GPP-success');
        $('input[id$="btnSave"]').attr("disabled", false);
    } else {
        $('input[id$="btnSave"]').addClass('btn3GPP-default');
        $('input[id$="btnSave"]').attr("disabled", true);
    }
}