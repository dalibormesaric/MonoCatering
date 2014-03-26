using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Mono.Data;
using Mono.Model;
using System.Net;
using System.Data;

namespace Mono.Areas.Auction.Controllers
{
    [Authorize(Roles = "user")]
    public class BasketController : Controller
    {      
        private UnitOfWork unitOfWork;

        public BasketController()
        {
            unitOfWork = new UnitOfWork();
        }

        public BasketController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        //
        // GET: /Auction/Basket/
        public ActionResult Index()
        {
            var userID = User.Identity.GetUserId();
            var foodIngredients = unitOfWork.FoodIngredientRepository.Get(fi => fi.UserID == userID && fi.OrderID == null).ToList();

            return View("Index", foodIngredients);
        }

        // GET: /Auction/Basket/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            FoodIngredient foodIngredient = unitOfWork.FoodIngredientRepository.GetByID((int)id);
            if (foodIngredient == null)
            {
                return HttpNotFound();
            }
            return View("Delete", foodIngredient);
        }

        // POST: /Auction/Basket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                FoodIngredient foodIngredient = unitOfWork.FoodIngredientRepository.GetByID(id);
                unitOfWork.FoodIngredientRepository.Delete(id);
                unitOfWork.Save();
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
	}
}