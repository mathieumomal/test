function GetRadWindow() {
    var oWindow = null;
    if (window.radWindow) oWindow = window.radWindow;
    else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
    return oWindow;
}

function closeRadWindow() {
    GetRadWindow().close();
}

function closeAndRefresh() {
    $('#versionSavedhf').val('True');
    closeRadWindow();
}

$(document).ready(function () {
    var $isDraft = $('#isDraftVersionhf');
    if ($isDraft && $isDraft.val() === 'True') {
        $('.mandatoryIndicator.meetingMandatoryIndicator').hide();
    } else {
        formValidator();
    }

    var versionSaved = location.search.split('versionSaved=')[1] ? location.search.split('versionSaved=')[1] : 'False';
    $('#versionSavedhf').val(versionSaved)
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
    var $rcbMeeting = $('input[name="meetingCtrl$rcbMeetings"]');
    var $btnSave = $('input[id$="btnSave"]');
    var fieldsValid = true;

    if ($.trim($rcbMeeting.val()) === "") {
        fieldsValid = false;
    }

    $btnSave.removeClass('btn3GPP-default').removeClass('btn3GPP-success');

    if (fieldsValid) {
        $btnSave.addClass('btn3GPP-success');
        $btnSave.attr("disabled", false);
    } else {
        $btnSave.addClass('btn3GPP-default');
        $btnSave.attr("disabled", true);
    }
}