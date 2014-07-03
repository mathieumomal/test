﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class ResponsibleGroupChairmanFakeDBSet : FakeDBSet<ResponsibleGroup_Chairman>
    {
        public override ResponsibleGroup_Chairman Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(p => p.TbId == keyValue);
        }
    }
}