using Etsi.Ultimate.WCF.Interface.Entities;

namespace Etsi.Ultimate.WCF.Service.Converters
{
    public class ChangeRequestInsideCrPackFacadeConverter : IConverter<ChangeRequestInsideCrPackFacade,DomainClasses.Facades.ChangeRequestInsideCrPackFacade>
    {
        public static DomainClasses.Facades.ChangeRequestInsideCrPackFacade ConvertServiceObjectToUltimateObject(ChangeRequestInsideCrPackFacade obj)
        {
            return new DomainClasses.Facades.ChangeRequestInsideCrPackFacade
            {
                Id = obj.Id
            };
        }

        public ChangeRequestInsideCrPackFacade ConvertUltimateObjectToServiceObject(DomainClasses.Facades.ChangeRequestInsideCrPackFacade obj)
        {
            return new ChangeRequestInsideCrPackFacade
            {
                Id = obj.Id
            };
        }

        DomainClasses.Facades.ChangeRequestInsideCrPackFacade IConverter<ChangeRequestInsideCrPackFacade, DomainClasses.Facades.ChangeRequestInsideCrPackFacade>.ConvertServiceObjectToUltimateObject(ChangeRequestInsideCrPackFacade obj)
        {
            return ConvertServiceObjectToUltimateObject(obj);
        }

        ChangeRequestInsideCrPackFacade IConverter<ChangeRequestInsideCrPackFacade, DomainClasses.Facades.ChangeRequestInsideCrPackFacade>.ConvertUltimateObjectToServiceObject(DomainClasses.Facades.ChangeRequestInsideCrPackFacade obj)
        {
            return ConvertUltimateObjectToServiceObject(obj);
        }
    }
}