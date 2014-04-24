﻿using Etsi.Ultimate.DomainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class SpecificationRapporteurFakeDbSet : FakeDBSet<SpecificationRapporteur>
    {
        public override SpecificationRapporteur Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(x => x.Pk_SpecificationRapporteurId == keyValue);
        }
    }
}
