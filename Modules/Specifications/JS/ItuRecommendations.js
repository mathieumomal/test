function OnClientValidationFailed() {
    alert('Only docx format are allowed. File will not be uploaded.');
}

var rcbStartRelease = $('div[id$="rcbStartRelease"]');
var rcbEndRelease = $('div[id$="rcbEndRelease"]');
var rcbItuRec = $('div[id$="rcbItuRec"]');

$(document).ready(function () {
    formValidator();
});

function formValidator() {
    $("#formItuRecommendation").validate({
        errorClass: "required",
        onsubmit: true,
        onKeyup: true,
        onChange: true,
        eachValidField: function () {
            if ($(rcbStartRelease).val() != "" && $(rcbStartRelease).val() != "-" &&
            $(rcbEndRelease).val() != "" && $(rcbEndRelease).val() != "-") {
                EnableITUBtnExport();
                if (rcbItuRec.val() == "ITU-T Q.1741") {
                    EnableITUPreliminary();
                } else {
                    DisableITUPreliminary();
                }
                return true;
            }
            DisableITUBtnExport();
            return false;
        },
        eachInvalidField: function () {
            DisableITUBtnExport();
            DisableITUPreliminary();
        }
    });
}

function EnableITUBtnExport() {
    $('input[id$="btnExport"]').removeClass('btn3GPP-default');
    $('input[id$="btnExport"]').addClass('btn3GPP-success');
    $('input[id$="btnExport"]').attr("disabled", false);
}

function DisableITUBtnExport() {
    $('input[id$="btnExport"]').addClass('btn3GPP-default');
    $('input[id$="btnExport"]').removeClass('btn3GPP-success');
    $('input[id$="btnExport"]').attr("disabled", true);
}

function EnableITUPreliminary() {
    $('input[id$="btnPreliminary"]').addClass('btn3GPP-success');
    $('input[id$="btnPreliminary"]').removeClass('btn3GPP-default');
    $('input[id$="btnPreliminary"]').attr("disabled", false);
}

function DisableITUPreliminary() {
    $('input[id$="btnPreliminary"]').addClass('btn3GPP-default');
    $('input[id$="btnPreliminary"]').removeClass('btn3GPP-success');
    $('input[id$="btnPreliminary"]').attr("disabled", true);
}