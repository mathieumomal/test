namespace Etsi.Ultimate.DomainClasses.Facades
{
    public class ChangeRequestListFacade
    {
        public int ChangeRequestId { get; set; }

        //Data to display
        public string SpecNumber { get; set; }
        public string ChangeRequestNumber { get; set; }
        public int Revision { get; set; }
        public string ImpactedVersion { get; set; }
        public string TargetRelease { get; set; }
        public string Title { get; set; }
        public string WgTdocNumber { get; set; }
        public string WgStatus { get; set; }
        public string TsgTdocNumber { get; set; }
        public string TsgStatus { get; set; }
        public string NewVersion { get; set; }

        //Additionnal infos
        public int SpecId { get; set; }
        public int TargetReleaseId { get; set; }
        public string NewVersionPath { get; set; }
    }
}
