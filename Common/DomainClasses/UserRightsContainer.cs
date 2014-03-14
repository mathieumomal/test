using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Etsi.Ultimate.DomainClasses
{

    /// <summary>
    ///  This object is used to store the user rights and to retrieve them
    /// </summary>
    [Serializable]
    public class UserRightsContainer
    {
        /// <summary>
        /// Represents the rights that are independant from the committees
        /// </summary>
        List<Enum_UserRights> completeRights { get; set; }

        /// <summary>
        /// Represents the rights that are dependant on the committees.
        /// </summary>
        Dictionary<int, List<Enum_UserRights>> committeeRights { get; set; }


        public UserRightsContainer()
        {
            completeRights = new List<Enum_UserRights>();
            committeeRights = new Dictionary<int, List<Enum_UserRights>>();
        }

        /// <summary>
        /// Returns true if user has a right, independant on the TB.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public bool HasRight(Enum_UserRights right)
        {
            return HasRight(right, null);
        }

        /// <summary>
        /// Returns true of a user has the right indicated in input.
        /// 
        /// If second parameter is null, system will seek only in the rights
        /// that are independent of the committes. 
        /// Else, system will seek in the rights related to the TBs.
        /// </summary>
        /// <param name="right"></param>
        /// <param name="committeeId"></param>
        /// <returns></returns>
        public bool HasRight(Enum_UserRights right, int? committeeId)
        {
            if (completeRights.Contains(right))
                return true;

            if (committeeId.HasValue)
            {
                int id = committeeId.Value;
                if (committeeRights.ContainsKey(id) && committeeRights[id] != null)
                    return committeeRights[id].Contains(right);
            }
            return false;
        }

        /// <summary>
        /// Adds a right for a user, indepdently of the committee.
        /// </summary>
        /// <param name="right"></param>
        public void AddRight(Enum_UserRights right)
        {
            AddRight(right, null);
        }

        /// <summary>
        /// Adds a right to the user for the given committee.
        /// 
        /// If committee is null, adds a right independently of any committee.
        /// </summary>
        /// <param name="right"></param>
        /// <param name="committeeId"></param>
        public void AddRight(Enum_UserRights right, int? committeeId)
        {
            if (!committeeId.HasValue && !completeRights.Contains(right))
            {
                completeRights.Add(right);
            }
            else
            {
                int id = committeeId.Value;
                if (!committeeRights.ContainsKey(id) || committeeRights[id] == null)
                    committeeRights[id] = new List<Enum_UserRights>();

                if (!committeeRights[id].Contains(right))
                    committeeRights[id].Add(right);
            }

        }

        /// <summary>
        /// Removes a right from the general rights of the users
        /// Removes the rights for the committees right as well if removeFromCommittees is true.
        /// </summary>
        /// <param name="right"></param>
        /// <param name="removeFromCommittees"></param>
        public void RemoveRight(Enum_UserRights right, bool removeFromCommittees)
        {
            RemoveRight(right, null);

            if (removeFromCommittees)
            {
                foreach (var key in committeeRights.Keys)
                {
                    RemoveRight(right, key);
                }
            }
        }

        /// <summary>
        /// Removes a right from the committee related rights of the user.
        /// </summary>
        /// <param name="right"></param>
        /// <param name="committee"></param>
        public void RemoveRight(Enum_UserRights right, int? committee)
        {
            if (!committee.HasValue)
                completeRights.Remove(right);
            else
            {
                int id = committee.Value;
                if (committeeRights.ContainsKey(id) && committeeRights[id] != null)
                {
                    committeeRights[id].Remove(right);
                }
            }
        }

    }
}
