namespace Etsi.Ultimate.WCF.Service.Converters
{
    public interface IConverter<S, U>
    {
        S ConvertUltimateObjectToServiceObject(U obj);
        U ConvertServiceObjectToUltimateObject(S obj);
    }
}
