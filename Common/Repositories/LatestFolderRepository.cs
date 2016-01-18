using System;
using System.Collections.Generic;
using System.Linq;
using Etsi.Ultimate.DomainClasses;
using System.Data.Entity;

namespace Etsi.Ultimate.Repositories
{
    public class LatestFolderRepository : ILatestFolderRepository
    {
        public IUltimateUnitOfWork UoW { get; set; }

        public void Dispose()
        {
            //context.Dispose();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdate(LatestFolder entity)
        {
            if (Exists(entity.FolderName))
            {
                //[1] Add Existing
                UoW.Context.SetModified(entity);
            }
            else
            {
                //[2] Add the Entity 
                UoW.Context.SetAdded(entity);
            }
        }

        public IQueryable<LatestFolder> All
        {
            get
            {
                return UoW.Context.LatestFolders;
            }
        }

        public IQueryable<LatestFolder> AllIncluding(params System.Linq.Expressions.Expression<Func<LatestFolder, object>>[] includeProperties)
        {
            IQueryable<LatestFolder> query = UoW.Context.LatestFolders;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public LatestFolder Find(int id)
        {
            return All.Where(v => v.Pk_LatestFolderId == id).FirstOrDefault();
        }

        public LatestFolder FindByName(string name)
        {
            return All.Where(v => v.FolderName.ToUpper() == name.ToUpper()).FirstOrDefault();
        }

        public bool Exists(string name)
        {
            return All.Any(v => v.FolderName.ToUpper() == name.ToUpper());
        }

        public void Add(string name, string username)
        {
            UoW.Context.SetAdded(new LatestFolder() { FolderName = name, UserName = username, CreationDate = DateTime.Now });
        }

        public void Update(LatestFolder latestFolder)
        {
            UoW.Context.SetModified(latestFolder);
        }

        /// <summary>
        /// Get the name of latest folder
        /// </summary>
        /// <returns>Ftp Folders Manager Status</returns>
        public string GetLatestFolderName()
        {
            var folderName = string.Empty;
            var latestFolder = All.OrderByDescending(x => x.Pk_LatestFolderId).FirstOrDefault();
            if (latestFolder != null)
                folderName = latestFolder.FolderName;

            return folderName;
        }
    }
    public interface ILatestFolderRepository : IEntityRepository<LatestFolder>
    {
        /// <summary>
        /// Add new latest folder
        /// </summary>
        /// <param name="name">The name of latest folder</param>
        /// <param name="username">User name</param>
        void Add(string name, string username);

        /// <summary>
        /// Check if folder exists
        /// </summary>
        /// <param name="name">The name of latest folder</param>
        /// <returns>True or False</returns>
        bool Exists(string name);

        /// <summary>
        /// Update latestFolder entity
        /// </summary>
        /// <param name="latestFolder">latest folder</param>
        void Update(LatestFolder latestFolder);

        /// <summary>
        /// Get latest folder by name
        /// </summary>
        /// <param name="name">name of latest folder</param>
        /// <returns>LatestFolder</returns>
        LatestFolder FindByName(string name);

        /// <summary>
        /// Get the name of latest folder
        /// </summary>
        /// <returns>Ftp Folders Manager Status</returns>
        string GetLatestFolderName();
    }
}
