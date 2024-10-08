﻿using BaSys.Host.DAL.TableManagers;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.Abstractions
{
    public interface ITableManagerFactory
    {
        void SetUp(IDbConnection connection);
        T Create<T>() where T :class, ITableManager;
        MetaObjectManager CreateMetaObjectManager(string kindName);
    }
}
