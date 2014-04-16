using Mono.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using Mono.Data;
using Mono.Areas.Admin.Models;
using Moq;
using System.Web.Helpers;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using System.Reflection;

namespace Mono.Tests.Admin.Fake
{
    public static class CategoryFake
    {
        public static Category categoryNull = null;
        public static Category category = new Category { Name = "category", SizeType = 0 };

        public static Category category1 = new Category { Name = "category1", SizeType = 0 };
        public static Category category2 = new Category { Name = "category2", SizeType = 0, ParentCategory = category1, ChildCategory = new List<Category>() };
        public static Category category3 = new Category { Name = "caadfgry3", SizeType = 0 };
        public static Category category4 = new Category { Name = "category4", SizeType = 0, ParentCategory = category1, ChildCategory = new List<Category>() };
        public static Category category5 = new Category { Name = "category5", SizeType = 0 };
        public static Category category6 = new Category { Name = "category6", SizeType = 0 };

        public static IQueryable<Category> categories = new List<Category> { category1, category2, category3, category4, category5, category6 }.AsQueryable().OrderBy(c => c.Name);
        public static IEnumerable<Category> categoriesPagedList = categories.ToPagedList(1, Global.PageSize);

        public static List<UnitOfWork.TypeSelectList> typeSelectList = new List<UnitOfWork.TypeSelectList> { new UnitOfWork.TypeSelectList { SizeValuesString = "Portion" } };
        public static List<Photo> photoList = new List<Photo> { new Photo { FileName = "fileName" } };      

        public static Category categoryWithChilds = new Category { Name = "category", SizeType = 0, ChildCategory = new List<Category> { category2, category4 } };

        public static Ingredient parentIngredient1 = new Ingredient { Name = "parentIngredient1", Category = parentCategory };
        public static Ingredient parentIngredient2 = new Ingredient { Name = "parentIngredient2", Category = parentCategory };

        public static Category parentCategory = new Category { ID = 3, Name = "parentCategory", Ingredients = new List<Ingredient> { parentIngredient1, parentIngredient2 } };

        public static Ingredient childIngredient1 = new Ingredient { Name = "childIngredient1", Category = childCategory };
        public static Ingredient childIngredient2 = new Ingredient { Name = "achildIngredient2", Category = childCategory };

        public static Category childCategory = new Category { ID = 6, Name = "childCategory", ParentCategoryID = 3, Ingredients = new List<Ingredient> { childIngredient1, childIngredient2 } };

        public static List<Ingredient> allIngredients = new List<Ingredient> { childIngredient2, childIngredient1, parentIngredient1, parentIngredient2 };
    }

    public static class CategorySizeFake
    {
        public static CategorySize categorySizeNull = null;
        public static CategorySize categorySize = new CategorySize { Value = "categorySize", Type = 0 };
        
        public static CategorySize categorySize1 = new CategorySize { Value = "categorySize1", Type = 0 };
        public static CategorySize categorySize2 = new CategorySize { Value = "cdfgegrySize2", Type = 0 };
        public static CategorySize categorySize3 = new CategorySize { Value = "casdfgrySize3", Type = 0 };
        public static CategorySize categorySize4 = new CategorySize { Value = "categorySize4", Type = 0 };
        public static CategorySize categorySize5 = new CategorySize { Value = "categorySize5", Type = 0 };
        public static CategorySize categorySize6 = new CategorySize { Value = "categorySize6", Type = 0 };

        public static IQueryable<CategorySize> categorySizes = new List<CategorySize> { categorySize1, categorySize2, categorySize3, categorySize4, categorySize5, categorySize6 }.AsQueryable().OrderBy(c => c.Order);
        public static IEnumerable<CategorySize> categoriesPagedList = categorySizes.ToPagedList(1, Global.PageSize);
    }

    public static class FoodFake
    {
        public static Food foodNull = null;
        public static Food food = new Food { CategoryID = 6, Ingredients = new List<Ingredient> { foodIngredient1, } };

        private static Category category = new Category { Name = "category" };

        public static Food food1 = new Food { Name = "food1", Category = category };
        public static Food food2 = new Food { Name = "cdfgegry2", Category = category };
        public static Food food3 = new Food { Name = "casdfgry3", Category = category };
        public static Food food4 = new Food { Name = "food4", Category = category };
        public static Food food5 = new Food { Name = "food5", Category = category };
        public static Food food6 = new Food { Name = "food6", Category = category };

        public static IQueryable<Food> foods = new List<Food> { food1, food2, food3, food4, food5, food6 }.AsQueryable().OrderBy(f => f.Name);
        public static IEnumerable<Food> foodsPagedList = foods.ToPagedList(1, Global.PageSize);

        public static Category category1 = new Category { ID = 6, Name = "category1", ChildCategory = new List<Category>(), Food = new List<Food>() };
        public static Category category2 = new Category { ID = 3, Name = "category2", ChildCategory = new List<Category>(), Food = new List<Food>() };
        
        public static IQueryable<Category> categoriesOrdered = new List<Category> { category1, category2 }.AsQueryable();

        public static Ingredient foodIngredient1 = new Ingredient { Name = "foodIngredient1" };

        public static List<Photo> photoList = new List<Photo> { new Photo { FileName = "fileName" } };

        public static Category categoryWithChilds = new Category { Name = "category", SizeType = 0, ChildCategory = new List<Category> { category1, category2 } , Food = new List<Food>() };

        public static Ingredient parentIngredient1 = new Ingredient { Name = "parentIngredient1" };
        public static Ingredient parentIngredient2 = new Ingredient { Name = "parentIngredient2" };

        public static Category parentCategory = new Category { ID = 3, Name = "parentCategory", Ingredients = new List<Ingredient> { parentIngredient1, parentIngredient2 } };
        
        public static Ingredient childIngredient1 = new Ingredient { Name = "childIngredient1", Category = childCategory };
        public static Ingredient childIngredient2 = new Ingredient { Name = "achildIngredient2", Category = childCategory };
        public static Category childCategory = new Category { ID = 6, Name = "childCategory", ParentCategoryID = 3, Ingredients = new List<Ingredient> { childIngredient1, childIngredient2 } };
        public static List<Ingredient> allIngreients = new List<Ingredient> { childIngredient2, childIngredient1, FoodFake.food.Ingredients.ElementAt(0), parentIngredient1, parentIngredient2 };
    }

    public static class IngredientFake
    {
        public static Ingredient ingredientNull = null;
        public static Ingredient ingredient = new Ingredient { CategoryID = 6 };

        private static Category category = new Category { Name = "category" };
        private static Food food = new Food { Name = "food" };
        
        public static Ingredient ingredient1 = new Ingredient { Name = "ingredient1", Category = category, Food = food };
        public static Ingredient ingredient2 = new Ingredient { Name = "ingrsfgent2", Category = category, Food = food };
        public static Ingredient ingredient3 = new Ingredient { Name = "afasgdient3", Category = category, Food = food };
        public static Ingredient ingredient4 = new Ingredient { Name = "ingredient4", Category = category, Food = food };
        public static Ingredient ingredient5 = new Ingredient { Name = "ingredient5", Category = category, Food = food };
        public static Ingredient ingredient6 = new Ingredient { Name = "ingredient6", Category = category, Food = food };

        public static IQueryable<Ingredient> ingredients = new List<Ingredient> { ingredient1, ingredient2, ingredient3, ingredient4, ingredient5, ingredient6 }.AsQueryable().OrderBy(i => i.Name);
        public static IEnumerable<Ingredient> ingredientsPagedList = ingredients.ToPagedList(1, Global.PageSize);

        public static Category category1 = new Category { ID = 6, Name = "category1" };
        public static Category category2 = new Category { ID = 3, Name = "category2" };

        public static IQueryable<Category> categoriesOrdered = new List<Category> { category1, category2 }.AsQueryable();

        public static Food food1 = new Food { ID = 6, Name = "food1" };
        public static Food food2 = new Food { ID = 3, Name = "food2" };
        public static IQueryable<Food> foodsOrdered = new List<Food> { food1, food2 }.AsQueryable();
    }

    public static class RestaurantFake
    {
        public static Restaurant restaurantNull = null;
        public static Restaurant restaurant = new Restaurant { Name = "restoran" };

        public static Restaurant restaurant1 = new Restaurant { Name = "restoran1", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
        public static Restaurant restaurant2 = new Restaurant { Name = "resafsds2", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
        public static Restaurant restaurant3 = new Restaurant { Name = "resafsds3", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
        public static Restaurant restaurant4 = new Restaurant { Name = "restoran4", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
        public static Restaurant restaurant5 = new Restaurant { Name = "restoran5", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
        public static Restaurant restaurant6 = new Restaurant { Name = "restoran6", Address = "adresa 45, grad", Phone = "031 555 555", OIB = "12345678901", Description = "opis" };
        
        public static IQueryable<Restaurant> restaurants = new List<Restaurant> { restaurant1, restaurant2, restaurant3, restaurant4, restaurant5, restaurant6 }.AsQueryable().OrderBy(r => r.Name);
        public static IEnumerable<Restaurant> restaurantsPagedList = restaurants.ToPagedList(1, Global.PageSize);

        public static MyUser user1 = new MyUser { FirstName = "zzzz", LastName = "z" };
        public static MyUser user2 = new MyUser { FirstName = "aaaa", LastName = "a" };
        public static IQueryable<MyUser> users = new List<MyUser>() { user1, user2 }.AsQueryable();
    }

    public static class UserFake
    {
        public static MyUser userNull = null;
        public static MyUser user = new MyUser { Id = "6" };

        public static MyUser user1 = new MyUser { UserName = "user1", FirstName = "first1", LastName = "last1"};
        public static MyUser user2 = new MyUser { UserName = "aser2", FirstName = "first2", LastName = "last2" };
        public static MyUser user3 = new MyUser { UserName = "udfg3", FirstName = "first3", LastName = "last3" };
        public static MyUser user4 = new MyUser { UserName = "user4", FirstName = "first4", LastName = "last4" };
        public static MyUser user5 = new MyUser { UserName = "user5", FirstName = "first5", LastName = "last5" };
        public static MyUser user6 = new MyUser { UserName = "user6", FirstName = "first6", LastName = "last6" };

        public static IQueryable<MyUser> users = new List<MyUser> { user1, user2, user3, user4, user5, user6 }.AsQueryable().OrderBy(u => u.UserName);

        public static Restaurant restaurant1 = new Restaurant { ID = 6, Name = "restaurant1" };
        public static Restaurant restaurant2 = new Restaurant { ID = 3, Name = "restaurant2" };

        public static IQueryable<Restaurant> restaurantsOrdered = new List<Restaurant> { restaurant1, restaurant2 }.AsQueryable();
    }

    public static class PhotoFake
    {
        public static Photo photoNull = null;
        public static Photo photo = new Photo { FileName = "fileName" };

        private static Mock mockImage = new Mock<System.Web.HttpPostedFileBase>();

        public static PhotoUploadViewModel photoUploadViewModelImageNull = new PhotoUploadViewModel { FileName = "fileName" };
        public static PhotoUploadViewModel photoUploadViewModel = new PhotoUploadViewModel { FileName = "fileName", Image = (System.Web.HttpPostedFileBase)mockImage.Object };

        public static PhotoEditViewModel photoEditViewModel = new PhotoEditViewModel { FileName = "fileName", NewFileName = "newFileName" };

        public static Photo photo1 = new Photo { FileName = "photo1" };
        public static Photo photo2 = new Photo { FileName = "asser2" };
        public static Photo photo3 = new Photo { FileName = "udsfg3" };
        public static Photo photo4 = new Photo { FileName = "photo4" };
        public static Photo photo5 = new Photo { FileName = "photo5" };
        public static Photo photo6 = new Photo { FileName = "photo6" };

        public static IQueryable<Photo> photos = new List<Photo> { photo1, photo2, photo3, photo4, photo5, photo6 }.AsQueryable().OrderBy(u => u.FileName);
        public static IEnumerable<Photo> photosPagedList = photos.ToPagedList(1, Global.PageSize);

        public static WebImage webImageNull = null;

        private static string path = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
        public static WebImage webImage = new WebImage(new FileStream(Path.Combine(Path.GetDirectoryName(path), @"Admin\Fakes\_.png"), FileMode.Open, FileAccess.Read));
    }
}
