﻿using System;

namespace Unclazz.Parsec
{
    sealed class CutParser<T> : Parser<T>
    {
        internal CutParser(Parser<T> original)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser<T> _original;

        public override ParseResult<T> Parse(ParserInput input)
        {
            var r = _original.Parse(input);
            if (r.Successful) return r.AllowBacktrack(false);
            return r.AllowBacktrack(true);
        }

        public override string ToString()
        {
            return string.Format("Cut({0})", _original);
        }
    }
}