using KutuphaneProje.Models;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KutuphaneProje.Controllers
{
    public class KutuphaneController : Controller
    {
        private KutuphaneEntities db = new KutuphaneEntities();
        // GET: Kutuphane
        public ActionResult Home()
        {
            var kitapOnerileri = db.KitapOneriler.ToList();
            return View(kitapOnerileri);
        }
        public ActionResult KitapListele()
        {
            var kitaplar = db.Kitap.Include(k => k.Kitap_Dili).Include(k => k.Kitap_Konusu).Include(k => k.Kitap_Turu).ToList();
            return View(kitaplar);
        }

        public ActionResult Iletisim()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Iletisim([Bind(Include = "Id,AdSoyad,Eposta,Mesaj")] Iletisim iletisim)
        {
            if (ModelState.IsValid)
            {
                db.Iletisim.Add(iletisim);
                db.SaveChanges();
                return RedirectToAction("Iletisim");
            }

            return View(iletisim);
        }
        public ActionResult Hakkında()
        {
            return View();
        }
      
    }
}