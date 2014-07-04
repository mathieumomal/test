using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;

namespace Etsi.Ultimate.Business
{
    class WpmRecordCreator
    {
        IUltimateUnitOfWork UoW;

        public WpmRecordCreator(IUltimateUnitOfWork iUoW)
        {
            UoW = iUoW;
        }

        public int AddWpmRecords(SpecVersion iVersion)
        {
            return 0;
        }
    }
}
