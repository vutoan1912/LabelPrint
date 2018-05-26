using LabelPrint.App_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
//using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LabelPrint.Business
{
    //public interface IGenericRepository<T> where T : class
    //{
    //    IEnumerable<T> SelectAll();
    //    T SelectOne(Expression<Func<T, bool>> predicate);
    //    IEnumerable<T> SelectWhere(Expression<Func<T, bool>> predicate);
    //    void Add(T entity);
    //    void Delete(T entity);
    //    void Edit(T entity);
    //    void Save();
    //}

    //public abstract class GenericRepository<C, T> :

    // IGenericRepository<T>
    //    where T : class
    //    where C : ObjectContext, new()
    //{

    //    private C db = new C();
    //    public C Context
    //    {
    //        get { return db; }
    //        set { db = value; }
    //    }

    //    public virtual IEnumerable<T> SelectAll()
    //    {
    //        IEnumerable<T> query = db.CreateObjectSet<T>();
    //        return query;
    //    }

    //    public virtual T SelectOne(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
    //    {
    //        T entity = db.CreateObjectSet<T>().Where(predicate).FirstOrDefault<T>();
    //        return entity;
    //    }

    //    public IEnumerable<T> SelectWhere(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
    //    {
    //        IQueryable<T> query = db.CreateObjectSet<T>().Where(predicate);
    //        return query;
    //    }

    //    public virtual void Add(T entity)
    //    {
    //        string entityType = entity.GetType().ToString();
    //        int start = entityType.LastIndexOf(".") + 1;
    //        string entityName = entityType.Substring(start, entityType.Length - start);
    //        db.AddObject(String.Format("{0}.{1}s", db.DefaultContainerName, entityName), entity);
    //    }

    //    public virtual void Delete(T entity)
    //    {
    //        db.DeleteObject(entity);
    //    }

    //    public virtual void Edit(T entity)
    //    {
    //        db.ObjectStateManager.ChangeObjectState(entity, System.Data.Entity.EntityState.Modified);
    //    }

    //    public virtual void Save()
    //    {
    //        db.SaveChanges();
    //    }
    //}

    public static class DbContextExtensions
    {
        public static ObjectContext ToObjectContext(this DbContext dbContext)
        {
            return (dbContext as IObjectContextAdapter).ObjectContext;
        }
    }

    public interface IGenericDataRepository<T> where T : class
    {
        IList<T> GetAll(params Expression<Func<T, object>>[] navigationProperties);
        IList<T> GetList(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);
        T GetSingle(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);
        void Add(params T[] items);
        void Update(params T[] items);
        void Remove(params T[] items);
    }

    public class GenericDataRepository<T> : IGenericDataRepository<T> where T : class
    {
        public virtual IList<T> GetAll(params Expression<Func<T, object>>[] navigationProperties)
        {
            List<T> list;
            using (var context = new erpEntities())
            {
                IQueryable<T> dbQuery = context.Set<T>();

                //Apply eager loading
                foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                    dbQuery = dbQuery.Include<T, object>(navigationProperty);

                list = dbQuery
                    .AsNoTracking()
                    .ToList<T>();
                context.Dispose();
            }
            return list;
        }

        public virtual IList<T> GetList(Func<T, bool> where,
             params Expression<Func<T, object>>[] navigationProperties)
        {
            List<T> list;
            using (var context = new erpEntities())
            {
                IQueryable<T> dbQuery = context.Set<T>();

                //Apply eager loading
                foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                    dbQuery = dbQuery.Include<T, object>(navigationProperty);

                list = dbQuery
                    .AsNoTracking()
                    .Where(where)
                    .ToList<T>();
                context.Dispose();
            }
            return list;
        }

        public virtual T GetSingle(Func<T, bool> where,
             params Expression<Func<T, object>>[] navigationProperties)
        {
            T item = null;
            using (var context = new erpEntities())
            {
                IQueryable<T> dbQuery = context.Set<T>();

                //Apply eager loading
                foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                    dbQuery = dbQuery.Include<T, object>(navigationProperty);

                item = dbQuery
                    .AsNoTracking() //Don't track any changes for the selected item
                    .FirstOrDefault(where); //Apply where clause
                context.Dispose();
            }
            return item;
        }

        public virtual void Add(params T[] items)
        {
            using (var context = new erpEntities())
            {
                foreach (T item in items)
                {
                    context.Entry(item).State = System.Data.Entity.EntityState.Added;
                }
                context.SaveChanges();
                //context.Dispose();
            }
        }

        public virtual void Update(params T[] items)
        {
            using (var context = new erpEntities())
            {
                foreach (T item in items)
                {
                    context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                context.SaveChanges();
                context.Dispose();
            }
        }

        public virtual void Remove(params T[] items)
        {
            using (var context = new erpEntities())
            {
                foreach (T item in items)
                {
                    context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                }
                context.SaveChanges();
                context.Dispose();
            }
        }
    }

    public interface IUserRepository : IGenericDataRepository<base_user>{}
    public interface IPrivilegeRepository : IGenericDataRepository<base_privilege_entity>{}
    public interface ITransferDetailsRepository : IGenericDataRepository<wh_transfer_details> { }
    public interface ISplitPackageRepository : IGenericDataRepository<wh_split_package> { }

    public class UserRepository : GenericDataRepository<base_user>, IUserRepository{}
    public class PrivilegeRepository : GenericDataRepository<base_privilege_entity>, IPrivilegeRepository{}
    public class TransferDetailsRepository : GenericDataRepository<wh_transfer_details>, ITransferDetailsRepository { }
    public class SplitPackageRepository : GenericDataRepository<wh_split_package>, ISplitPackageRepository { }

}