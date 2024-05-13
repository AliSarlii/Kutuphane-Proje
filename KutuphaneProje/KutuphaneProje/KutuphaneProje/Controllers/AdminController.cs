using KutuphaneProje.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace KutuphaneProje.Controllers
{
    public class AdminController : Controller
    {

        private KutuphaneEntities db = new KutuphaneEntities();

        //Admin giriş sayfası
        public ActionResult Giris()
        {
            return View();
        }

        //Admin giriş sayfası POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Giris([Bind(Include = "Id,Eposta , Sifre")] Admin admin)
        {
            if(admin.Eposta != null && admin.Sifre !=null)
            {
                var login = db.Admin.Where(x => x.Eposta == admin.Eposta).SingleOrDefault();
                if (login == null)
                {
                    ViewBag.yanlis = true;
                    return View();
                }
                if(admin.Eposta==login.Eposta && admin.Sifre == login.Sifre)
                {
                    ViewBag.yanlis = false;
                    Session["adminid"] = login.Id;
                    Session["eposta"] = login.Eposta;
                    Session["sifre"] = login.Sifre;
                    return RedirectToAction("Home", "Admin");   
                }
                else
                {
                    ViewBag.yanlis = true; 
                    return View();
                }
            }
            else 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        //Çıkış Sayfası
        public ActionResult Cikis()
        {
            Session["adminid"] = null;
            Session["eposta"] = null;
            return RedirectToAction("Giris","Admin");
        }
        //Home sayfası
        public ActionResult Home()
        {
            return View();
        }


        //----------------------------------------------KİTAP İŞLEMLERİ----------------------------------------------
        
        // Kitap Listele sayfası
        public ActionResult KitapIndex()
        {
            var kitap = db.Kitap.Include(k => k.Kitap_Dili).Include(k => k.Kitap_Konusu).Include(k => k.Kitap_Turu);
            return View(kitap.ToList());
        }

       // GET: Kitap/Create
        public ActionResult KitapCreate()
        {
            ViewBag.KitapDil = new SelectList(db.Kitap_Dili, "Id", "KitapDili");
            ViewBag.KitapKonu = new SelectList(db.Kitap_Konusu, "Id", "KitapKonusu");
            ViewBag.KitapTur = new SelectList(db.Kitap_Turu, "Id", "KitapTuru");
            return View();
        }

        // POST: Kitap/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KitapCreate([Bind(Include = "Id,KitapAd,KitapTur,KitapKonu,KitapYazar,KitapDil,KitapYıl,KitapSayfa,KitapFoto,KitapDoluBos")] Kitap kitap)
        {
            if (ModelState.IsValid)
            {
                db.Kitap.Add(kitap);
                db.SaveChanges();
                return RedirectToAction("KitapIndex");
            }

            ViewBag.KitapDil = new SelectList(db.Kitap_Dili, "Id", "KitapDili", kitap.KitapDil);
            ViewBag.KitapKonu = new SelectList(db.Kitap_Konusu, "Id", "KitapKonusu", kitap.KitapKonu);
            ViewBag.KitapTur = new SelectList(db.Kitap_Turu, "Id", "KitapTuru", kitap.KitapTur);
            return View(kitap);
        }

        // GET: Kitap/Edit/5
        public ActionResult KitapEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kitap kitap = db.Kitap.Find(id);
            if (kitap == null)
            {
                return HttpNotFound();
            }
            ViewBag.KitapDil = new SelectList(db.Kitap_Dili, "Id", "KitapDili", kitap.KitapDil);
            ViewBag.KitapKonu = new SelectList(db.Kitap_Konusu, "Id", "KitapKonusu", kitap.KitapKonu);
            ViewBag.KitapTur = new SelectList(db.Kitap_Turu, "Id", "KitapTuru", kitap.KitapTur);
            return View(kitap);
        }

        // POST: Kitap/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KitapEdit([Bind(Include = "Id,KitapAd,KitapTur,KitapKonu,KitapYazar,KitapDil,KitapYıl,KitapSayfa,KitapFoto,KitapDoluBos")] Kitap kitap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kitap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("KitapIndex");
            }
            ViewBag.KitapDil = new SelectList(db.Kitap_Dili, "Id", "KitapDili", kitap.KitapDil);
            ViewBag.KitapKonu = new SelectList(db.Kitap_Konusu, "Id", "KitapKonusu", kitap.KitapKonu);
            ViewBag.KitapTur = new SelectList(db.Kitap_Turu, "Id", "KitapTuru", kitap.KitapTur);
            return View(kitap);
        }

        // GET: Kitap/Delete/5
        public ActionResult KitapDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kitap kitap = db.Kitap.Find(id);
            if (kitap == null)
            {
                return HttpNotFound();
            }
            return View(kitap);
        }

        // POST: Kitap/Delete/5
        [HttpPost, ActionName("KitapDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult KitapDeleteConfirmed(int id)
        {
            Kitap kitap = db.Kitap.Find(id);
            var kitapOneriler = db.KitapOneriler.Where(o => o.KitapId == kitap.Id).ToList();
            if (kitapOneriler != null)
            {
                foreach (var oneri in kitapOneriler)
                {
                    db.KitapOneriler.Remove(oneri);
                }
            }
            db.Kitap.Remove(kitap);
            db.SaveChanges();
            return RedirectToAction("KitapIndex");
        }
        //------------Kitap Konusu İşlemleri------------
        public ActionResult KitapKonuIndex()
        {
            ViewBag.veri = db.Kitap_Konusu.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KitapKonuIndex([Bind(Include = "Id,KitapKonusu")] Kitap_Konusu kitap_Konusu)
        {
            if (ModelState.IsValid)
            {
                if (kitap_Konusu == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    db.Kitap_Konusu.Add(kitap_Konusu);
                    db.SaveChanges();
                    return RedirectToAction("KitapKonuIndex");
                }
                
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult KitapKonuDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Kitap_Konusu kitapkonu = db.Kitap_Konusu.Find(id);
                if (kitapkonu == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    var kitapkonuliste = db.Kitap.Where(o => o.KitapKonu == kitapkonu.Id).ToList();
                    foreach (var kitapp in kitapkonuliste)
                    {
                        kitapp.KitapKonu = null;
                    }
                    db.Kitap_Konusu.Remove(kitapkonu);
                    db.SaveChanges();
                    return RedirectToAction("KitapKonuIndex");
                }
            }
        }

        //------------Kitap Türü İşlemleri------------
        public ActionResult KitapTurIndex()
        {
            ViewBag.veri = db.Kitap_Turu.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KitapTurIndex([Bind(Include = "Id,KitapTuru")] Kitap_Turu kitap_Turu)
        {
            if (ModelState.IsValid)
            {
                if (kitap_Turu == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    db.Kitap_Turu.Add(kitap_Turu);
                    db.SaveChanges();
                    return RedirectToAction("KitapTurIndex");
                }

            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult KitapTurDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Kitap_Turu kitapturu = db.Kitap_Turu.Find(id);
                if (kitapturu == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    var kitapturliste = db.Kitap.Where(o => o.KitapTur == kitapturu.Id).ToList();
                    foreach (var kitapp in kitapturliste)
                    {
                        kitapp.KitapTur = null;
                    }
                    db.Kitap_Turu.Remove(kitapturu);
                    db.SaveChanges();
                    return RedirectToAction("KitapTurIndex");
                }
            }
        }

        //------------Kitap Dili İşlemleri------------
        public ActionResult KitapDilIndex()
        {
            ViewBag.veri = db.Kitap_Dili.ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KitapDilIndex([Bind(Include = "Id,KitapDili")] Kitap_Dili kitap_Dili)
        {
            if (ModelState.IsValid)
            {
                if (kitap_Dili == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    db.Kitap_Dili.Add(kitap_Dili);
                    db.SaveChanges();
                    return RedirectToAction("KitapDilIndex");
                }

            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        public ActionResult KitapDilDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Kitap_Dili kitapdili = db.Kitap_Dili.Find(id);
                if (kitapdili == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    var kitapdilliste = db.Kitap.Where(o => o.KitapDil == kitapdili.Id).ToList();
                    foreach (var kitapp in kitapdilliste)
                    {
                        kitapp.KitapDil = null;
                    }
                    db.Kitap_Dili.Remove(kitapdili);
                    db.SaveChanges();
                    return RedirectToAction("KitapDilIndex");
                }
            }
        }


        //---------------------------------------------İLETİŞİM İŞLEMLERİ---------------------------------------------
        public ActionResult IletisimIndex()
        {
            return View(db.Iletisim.ToList());
        }
        
        // GET: Iletisim/Delete/5
        public ActionResult IletisimDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Iletisim iletisim = db.Iletisim.Find(id);
                if (iletisim == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    db.Iletisim.Remove(iletisim);
                    db.SaveChanges();
                    return RedirectToAction("IletisimIndex");
                }
            }
        }

        //-------------------------------------------ÖNE ÇIKARILAN İŞLEMLERİ-------------------------------------------

        public ActionResult OneCikarilanIndex()
        {
            var kitapOnerileri = db.KitapOneriler.ToList();
            ViewBag.Count = kitapOnerileri.Count();
            return View(kitapOnerileri);
        }
        public ActionResult OneCikarilanEkle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Kitap kitap = db.Kitap.Find(id);
                if (kitap == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    var kitapOnerileri = db.KitapOneriler.Where(o => o.KitapId == kitap.Id).ToList();
                    if(kitapOnerileri.Count !=0)
                    {
                        return RedirectToAction("KitapIndex");
                    }
                    else
                    {
                        KitapOneriler kitapOneriler = new KitapOneriler();
                        kitapOneriler.Kitap = kitap;
                        db.KitapOneriler.Add(kitapOneriler);
                        db.SaveChanges();
                        return RedirectToAction("KitapIndex");
                    }
                    
                }
            }
            
        }
        public ActionResult OneCikarilanDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                KitapOneriler kitapOneriler = db.KitapOneriler.Find(id);
                if (kitapOneriler == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    db.KitapOneriler.Remove(kitapOneriler);
                    db.SaveChanges();
                    return RedirectToAction("OneCikarilanIndex");
                }
            }
        }

        public ActionResult AdminBilgi()
        {
 
            return View(db.Admin.ToList());
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