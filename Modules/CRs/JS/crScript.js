function openSpecification(address, specId) {
    var popUp = window.open(address, 'Specification-' + specId, 'height=690,width=674,toolbar=no,location=no, directories=no,status=no,menubar=no,scrollbars=no,resizable=no');
    popUp.focus();
}

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

