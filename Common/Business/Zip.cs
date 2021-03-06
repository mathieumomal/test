﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ionic.Zip;
using System.IO;


namespace Etsi.Ultimate.Business
{
    /// <summary>
    /// Compress file/Extract zip files
    /// </summary>
    public class Zip
    {
        /// <summary>
        /// Creates a zip file for the suppplied file path
        /// </summary>
        /// <param name="fileToCompress">Full path of the file to compress</param>
        /// <param name="outputPath">Full path of the output directory</param>
        public static void Compress(string fileToCompress, string outputPath)
        {
            using (var zip = new ZipFile())
            {
                zip.AddFile(fileToCompress, String.Empty);
                zip.Save(Path.Combine(outputPath, Path.GetFileNameWithoutExtension(fileToCompress) + ".zip"));
            }
        }

        /// <summary>
        /// Creates a zip file for the suppplied file path
        /// </summary>
        /// <param name="fileToCompress">Full path of the file to compress</param>
        /// <param name="outputPath">Full path of the output directory</param>
        public static void CompressSetOfFiles(string saveName, List<string> filesToCompress, string outputPath)
        {
            using (var zip = new ZipFile())
            {
                foreach (string fileToCompress in filesToCompress)
                {
                    zip.AddFile(fileToCompress, String.Empty);
                }
                zip.Save(outputPath + saveName + ".zip");
            }
        }

        /// <summary>
        /// Extract file(s) from a given zip file
        /// </summary>
        /// <param name="zipFilePath">Full path of the zip file</param>
        /// <param name="outputPath">Full path of the output directory</param>
        /// <param name="createOwnDirectory">true to create a seperate directory for the extracted files</param>
        public static List<string> Extract(string zipFilePath, string outputPath, bool createOwnDirectory)
        {
            var resultList = new List<string>();
            if (!string.IsNullOrEmpty(zipFilePath))
            {
                string FileName = Path.GetFileNameWithoutExtension(zipFilePath);
                if (outputPath == null) outputPath = Path.GetDirectoryName(zipFilePath);
                using (var zip = ZipFile.Read(zipFilePath))
                {
                    var path = createOwnDirectory ? outputPath + "\\" + FileName : outputPath;
                    foreach (var entry in zip.Entries)
                    {
                        entry.Extract(path, ExtractExistingFileAction.OverwriteSilently);
                        resultList.Add(path + "\\"+ entry.FileName);
                    }
                }
            }
            return resultList;
        }

        /// <summary>
        /// Extract file(s) to the sampe path from a given zip file
        /// </summary>
        /// <param name="zipFilePath">Full path of the zip file</param>
        /// <param name="createOwnDirectory">true to create a seperate directory for the extracted files</param>
        public static List<string> Extract(string zipFilePath, bool createOwnDirectory)
        {
            return Extract(zipFilePath, null, createOwnDirectory);
        }
    }
}