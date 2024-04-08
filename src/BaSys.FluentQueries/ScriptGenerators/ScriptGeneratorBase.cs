using BaSys.FluentQueries.Enums;
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

        protected ScriptGeneratorBase(SqlDialectKinds sqlDialect)
        {
            _sqlDialect = sqlDialect;
            _wrapperOpen = NameWrapperOpen(_sqlDialect);
            _wrapperClose = NameWrapperClosed(_sqlDialect);
        }

        private char NameWrapperOpen(SqlDialectKinds dialectKind)
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

        private char NameWrapperClosed(SqlDialectKinds dialectKind)
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
    }
}
