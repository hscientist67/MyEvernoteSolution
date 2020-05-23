using MyEvernote.BusinessLayer;
using MyEvernote.Entities;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyEvernote.WebApp.Controllers
{
    [Exc]
    [Auth]
    public class CommentController : Controller
    {
        private NoteManager nm = new NoteManager();
        private CommentManager cm = new CommentManager();

        // GET: Comment
        public ActionResult ShowNoteComments(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note not = nm.Find(x => x.Id == id);

            if (not == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PartialComments", not.Comments);
        }
        [HttpPost]
        public ActionResult Edit(int? id, string text)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment yorum = cm.Find(x => x.Id == id);

            if (yorum == null)
            {
                return new HttpNotFoundResult();
            }
            yorum.Text = text;
            int res = cm.Update(yorum);

            if (res > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment yorum = cm.Find(x => x.Id == id);

            if (yorum == null)
            {
                return new HttpNotFoundResult();
            }
            int res = cm.Delete(yorum);

            if (res > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Insert(Comment yorum, int? noteid)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                if (noteid == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Note not = nm.Find(x => x.Id == noteid);

                if (not == null)
                {
                    return new HttpNotFoundResult();
                }
                yorum.Note = not;
                yorum.Owner = SessionHelper.User;

                int res = cm.Insert(yorum);

              
                if (res > 0)
                {
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
    }
}