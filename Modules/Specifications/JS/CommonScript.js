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
    win.setSize(650, 415);
    win.set_behaviors(Telerik.Web.UI.WindowBehaviors.Move + Telerik.Web.UI.WindowBehaviors.Close);
    win.set_modal(true);
    win.set_visibleStatusbar(false);
    win.set_title(title);
    win.show();
    return false;
}