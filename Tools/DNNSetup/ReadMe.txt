===========================================================================
===========  Install 3GU & NGPP services on your local machine  ===========
===========================================================================
[1] Right Click => Edit on "Launch_Online_DNNSetup.bat"

[2] Change the following parameters
    [a] PATH_ULTIMATE       ==> Base path of Ultimate Solution  Ex: D:\3GPP\SourceCode\ULTIMATE\trunk
    [b] PATH_NGPP           ==> Base path of NGPP solutions     Ex: D:\3GPP\SourceCode\NGPP
    [c] BASE_PATH_TARGET    ==> Where to install services       Ex: D:\EtsiPortalServices
    [d] SERVICE_LOGIN       ==> Your windows login              Ex: CORP\userid
    [e] SERVICE_PASSWORD    ==> Your windows password           Ex: password

[3] Save & Exit

[4] Double click on "Launch_Online_DNNSetup.bat" to install the following services
    [a] ETSI User Rights Service
    [b] ETSI Remote Consensus Service
    [c] ETSI Host Service
    [d] ETSI VFS Service
    [e] ETSI FTP Service
    [f] 3GU Synchronization Service
    [g] 3GU Ultimate Service



=====================================================================
===========  Install Offline client to sync offline data  ===========
=====================================================================
[1] Right Click => Edit on "Launch_Offline_DNNSetup.bat"

[2] Change the following parameters
    [a] PATH_ULTIMATE       ==> Base path of Ultimate Solution  Ex: D:\3GPP\SourceCode\ULTIMATE\trunk
    [b] BASE_PATH_TARGET    ==> Where to install services       Ex: D:\EtsiPortalServices
    [c] SERVICE_LOGIN       ==> Your windows login              Ex: CORP\userid
    [e] SERVICE_PASSWORD    ==> Your windows password           Ex: password

[3] Save & Exit

[4] Double click on "Launch_Offline_DNNSetup.bat" to install the following services
    [a] 3GU Synchronization Client