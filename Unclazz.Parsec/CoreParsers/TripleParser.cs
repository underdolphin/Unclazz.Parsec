﻿using System;

namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// 2つのパーサーを連結しシーケンスを構成するパーサーの抽象クラスです。
    /// 結果値は片方のパーサーが返す単一値ともう片方のパーサーが返すタプル（ペア）をまとめ
    /// 新しいタプル（トリプル）としてまとめたものとなります。
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    abstract class TripleParser<T1,T2,T3>: Parser<Tuple<T1, T2, T3>>
    {
        public static TripleParser<T1,T2,T3> Create(Parser<T1> left, Parser<Tuple<T2, T3>> right)
        {
            return new ThenDoubleParser<T1, T2, T3>(left.Configuration, left, right);
        }
        public static TripleParser<T1,T2,T3> Create(Parser<Tuple<T1, T2>> left, Parser<T3> right)
        {
            return new DoubleThenParser<T1, T2, T3>(left.Configuration, left, right);
        }

        internal TripleParser(IParserConfiguration conf) : base(conf) { }

        /// <summary>
        /// <see cref="TripleParser{T1, T2, T3}"/>から派生した具象クラスです。
        /// 左被演算子として単一値を返すパーサー、右被演算子としてタプル（ペア）を返すパーサーをとります。
        /// </summary>
        /// <typeparam name="U1">左被演算子のパーサーの結果型</typeparam>
        /// <typeparam name="U2">右被演算子のパーサーの結果型タプルの第1要素型</typeparam>
        /// <typeparam name="U3">右被演算子のパーサーの結果型タプルの第2要素型</typeparam>
        sealed class ThenDoubleParser<U1, U2, U3> : TripleParser<U1, U2, U3>
        {
            internal ThenDoubleParser(IParserConfiguration conf, Parser<U1> left, Parser<Tuple<U2, U3>> right) : base(conf)
            {
                Left = left ?? throw new ArgumentNullException(nameof(left));
                Right = right ?? throw new ArgumentNullException(nameof(right));
            }

            public Parser<U1> Left { get; }
            public Parser<Tuple<U2, U3>> Right { get; }

            protected override ResultCore<Tuple<U1, U2, U3>> DoParse(Reader input)
            {
                var leftResult = Left.Parse(input);
                if (!leftResult.Successful)
                {
                    return leftResult.Retyped<Tuple<U1, U2, U3>>();
                }

                var rightResult = Right.Parse(input);
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;
                if (!rightResult.Successful)
                {
                    return rightResult.Retyped<Tuple<U1, U2, U3>>().AllowBacktrack(canBacktrack);
                }

                var rightTuple = rightResult.Value;
                var finalTuple = new Tuple<U1, U2, U3>(leftResult.Value,
                    rightTuple == null ? default(U2) : rightTuple.Item1,
                    rightTuple == null ? default(U3) : rightTuple.Item2);
                return Success(finalTuple, canBacktrack);
            }
            public override string ToString()
            {
                return string.Format("Triple({0}, {1})", Left, Right);
            }
        }
        /// <summary>
        /// <see cref="TripleParser{T1, T2, T3}"/>から派生した具象クラスです。
        /// 左被演算子としてタプル（ペア）を返すパーサー、右被演算子として単一値を返すパーサーをとります。
        /// </summary>
        /// <typeparam name="U1">左被演算子のパーサーの結果型タプルの第1要素型</typeparam>
        /// <typeparam name="U2">左被演算子のパーサーの結果型タプルの第2要素型</typeparam>
        /// <typeparam name="U3">右被演算子のパーサーの結果型</typeparam>
        sealed class DoubleThenParser<U1, U2, U3> : TripleParser<U1, U2, U3>
        {
            internal DoubleThenParser(IParserConfiguration conf, Parser<Tuple<U1, U2>> left, Parser<U3> right) : base(conf)
            {
                Left = left ?? throw new ArgumentNullException(nameof(left));
                Right = right ?? throw new ArgumentNullException(nameof(right));
            }

            public Parser<Tuple<U1, U2>> Left { get; }
            public Parser<U3> Right { get; }

            protected override ResultCore<Tuple<U1, U2, U3>> DoParse(Reader input)
            {
                var leftResult = Left.Parse(input);
                if (!leftResult.Successful)
                {
                    return leftResult.Retyped<Tuple<U1, U2, U3>>();
                }

                var rightResult = Right.Parse(input);
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;
                if (!rightResult.Successful)
                {
                    return rightResult.Retyped<Tuple<U1, U2, U3>>().AllowBacktrack(canBacktrack);
                }

                var leftTuple = leftResult.Value;
                var finalTuple = new Tuple<U1, U2, U3>(
                    leftTuple == null ? default(U1) : leftTuple.Item1,
                    leftTuple == null ? default(U2) : leftTuple.Item2,
                    rightResult.Value);
                return Success(finalTuple, canBacktrack);
            }
            public override string ToString()
            {
                return string.Format("Triple({0}, {1})", Left, Right);
            }
        }
    }
}
