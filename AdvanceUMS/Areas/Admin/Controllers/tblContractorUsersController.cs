using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdvanceUMS.Models;

namespace AdvanceUMS.Areas.Admin.Controllers
{
    public class tblContractorUsersController : Controller
    {
        private Entities db = new Entities();

        // GET: Admin/tblContractorUsers
        public ActionResult Index()
        {
            return View(db.tblContractorUsers.ToList());
        }

        // GET: Admin/tblContractorUsers/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblContractorUser tblContractorUser = db.tblContractorUsers.Find(id);
            if (tblContractorUser == null)
            {
                return HttpNotFound();
            }
            return View(tblContractorUser);
        }

        // GET: Admin/tblContractorUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/tblContractorUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "uqId,strUsername,strPassword,uqContractorId,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblContractorUser tblContractorUser)
        {
            if (ModelState.IsValid)
            {
                tblContractorUser.uqId = Guid.NewGuid();
                db.tblContractorUsers.Add(tblContractorUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblContractorUser);
        }

        // GET: Admin/tblContractorUsers/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblContractorUser tblContractorUser = db.tblContractorUsers.Find(id);
            if (tblContractorUser == null)
            {
                return HttpNotFound();
            }
            return View(tblContractorUser);
        }

        // POST: Admin/tblContractorUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "uqId,strUsername,strPassword,uqContractorId,dtCreatedAt,dtMoidifyAt,intCreatedBy,intModifyAt")] tblContractorUser tblContractorUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblContractorUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblContractorUser);
        }

        // GET: Admin/tblContractorUsers/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblContractorUser tblContractorUser = db.tblContractorUsers.Find(id);
            if (tblContractorUser == null)
            {
                return HttpNotFound();
            }
            return View(tblContractorUser);
        }

        // POST: Admin/tblContractorUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            tblContractorUser tblContractorUser = db.tblContractorUsers.Find(id);
            db.tblContractorUsers.Remove(tblContractorUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
