function GetRadWindow() {
    var oWindow = null;
    if (window.radWindow) oWindow = window.radWindow;
    else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
    return oWindow;
}

function closeRadWindow() {
    GetRadWindow().close();
}

function waitForSaving() {
    $('#btnSave').css('visibility', 'hidden');
    $('#btnSave').attr('disabled', 'disabled');
    $('#btnSave').attr('value', 'Save...');
    $('#btnSave').css('visibility', 'visible');
    return true;
}