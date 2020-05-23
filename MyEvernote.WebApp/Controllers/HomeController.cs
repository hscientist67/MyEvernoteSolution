using MyEvernote.BusinessLayer;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ValueObjects;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Models;
using MyEvernote.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace MyEvernote.WebApp.Controllers
{
    [Exc]
    public class HomeController : Controller
    {
        private NoteManager nm = new NoteManager();
        private CategoryManager cm = new CategoryManager();
        private UserManager um = new UserManager();

        // GET: Home
        public ActionResult Index()
        {
          
            //Bu kısımda repository imizi test ettik.
            // BusinessLayer.Test test = new BusinessLayer.Test();
            // test.InserTest();
            // test.UpdateTest();
            //test.DeleteTest();
            //test.CommentTest();

            //CategoryControllerından tempdata ile alına  verileri listeleme
            //if (TempData["mm"] != null)
            //{
            //    return View(TempData["mm"] as List<Note>);
            //}


            //Bu verileri index view unda göster.
            return View(nm.ListQueryable().Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList());
            //return View(nm.GetAllNotesQueryable().OrderByDescending(x => x.ModifiedOn)).ToList();
        }

        public ActionResult ByCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<Note> notes = nm.ListQueryable().Where(x => x.IsDraft == false && x.CategoriId == id).OrderByDescending(x => x.ModifiedOn).ToList();

            return View("Index", notes);
        }

        public ActionResult Mostliked()
        {
            //Bu verileri index view unda göster.
            return View("Index", nm.ListQueryable().OrderByDescending(x => x.LikeCount).ToList());
        }

        public ActionResult About()
        {
            return View();
        }

        [Auth]
        public ActionResult ShowProfile()
        {
            BusinessLayerResult<EvernoteUser> res = um.GetUserById(SessionHelper.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorModel = new ErrorViewModel()
                {
                    Title = "Hata oluştu.",
                    items = res.Errors
                };

                return View("Error", errorModel);
            }

            return View(res.Result);
        }
        [Auth]
        public ActionResult EditProfile()
        {
            BusinessLayerResult<EvernoteUser> res = um.GetUserById(SessionHelper.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorModel = new ErrorViewModel()
                {
                    Title = "Hata oluştu.",
                    items = res.Errors
                };

                return View("Error", errorModel);
            }

            return View(res.Result);
        }
        [Auth]
        [HttpPost]
        public ActionResult EditProfile(EvernoteUser userModel, HttpPostedFileBase ProfileImage)
        {
            //Bu model elementini çıkar ondan sonra kontrol yap demiş olduk.
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                if (ProfileImage != null && (ProfileImage.ContentType == "image/jpeg" || ProfileImage.ContentType == "image/jpg" || ProfileImage.ContentType == "image/png"))
                {
                    string filename = $"user_{userModel.Id}.{ProfileImage.ContentType.Split('/')[1]}";

                    ProfileImage.SaveAs(Server.MapPath($"~/images/{filename}"));
                    userModel.ProfileImageFileName = filename;
                }


                BusinessLayerResult<EvernoteUser> res = um.UpdateProfile(userModel);

                if (res.Errors.Count > 0)
                {
                    ErrorViewModel errorModel = new ErrorViewModel()
                    {
                        Title = "Profil Güncellenemedi.",
                        RedirectingUrl = "/Home/EditProfile",
                        items = res.Errors
                    };

                    return View("Error", errorModel);
                }

                SessionHelper.Set<EvernoteUser>("login", res.Result);//profil güncellendiği için session güncellendi.

                return RedirectToAction("ShowProfile");
            }

            return View(userModel);
        }
        [Auth]
        public ActionResult DeleteProfile()
        {
            BusinessLayerResult<EvernoteUser> res = um.RemoveUserById(SessionHelper.User.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorModel = new ErrorViewModel()
                {
                    Title = "Profil Silinemedi.",
                    RedirectingUrl = "/Home/ShowProfile",
                    items = res.Errors
                };

                return View("Error", errorModel);
            }

            Session.Clear();

            return RedirectToAction("Index");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel loginModel)
        {
            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> res = um.LoginUser(loginModel);

                if (res.Errors.Count > 0)
                {
                    //tüm hata listesinde dön ve modelstate e ekle.
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(loginModel);
                }

                SessionHelper.Set<EvernoteUser>("login", res.Result);   //session da kullanıcı bilgisi saklama
                return RedirectToAction("Index"); //yönlendirme
            }
            return View(loginModel);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> res = um.RegisterUser(registerModel);

                if (res.Errors.Count > 0)
                {
                    //Error messajlarını UI tarafında ekstra kontrol etme imkanı sağlar.
                    if (res.Errors.Find(x => x.Code == ErrorMessageCode.UserIsNotActive) != null)
                    {
                        ViewBag.SetLink = "http://Activate/123456";
                    }
                    //tüm hata listesinde dön ve modelstate e ekle.
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(registerModel);
                }


                //Bu kısımda eğer farklı bir hata var ise bu hatayı ekrana göndermek için kullanırız.
                //foreach (var item in ModelState)
                //{
                //    if (item.Value.Errors.Count > 0)
                //    {
                //        return View(registerdata);
                //    }
                //}

                OkViewModel okModel = new OkViewModel();

                okModel.Title = "Kayıt işlemi gerçekleşti.";
                okModel.RedirectingUrl = "/Home/Login";
                okModel.items.Add("Lütfen e-posta adresine gönderdiğimiz aktivasyon linkine tıklayarak hesabınızı aktive edebilirsiniz.");

                return View("Ok", okModel);
            }
            return View(registerModel);
        }

        // Burada id parametresinden uri elde edildiği için routedan dolayı "id" ismiyle verilmeli aksi halde hata veriyor.
        public ActionResult UserActivate(Guid id)
        {
            BusinessLayerResult<EvernoteUser> res = um.ActivateUser(id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorModel = new ErrorViewModel()
                {
                    Title = "Geçersiz işlem",
                    items = res.Errors
                };

                return View("Error", errorModel);
            }

            OkViewModel okModel = new OkViewModel();

            okModel.Title = "Hesabınız aktifleştirildi.";
            okModel.RedirectingUrl = "/Home/Login";
            okModel.items.Add("Artık hesabınız üzerinden not paylaşımı yapabilirsiniz.");

            return View("Ok", okModel);
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult HasError()
        {
            return View();
        }
    }
}