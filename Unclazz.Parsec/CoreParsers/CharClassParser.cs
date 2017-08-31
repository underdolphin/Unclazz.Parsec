﻿using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class CharClassParser : Parser
    {
        internal CharClassParser(IParserConfiguration conf, CharClass clazz) : base(conf)
        {
            _clazz = clazz ?? throw new ArgumentNullException(nameof(clazz));
        }

        readonly CharClass _clazz;

        protected override ResultCore DoParse(Reader input)
        {
            var ch = input.Read();
            if (0 <= ch && _clazz.Contains((char)ch))
            {
                return Success();
            }
            else
            {
                return Failure(string.Format("expected a member of {0} but found {1}.", 
                    _clazz, ParsecUtility.CharToString(ch)));
            }
        }
        public override string ToString()
        {
            return string.Format("CharClass({0})", _clazz);
        }
    }
}
