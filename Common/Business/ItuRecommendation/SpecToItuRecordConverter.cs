using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business.ItuRecommendation
{
    /// <summary>
    /// Implementation of ISpecToItuRecordConverter
    /// </summary>
    public class SpecToItuRecordConverter : ISpecToItuRecordConverter
    {
        /// <summary>
        /// Ultimate Unit of work, with access to U3GPPDB
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Builds the ITU record by fetching data from tables and views of U3GPPDB.
        /// </summary>
        /// <param name="clausesAndSpecNumbers"></param>
        /// <param name="startReleaseId"></param>
        /// <param name="endReleaseId"></param>
        /// <param name="lastMeetingId"></param>
        /// <returns></returns>
        public ServiceResponse<List<ItuRecord>> BuildItuRecordsForSpec(List<KeyValuePair<string, string>> clausesAndSpecNumbers, int startReleaseId, int endReleaseId, int lastMeetingId)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interface in charge of retrieving all the data necessary to build Itu records
    /// </summary>
    public interface ISpecToItuRecordConverter
    {
        /// <summary>
        /// Unit of work from where data can be fetched.
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Functions that takes in parameter a list of spec numbers associated to their clauses and output a list
        /// of corresponding ITU records
        /// </summary>
        /// <param name="clausesAndSpecNumbers"></param>
        /// <param name="startReleaseId"></param>
        /// <param name="endReleaseId"></param>
        /// <param name="lastMeetingId"></param>
        /// <returns></returns>
        ServiceResponse<List<ItuRecord>> BuildItuRecordsForSpec(List<KeyValuePair<string, string>> clausesAndSpecNumbers, int startReleaseId, int endReleaseId, int lastMeetingId);
    }
}
