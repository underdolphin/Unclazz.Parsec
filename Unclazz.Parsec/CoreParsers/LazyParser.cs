﻿using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class LazyParser : Parser
    {
        internal LazyParser(IParserConfiguration conf, Func<Parser> factory) : base(conf)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
        readonly Func<Parser> _factory;
        Parser _cache;
        protected override ResultCore DoParse(Reader input)
        {
            return (_cache ?? (_cache = _factory())).Parse(input);
        }
        public override string ToString()
        {
            return string.Format("Lazy<{0}>()", ParsecUtility.ObjectTypeToString(_factory));
        }
    }
}
