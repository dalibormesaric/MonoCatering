using System;
namespace Mono.Data
{
    public interface IUnitOfWork
    {
        global::Mono.Data.GenericRepository<global::Mono.Model.Category> CategoryRepository { get; }
        global::Mono.Data.GenericRepository<global::Mono.Model.CategorySize> CategorySizeRepository { get; }
        void Dispose();
        global::Mono.Data.GenericRepository<global::Mono.Model.FoodIngredient> FoodIngredientRepository { get; }
        global::Mono.Data.GenericRepository<global::Mono.Model.Food> FoodRepository { get; }
        global::Mono.Data.GenericRepository<global::Mono.Model.Ingredient> IngredientRepository { get; }
        global::System.Collections.Generic.List<global::Mono.Model.Ingredient> IngredientsForFood(global::Mono.Model.Food food);
        global::System.Collections.Generic.List<global::Mono.Model.Ingredient> IngredientsForFoodIngredient(global::System.Collections.Generic.List<int> ingredientsID);
        global::Mono.Data.GenericRepository<global::Mono.Model.Offer> OfferRepository { get; }
        global::Mono.Data.GenericRepository<global::Mono.Model.Order> OrderRepository { get; }
        global::Mono.Data.GenericRepository<global::Mono.Model.Restaurant> RestaurantRepository { get; }
        void Save();
        global::System.Collections.Generic.List<global::Mono.Model.CategorySize> SizeValues(int sizeType);
        global::System.Collections.Generic.List<global::Mono.Data.UnitOfWork.TypeSelectList> SizeValuesSelectList();
        string SizeValuesString(int sizeType);
        global::Mono.Data.GenericRepository<global::Mono.Model.MyUser> UserRepository { get; }
    }
}
