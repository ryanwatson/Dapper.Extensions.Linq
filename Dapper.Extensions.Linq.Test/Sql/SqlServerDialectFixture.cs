﻿using System;
using System.Collections.Generic;
using Dapper.Extensions.Linq.Sql;
using Dapper.Extensions.Linq.Test.Helpers;
using NUnit.Framework;

namespace Dapper.Extensions.Linq.Test.Sql
{
    [TestFixture]
    public class SqlServerDialectFixture
    {
        public abstract class SqlServerDialectFixtureBase
        {
            protected SqlServerDialect Dialect;

            [SetUp]
            public void Setup()
            {
                Dialect = new SqlServerDialect();
            }
        }

        [TestFixture]
        public class Methods : SqlServerDialectFixtureBase
        {
            [Test]
            public void Nolock()
            {
                Assert.That(Dialect.SetNolock("SELECT * FROM Person").Contains(" (NOLOCK)"));
                Assert.That(Dialect.SetNolock("SELECT * FROM Person WHERE Id = 1").Contains(" (NOLOCK) "));
                Assert.That(Dialect.SetNolock("SELECT * FROM Person ORDER BY Id DESC").Contains(" (NOLOCK) "));
            }
        }

        [TestFixture]
        public class Properties : SqlServerDialectFixtureBase
        {
            [Test]
            public void CheckSettings()
            {
                Assert.AreEqual('[', Dialect.OpenQuote);
                Assert.AreEqual(']', Dialect.CloseQuote);
                Assert.AreEqual(";" + Environment.NewLine, Dialect.BatchSeperator);
                Assert.AreEqual('@', Dialect.ParameterPrefix);
                Assert.IsTrue(Dialect.SupportsMultipleStatements);
            }
        }

        [TestFixture]
        public class GetPagingSqlMethod : SqlServerDialectFixtureBase
        {
            [Test]
            public void NullSql_ThrowsException()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => Dialect.GetPagingSql(null, 0, 10, new Dictionary<string, object>()));
                Assert.AreEqual("SQL", ex.ParamName);
                StringAssert.Contains("cannot be null", ex.Message);
            }

            [Test]
            public void EmptySql_ThrowsException()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => Dialect.GetPagingSql(string.Empty, 0, 10, new Dictionary<string, object>()));
                Assert.AreEqual("SQL", ex.ParamName);
                StringAssert.Contains("cannot be null", ex.Message);
            }

            [Test]
            public void NullParameters_ThrowsException()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => Dialect.GetPagingSql("SELECT [schema].[column] FROM [schema].[table]", 0, 10, null));
                Assert.AreEqual("Parameters", ex.ParamName);
                StringAssert.Contains("cannot be null", ex.Message);
            }

            [Test]
            public void Select_ReturnsSql()
            {
                var parameters = new Dictionary<string, object>();
                string sql = "SELECT [column] FROM [schema].[table] ORDER BY [column] OFFSET @firstResult ROWS FETCH NEXT @maxResults ROWS ONLY";
                var result = Dialect.GetPagingSql("SELECT [column] FROM [schema].[table] ORDER BY [column]", 0, 10, parameters);
                Assert.AreEqual(sql, result);
                Assert.AreEqual(2, parameters.Count);
                Assert.AreEqual(parameters["@maxResults"], 10);
                Assert.AreEqual(parameters["@firstResult"], 0);
            }

            [Test]
            public void SelectDistinct_ReturnsSql()
            {
                var parameters = new Dictionary<string, object>();
                string sql = "SELECT DISTINCT [column] FROM [schema].[table] ORDER BY [column] OFFSET @firstResult ROWS FETCH NEXT @maxResults ROWS ONLY";
                var result = Dialect.GetPagingSql("SELECT DISTINCT [column] FROM [schema].[table] ORDER BY [column]", 0, 10, parameters);
                Assert.AreEqual(sql, result);
                Assert.AreEqual(2, parameters.Count);
                Assert.AreEqual(parameters["@maxResults"], 10);
                Assert.AreEqual(parameters["@firstResult"], 0);
            }

            [Test]
            public void SelectOrderBy_ReturnsSql()
            {
                var parameters = new Dictionary<string, object>();
                string sql = "SELECT [column] FROM [schema].[table] ORDER BY [column] DESC OFFSET @firstResult ROWS FETCH NEXT @maxResults ROWS ONLY";
                var result = Dialect.GetPagingSql("SELECT [column] FROM [schema].[table] ORDER BY [column] DESC", 0, 10, parameters);
                Assert.AreEqual(sql, result);
                Assert.AreEqual(2, parameters.Count);
                Assert.AreEqual(parameters["@maxResults"], 10);
                Assert.AreEqual(parameters["@firstResult"], 0);
            }
        }

        [TestFixture]
        public class GetOrderByClauseMethod : SqlServerDialectFixtureBase
        {
            [Test]
            public void NoOrderBy_Returns()
            {
                var result = Dialect.TestProtected().RunMethod<string>("GetOrderByClause", "SELECT * FROM Table");
                Assert.IsNull(result);
            }

            [Test]
            public void OrderBy_ReturnsItemsAfterClause()
            {
                var result = Dialect.TestProtected().RunMethod<string>("GetOrderByClause", "SELECT * FROM Table ORDER BY Column1 ASC, Column2 DESC");
                Assert.AreEqual("ORDER BY Column1 ASC, Column2 DESC", result);
            }

            [Test]
            public void OrderByWithWhere_ReturnsOnlyOrderBy()
            {
                var result = Dialect.TestProtected().RunMethod<string>("GetOrderByClause", "SELECT * FROM Table ORDER BY Column1 ASC, Column2 DESC WHERE Column1 = 'value'");
                Assert.AreEqual("ORDER BY Column1 ASC, Column2 DESC", result);
            }
        }
    }
}