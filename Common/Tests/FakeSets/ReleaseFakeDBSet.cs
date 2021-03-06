﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class ReleaseFakeDBSet : FakeDBSet<Release>
    {
        public override Release Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(relea => relea.Pk_ReleaseId == keyValue);
        }
    }
}
