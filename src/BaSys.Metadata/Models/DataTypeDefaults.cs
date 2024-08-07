﻿using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

/// <summary>
/// The DataTypeDefaults class provides a centralized repository for predefined DataType
/// instances representing common primitive data types.This static class facilitates easy access
/// to default data types such as String, Integer, Boolean, Decimal, DateTime, and UniqueIdentifier,
/// each initialized with unique identifiers and specific database type mappings.
/// It also offers a method to retrieve a list of all predefined data types.
/// </summary>
public static class DataTypeDefaults
{
    public static readonly DataType String = new DataType(new Guid("0234c067-7868-46b2-ba8e-e22fae5255cb"))
    {
        Title = "String",
        IsPrimitive = true,
        DbType = DbType.String,
        Type = typeof(string)
    };

    public static readonly DataType Int = new DataType(new Guid("b327f82a-ea96-416f-9836-785db28eccac"))
    {
        Title = "Int",
        IsPrimitive = true,
        DbType = DbType.Int32,
        Type = typeof(int)
    };

    public static readonly DataType Long = new DataType(new Guid("daa57cb0-32eb-4709-b61f-4ea023ae31c3"))
    {
        Title = "Long",
        IsPrimitive = true,
        DbType = DbType.Int64,
        Type = typeof(long)
    };

    public static readonly DataType Bool = new DataType(new Guid("4bff64cf-eb01-4933-9f3d-b902336751f4"))
    {
        Title = "Boolean",
        IsPrimitive = true,
        DbType = DbType.Boolean,
        Type = typeof(bool)
    };

    public static readonly DataType Decimal = new DataType(new Guid("a05516ac-baae-4f66-9b67-6703998a6a1b"))
    {
        Title = "Decimal",
        IsPrimitive = true,
        DbType = DbType.Decimal,
        Type = typeof(decimal)
    };

    public static readonly DataType DateTime = new DataType(new Guid("9001eafb-efb1-442f-b288-723bb8002b12"))
    {
        Title = "DateTime",
        IsPrimitive = true,
        DbType = DbType.DateTime,
        Type = typeof(DateTime)
    };

    public static readonly DataType UniqueIdentifier = new DataType(new Guid("6fa9c45b-f514-4fea-a480-8e940636a1df"))
    {
        Title = "UniqueIdentifier",
        IsPrimitive = true,
        DbType = DbType.Guid,
        Type = typeof(Guid)
    };

    public static IList<DataType> AllTypes()
    {
        var collection = new List<DataType>
        {
            String,
            Int,
            Long,
            Bool,
            Decimal,
            DateTime,
            UniqueIdentifier
        };

        return collection;
    }

    public static IList<DataType> GetPrimaryKeyTypes()
    {
        var collection = new List<DataType>
        {
            String,
            Int,
            Long,
            UniqueIdentifier
        };

        return collection;
    }
}
