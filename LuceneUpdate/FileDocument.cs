﻿using System;
using System.IO;
using Lucene.Net.Documents;

namespace LuceneUpdate
{

    /// <summary>A utility for making Lucene Documents from a File. </summary>

    public static class FileDocument
    {
        /// <summary>Makes a document for a File.
        /// <p>
        /// The document has three fields:
        /// <ul>
        /// <li><c>path</c>--containing the pathname of the file, as a stored,
        /// untokenized field;</li>
        /// <li><c>modified</c>--containing the last modified date of the file as
        /// a field as created by <a href="lucene.document.DateTools.html">DateTools</a>;</li>
        /// <li><c>contents</c>--containing the full contents of the file, as a
        /// Reader field;</li>
        /// </ul>
        /// </p>
        /// </summary>
        public static Document Document(FileInfo f)
        {

            // make a new, empty document
            Document doc = new Document();

            // Add the path of the file as a field named "path".  Use a field that is 
            // indexed (i.e. searchable), but don't tokenize the field into words.
            doc.Add(new Field("path", f.FullName, Field.Store.YES, Field.Index.NOT_ANALYZED));

            // Add the last modified date of the file a field named "modified".  Use 
            // a field that is indexed (i.e. searchable), but don't tokenize the field
            // into words.
            doc.Add(new Field("modified", DateTools.DateToString(File.GetLastWriteTime(f.FullName), DateTools.Resolution.MINUTE), Field.Store.YES, Field.Index.NOT_ANALYZED));

            // Add the contents of the file to a field named "contents".  Specify a Reader,
            // so that the text of the file is tokenized and indexed, but not stored.
            // Note that FileReader expects the file to be in the system's default encoding.
            // If that's not the case searching for special characters will fail.
            doc.Add(new Field("contents", new StreamReader(f.FullName, System.Text.Encoding.Default)));

            // return the document
            return doc;
        }
    }
}

