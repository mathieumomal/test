
// -------- Functions the manage the rad box.
function OnClientValidationFailed() {
    alert('Only docx format are allowed. File will not be uploaded.');
}

var rcbStartRelease = $('div[id$="rcbStartRelease"]');
var rcbEndRelease = $('div[id$="rcbEndRelease"]');
var rcbItuRec = $('div[id$="rcbItuRec"]');
var rcbMeeting = $('div[id$=rcbMeetings]');

$(document).ready(function () {
    formValidator();
    unvalidateForm();
});

function formValidator() {
    $("#formItuRecommendation").validate({
        errorClass: "required",
        onsubmit: true,
        onKeyup: true,
        onChange: true,
        eachValidField: validateForm,
        eachInvalidField: unvalidateForm
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

//// Validation and unvalidation function
var validateForm = function () {
    var asyncUpload = $find('rdauUploadSeedFile');
    
    /**Form invalid by default**/
    unvalidateForm();

    /**If all three fields are filled...**/
    if ($(rcbMeeting).val() != null && 
        $(rcbMeeting).val().trim() != "" && 
        $(rcbStartRelease).val() != "" && 
        $(rcbStartRelease).val() != "-" && 
        $(rcbEndRelease).val() != "" && 
        $(rcbEndRelease).val() != "-") {
        /**and there is one seed file : button export enabled**/
        if(asyncUpload.getUploadedFiles().length > 0){
            EnableITUBtnExport();
        }
        /**and ITU-T Q.1741 selected : button preliminary enabled**/
        if (rcbItuRec.val() == "ITU-T Q.1741") {
            EnableITUPreliminary();
        }
        return true;
    }
    return false;
}

var unvalidateForm = function () {
    DisableITUBtnExport();
    DisableITUPreliminary();
}