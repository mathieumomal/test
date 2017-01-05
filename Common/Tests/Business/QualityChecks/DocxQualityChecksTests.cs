using System;
using Etsi.Ultimate.Business;
using Etsi.Ultimate.Business.Versions.Interfaces;
using Etsi.Ultimate.Business.Versions.QualityChecks;
using NUnit.Framework;

namespace Etsi.Ultimate.Tests.Business.QualityChecks
{
    public class DocxQualityChecksTests : BaseEffortTest
    {
        private readonly string _uploadPath = Environment.CurrentDirectory + "\\TestData\\QualityChecks\\DOCS\\";

        private DocDocumentManager _docDocumentMgr;

        /// <summary>
        /// These tests have been disabled because:
        /// - too long 
        /// - require DCOM object installed on the execution environment
        /// But could be executed for development purpose if necessary
        /// </summary>
        private readonly bool _enabledtests = false;

        [TearDown]
        public override void TearDown()
        {
            if(_docDocumentMgr != null)
                _docDocumentMgr.Dispose();
        }

        [Test]
        public void NominalCase()
        {
            if (_enabledtests)
            {
                _docDocumentMgr = new DocDocumentManager(_uploadPath + "SuccessFile.docx");
                var biz = new DocXQualityChecks(_docDocumentMgr);
                var hasTrackedRevisions = biz.HasTrackedRevisions();
                var changeHistoryTableIsTheLastOne = biz.ChangeHistoryTableIsTheLastOne();
                var isHistoryVersionCorrect = biz.IsHistoryVersionCorrect("12.4.0");
                var isCoverPageVersionCorrect = biz.IsCoverPageVersionCorrect("12.4.0");
                var isCoverPageDateCorrect = biz.IsCoverPageDateCorrect(new DateTime(2016, 9, 1));
                var isCopyRightYearCorrect = biz.IsCopyRightYearCorrect(new DateTime(2016, 1, 9));//Expect 2016
                var isFirstTwoLinesOfTitleCorrect = biz.IsFirstTwoLinesOfTitleCorrect("Technical Specification Group Radio Access Network");
                var isTitleCorrect =
                    biz.IsTitleCorrect(
                        "User Equipment (UE) conformance specification; Radio transmission and reception (FDD); Part 2: Implementation Conformance Statement (ICS)");
                var isReleaseCorrect = biz.IsReleaseCorrect("Release 12");
                var isReleaseStyleCorrect = biz.IsReleaseStyleCorrect("Release 12");
                var isAutomaticNumberingPresent = biz.IsAutomaticNumberingPresent();
                //var isAnnexureStylesCorrect = biz.IsAnnexureStylesCorrect(true);

                Assert.IsFalse(hasTrackedRevisions);
                Assert.IsTrue(changeHistoryTableIsTheLastOne);
                Assert.IsTrue(isHistoryVersionCorrect);
                Assert.IsTrue(isCoverPageVersionCorrect);
                Assert.IsTrue(isCoverPageDateCorrect);
                Assert.IsTrue(isCopyRightYearCorrect);
                Assert.IsTrue(isFirstTwoLinesOfTitleCorrect);
                Assert.IsTrue(isTitleCorrect);
                Assert.IsTrue(isReleaseCorrect);
                Assert.IsTrue(isReleaseStyleCorrect);
                Assert.IsFalse(isAutomaticNumberingPresent);
                //Assert.IsTrue(isAnnexureStylesCorrect);
            }
        }

        [Test]
        public void HasTrackedRevisions()
        {
            if (_enabledtests)
            {
                var docDocumentMgr = ManagerFactory.ResolveWithString<IDocDocumentManager>(_uploadPath + "HasTrackedRevisions_ErrorCase.docx");
                var biz = ManagerFactory.ResolveWithIDocDocumentManager<IQualityChecks>(docDocumentMgr);
                var result = biz.HasTrackedRevisions();
                docDocumentMgr.Dispose();

                Assert.IsTrue(result);
            }
        }

        [Test]
        public void ChangeHistoryTableIsTheLastOne()
        {
            var docDocumentMgr = ManagerFactory.ResolveWithString<IDocDocumentManager>(_uploadPath + "ChangeHistoryTableIsTheLastOne_ErrorCase.docx");
            var biz = ManagerFactory.ResolveWithIDocDocumentManager<IQualityChecks>(docDocumentMgr);
            var result = biz.ChangeHistoryTableIsTheLastOne();//Change history table should not be the last one
            docDocumentMgr.Dispose();

            Assert.IsFalse(result);
        }

        [Test]
        public void ChangeHistoryTableIsTheLastOne_OkEvenIfChangeHistoryTitleIntegratedToTheTable()
        {
            var docDocumentMgr = ManagerFactory.ResolveWithString<IDocDocumentManager>(_uploadPath + "ChangeHistoryTableIsTheLastOne_SuccessCase_ChangeHistoryIntegratedInsideTable.docx");
            var biz = ManagerFactory.ResolveWithIDocDocumentManager<IQualityChecks>(docDocumentMgr);
            var result = biz.ChangeHistoryTableIsTheLastOne();//Change history table should be the last one but the 'Change history' text has been detected as a table header
            docDocumentMgr.Dispose();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsHistoryVersionCorrect()
        {
            var docDocumentMgr = ManagerFactory.ResolveWithString<IDocDocumentManager>(_uploadPath + "IsHistoryVersionCorrect_ErrorCase.docx");
            var biz = ManagerFactory.ResolveWithIDocDocumentManager<IQualityChecks>(docDocumentMgr);
            var result = biz.IsHistoryVersionCorrect("12.4.0");//Should be 12.4.1
            docDocumentMgr.Dispose();

            Assert.IsFalse(result);
        }

        [Test]
        public void IsCoverPageVersionCorrect()
        {
            var docDocumentMgr = ManagerFactory.ResolveWithString<IDocDocumentManager>(_uploadPath + "IsCoverPageVersionCorrect_ErrorCase.docx");
            var biz = ManagerFactory.ResolveWithIDocDocumentManager<IQualityChecks>(docDocumentMgr);
            var result = biz.IsCoverPageVersionCorrect("12.4.0");//Should be 12.4.1
            docDocumentMgr.Dispose();

            Assert.IsFalse(result);
        }

        [Test]
        public void IsCoverPageDateCorrect()
        {
            var docDocumentMgr = ManagerFactory.ResolveWithString<IDocDocumentManager>(_uploadPath + "IsCoverPageDateCorrect_ErrorCase.docx");
            var biz = ManagerFactory.ResolveWithIDocDocumentManager<IQualityChecks>(docDocumentMgr);
            var result = biz.IsCoverPageDateCorrect(new DateTime(2017, 1, 1));//Should be 2016 10
            docDocumentMgr.Dispose();

            Assert.IsFalse(result);
        }

        [Test]
        public void IsCopyRightYearCorrect()
        {
            var docDocumentMgr = ManagerFactory.ResolveWithString<IDocDocumentManager>(_uploadPath + "IsCopyRightYearCorrect_ErrorCase.docx");
            var biz = ManagerFactory.ResolveWithIDocDocumentManager<IQualityChecks>(docDocumentMgr);
            var result = biz.IsCopyRightYearCorrect(new DateTime(2018, 2, 1));//Should be absolutely 2018 but is 2016
            var resultBis = biz.IsCopyRightYearCorrect(new DateTime(2018, 1, 1));//Should be 2018 or 2017 (because january) but is 2016
            docDocumentMgr.Dispose();

            Assert.IsFalse(result);
            Assert.IsFalse(resultBis);
        }

        [Test]
        public void IsFirstTwoLinesOfTitleCorrect()
        {
            var docDocumentMgr = ManagerFactory.ResolveWithString<IDocDocumentManager>(_uploadPath + "IsFirstTwoLinesOfTitleCorrect_ErrorCase.docx");
            var biz = ManagerFactory.ResolveWithIDocDocumentManager<IQualityChecks>(docDocumentMgr);
            var result = biz.IsFirstTwoLinesOfTitleCorrect("Technical Specification Group Radio Access Network");//Should be TEST
            docDocumentMgr.Dispose();

            Assert.IsFalse(result);
        }

        [Test]
        public void IsTitleCorrect()
        {
            var docDocumentMgr = ManagerFactory.ResolveWithString<IDocDocumentManager>(_uploadPath + "IsTitleCorrect_ErrorCase.docx");
            var biz = ManagerFactory.ResolveWithIDocDocumentManager<IQualityChecks>(docDocumentMgr);
            var result = biz.IsTitleCorrect("TEST");//Should be 'MY SPEC TITLE'
            docDocumentMgr.Dispose();

            Assert.IsFalse(result);
        }

        [Test]
        public void IsReleaseCorrect()
        {
            var docDocumentMgr = ManagerFactory.ResolveWithString<IDocDocumentManager>(_uploadPath + "IsReleaseCorrect_ErrorCase.docx");
            var biz = ManagerFactory.ResolveWithIDocDocumentManager<IQualityChecks>(docDocumentMgr);
            var result = biz.IsReleaseCorrect("Release 12");//Is correct but release is before title '3rd generation partnership project;'
            var resultBis = biz.IsReleaseCorrect("Release 8");//Should be Release 12
            docDocumentMgr.Dispose();

            Assert.IsFalse(result);
            Assert.IsFalse(resultBis);
        }

        [Test]
        public void IsReleaseStyleCorrect()
        {
            var docDocumentMgr = ManagerFactory.ResolveWithString<IDocDocumentManager>(_uploadPath + "IsReleaseStyleCorrect_ErrorCase.docx");
            var biz = ManagerFactory.ResolveWithIDocDocumentManager<IQualityChecks>(docDocumentMgr);
            var result = biz.IsReleaseStyleCorrect("Release 12");//Should be nothing (ZGSM style has been removed)
            docDocumentMgr.Dispose();

            Assert.IsFalse(result);
        }

        [Test]
        public void IsAutomaticNumberingPresent_NumberedListFound()
        {
            var docDocumentMgr = ManagerFactory.ResolveWithString<IDocDocumentManager>(_uploadPath + "IsAutomaticNumberingPresent_ErrorCase.docx");
            var biz = ManagerFactory.ResolveWithIDocDocumentManager<IQualityChecks>(docDocumentMgr);
            var result = biz.IsAutomaticNumberingPresent();//Should find one numbered list
            docDocumentMgr.Dispose();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsAutomaticNumberingPresent_NumberedListNotFound()
        {
            var docDocumentMgr = ManagerFactory.ResolveWithString<IDocDocumentManager>(_uploadPath + "IsAutomaticNumberingPresent_SuccessCase.docx");
            var biz = ManagerFactory.ResolveWithIDocDocumentManager<IQualityChecks>(docDocumentMgr);
            var result = biz.IsAutomaticNumberingPresent();//Should not find numbered list
            docDocumentMgr.Dispose();

            Assert.IsFalse(result);
        }
        
        /*
        [Test]
        public void IsAnnexureStylesCorrect()
        {
            var docDocumentMgr = new DocDocumentManager(true);
            docDocumentMgr.GetWordprocessingDocument(_uploadPath + "IsAnnexureStylesCorrect_ErrorCase.docx");
            var biz = new DocXQualityChecks(docDocumentMgr);
            var result = biz.IsAnnexureStylesCorrect(true);//

            Assert.IsFalse(result);

            docDocumentMgr.Dispose();
        }
        */
    }
}
