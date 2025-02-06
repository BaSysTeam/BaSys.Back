using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BaSys.FluentQueries.Models
{
    public sealed class TableColumn
    {
        public string Name { get; set; } = string.Empty;
        public DbType DbType { get; set; }
        public int StringLength { get; set; }
        public int NumberDigits { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Required { get; set; }
        public bool Unique { get; set; }

        public TableColumn()
        {
            
        }

        public TableColumn(string name, Type type)
        {
            Name = name.ToLower();
            DbType = ToDbType(type);
            Required = RequiredFromType(type);
        }

        public TableColumn MaxLength(int length)
        {
            StringLength = length;
            return this;
        }

        public TableColumn MaxDigits(int digits)
        {
            NumberDigits = digits;
            return this;
        }

        public TableColumn IsPrimaryKey()
        {
            PrimaryKey = true;
            return this;
        }

        public TableColumn IsUnique()
        {
            Unique = true;
            return this;
        }

        public TableColumn IsRequired()
        {
            Required = true;
            return this;
        }

        public TableColumn IsOptional()
        {
            Required = false;
            return this;
        }

        public TableColumn ToType(DbType dbType)
        {
            DbType = dbType;
            return this;
        }

        public TableColumn Clone()
        {
            var clone = new TableColumn();
            clone.Name = Name;
            clone.DbType = DbType;
            clone.NumberDigits = NumberDigits;
            clone.StringLength = StringLength;
            clone.PrimaryKey = PrimaryKey;
            clone.Unique = Unique;
            clone.Required = Required;
            return clone;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();   
            sb.Append(Name);
            sb.Append(":");
            sb.Append(DbType);

            if (DbType == DbType.String)
            {
                sb.Append("(");
                sb.Append(StringLength.ToString());
                sb.Append(")");
            }

            if (DbType == DbType.Decimal)
            {
                sb.Append("(");
                sb.Append(NumberDigits.ToString());
                sb.Append(")");
            }

            sb.Append(" ");
            if (Required)
            {
               
                sb.Append("NOT NULL");
            }
            else
            {
                sb.Append("NULL");
            }

            if (Unique)
            {
                sb.Append(" ");
                sb.Append("UNIQUE");
            }

            if (PrimaryKey) {

                sb.Append(" ");
                sb.Append("PK");
            }

            return sb.ToString();
        }
        private DbType ToDbType(Type type)
        {
            if (type == typeof(byte)) return DbType.Byte;
            else if (type == typeof(sbyte)) return DbType.SByte;
            else if (type == typeof(short)) return DbType.Int16;
            else if (type == typeof(ushort)) return DbType.UInt16;
            else if (type == typeof(int)) return DbType.Int32;
            else if (type == typeof(uint)) return DbType.UInt32;
            else if (type == typeof(long)) return DbType.Int64;
            else if (type == typeof(ulong)) return DbType.UInt64;
            else if (type == typeof(float)) return DbType.Single;
            else if (type == typeof(double)) return DbType.Double;
            else if (type == typeof(decimal)) return DbType.Decimal;
            else if (type == typeof(bool)) return DbType.Boolean;
            else if (type == typeof(string)) return DbType.String;
            else if (type == typeof(char)) return DbType.StringFixedLength; // DbType.Char does not exist, mapping to closest equivalent
            else if (type == typeof(Guid)) return DbType.Guid;
            else if (type == typeof(DateTime)) return DbType.DateTime;
            else if (type == typeof(DateTimeOffset)) return DbType.DateTimeOffset;
            else if (type == typeof(byte[])) return DbType.Binary;
            else if (type == typeof(byte?)) return DbType.Byte;
            else if (type == typeof(sbyte?)) return DbType.SByte;
            else if (type == typeof(short?)) return DbType.Int16;
            else if (type == typeof(ushort?)) return DbType.UInt16;
            else if (type == typeof(int?)) return DbType.Int32;
            else if (type == typeof(uint?)) return DbType.UInt32;
            else if (type == typeof(long?)) return DbType.Int64;
            else if (type == typeof(ulong?)) return DbType.UInt64;
            else if (type == typeof(float?)) return DbType.Single;
            else if (type == typeof(double?)) return DbType.Double;
            else if (type == typeof(decimal?)) return DbType.Decimal;
            else if (type == typeof(bool?)) return DbType.Boolean;
            else if (type == typeof(char?)) return DbType.StringFixedLength; // DbType.Char does not exist, mapping to closest equivalent
            else if (type == typeof(Guid?)) return DbType.Guid;
            else if (type == typeof(DateTime?)) return DbType.DateTime;
            else if (type == typeof(DateTimeOffset?)) return DbType.DateTimeOffset;
            else if (type.IsEnum) return ToDbType( Enum.GetUnderlyingType(type));
            else if (IsNullableEnum(type)) return DbType.Byte;
            else throw new ArgumentException("Unsupported type");
        }

        private bool RequiredFromType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);

            return underlyingType == null;
           
        }

        private bool IsNullableEnum(Type type)
        {
            type = Nullable.GetUnderlyingType(type);
            return (type != null) && type.IsEnum;
        }

       
    }
}
