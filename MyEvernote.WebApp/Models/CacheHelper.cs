using MyEvernote.BusinessLayer;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace MyEvernote.WebApp.Models
{
    public class CacheHelper
    {
        public static List<Category> GetCategoriesFromCache()
        {
            var result = WebCache.Get("category-cache");
            if (result == null)
            {
                CategoryManager cm = new CategoryManager();
                result = cm.List();
          
                //dönen listeyi cahche at 20 dk kalsın.
                WebCache.Set("category-cache", result, 20, true);

            }
            return result;
        }

        public static void RemoveCache(string key)
        {
            WebCache.Remove(key);
        }
    }
}