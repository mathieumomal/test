using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.DomainClasses
{
    public class SpecVersionCurrentAndNew
    {
        public SpecVersionCurrentAndNew()
        {
            this.NewSpecVersion = new SpecVersion();
        }
        public SpecVersion CurrentSpecVersion { get; set; }
        public SpecVersion NewSpecVersion { get; set; }
    }
}
