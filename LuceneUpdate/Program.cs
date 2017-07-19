using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;

using FSDirectory = Lucene.Net.Store.FSDirectory;
using Version = Lucene.Net.Util.Version;
using System.IO;
using Lucene.Net.Search;
using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers;
using Lucene.Net.Documents;

namespace LuceneUpdate
{
    class Program
    {
        static string indexLocation = @"C:\clsdev\IndexFiles.index3";
        internal static readonly DirectoryInfo INDEX_DIR = new DirectoryInfo(indexLocation);
        static string[] exts = new string[] { ".spl", ".txt", ".pcs", ".sh", ".py" };
        static DateTime indexUpdateTime;

        static void Main(string[] args)
        {
            var docDir = new DirectoryInfo(@"C:\clsdev");

            indexUpdateTime = Directory.GetLastWriteTime(indexLocation);

            try
            {
                using (var writer = new IndexWriter(FSDirectory.Open(INDEX_DIR), new CustomAnalyzer(), false, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    Console.Out.WriteLine("Indexing to directory '" + INDEX_DIR + "'...");
                    IndexDirectory(writer, docDir);
                    Console.Out.WriteLine("Optimizing...");
                    writer.Optimize();
                    writer.Commit();
                }
                var end = DateTime.Now;
                //Console.Out.WriteLine(end.Millisecond - start.Millisecond + " total milliseconds");
            }
            catch (IOException e)
            {
                Console.Out.WriteLine(" caught a " + e.GetType() + "\n with message: " + e.Message);
            }
            deleteDocs();
        }

            //readonly Lucene.Net.Util.Version LuceneVersion = Lucene.Net.Util.Version.LUCENE_29;
            //var IndexLocationPath = INDEX_DIR; // Set to your location
            //var directoryInfo = new DirectoryInfo(IndexLocationPath);
            //var directory = FSDirectory.Open(directoryInfo);
            //var writer = new IndexWriter(directory,
            //            new StandardAnalyzer(LuceneVersion),
            //            false, // Don't create index
            //            IndexWriter.MaxFieldLength.LIMITED);
            //writer.UpdateDocument(new Term("patient_id", document.Get("patient_id")), document);
            //writer.Optimize(); // Should be done with low load only ...
            //writer.Close();


                    internal static void IndexDirectory(IndexWriter writer, DirectoryInfo directory)
        {
           // Console.Out.WriteLine("adding " + directory.FullName);

            foreach (var subDirectory in directory.GetDirectories())
                IndexDirectory(writer, subDirectory);

            foreach (var file in directory.GetFiles().Where(s => exts.Contains(Path.GetExtension(s.FullName))))
                IndexDocs(writer, file);
        }

        internal static void IndexDocs(IndexWriter writer, FileInfo file)
        {
            //Console.Out.WriteLine("adding " + file);
            if (!exts.Contains(file.Extension)) return;
            try
            {
                IndexReader indexReader = null;
                try
                {
                    indexReader = IndexReader.Open(FSDirectory.Open(INDEX_DIR), true); // only searching, so read-only=true

                    Searcher searcher = new IndexSearcher(indexReader);
                    Analyzer analyzer = new CustomAnalyzer();

                    //Query query = new TermQuery(new Term("3", "\"Fuel Tank Capacity\"@en"));
                    //var parser = new QueryParser(Version.LUCENE_30, "path", analyzer);
                    //Query query = parser.Parse(file.FullName);
                    var q = new TermQuery(new Term("path", file.FullName));
                    var search = searcher.Search(q, null, 1);
                    var hits = search.ScoreDocs;
                    if (hits.Length > 0 )
                    {
                        Document doc = searcher.Doc(hits[0].Doc);
                        DateTime modifiedStored = DateTools.StringToDate(doc.Get("modified"));
                        DateTime actuallyModified = File.GetLastWriteTime(file.FullName);
                        string actuallyModifiedString = DateTools.DateToString(actuallyModified, DateTools.Resolution.MINUTE);
                        actuallyModified  = DateTools.StringToDate(actuallyModifiedString);
                        if (modifiedStored < actuallyModified)
                        {
                            //Console.WriteLine(File.GetLastWriteTime(file.FullName));
                            //string _ = DateTools.DateToString(File.GetLastWriteTime(file.FullName),DateTools.Resolution.MINUTE);
                            //Console.WriteLine(_);
                            //DateTime __ = DateTools.StringToDate(_);
                            //Console.WriteLine(DateTools.DateToString(__, DateTools.Resolution.MINUTE));
                            //Console.WriteLine(DateTools.TimeToString(file.LastWriteTime.Millisecond, DateTools.Resolution.MINUTE));
                            writer.UpdateDocument(new Term("path", doc.Get("path")), FileDocument.Document(file));
                            Console.WriteLine("Updating " + file.FullName);
                        }
                    }
                    else
                    {
                        writer.AddDocument(FileDocument.Document(file));
                        Console.WriteLine("Adding " + file.FullName);
                    }
                }
                finally
                {
                    if (indexReader != null)
                    {
                        indexReader.Dispose();
                    }
                }
               
                //else
                //{

                //}
                //writer.UpdateDocument(FileDocument.Document(file));
            }
            catch (FileNotFoundException)
            {
                // At least on Windows, some temporary files raise this exception with an
                // "access denied" message checking if the file can be read doesn't help.
            }
            catch (UnauthorizedAccessException)
            {
                // Handle any access-denied errors that occur while reading the file.    
            }
            catch (IOException)
            {
                // Generic handler for any io-related exceptions that occur.
            }
        }
        public static void deleteDocs()
        {
            IndexReader reader = IndexReader.Open(FSDirectory.Open(INDEX_DIR),true);
            var writer = new IndexWriter(FSDirectory.Open(INDEX_DIR), new CustomAnalyzer(), false, IndexWriter.MaxFieldLength.UNLIMITED);
            for (int i = 0; i < reader.NumDocs(); i++)
            {

                Document doc = reader.Document(i);
                String docId = doc.Get("path");
                if (!File.Exists(docId))
                {
                    //var writer = new IndexWriter(INDEX_DIR, analyzer, false, IndexWriter.MaxFieldLength.LIMITED);
                    var term = new Term("path", docId);
                    writer.DeleteDocuments(term);
                    Console.WriteLine("Deleting " + docId);
                }
            }
            writer.Optimize();
            writer.Commit();
            writer.Dispose();
        }
    }
}

