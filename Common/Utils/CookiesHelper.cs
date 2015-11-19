using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using Etsi.Ultimate.Utils.Core;

namespace Etsi.Ultimate.Utils
{
    /// <summary>
    /// Cookies helper : to get and set cookies
    /// </summary>
    public static class CookiesHelper
    {
        #region public
        /// <summary>
        /// Set cookies with T object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetCookie<T>(HttpResponse response, string name, T value)
        {
            //Create a new cookie, passing the name into the constructor
            HttpCookie cookie = new HttpCookie(name);

            //Set the cookies value
            try
            {
                cookie.Value = Serialize(value);
            }
            catch (Exception e)
            {
                LogManager.Error(string.Format("CookiesHelper - SetCookie<T> : Not able to serialize cookie: {0}", name), e);
            }

            //Set the cookie to expire in 1 day
            var dtNow = DateTime.Now;
            var tsExpire = new TimeSpan(1, 0, 0, 0);
            cookie.Expires = dtNow + tsExpire;

            //Add the cookie
            response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Get cookies which contains T object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetCookie<T>(HttpRequest request, string name)
        {
            T result = default(T);

            //Grab the cookie
            HttpCookie cookie = request.Cookies[name];

            //Check to make sure the cookie exists
            if (cookie != null)
            {
                try
                {
                    result = Deserialize<T>(cookie.Value);
                }
                catch (Exception e)
                {
                    LogManager.Error(string.Format("CookiesHelper - GetCookie<T> : Not able to deserialize cookie: {0}", name), e);
                }
            }

            return result;
        }

        /// <summary>
        /// Test if browser support cookies
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool CookieSupported(HttpRequest request)
        {
            return request.Browser.Cookies;
        }
        #endregion

        #region private
        /// <summary>
        /// Serialize an object in Base64
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string Serialize<T>(T obj)
        {
            string result;
            Stream myStream = new MemoryStream();
            try
            {
                // Create a binary formatter and serialize the
                // myClass into the memorystream
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(myStream, obj);
                // Go to the beginning of the stream and
                // fill a byte array with the contents of the
                // memory stream
                myStream.Seek(0, SeekOrigin.Begin);
                byte[] buffer = new byte[myStream.Length];
                myStream.Read(buffer, 0, (int)myStream.Length);
                // Store the buffer as a base64 string in the cookie
                result = Convert.ToBase64String(buffer);
            }
            finally
            {
                // ... and remember to close the stream
                myStream.Close();
            }
            return result;
        }

        /// <summary>
        /// Deserialize an object in Base64
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="base64Value"></param>
        /// <returns></returns>
        private static T Deserialize<T>(string base64Value)
        {
            T result;

            // Convert the base64 string into a byte array
            byte[] buffer = Convert.FromBase64String(base64Value);
            // Create a memory stream from the byte array
            Stream myStream = new MemoryStream(buffer);
            try
            {
                // Create a binary formatter and deserialize the
                // contents of the memory stream into MyClass
                IFormatter formatter = new BinaryFormatter();
                result = (T)formatter.Deserialize(myStream);
            }
            finally
            {
                // ... and as always, close the stream
                myStream.Close();
            }

            return result;
        }
        #endregion
    }
}
