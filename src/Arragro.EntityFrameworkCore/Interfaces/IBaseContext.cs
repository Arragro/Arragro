﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace Arragro.EntityFrameworkCore.Interfaces
{
    public interface IBaseContext : IDisposable
    {
        EntityEntry Entry(object entity);
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        void SetModified(object entity);
        int SaveChanges();
    }
}
