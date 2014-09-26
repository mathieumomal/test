using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Repositories;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Etsi.Ultimate.Business
{
    public class ChangeRequestManager : IChangeRequestManager
    {
        /// <summary>
        /// Gets or sets the uoW.
        /// </summary>
        public IUltimateUnitOfWork UoW { get; set; }
        private const string CacheKey = "ULT_BIZ_CHANGEREQUESTCATEGORY_ALL";

        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Primary key of newly inserted change request</returns>
        public bool CreateChangeRequest(int personId, ChangeRequest changeRequest)
        {
            var isSuccess = true;
            try
            {
                changeRequest.CRNumber = GenerateCrNumber(changeRequest.Fk_Specification);
                var repo = RepositoryFactory.Resolve<IChangeRequestRepository>();
                repo.UoW = UoW;
                repo.InsertOrUpdate(changeRequest);
            }
            catch (Exception)
            {
                //LogManager.Error("[Business] Failed to create change request: " + ex.Message);
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// Generates the cr number.
        /// </summary>
        /// <param name="specificationId">The specification identifier.</param>
        /// <returns>Cr number</returns>
        public string GenerateCrNumber(int? specificationId)
        {
            List<int> crNumberList = new List<int>();
            var alphaCharacter = string.Empty;
            var heighestCrNumber = 0;
            if (specificationId > 0)
            {
                var repo = RepositoryFactory.Resolve<IChangeRequestRepository>();
                repo.UoW = UoW;
                var crNumber = repo.FindBySpecificationId(specificationId);
                crNumber.ForEach(x => crNumberList.Add(GetCrNumber(x)));
                heighestCrNumber = crNumberList.Max();
                alphaCharacter = GetAlphaCharacter(crNumberList, alphaCharacter, heighestCrNumber, crNumber);
                heighestCrNumber++;
            }
            return new StringBuilder().Append(alphaCharacter).Append(heighestCrNumber.ToString(new String('0', 4))).ToString();
        }

        /// <summary>
        /// Gets the alpha character.
        /// </summary>
        /// <param name="crNumberList">The cr number list.</param>
        /// <param name="Alpha">The alpha.</param>
        /// <param name="heighestCrNumber">The heighest cr number.</param>
        /// <param name="crNumber">The cr number.</param>
        /// <returns>alpha character</returns>
        private static string GetAlphaCharacter(List<int> crNumberList, string alphaCharacter, int heighestCrNumber, List<string> crNumber)
        {
            var alphaIndex = crNumberList.IndexOf(heighestCrNumber);
            Regex regex = new Regex("[^a-zA-Z]");
            alphaCharacter = regex.Replace(crNumber[alphaIndex], "");
            return alphaCharacter;
        }
        /// <summary>
        /// Gets the number.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>Numeric number</returns>
        private int GetCrNumber(string strCrNumber)
        {
            var crNumericNumber = 0;
            if (!String.IsNullOrEmpty(strCrNumber))
            {
                Regex regex = new Regex("[^0-9]");
                Int32.TryParse(regex.Replace(strCrNumber, ""), out crNumericNumber);
            }
            return crNumericNumber;
        }

        /// <summary>
        /// Gets the change request categories.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <returns>
        /// CR Category list
        /// </returns>
        public List<Enum_CRCategory> GetChangeRequestCategories(int personId)
        {
            //cheeck cache data need to check User right
            var cachedData = (List<Enum_CRCategory>)CacheManager.Get(CacheKey);
            try
            {
                if (cachedData == null)
                {
                    var repo = RepositoryFactory.Resolve<IEnum_CrCategoryRepository>();
                    repo.UoW = UoW;
                    cachedData = repo.All.ToList();

                    if (CacheManager.Get(CacheKey) == null)
                    {
                        CacheManager.Insert(CacheKey, cachedData);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("[Business] Failed to GetChangeRequestCategories:" + ex.Message);
            }
            return cachedData;
        }
    }

    /// <summary>
    /// IChangeRequestManager
    /// </summary>
    public interface IChangeRequestManager
    {
        /// <summary>
        /// Gets or sets the uoW.
        /// </summary>
        IUltimateUnitOfWork UoW { get; set; }

        /// <summary>
        /// Creates the change request.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <param name="changeRequest">The change request.</param>
        /// <returns>Primary key of newly inserted change request</returns>
        bool CreateChangeRequest(int personId, ChangeRequest changeRequest);

        /// <summary>
        /// Gets the change request categories.
        /// </summary>
        /// <param name="personId">The person identifier.</param>
        /// <returns>CR Category list</returns>
        List<Enum_CRCategory> GetChangeRequestCategories(int personId);
    }
}
