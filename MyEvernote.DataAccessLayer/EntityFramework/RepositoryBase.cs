using MyEvernote.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    // Bu basit bir singleton tekniğidir. Singleton tekniğinde database context new lenmeden(Çünkü her repository nesnesi kullanılırken db ye ihtiyaç var ama bunların birbiri
    //ile kullanımı söz konusu olabilmesi için tek context de çalışılmalıdır.) tek context üzerinde çalışılır. Bunu kontrol için bu teknik kullanılır. 
    // Aşağıdaki "lock" da biz bu kontrolü yapmaya çalışsak da multi işlemler burayı tehdit ederse bunu öneler. Bir işlem bitmeden diğeri bunu kullanamaz der.
    public class RepositoryBase
    {
        //Miras alan classlar bu db yi görsün diye protected yaptık.
        protected static DatabaseContext db;
        private static object lockobj = new object();

        //Bu constructor new lenemez. Sadece miras alan bunu new leyebilir. 
        protected RepositoryBase()
        {
             CreateContext();
        }

        private static void  CreateContext()
        {
            if (db == null)
            {
                lock (lockobj)
                {
                    if (db == null)
                    {
                        db = new DatabaseContext();
                    }
                }
            }
           
        }
    }
}
