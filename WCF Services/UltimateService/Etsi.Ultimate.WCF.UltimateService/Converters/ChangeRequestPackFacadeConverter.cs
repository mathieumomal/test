using Etsi.Ultimate.WCF.Interface.Entities;

namespace Etsi.Ultimate.WCF.Service.Converters
{
    public class ChangeRequestPackFacadeConverter : IConverter<ChangeRequestPackFacade, DomainClasses.Facades.ChangeRequestPackFacade>
    {
        public static DomainClasses.Facades.ChangeRequestPackFacade ConvertServiceObjectToUltimateObject(ChangeRequestPackFacade obj)
        {
            return new DomainClasses.Facades.ChangeRequestPackFacade
            {
                Uid = obj.Uid,
                MeetingId = obj.MeetingId,
                Source = obj.Source
            };
        }

        public static ChangeRequestPackFacade ConvertUltimateObjectToServiceObject(DomainClasses.Facades.ChangeRequestPackFacade obj)
        {
            return new ChangeRequestPackFacade
            {
                Uid = obj.Uid,
                MeetingId = obj.MeetingId,
                Source = obj.Source
            };
        }

        DomainClasses.Facades.ChangeRequestPackFacade IConverter<ChangeRequestPackFacade, DomainClasses.Facades.ChangeRequestPackFacade>.ConvertServiceObjectToUltimateObject(ChangeRequestPackFacade obj)
        {
            return ConvertServiceObjectToUltimateObject(obj);
        }

        ChangeRequestPackFacade IConverter<ChangeRequestPackFacade, DomainClasses.Facades.ChangeRequestPackFacade>.ConvertUltimateObjectToServiceObject(DomainClasses.Facades.ChangeRequestPackFacade obj)
        {
            return ConvertUltimateObjectToServiceObject(obj);
        }
    }
}