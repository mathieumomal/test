using System;
using System.IO;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Versions.QualityChecks;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Business.QualityChecks
{
    public class DocDocumentManagerTests : BaseEffortTest
    {
        private readonly string _uploadPath = Environment.CurrentDirectory + "\\TestData\\QualityChecks\\";
        
        /// <summary>
        /// These tests have been disabled because:
        /// - too long 
        /// - require DCOM object installed on the execution environment
        /// But could be executed for development purpose if necessary
        /// </summary>
        private readonly bool _enabledtests = false;
        
        [Test]
        public void Constructor_Doc()
        {
            if (_enabledtests)
            {
                var initialFilePath = _uploadPath + "DOC\\XXX.doc";
                using (var docDocumentManager =
                                    ManagerFactory.ResolveWithString<IDocDocumentManager>(initialFilePath))
                {
                    Assert.IsNotNull(docDocumentManager.Word);
                    Assert.IsNotNull(docDocumentManager.MemoryStream);
                    Assert.IsNotNull(docDocumentManager.WordProcessingDocument);
                    Assert.AreEqual(".doc", docDocumentManager.Extension);
                    Assert.AreEqual(initialFilePath, docDocumentManager.PathOfInitialFileToAnalyse);
                    Assert.AreEqual(initialFilePath + "x", docDocumentManager.PathOfDocxToAnalyse);
                    Assert.IsTrue(File.Exists(initialFilePath + "x"));
                }

                Assert.IsFalse(File.Exists(initialFilePath + "x"));
            }
        }

        [Test]
        public void Constructor_Docx()
        {
            if (_enabledtests)
            {
                var initialFilePath = _uploadPath + "DOCX\\XXX.docx";
                using (var docDocumentManager =
                    ManagerFactory.ResolveWithString<IDocDocumentManager>(initialFilePath))
                {
                    Assert.IsNotNull(docDocumentManager.Word);
                    Assert.IsNotNull(docDocumentManager.MemoryStream);
                    Assert.IsNotNull(docDocumentManager.WordProcessingDocument);
                    Assert.AreEqual(".docx", docDocumentManager.Extension);
                    Assert.AreEqual(initialFilePath, docDocumentManager.PathOfInitialFileToAnalyse);
                    Assert.AreEqual(initialFilePath + "_temp", docDocumentManager.PathOfDocxToAnalyse);
                    Assert.IsTrue(File.Exists(initialFilePath + "_temp"));
                }

                Assert.IsFalse(File.Exists(initialFilePath + "temp"));
            }
        }
    }
}
