using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace SyncService
{
    /// <summary>
    /// Validates a username and password.
    /// </summary>
    public class ServiceCredentialValidator : UserNamePasswordValidator
    {
        #region Variables

        private string _userName = "dnn3gpp";
        private string _password = "439FA15A-8EF2-4479-B15B-F0A063B94DB2"; 

        #endregion

        #region Override Methods (UserNamePasswordValidator)

        /// <summary>
        /// Validates the specified username and password.
        /// </summary>
        /// <param name="userName">The username to validate.</param>
        /// <param name="password">The password to validate.</param>
        public override void Validate(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
            {
                throw new SecurityTokenException("Username and password is required");
            }

            if (!(userName == _userName && password == _password))
            {
                throw new SecurityTokenException("Invalid Username or Password");
            }
        }

        #endregion
    }
}