﻿using NUnit.Framework;
using System;
using System.Linq;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class CastParserTest
    {
        [Test]
        public void Parse_Case3()
        {
            // Arrange
            var kp = Keyword("0123");
            var cp = kp.Cast("3210");

            // Act
            var res = cp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Value, Is.EqualTo("3210"));
        }
    }
}
