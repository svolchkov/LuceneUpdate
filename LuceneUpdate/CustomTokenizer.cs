using Lucene.Net.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Lucene.Net.Util;

namespace LuceneUpdate
{
    public class CustomTokenizer : CharTokenizer
    {
        public CustomTokenizer(TextReader input) : base(input)
        {
        }

        public CustomTokenizer(AttributeFactory factory, TextReader input) : base(factory, input)
        {
        }

        public CustomTokenizer(AttributeSource source, TextReader input) : base(source, input)
        {
        }

        protected override bool IsTokenChar(char c)
        {
            return System.Char.IsLetterOrDigit(c) || c == '_' || c == '-';
        }
    }
}

