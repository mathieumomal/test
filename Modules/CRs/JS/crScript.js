function openRelease(address, releaseId) {
    var popUp = window.open(address,
        'Release-' + releaseId, 'height=690,width=670,toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no');
    popUp.focus();
}

function openTdoc(address, TdocNumber) {
    var popUp = window.open(address, 'TDoc-' + TdocNumber, 'height=720,width=616,toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no');
    popUp.focus();
}

/** open or close RadPanelBar only if element has openCloseRadPanelBar class **/
function PreventCrSearchCollapse(sender, eventArgs) {
    if (eventArgs.get_domEvent().target.className != "openCloseRadPanelBar") {
        eventArgs.set_cancel(true);
    }
}

//Adapt RagGrid height based on the "contentHeight" event (in the mainpage.ascx)
$("#content").on('contentHeight', function (event, hContent) {
    var crGrid = $('#rgCrList');
    var gridDiv = crGrid.find(".rgDataDiv")[0];
    var securityValue;
    if ($('.livetabssouthstreet').length == 0) {
        securityValue = 155;
    } else {
        securityValue = 250;
    }
    gridDiv.style.height = (hContent - securityValue) + "px";
});

