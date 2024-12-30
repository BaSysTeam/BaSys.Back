using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.ScriptGenerators
{
    public abstract class ScriptGeneratorBase
    {
        protected char _wrapperOpen;
        protected char _wrapperClose;
        protected SqlDialectKinds _sqlDialect;
        protected StringBuilder _sb;

        protected ScriptGeneratorBase(SqlDialectKinds sqlDialect)
        {
            _sqlDialect = sqlDialect;
            _wrapperOpen = NameWrapperOpen(_sqlDialect);
            _wrapperClose = NameWrapperClosed(_sqlDialect);
            _sb = new StringBuilder();
        }

        protected void AppendName(string name)
        {
            _sb.Append(_wrapperOpen);
            _sb.Append(name);
            _sb.Append(_wrapperClose);
        }

        protected void Append(string text)
        {
            _sb.Append(text);
        }

        protected void Append(char symbol)
        {
            _sb.Append(symbol);
        }

        protected void AppendIf(string text, bool condition)
        {
            if (condition)
            {
                _sb.Append(text);
            }
        }

        protected void AppendIf(char symbol, bool condition)
        {
            if (condition)
            {
                _sb.Append(symbol);
            }
        }

        protected void AppendLine(string text = null)
        {
            _sb.AppendLine(text);
        }

        protected void AppendLineIf(string text, bool condition)
        {
            if (condition)
            {
                _sb.AppendLine(text);
            }
        }

        protected int AddUniqueConstraintQuery(string tableName, TableColumn column, int counter)
        {
            if (counter > 1)
                AppendLine("");

            Append("ALTER TABLE ");
            AppendName(tableName);
            Append(' ');

            var constraintName = UniqueConstraintName(tableName, column.Name);
            Append("ADD CONSTRAINT ");
            Append(constraintName);
            Append(" UNIQUE (");
            AppendName(column.Name);
            Append(')');
            Append(';');

            return counter + 1;
        }

        protected string UniqueConstraintName(string tableName, string columnName)
        {
            return $"uq_{tableName}_{columnName}";
        }

        public static char NameWrapperOpen(SqlDialectKinds dialectKind)
        {
            switch (dialectKind)
            {
                case SqlDialectKinds.MsSql:
                    return '[';
                case SqlDialectKinds.PgSql:
                    return '"';
                default:
                    throw new NotImplementedException();
            }
        }

        public static char NameWrapperClosed(SqlDialectKinds dialectKind)
        {
            switch (dialectKind)
            {
                case SqlDialectKinds.MsSql:
                    return ']';
                case SqlDialectKinds.PgSql:
                    return '"';
                default:
                    throw new NotImplementedException();
            }
        }

        public static string WrapName(string input, SqlDialectKinds dialectKind) {

            return $"{NameWrapperOpen(dialectKind)}{input}{NameWrapperClosed(dialectKind)}";
        }
    }
}
