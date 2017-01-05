using System;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using Etsi.Ultimate.Utils.Core;
using Microsoft.Office.Interop.Word;

namespace Etsi.Ultimate.Business.Versions.QualityChecks
{
    /// <summary>
    /// Contains all necessary object to analyse doc/docx file
    /// 
    /// INFORMATION:
    /// If you have any problem inside a specific environment like 'word application refused to open a doc':
    /// Please change DCOM config / Word / Propriety / Identity -> to interactive user. 
    /// By this way the environment will display warning popups and you will be able to define what is the problem. 
    /// </summary>
    public class DocDocumentManager : IDocDocumentManager
    {
        #region properties

        public Application Word { get; set; }

        public MemoryStream MemoryStream { get; set; }

        public WordprocessingDocument WordProcessingDocument { get; set; }

        public string Extension { get; set; }

        public string PathOfDocxToAnalyse { get; set; }

        public string PathOfInitialFileToAnalyse { get; set; }

        #endregion

        #region constructor
        /// <summary>
        /// Get all necessary object to be able to analyse the doc or docx document
        /// </summary>
        /// <param name="myVar">Initial path of the document</param>
        public DocDocumentManager(string myVar)
        {
            LogManager.Debug("DocDocumentManager - Init document: " + myVar);
            PathOfInitialFileToAnalyse = myVar;

            // Get initial file info
            var initialFileInfo = new FileInfo(PathOfInitialFileToAnalyse);
            Extension = initialFileInfo.Extension;
            if (Extension == null)
                throw new Exception("Invalid doc document extension");

            // If file is doc, convert to docx and get the path
            if (Extension.ToLower().Equals(".doc"))//doc
            {
                LogManager.Debug("DocDocumentManager - Document is a doc ");
                var word = new Application ();
                var document = word.Documents.OpenNoRepairDialog(initialFileInfo.FullName, ReadOnly: true);
                PathOfDocxToAnalyse = ConvertDocToDocx(document, initialFileInfo, PathOfInitialFileToAnalyse);
                CloseWord(word);
            }
            else//docx
            {
                LogManager.Debug("DocDocumentManager - Document is a docx");
                //Copy docx to be able to open two copy of the file
                File.Copy(myVar, PathOfInitialFileToAnalyse + "_temp");
                PathOfDocxToAnalyse = PathOfInitialFileToAnalyse + "_temp";
            }

            GetWordprocessingDocument(PathOfDocxToAnalyse);

            LogManager.Debug("DocDocumentManager - Opening doc...");
            // Open document with word (usefull for some analyses)
            Word = new Application ();
            Word.Documents.OpenNoRepairDialog(initialFileInfo.FullName, ReadOnly: true);
            LogManager.Debug("DocDocumentManager - Doc open");
        }

        #endregion

        #region logic

        public void GetWordprocessingDocument(string path)
        {
            LogManager.Debug("DocDocumentManager - Opening docx thanks to OpenXML library...");
            // Get memoryStream of the docx document
            var stream = new FileStream(path, FileMode.Open);
            MemoryStream = new MemoryStream();
            stream.CopyTo(MemoryStream);
            stream.Close();

            // Get WordProcessingDocument of the docx document
            WordProcessingDocument = WordprocessingDocument.Open(MemoryStream, false);
            LogManager.Debug("DocDocumentManager - Docx open thanks to OpenXML library");
        }

        public string ConvertDocToDocx(Document doc, FileInfo fileinfo, string path)
        {
            LogManager.Debug("DocDocumentManager - Converting doc to docx...");
            var newPathForDocx = Path.ChangeExtension(fileinfo.FullName, ".docx");
            doc.SaveAs(newPathForDocx, WdSaveFormat.wdFormatXMLDocument);
            LogManager.Debug("DocDocumentManager - Doc converted as docx...");

            return Path.ChangeExtension(path, ".docx");
        }

        #endregion

        #region IDisposabe

        private void CloseWord(Application word)
        {
            LogManager.Debug("DocDocumentManager - Closing word application [ActiveDocument close]...");
            if (word != null && word.ActiveDocument != null)
            {
                word.ActiveDocument.Close(false);
            }
            LogManager.Debug("DocDocumentManager - Closing word application [Application close]...");
            if (word != null)
            {
                word.Quit(false);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(word);
                word = null;
                GC.Collect();
            }
            LogManager.Debug("DocDocumentManager - Word application closed");
        }
        public void Dispose()
        {
            CloseWord(Word);

            LogManager.Debug("DocDocumentManager - Closing docx stream and word processing objects...");
            if (MemoryStream != null)
            {
                MemoryStream.Close();
            }
            if (WordProcessingDocument != null)
            {
                WordProcessingDocument.Close();
            }
            LogManager.Debug("DocDocumentManager -Docx stream and word processing objects closed");
            LogManager.Debug("DocDocumentManager - Removing temporary docx...");
            if(File.Exists(PathOfDocxToAnalyse))
                File.Delete(PathOfDocxToAnalyse);
            LogManager.Debug("DocDocumentManager - Temporary docx removed");
        }
        #endregion
    }

    public interface IDocDocumentManager : IDisposable
    {
        Application Word { get; set; }
        MemoryStream MemoryStream { get; set; }
        WordprocessingDocument WordProcessingDocument { get; set; }
        string Extension { get; set; }
        string PathOfDocxToAnalyse { get; set; }
        string PathOfInitialFileToAnalyse { get; set; }
    }
}
