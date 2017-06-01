using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Etsi.Ultimate.Business
{
    public static class ExtensionLogger
    {
        #region Exception
        /// <summary>
        /// Method to log exceptions and get a maximum of information about it
        /// LIMIT: Please provide only parameters for which you have control on it and which does not contain circular references. 
        /// For example, avoid .NET framework objects => These kind of objects are too heavy and/or could have circular references. 
        /// 
        /// WARNING: So, if you need to pass complex object -> test it. Actually, system could not be able to serialize them because of references loop... 
        /// In that case, just path yourobject.ToString() by overiding this method.
        /// Example: SpecVersion,...
        /// </summary>
        /// <param name="e"></param>
        /// <param name="objs"></param>
        /// <param name="message"></param>
        /// <param name="myClass"></param>
        /// <param name="myMethod"></param>
        public static void Exception(Exception e, List<object> objs, string myClass = "", string myMethod = "", string message = "")
        {
            LogManager.Error(string.Format("### EXCEPTION ###     CLASS: {0}, METHOD: {1}, MESSAGE: {2}", myClass, myMethod, message), e);
            LogManager.Error("### EXCEPTION ###     -> Please find entry parameters of the method which failed:");

            var index = 1;
            var serializer = new JavaScriptSerializer();
            serializer.RecursionLimit = 20;

            if (objs != null && objs.Count > 0)
            {
                foreach (var obj in objs)
                {
                    try
                    {
                        var json = serializer.Serialize(ToStringComplexObject(obj));
                        LogManager.Info(string.Format("### EXCEPTION ###    PARAM num {0}, VALUE: {1}", index, json));
                    }
                    catch (Exception ex)
                    {
                        //Voluntary not correctly logging error here => Message is sufficiant.
                        LogManager.Debug(string.Format("### EXCEPTION ###    (Serialization impossible for this object (NUM: {0}, ERROR: {1}, OBJECT TYPE: {2}). Please contact dev team to fix the problem.)", index, ex.Message, GetObjectType(obj)));
                    }
                    finally
                    {
                        index++;
                    }
                }
            }
        }
        #endregion

        #region Info
        /// <summary>
        /// WARNING: If you need to pass complex object -> test it. Actually, system could not be able to serialize them because of references loop... 
        /// In that case, just path yourobject.ToString() by overiding this method.  
        /// 
        /// Example: SpecVersion,...
        /// </summary>
        /// <param name="message"></param>
        /// <param name="objs"></param>
        public static void Info(string message, List<KeyValuePair<string, object>> objs)
        {
            LogManager.Info(message);

            var index = 1;
            var serializer = new JavaScriptSerializer();
            serializer.RecursionLimit = 20;

            if (objs != null && objs.Count > 0)
            {
                foreach (var obj in objs)
                {
                    try
                    {
                        var json = serializer.Serialize(ToStringComplexObject(obj.Value));
                        LogManager.Info(string.Format("    PARAM num {0}, KEY: {1}, VALUE: {2}", index, obj.Key, json));
                    }
                    catch (Exception ex)
                    {
                        //Voluntary not correctly logging error here => Message is sufficiant.
                        LogManager.Debug(string.Format("    (Serialization impossible for this object (NUM: {0}, KEY: {1}, ERROR: {2}, OBJECT TYPE: {3}). Please contact dev team to fix the problem.)", index, obj.Key, ex.Message, GetObjectType(obj)));
                    }
                    finally
                    {
                        index++;
                    }
                }
            }
        }
        #endregion

        #region Complex objects
        public static object ToStringComplexObject(object obj)
        {
            if (obj == null)
                return "[OBJECT IS NULL]";

            if (obj is SpecVersion
                || obj is Specification
                || obj is WorkItem
                || obj is Release
                || obj is ChangeRequest)
            {
                return obj.ToString();
            }
            return obj;
        }

        private static string GetObjectType(object obj)
        {
            if (obj != null
                && obj.GetType() != null
                && obj.GetType().BaseType != null
                && obj.GetType().BaseType.Name != null)
                return obj.GetType().FullName + obj.GetType().BaseType.Name;
            return "NULL";
        }
        #endregion
    }
}
