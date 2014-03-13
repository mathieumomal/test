using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Tests.FakeSets
{
    public class ShortUrlFakeDBSet : FakeDBSet<ShortUrl>
    {
        public override ShortUrl Find(params object[] keyValues)
        {
            var keyValue = (int)keyValues.FirstOrDefault();
            return this.SingleOrDefault(su => su.Pk_Id == keyValue);
        }
    }
}
