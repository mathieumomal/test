function openRemarksPopup(remarksModule, remarksModulePrimaryKey, isEditMode, title) {
    var win = radopen("/desktopmodules/Specifications/RemarksPopup.aspx?remarksModule=" + remarksModule + "&remarksModulePrimaryKey=" + remarksModulePrimaryKey + "&isEditMode=" + isEditMode, "Remarks");
    var height = 240;
    if (isEditMode == 'True') {
        height = height + 45;
    }
    win.setSize(650, height);
    win.set_behaviors(Telerik.Web.UI.WindowBehaviors.Move + Telerik.Web.UI.WindowBehaviors.Close);
    win.set_modal(true);
    win.set_visibleStatusbar(false);
    win.set_title(title);
    win.show();
    return false;
}

function openVersionDetailsPopup(versionId, isEditMode, title) {
    var win = radopen("/desktopmodules/Versions/VersionPopup.aspx?versionId=" + versionId + "&isEditMode=" + isEditMode, title);
    win.setSize(650, 520);
    win.set_behaviors(Telerik.Web.UI.WindowBehaviors.Move + Telerik.Web.UI.WindowBehaviors.Close);
    win.set_modal(true);
    win.set_visibleStatusbar(false);
    win.set_title(title);
    win.add_beforeClose(refreshParentPageAfterVersionSaved);
    win.show();
    return false;
}

// Reload parent page, on release tab.
function refreshParentPageAfterVersionSaved(sender) {
    //remove the handler
    sender.remove_beforeClose(refreshParentPageAfterVersionSaved);
    if ($('iframe').contents().find('#versionSavedhf').val() === 'True') {
        refreshSpecificationDetailsPage('Releases');
    }
}

function refreshSpecificationDetailsPage(selectedTab) {
    var newLocation = removeParam("selectedTab", location.href);
    location.href = newLocation + "&selectedTab=" + selectedTab;
}

function removeParam(key, sourceURL) {
    var rtn = sourceURL.split("?")[0],
        param,
        params_arr = [],
        queryString = (sourceURL.indexOf("?") !== -1) ? sourceURL.split("?")[1] : "";
    if (queryString !== "") {
        params_arr = queryString.split("&");
        for (var i = params_arr.length - 1; i >= 0; i -= 1) {
            param = params_arr[i].split("=")[0];
            if (param === key) {
                params_arr.splice(i, 1);
            }
        }
        rtn = rtn + "?" + params_arr.join("&");
    }
    return rtn;
}
