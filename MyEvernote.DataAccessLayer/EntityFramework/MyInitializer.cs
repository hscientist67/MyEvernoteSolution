using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MyEvernote.Entities;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    //Bu initializer eğer db yoksa oluşturmak için çalışacak.
    public class MyInitializer : CreateDatabaseIfNotExists<DatabaseContext>
    {
        //Database oluştuktan sonra örnek datab basımında kullanılan metotdur.
        protected override void Seed(DatabaseContext context)
        {
            //admin user
            EvernoteUser admin = new EvernoteUser()
            {
                Name = "Hülya",
                Surname = "Bilgin Herkiloğlu",
                Email = "hlya.bilgin1991@hotmail.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = true,
                Username = "hulyabilgin",
                Password = "1234",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddMinutes(5),
                ModifiedUsername = "hulyabilgin",
                ProfileImageFileName="user_hb.jpg"
            };

            //standart user
            EvernoteUser standartuser = new EvernoteUser()
            {
                Name = "Oğuzhan",
                Surname = "Herkiloğlu",
                Email = "oguz@bitnet.com.tr",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false,
                Username = "herkuloglu",
                Password = "123456",
                CreatedOn = DateTime.Now.AddHours(1),
                ModifiedOn = DateTime.Now.AddMinutes(75),
                ModifiedUsername = "herkuloglu",
                ProfileImageFileName="user.png"
            };

            context.EvernoteUsers.Add(admin);
            context.EvernoteUsers.Add(standartuser);

            //fake data user ekleme
            for (int i = 0; i < 8; i++)
            {
                EvernoteUser user = new EvernoteUser()
                {
                    Name = FakeData.NameData.GetFirstName(),
                    Surname = FakeData.NameData.GetSurname(),
                    Email = FakeData.NetworkData.GetEmail(),
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = true,
                    IsAdmin = false,
                    Username = $"user{i}",
                    Password = "123",
                    CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedUsername = $"user{i}",
                    ProfileImageFileName="user.png"
                };
                context.EvernoteUsers.Add(user);
            }
         

            context.SaveChanges();

            //USER LİSTESİ
            List<EvernoteUser> userlist = context.EvernoteUsers.ToList();

            //Kategorilere fake data ekleyelim.

            for (int i = 0; i < 10; i++)
            {
                Category kategori = new Category()
                {
                    Title = FakeData.PlaceData.GetStreetName(),
                    Description = FakeData.PlaceData.GetAddress(),
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now.AddMinutes(6),
                    ModifiedUsername = "hulyabilgin"
                };

                context.Categories.Add(kategori);

                //Fake data not ekleme
                for (int k = 0; k < FakeData.NumberData.GetNumber(5, 9); k++)
                {
                    EvernoteUser note_owner = userlist[FakeData.NumberData.GetNumber(0, userlist.Count - 1)];

                    Note not = new Note()
                    {
                        Title = FakeData.TextData.GetAlphabetical(FakeData.NumberData.GetNumber(5, 25)),
                        Text = FakeData.TextData.GetSentences(FakeData.NumberData.GetNumber(1, 3)),
                        Categori = kategori,
                        IsDraft = false,
                        LikeCount = FakeData.NumberData.GetNumber(1, 9),
                        Owner = note_owner,
                        CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedUsername = note_owner.Username,
                    };

                    kategori.Notes.Add(not);

                    //fake data yorum ekleme

                    for (int j = 0; j < FakeData.NumberData.GetNumber(3, 5); j++)
                    {
                        EvernoteUser comment_owner = userlist[FakeData.NumberData.GetNumber(0, userlist.Count - 1)];

                        Comment yorum = new Comment()
                        {
                            Text = FakeData.TextData.GetSentence(),
                            Owner = comment_owner,
                            CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedUsername = comment_owner.Username

                        };
                        not.Comments.Add(yorum);
                    }

                    //fake data like ekleme
                   

                    for (int m = 0; m < not.LikeCount; m++)
                    {
                        Liked Begen = new Liked()
                        {
                            LikedUser = userlist[m],
                        };

                        not.Likes.Add(Begen);

                    }
                }
            }

            //category,like,comment ve note save edilir. 
            context.SaveChanges();
        }
    }
}
