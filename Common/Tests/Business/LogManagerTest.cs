using Etsi.Ultimate.DomainClasses;
using Etsi.Ultimate.Utils;
using Etsi.Ultimate.Utils.Core;
using log4net.Appender;
using log4net.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Etsi.Ultimate.Tests.Business
{
    class LogManagerTest: BaseTest
    {
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

        [Test]
        public void LogManager_Debug()
        {
            Exception testException = new Exception();
            DateTime dateValue= new DateTime(2009, 9, 1, 18, 32, 0);
            CultureInfo frCulture = new CultureInfo("fr-Fr");

            LogManager.Debug("Debug Log 1");
            LogManager.Debug("Debug Log 2", testException);
            LogManager.DebugFormat("Debug Format {0} {1}", "Value 1", "Value 2");
            LogManager.DebugFormat(frCulture, "{0:D}", dateValue);

            LoggingEvent[] events = memoryAppender.GetEvents();

            Assert.AreEqual(4, events.Length);

            Assert.AreEqual("Debug Log 1", events[0].MessageObject.ToString());
            Assert.AreEqual(Level.Debug, events[0].Level);

            Assert.AreEqual("Debug Log 2", events[1].MessageObject.ToString());
            Assert.AreEqual(Level.Debug, events[1].Level);
            Assert.AreEqual(testException, events[1].ExceptionObject);

            Assert.AreEqual("Debug Format Value 1 Value 2", events[2].MessageObject.ToString());
            Assert.AreEqual(Level.Debug, events[2].Level);

            Assert.AreEqual("mardi 1 septembre 2009", events[3].MessageObject.ToString());
            Assert.AreEqual(Level.Debug, events[3].Level);
        }

        [Test]
        public void LogManager_Error()
        {
            Exception testException = new Exception();
            double value = 9164.32;
            CultureInfo enCulture = new CultureInfo("en-US");

            LogManager.Error("Error Log 1");
            LogManager.Error("Error Log 2", testException);
            LogManager.ErrorFormat("Error Format {0} {1}", "Value 1", "Value 2");
            LogManager.ErrorFormat(enCulture, "{0:N}", value);

            LoggingEvent[] events = memoryAppender.GetEvents();

            Assert.AreEqual(4, events.Length);

            Assert.AreEqual("Error Log 1", events[0].MessageObject.ToString());
            Assert.AreEqual(Level.Error, events[0].Level);

            Assert.AreEqual("Error Log 2", events[1].MessageObject.ToString());
            Assert.AreEqual(Level.Error, events[1].Level);
            Assert.AreEqual(testException, events[1].ExceptionObject);

            Assert.AreEqual("Error Format Value 1 Value 2", events[2].MessageObject.ToString());
            Assert.AreEqual(Level.Error, events[2].Level);

            Assert.AreEqual("9,164.32", events[3].MessageObject.ToString());
            Assert.AreEqual(Level.Error, events[3].Level);
        }

        [Test]
        public void LogManager_Fatal()
        {
            Exception testException = new Exception();

            LogManager.Fatal("Fatal Log 1");
            LogManager.Fatal("Fatal Log 2", testException);
            LogManager.FatalFormat("Fatal Format {0} {1}", "Value 1", "Value 2");

            LoggingEvent[] events = memoryAppender.GetEvents();

            Assert.AreEqual(3, events.Length);

            Assert.AreEqual("Fatal Log 1", events[0].MessageObject.ToString());
            Assert.AreEqual(Level.Fatal, events[0].Level);

            Assert.AreEqual("Fatal Log 2", events[1].MessageObject.ToString());
            Assert.AreEqual(Level.Fatal, events[1].Level);
            Assert.AreEqual(testException, events[1].ExceptionObject);

            Assert.AreEqual("Fatal Format Value 1 Value 2", events[2].MessageObject.ToString());
            Assert.AreEqual(Level.Fatal, events[2].Level);
        }

        [Test]
        public void LogManager_Info()
        {
            Exception testException = new Exception();
            DateTime dateValue = new DateTime(2009, 9, 1, 18, 32, 0);
            CultureInfo frCulture = new CultureInfo("fr-Fr");

            LogManager.Info("Info Log 1");
            LogManager.Info("Info Log 2", testException);
            LogManager.InfoFormat("Info Format {0} {1}", "Value 1", "Value 2");
            LogManager.InfoFormat(frCulture, "{0:D}", dateValue);

            LoggingEvent[] events = memoryAppender.GetEvents();

            Assert.AreEqual(4, events.Length);

            Assert.AreEqual("Info Log 1", events[0].MessageObject.ToString());
            Assert.AreEqual(Level.Info, events[0].Level);

            Assert.AreEqual("Info Log 2", events[1].MessageObject.ToString());
            Assert.AreEqual(Level.Info, events[1].Level);
            Assert.AreEqual(testException, events[1].ExceptionObject);

            Assert.AreEqual("Info Format Value 1 Value 2", events[2].MessageObject.ToString());
            Assert.AreEqual(Level.Info, events[2].Level);

            Assert.AreEqual("mardi 1 septembre 2009", events[3].MessageObject.ToString());
            Assert.AreEqual(Level.Info, events[3].Level);
        }

        [Test]
        public void LogManager_Trace()
        {
            Exception testException = new Exception();
            DateTime dateValue = new DateTime(2009, 9, 1, 18, 32, 0);
            CultureInfo frCulture = new CultureInfo("fr-Fr");

            LogManager.Trace("Trace Log 1");
            LogManager.Trace("Trace Log 2", testException);
            LogManager.TraceFormat("Trace Format {0} {1}", "Value 1", "Value 2");
            LogManager.TraceFormat(frCulture, "{0:D}", dateValue);

            LoggingEvent[] events = memoryAppender.GetEvents();

            Assert.AreEqual(4, events.Length);

            Assert.AreEqual("Trace Log 1", events[0].MessageObject.ToString());
            Assert.AreEqual(Level.Trace, events[0].Level);

            Assert.AreEqual("Trace Log 2", events[1].MessageObject.ToString());
            Assert.AreEqual(Level.Trace, events[1].Level);
            Assert.AreEqual(testException, events[1].ExceptionObject);

            Assert.AreEqual("Trace Format Value 1 Value 2", events[2].MessageObject.ToString());
            Assert.AreEqual(Level.Trace, events[2].Level);

            Assert.AreEqual("mardi 1 septembre 2009", events[3].MessageObject.ToString());
            Assert.AreEqual(Level.Trace, events[3].Level);
        }

        [Test]
        public void LogManager_Warn()
        {
            Exception testException = new Exception();
            DateTime dateValue = new DateTime(2009, 9, 1, 18, 32, 0);
            CultureInfo frCulture = new CultureInfo("fr-Fr");

            LogManager.Warn("Warn Log 1");
            LogManager.Warn("Warn Log 2", testException);
            LogManager.WarnFormat("Warn Format {0} {1}", "Value 1", "Value 2");
            LogManager.WarnFormat(frCulture, "{0:D}", dateValue);

            LoggingEvent[] events = memoryAppender.GetEvents();

            Assert.AreEqual(4, events.Length);

            Assert.AreEqual("Warn Log 1", events[0].MessageObject.ToString());
            Assert.AreEqual(Level.Warn, events[0].Level);

            Assert.AreEqual("Warn Log 2", events[1].MessageObject.ToString());
            Assert.AreEqual(Level.Warn, events[1].Level);
            Assert.AreEqual(testException, events[1].ExceptionObject);

            Assert.AreEqual("Warn Format Value 1 Value 2", events[2].MessageObject.ToString());
            Assert.AreEqual(Level.Warn, events[2].Level);

            Assert.AreEqual("mardi 1 septembre 2009", events[3].MessageObject.ToString());
            Assert.AreEqual(Level.Warn, events[3].Level);
        }
    }
}
