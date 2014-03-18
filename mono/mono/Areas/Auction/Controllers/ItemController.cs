using Mono.Areas.Auction.Models;
using Mono.Data;
using Mono.Model;
using Mono.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data;

namespace Mono.Areas.Auction.Controllers
{
    [Authorize(Roles = "user")]
    public class ItemController : Controller
    {
        private MonoDbContext db = new MonoDbContext();
        
        private UnitOfWork unitOfWork;

        public ItemController()
        {
            unitOfWork = new UnitOfWork();
        }

        public ItemController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        //
        // GET: /Auction/Item/
        public ActionResult Index(int? id)
        {
            ItemViewModel itemViewModel = new ItemViewModel();
            itemViewModel.ListCategoryFood = new List<ListCategoryFoodItem>();

            if (id == null)
            {
                itemViewModel.Name = "All";
                
                var categories = unitOfWork.CategoryRepository.Get(c => c.ParentCategoryID == null).OrderBy(c => c.Name);

                foreach (var category in categories)
                {
                    ListCategoryFoodItem listCategoryFoodItem = new ListCategoryFoodItem
                    {
                        Name = category.Name,
                        ItemID = category.ID,
                        Type = ListCategoryFoodItemType.Category
                    };
                    itemViewModel.ListCategoryFood.Add(listCategoryFoodItem);
                }
            }
            else
            {
                Category category = unitOfWork.CategoryRepository.GetByID(id);

                if (category == null)
                    return HttpNotFound();

                itemViewModel.Name = category.Name;
                itemViewModel.ParentCategoryID = category.ParentCategoryID;
                AddCategoryFood(itemViewModel, category);
            }
            
            if(Request.IsAjaxRequest())
            {
                return PartialView("_Category", itemViewModel);
            }

            return View("Index", itemViewModel);
        }

        //
        // GET: /Auction/Add/
        public ActionResult Add(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Food food = unitOfWork.FoodRepository.GetByID(id);

            if (food == null)
            {
                return HttpNotFound();
            }

            Category category = unitOfWork.CategoryRepository.GetByID(food.CategoryID);
            ViewBag.CategorySizeID = new SelectList(unitOfWork.SizeValues(category.SizeType), "ID", "Value");
            ViewBag.IngredientsList = new MultiSelectList(unitOfWork.IngredientsForFood(food), "ID", "Name");

            FoodIngredientViewModel foodIngredientViewModel = new FoodIngredientViewModel();
            foodIngredientViewModel.FoodID = (int)id;
            foodIngredientViewModel.FoodName = food.Name;
            foodIngredientViewModel.Pieces = 1;

            return View("Add", foodIngredientViewModel);
        }

        // POST: /Auction/Item/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(FoodIngredientViewModel foodIngredientViewModel)
        {
            if (foodIngredientViewModel.FoodID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (unitOfWork.FoodRepository.GetByID(foodIngredientViewModel.FoodID) == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                FoodIngredient foodIngredient = new FoodIngredient();

                foodIngredient.UserID = User.Identity.GetUserId();
                foodIngredient.FoodID = (int)foodIngredientViewModel.FoodID;
                foodIngredient.Description = foodIngredientViewModel.Description;
                foodIngredient.CategorySizeID = foodIngredientViewModel.CategorySizeID;
                foodIngredient.Pieces = foodIngredientViewModel.Pieces;

                foodIngredient.Ingredients = new List<Ingredient>();

                if (foodIngredientViewModel.Ingredients != null)
                    foreach (string stringId in foodIngredientViewModel.Ingredients)
                    {
                        int id;
                    
                        if (Int32.TryParse(stringId, out id))
                        {
                            Ingredient ingredient = unitOfWork.IngredientRepository.GetByID(id);

                            if (ingredient != null)
                            {
                                foodIngredient.Ingredients.Add(ingredient);
                            }
                        }
                    }

                unitOfWork.FoodIngredientRepository.Insert(foodIngredient);
                unitOfWork.Save();

                return RedirectToAction("Index");
            }

            if (foodIngredientViewModel.FoodID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Food food = unitOfWork.FoodRepository.GetByID(foodIngredientViewModel.FoodID);

            if (food == null)
            {
                return HttpNotFound();
            }

            Category category = unitOfWork.CategoryRepository.GetByID(food.CategoryID);

            ViewBag.CategorySizeID = new SelectList(unitOfWork.SizeValues(category.SizeType), "ID", "Value");
            ViewBag.IngredientsList = new MultiSelectList(unitOfWork.IngredientsForFood(food), "ID", "Name");

            return View(foodIngredientViewModel);
        }


        //
        // GET: /Auction/Order/Basket
        public ActionResult Basket()
        {
            var userID = User.Identity.GetUserId();
            var foodIngredients = unitOfWork.FoodIngredientRepository.Get(fi => fi.UserID == userID && fi.OrderID == null).ToList();

            return View("Basket", foodIngredients);
        }

        // GET: /Auction/Order/Delete/5
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

        // POST: /Auction/Order/Delete/5
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
            return RedirectToAction("Basket");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        #region HelperFunctions

        private void AddCategoryFood(ItemViewModel itemViewModel, Category category)
        {
            var subCategories = category.ChildCategory.OrderBy(c => c.Name).ToList();

            foreach (var subcategory in subCategories)
            {
                ListCategoryFoodItem listCategoryFoodItem = new ListCategoryFoodItem
                {
                    Name = subcategory.Name,
                    ItemID = subcategory.ID,
                    Type = ListCategoryFoodItemType.Category
                };
                itemViewModel.ListCategoryFood.Add(listCategoryFoodItem);
            }

            var foods = category.Food.OrderBy(f => f.Name).ToList();

            foreach (var food in foods)
            {
                ListCategoryFoodItem listCategoryFoodItem = new ListCategoryFoodItem
                {
                    Name = food.Name,
                    ItemID = food.ID,
                    Type = ListCategoryFoodItemType.Food
                };
                itemViewModel.ListCategoryFood.Add(listCategoryFoodItem);
            }
        }

        #endregion
    }
}