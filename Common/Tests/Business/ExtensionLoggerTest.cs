using Etsi.Ultimate.Business;
using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Utils.Core;
using log4net.Appender;
using log4net.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etsi.Ultimate.Tests.Business
{
    public class ExtensionLoggerTest : BaseTest
    {
        #region Init methods
        MemoryAppender memoryAppender;

        [TestFixtureSetUp]
        public void Init()
        {
            string configFileName = Directory.GetCurrentDirectory() + "\\TestData\\LogManager\\Test.log4net.config";
            LogManager.SetConfiguration(configFileName, "TestLogger");
        }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            memoryAppender = ((log4net.Core.LoggerWrapperImpl)(LogManager.Logger)).Logger.Repository.GetAppenders()[0] as MemoryAppender;
            memoryAppender.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            memoryAppender.Clear();
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            LogManager.SetConfiguration(String.Empty, String.Empty);
        }
        #endregion

        #region Exception
        [Test(Description = "Log exception with simple objects")]
        public void LogManager_Exception()
        {
            var ex = new Exception("ERROR");
            ExtensionLogger.Exception(ex,
                new List<object> { 
                    1, 
                    "test", 
                    null, 
                    new List<string>{ "test", null }, 
                    new KeyValuePair<string, int>("test", 2), 
                    new List<float>() 
                },
                "MYCLASS", "MYMETHOD", "MYMESSAGE");

            LoggingEvent[] events = memoryAppender.GetEvents();

            Assert.AreEqual(8, events.Length);

            var log = events[0];
            Assert.AreEqual("### EXCEPTION ###     CLASS: MYCLASS, METHOD: MYMETHOD, MESSAGE: MYMESSAGE", log.MessageObject.ToString());
            Assert.AreEqual(Level.Error, log.Level);
            Assert.AreEqual(ex, log.ExceptionObject);

            log = events[1];
            Assert.AreEqual("### EXCEPTION ###     -> Please find entry parameters of the method which failed:", log.MessageObject.ToString());
            Assert.AreEqual(Level.Error, log.Level);
            Assert.IsNull(log.ExceptionObject);

            log = events[2];
            Assert.AreEqual("### EXCEPTION ###    PARAM num 1, VALUE: 1", log.MessageObject.ToString());
            Assert.AreEqual(Level.Info, log.Level);
            Assert.IsNull(log.ExceptionObject);

            log = events[3];
            Assert.AreEqual("### EXCEPTION ###    PARAM num 2, VALUE: \"test\"", log.MessageObject.ToString());
            Assert.AreEqual(Level.Info, log.Level);
            Assert.IsNull(log.ExceptionObject);

            log = events[4];
            Assert.AreEqual("### EXCEPTION ###    PARAM num 3, VALUE: \"[OBJECT IS NULL]\"", log.MessageObject.ToString());
            Assert.AreEqual(Level.Info, log.Level);
            Assert.IsNull(log.ExceptionObject);

            log = events[5];
            Assert.AreEqual("### EXCEPTION ###    PARAM num 4, VALUE: [\"test\",null]", log.MessageObject.ToString());
            Assert.AreEqual(Level.Info, log.Level);
            Assert.IsNull(log.ExceptionObject);

            log = events[6];
            Assert.AreEqual("### EXCEPTION ###    PARAM num 5, VALUE: {\"Key\":\"test\",\"Value\":2}", log.MessageObject.ToString());
            Assert.AreEqual(Level.Info, log.Level);
            Assert.IsNull(log.ExceptionObject);

            log = events[7];
            Assert.AreEqual("### EXCEPTION ###    PARAM num 6, VALUE: []", log.MessageObject.ToString());
            Assert.AreEqual(Level.Info, log.Level);
            Assert.IsNull(log.ExceptionObject);
        }
        #endregion

        #region Info
        [Test(Description = "Log INFO with objects")]
        public void LogManager_InfoWithArguments()
        {
            ExtensionLogger.Info("MESSAGE", new List<KeyValuePair<string, object>>{
                new KeyValuePair<string,object>("int", 1),
                new KeyValuePair<string,object>("string", "test"),
                new KeyValuePair<string,object>("null", null),
                new KeyValuePair<string,object>("liststring", new List<string>{ "test", null }),
                new KeyValuePair<string,object>("keyvaluepair", new KeyValuePair<string, int>("test", 2)),
                new KeyValuePair<string,object>("listfloat", new List<float>())
            });

            LoggingEvent[] events = memoryAppender.GetEvents();

            Assert.AreEqual(7, events.Length);

            var log = events[0];
            Assert.AreEqual("MESSAGE", log.MessageObject.ToString());
            Assert.AreEqual(Level.Info, log.Level);

            log = events[1];
            Assert.AreEqual("    PARAM num 1, KEY: int, VALUE: 1", log.MessageObject.ToString());
            Assert.AreEqual(Level.Info, log.Level);

            log = events[2];
            Assert.AreEqual("    PARAM num 2, KEY: string, VALUE: \"test\"", log.MessageObject.ToString());
            Assert.AreEqual(Level.Info, log.Level);

            log = events[3];
            Assert.AreEqual("    PARAM num 3, KEY: null, VALUE: \"[OBJECT IS NULL]\"", log.MessageObject.ToString());
            Assert.AreEqual(Level.Info, log.Level);

            log = events[4];
            Assert.AreEqual("    PARAM num 4, KEY: liststring, VALUE: [\"test\",null]", log.MessageObject.ToString());
            Assert.AreEqual(Level.Info, log.Level);

            log = events[5];
            Assert.AreEqual("    PARAM num 5, KEY: keyvaluepair, VALUE: {\"Key\":\"test\",\"Value\":2}", log.MessageObject.ToString());
            Assert.AreEqual(Level.Info, log.Level);

            log = events[6];
            Assert.AreEqual("    PARAM num 6, KEY: listfloat, VALUE: []", log.MessageObject.ToString());
            Assert.AreEqual(Level.Info, log.Level);
        }
        #endregion

        #region ToStringComplexObjects
        private IEnumerable<object[]> ComplexObjects
        {
            get
            {
                yield return new object[] { new SpecVersion { Pk_VersionId = 1 }, "ID: 1, Version: 0.0.0, Release: , Spec: , WKI: " };
                yield return new object[] { new Specification { Pk_SpecificationId = 1 }, "ID: 1, Number: " };
                yield return new object[] { new WorkItem { Pk_WorkItemUid = 1, WorkplanId = 2 }, "ID: 1, WorkplanId: 2, Acronym: " };
                yield return new object[] { new Release { Pk_ReleaseId = 1 }, "ID: 1, Shortname: , Code: " };
                yield return new object[] { new ChangeRequest { Pk_ChangeRequest = 1 }, "ID: 1, CRNumber: , Revision: , Release: , Spec: " };
                yield return new object[] { null, "[OBJECT IS NULL]" };
                yield return new object[] { 1, 1 };
                yield return new object[] { "text", "text" };
            }
        }
        [Test, TestCaseSource("ComplexObjects")]
        public void ToStringComplexObject(object obj, object expectedResult)
        {
            Assert.AreEqual(expectedResult, ExtensionLogger.ToStringComplexObject(obj));
        }
        #endregion
    }
}
