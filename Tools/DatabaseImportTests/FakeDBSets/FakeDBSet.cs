﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace DatabaseImportTests.FakeDBSets
{
    public abstract class FakeDBSet<T> : IDbSet<T> where T : class, new()
    {
        readonly ObservableCollection<T> collection;
        readonly IQueryable query;

        #region IDbSet<T> Members

        public FakeDBSet()
        {
            collection = new ObservableCollection<T>();
            query = collection.AsQueryable();
        }

        public T Add(T entity)
        {
            collection.Add(entity);
            return entity;
        }

        public T Attach(T entity)
        {
            collection.Add(entity);
            return entity;
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public T Create()
        {
            T aT = new T();
            return aT;
        }

        public abstract T Find(params object[] keyValues);

        public System.Collections.ObjectModel.ObservableCollection<T> Local
        {
            get { throw new NotImplementedException(); }
        }

        public T Remove(T entity)
        {
            collection.Remove(entity);
            return entity;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Membres

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        #endregion

        #region IQueryable Membres

        public Type ElementType
        {
            get { return query.ElementType; }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return query.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return query.Provider; }
        }

        #endregion
    }
}