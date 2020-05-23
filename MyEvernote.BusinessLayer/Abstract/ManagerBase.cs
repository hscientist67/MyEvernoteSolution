using MyEvernote.Core.DataAccess;
using MyEvernote.DataAccessLayer.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer.Abstract
{
    public abstract class ManagerBase<T> : IDataAccess<T> where T : class
    {
        //Buradaki metotlara "virtual" ekledik çünkü bazı durumlarda bunları ovverride etmemiz gerekebiliyor.
        private Repository<T> repo = new Repository<T>();

        public virtual int Delete(T obj)
        {
           return repo.Delete(obj);
        }

        public virtual T Find(Expression<Func<T, bool>> kosul)
        {
            return repo.Find(kosul);
        }

        public virtual int Insert(T obj)
        {
            return repo.Insert(obj);
        }

        public virtual List<T> List()
        {
            return repo.List();
        }

        public virtual List<T> List(Expression<Func<T, bool>> kosul)
        {
            return repo.List(kosul);
        }

        public virtual IQueryable<T> ListQueryable()
        {
            return repo.ListQueryable();
        }

        public virtual int Save()
        {
            return repo.Save();
        }

        public virtual int Update(T obj)
        {
            return repo.Update(obj);
        }
    }
}
