﻿using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class ThenParser : Parser
    {
        internal ThenParser(IParserConfiguration conf, Parser left, Parser right) : base(conf)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public Parser Left { get; }
        public Parser Right { get; }

        protected override ResultCore DoParse(Reader input)
        {
            var leftResult = Left.Parse(input);
            if (leftResult.Successful)
            {
                var rightResult = Right.Parse(input);
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;

                return rightResult.AllowBacktrack(canBacktrack);
            }
            return leftResult;
        }
        public override string ToString()
        {
            return string.Format("Then({0}, {1})", Left, Right);
        }
    }
}
