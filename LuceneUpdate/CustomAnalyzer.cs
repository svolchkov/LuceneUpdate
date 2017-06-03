using Lucene.Net.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneUpdate
{
    public sealed class CustomAnalyzer : Analyzer
    {
        public override TokenStream TokenStream(System.String fieldName, System.IO.TextReader reader)
        {
            return new LowerCaseFilter(new CustomTokenizer(reader));
        }

        //public override TokenStream ReusableTokenStream(System.String fieldName, System.IO.TextReader reader)
        //{
        //    SavedStreams streams = (SavedStreams)GetPreviousTokenStream();
        //    if (streams == null)
        //    {
        //        streams = new SavedStreams();
        //        SetPreviousTokenStream(streams);
        //        streams.tokenStream = new WhiteSpaceTokenizer(reader);
        //        streams.filteredTokenStream = new LowerCaseFilter(streams.tokenStream);
        //    }
        //    else
        //    {
        //        streams.tokenStream.Reset(reader);
        //    }
        //    return streams.filteredTokenStream;
        //}

        //      public override TokenStream TokenStream
        //(string fieldName, System.IO.TextReader reader)
        //      {
        //          //create the tokenizer
        //          TokenStream result = new CustomTokenizer(reader);

        //          //add in filters
        //          // first normalize the StandardTokenizer
        //          result = new StandardFilter(result);

        //          // makes sure everything is lower case
        //          result = new LowerCaseFilter(result);

        //          // use the default list of Stop Words, provided by the StopAnalyzer class.
        //          result = new StopFilter(result, StopAnalyzer.ENGLISH_STOP_WORDS);

        //          // injects the synonyms.
        //          result = new SynonymFilter(result, SynonymEngine);

        //          //return the built token stream.
        //          return result;
        //      }
    }

}
