using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Core.DataAccess
{
    //Bu IRepository sayesinde şuan da EntityFramework ile çalışan sistem  sonrasında istersek mysql, oracle gibi diğer sisttemler ile direk olarak çalışmasi için 
    //Soyut bir yapı olulturuldu. Bıu sayede iskelet oluşturuldu. Biz duruma göre iskeletin içini dolduracağız.
    //interface içerisinde erişim belirleyiciler ve metot gövdeleri olmaz. 
    //Bir interface i miras alan her sınıf içerisindeki tüm metotları implemente etmek zorundadır.
    public interface IDataAccess<T>
    {
        List<T> List();
        IQueryable<T> ListQueryable();
        List<T> List(Expression<Func<T, bool>> kosul);
        int Insert(T obj);
        int Update(T obj);
        int Delete(T obj);
        //etkilenen kayıt sayısını döner.
        int Save();
        T Find(Expression<Func<T, bool>> kosul);
    }
}
