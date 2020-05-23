using MyEvernote.Entities;
using MyEvernote.DataAccessLayer.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer
{
    public class Test
    {
        private Repository<EvernoteUser> repo_user = new Repository<EvernoteUser>();
        private Repository<Category> repo_category = new Repository<Category>();
        private Repository<Comment> repo_comment = new Repository<Comment>();
        private Repository<Note> repo_note = new Repository<Note>();

        public Test()
        {
            //Buradaki kod DAL a giderek gerekli komutları verecek olan kısımdır.
            //DataAccessLayer.DatabaseContext db = new DataAccessLayer.DatabaseContext();
            //db.Categories.ToList();

            // Repository class yardımıyla datalar çekiliyor.

            List<Category> kategoriler = repo_category.List();
            List<Category> kategoriler_filtered = repo_category.List(x => x.Id > 5);
        }

        public void InserTest()
        {
            int result = repo_user.Insert(new EvernoteUser()
            {
                Name = "aaa",
                Surname = "bbb",
                Email = "bbb@bitnet.com.tr",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false,
                Username = "aaa_bbb",
                Password = "123",
                CreatedOn = DateTime.Now.AddHours(1),
                ModifiedOn = DateTime.Now.AddMinutes(75),
                ModifiedUsername = "aaa_bbb"
            });
        }

        public void UpdateTest()
        {
            EvernoteUser user = repo_user.Find(x => x.Username == "aaa_bbb");

            if (user != null)
            {
                user.Username = "xxx";
                int result = repo_user.Save();
            }
        }

        public void DeleteTest()
        {
            EvernoteUser user = repo_user.Find(x => x.Username == "xxx");

            if (user != null)
            {

                repo_user.Delete(user);
            }
        }

        public void CommentTest()
        {
            EvernoteUser user = repo_user.Find(x => x.Id == 1);
            Note note = repo_note.Find(x => x.Id == 3);

            Comment yorum = new Comment()
            {
                Text = "Bu bir Test yorumudur.",
                CreatedOn = DateTime.Now,
                ModifiedUsername = "hulyabilgin",
                Note = note,
                Owner = user,
                ModifiedOn = DateTime.Now
            };

            repo_comment.Insert(yorum);

        }
    }
}
