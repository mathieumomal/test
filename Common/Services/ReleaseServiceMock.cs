﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etsi.Ultimate.DomainClasses;

namespace Etsi.Ultimate.Services
{
    public class ReleaseServiceMock : IReleaseService
    {
        #region IReleaseService Membres

        public List<DomainClasses.Release> GetAllReleases()
        {
            var releases = new List<Release>();
            var statusFrozen = new Enum_ReleaseStatus() {Enum_ReleaseStatusId = 2, ReleaseStatus = "Frozen"};
            var statusClosed = new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 3, ReleaseStatus = "Closed" };
            var statusOpen = new Enum_ReleaseStatus() { Enum_ReleaseStatusId = 1, ReleaseStatus = "Open" };
            releases.Add(new DomainClasses.Release() { 
                Pk_ReleaseId = 1, 
                Name = "First release", 
                Fk_ReleaseStatus = 1, 
                StartDate = new DateTime(2010, 1, 18), 
                ClosureDate = new DateTime(2015, 10, 12), 
                Enum_ReleaseStatus = statusOpen,
                Stage1FreezeDate = DateTime.Today.AddDays(1),
                Stage2FreezeDate = DateTime.Today.AddDays(2),
                Stage3FreezeDate = DateTime.Today.AddDays(3),  
            });
            releases.Add(new DomainClasses.Release()
            {
                Pk_ReleaseId = 2, 
                Name = "Second release", 
                Fk_ReleaseStatus = 1, 
                StartDate = new DateTime(2008, 9, 18), 
                ClosureDate = new DateTime(2015, 10, 12), 
                Enum_ReleaseStatus = statusFrozen,
                Stage1FreezeDate = DateTime.Today.AddDays(-5), 
                Stage1FreezeMtgRef = "d3",
                Stage2FreezeDate = DateTime.Today.AddDays(-3), 
                Stage2FreezeMtgRef = "d4",
                Stage3FreezeDate = DateTime.Today.AddDays(1), 
                Stage3FreezeMtgRef = ("a pas afficher"),
            });
            releases.Add(new DomainClasses.Release()
            {
                Pk_ReleaseId = 3, 
                Name = "Third release", 
                Fk_ReleaseStatus = 1, 
                StartDate = new DateTime(2009, 1, 21), 
                EndDate = new DateTime(2013, 10, 11), 
                ClosureDate = new DateTime(2013, 10, 12), 
                Stage1FreezeDate = DateTime.Today.AddDays(-3), 
                Stage1FreezeMtgRef = ("3c"),
                Stage2FreezeDate = DateTime.Today.AddDays(-2), 
                Stage2FreezeMtgRef = ("4c"),
                Stage3FreezeDate = DateTime.Today.AddDays(-1), 
                Stage3FreezeMtgRef = ("5c"),
                ClosureMtgRef="fdf8ddsd8", 
                Enum_ReleaseStatus = statusClosed
            });

            return releases;
        }

        #endregion
    }
}