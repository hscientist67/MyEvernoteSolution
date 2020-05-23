using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyEvernote.BusinessLayer;
using MyEvernote.Entities;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Models;

namespace MyEvernote.WebApp.Controllers
{
    [Exc]
    public class NoteController : Controller
    {
        private NoteManager nm = new NoteManager();
        private CategoryManager cm = new CategoryManager();
        private LikedManager lm = new LikedManager();
        [Auth]
        public ActionResult Index()
        {
            //Burada listqueryable ile liste çekilir, include ile ilişkili category tablosunu da join ile getir denilir, sonrasında where atılır. en sonunda tolist denilince 
            // değerler gelir.
            //Buradaki categori ve owner isimleri "notemaneger" daki property isimleridir.
            var notes = nm.ListQueryable().Include("Categori").Include("Owner").Where(x => x.Owner.Id == SessionHelper.User.Id).OrderByDescending(x => x.ModifiedOn);
            return View(notes.ToList());
        }
        [Auth]
        public ActionResult MyLikedNotes()
        {
            var notes = lm.ListQueryable().Include("LikedUser").Include("Note").Where(x => x.LikedUser.Id == SessionHelper.User.Id).Select(x => x.Note).Include("Categori").Include("Owner").OrderByDescending(x => x.ModifiedOn);
            return View("index", notes.ToList());
        }
        [Auth]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = nm.Find(x => x.Id == id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }
        [Auth]
        public ActionResult Create()
        {
            ViewBag.CategoriId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title");
            return View();
        }
        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Note note)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                note.Owner = SessionHelper.User;
                nm.Insert(note);
                return RedirectToAction("Index");
            }

            ViewBag.CategoriId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoriId);
            return View(note);
        }
        [Auth]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = nm.Find(x => x.Id == id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoriId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoriId);
            return View(note);
        }
        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Note note)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                Note db_note = nm.Find(x => x.Id == note.Id);
                db_note.IsDraft = note.IsDraft;
                db_note.CategoriId = note.CategoriId;
                db_note.Text = note.Text;
                db_note.Title = note.Title;

                nm.Update(db_note);

                return RedirectToAction("Index");
            }
            ViewBag.CategoriId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoriId);
            return View(note);
        }
        [Auth]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = nm.Find(x => x.Id == id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }
        [Auth]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Note note = nm.Find(x => x.Id == id);
            nm.Delete(note);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GetLiked(int[] ids)
        {
            if (SessionHelper.User != null)
            {
                List<int> likedNoteIds = lm.List(
                    x => x.LikedUser.Id == SessionHelper.User.Id && ids.Contains(x.Note.Id)).Select(
                    x => x.Note.Id).ToList();

                return Json(new { result = likedNoteIds });
            }
            else
            {
                return Json(new { result = new List<int>() });
            }
        }

        [HttpPost]
        public ActionResult SetLikeState(int noteid, bool liked)
        {
            int res = 0;

            Liked like = lm.Find(x => x.Note.Id == noteid && x.LikedUser.Id == SessionHelper.User.Id);

            Note note = nm.Find(x => x.Id == noteid);

            if (like != null && liked == false)
            {
                res = lm.Delete(like);
            }
            else if (like == null && liked == true)
            {
                res = lm.Insert(new Liked()
                {
                    LikedUser = SessionHelper.User,
                    Note = note
                });
            }

            if (res > 0)
            {
                if (liked)
                {
                    note.LikeCount++;
                }
                else
                {
                    note.LikeCount--;
                }
                res = nm.Update(note);

                return Json(new { hasError = false, errorMessage = string.Empty, result = note.LikeCount });
            }
            return Json(new { hasError = true, errorMessage = "Beğenmede hata oluştu...", result = note.LikeCount });
        }

        public ActionResult GetNoteText(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = nm.Find(x => x.Id == id);

            if (note == null)
            {
                return HttpNotFound();
            }


            return PartialView("_PartialNoteText", note);
        }
    }
}
