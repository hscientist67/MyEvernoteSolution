using MyEvernote.Core.DataAccess;
using MyEvernote.Entities;
using MyEvernoteCommon;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MyEvernote.DataAccessLayer.EntityFramework
{

    //Burada T class olmak zorundadır dedik.
    //RepositoryBase den Entityframework ün birlikte çalıştığı  "Database context" alındı.
    //IRepository ise repository metotları override edildi.
    public class Repository<T> : RepositoryBase, IDataAccess<T> where T : class
    {
        // ilgili set i bul diyerek işlem yapıyoruz.
        private DbSet<T> _objectset;

        public Repository()
        {
            //singleton pattern burada kullanıldı.

            _objectset = db.Set<T>();
        }

        public List<T> List()
        {
            return _objectset.ToList();
        }

        public IQueryable<T> ListQueryable()
        {
            return _objectset.AsQueryable();
        }

        public List<T> List(Expression<Func<T, bool>> kosul)
        {
            return _objectset.Where(kosul).ToList();
        }

        public int Insert(T obj)
        {
            _objectset.Add(obj);

            if (obj is MyEntityBase)
            {
                MyEntityBase o = obj as MyEntityBase;
                DateTime now = DateTime.Now;

                o.CreatedOn = now;
                o.ModifiedOn = now;
                o.ModifiedUsername = App.Common.GetCurrentUserName();
            }
            return Save();
        }

        public int Update(T obj)
        {
            if (obj is MyEntityBase)
            {
                MyEntityBase o = obj as MyEntityBase;

                o.ModifiedOn = DateTime.Now;
                o.ModifiedUsername = App.Common.GetCurrentUserName();
            }
            return Save();
        }

        public int Delete(T obj)
        {
            _objectset.Remove(obj);
            return Save();
        }

        //etkilenen kayıt sayısını döner.
        public int Save()
        {
            try
            {
                return db.SaveChanges();
            }
            catch (DbEntityValidationException dbValEx)
            {
                //Bu kodlar eğer save esnasında bir hata medana gelirse, o hatanın hangi alandan kaydnaklandığını yakalar.
                var outputLines = new StringBuilder();
                foreach (var eve in dbValEx.EntityValidationErrors)
                {
                    outputLines.AppendFormat("{0}: Entity of type {1} in state {2} has the following validation errors:", DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry);

                    foreach (var ve in eve.ValidationErrors)
                    {
                        outputLines.AppendFormat("- Property: {0}, Error: {1}", ve.PropertyName, ve.ErrorMessage);
                    }
                }

                //Tools.Notify(this, outputLines.ToString(),"error");
                throw new DbEntityValidationException(string.Format("Validation errorsrn{0}"
                 , outputLines.ToString()), dbValEx);
            }

        }

        public T Find(Expression<Func<T, bool>> kosul)
        {

            return _objectset.FirstOrDefault(kosul);
        }

    }
}
