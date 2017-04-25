﻿using System;
using Aromato.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Aromato.Infrastructure
{
    public class EfUnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly IDbContextTransaction _transaction;

        public AromatoContext Context { get; }

        public EfUnitOfWork(AromatoContext context)
        {
            Context = context;
            _transaction = context.Database.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
        }
    }
}