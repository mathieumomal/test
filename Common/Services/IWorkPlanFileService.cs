﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Services
{
    public interface IWorkPlanFileService
    {
        
        void AddWorkPlanFile(WorkPlanFile workPlanFile);
        WorkPlanFile GetLastWorkPlanFile();
    }
}
