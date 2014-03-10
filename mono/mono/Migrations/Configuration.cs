namespace mono.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using mono.Models;
    using mono.DAL;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<mono.DAL.MonoDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(mono.DAL.MonoDbContext context)
        {
            var userManager = new UserManager<MyUser>(new UserStore<MyUser>(new MonoDbContext()));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new MonoDbContext()));

            roleManager.Create(new IdentityRole("admin"));
            roleManager.Create(new IdentityRole("restaurant"));
            roleManager.Create(new IdentityRole("user"));

            var adminUser = new MyUser() { 
                UserName = "admin", 
                FirstName = "adminFirst", 
                LastName = "adminLast", 
                Email = "u.a@mail.com",
                Address = "Addressa 44"
            };
            userManager.Create(adminUser, "admin12345");
            userManager.AddToRole(adminUser.Id, "admin");

            var userUser = new MyUser() { 
                UserName = "user", 
                FirstName = "userFirst", 
                LastName = "userLast", 
                Email = "u.u@mail.com",
                Address = "Addressu 66"
            };
            userManager.Create(userUser, "user12345");
            userManager.AddToRole(userUser.Id, "user");

            var unitOfWork = new UnitOfWork();

            Restaurant restaurant1 = new Restaurant { 
                Name = "restaurant1", 
                Description = "Description restaurant 1", 
                Address = "Address 11, City", 
                Phone = "111 111 111",
                OIB = "1111111111"
            };
            Restaurant restaurant2 = new Restaurant
            {
                Name = "restaurant2",
                Description = "Description restaurant 2",
                Address = "Address 22, City",
                Phone = "222 222 222",
                OIB = "2222222222"
            };
            Restaurant restaurant3 = new Restaurant
            {
                Name = "restaurant3",
                Description = "Description restaurant 3",
                Address = "Address 33, City",
                Phone = "333 333 333",
                OIB = "3333333333"
            };

            unitOfWork.RestaurantRepository.Insert(restaurant1);
            unitOfWork.RestaurantRepository.Insert(restaurant2);
            unitOfWork.RestaurantRepository.Insert(restaurant3);
            unitOfWork.Save();

            var restaurant1User = new MyUser()
            {
                UserName = "restaurant1",
                FirstName = "userFirstR1",
                LastName = "userLastR3",
                Email = "r.1@mail.com",
                RestaurantID = restaurant1.ID,
                Address = "Addressr1 1"
            };
            userManager.Create(restaurant1User, "restaurant1");
            userManager.AddToRole(restaurant1User.Id, "restaurant");

            var restaurant2User = new MyUser()
            {
                UserName = "restaurant2",
                FirstName = "userFirstR2",
                LastName = "userLastR3",
                Email = "r.2@mail.com",
                RestaurantID = restaurant1.ID,
                Address = "Addressr2 2"
            };
            userManager.Create(restaurant2User, "restaurant2");
            userManager.AddToRole(restaurant2User.Id, "restaurant");

            var restaurant3User = new MyUser()
            {
                UserName = "restaurant3",
                FirstName = "userFirstR3",
                LastName = "userLastR3",
                Email = "r.3@mail.com",
                RestaurantID = restaurant2.ID,
                Address = "Addressr3 3"
            };
            userManager.Create(restaurant3User, "restaurant3");
            userManager.AddToRole(restaurant3User.Id, "restaurant");

            CategorySize sizePortion = new CategorySize
            {
                Type = 0,
                Value = "Portion",
                Order = 0
            };
            CategorySize sizeSmall = new CategorySize
            {
                Type = 1,
                Value = "Small",
                Order = 0
            };
            CategorySize sizeBig = new CategorySize
            {
                Type = 1,
                Value = "Big",
                Order = 1
            };
            CategorySize sizeJumbo = new CategorySize
            {
                Type = 1,
                Value = "Jumbo",
                Order = 2
            };

            unitOfWork.CategorySizeRepository.Insert(sizePortion);
            unitOfWork.CategorySizeRepository.Insert(sizeSmall);
            unitOfWork.CategorySizeRepository.Insert(sizeBig);
            unitOfWork.CategorySizeRepository.Insert(sizeJumbo);

            Category food = new Category 
            { 
                Name = "FOOD",
                SizeType = 0
            };           
            Category fish = new Category 
            { 
                Name = "FISH",
                SizeType = 0,
                ParentCategory = food
            };
            Food carp = new Food
            {
                Name = "CARP",
                Category = fish
            };
            Food trout = new Food
            {
                Name = "TROUT",
                Category = fish
            };
            Category italian = new Category { 
                Name = "ITALIAN", 
                SizeType = 0,
                ParentCategory = food
            };
            Food spaghetti = new Food
            {
                Name = "SPAGHETII",
                Category = italian
            };
            Category pizza = new Category {
                Name = "PIZZA",
                SizeType = 1,
                ParentCategory = italian
            };                    
            Food margarita = new Food { 
                Name = "MARGARITA", 
                Category = pizza
            };
            Food capricciosa = new Food { 
                Name = "CAPRICCIOSA", 
                Category = pizza
            };
            Food funghi = new Food { 
                Name = "FUNGHI", 
                Category = pizza
            };
            Food napolitana = new Food { 
                Name = "NAPOLITANA", 
                Category = pizza
            };
            Food milaneze = new Food { 
                Name = "MILANEZE", 
                Category = pizza
            };
            Category beverage = new Category
            {
                Name = "BEVERAGE",
                SizeType = 0
            };

            unitOfWork.CategoryRepository.Insert(food);
            unitOfWork.CategoryRepository.Insert(italian);
            unitOfWork.FoodRepository.Insert(spaghetti);
            unitOfWork.CategoryRepository.Insert(pizza);
            unitOfWork.FoodRepository.Insert(margarita);
            unitOfWork.FoodRepository.Insert(capricciosa);
            unitOfWork.FoodRepository.Insert(funghi);
            unitOfWork.FoodRepository.Insert(napolitana);
            unitOfWork.FoodRepository.Insert(milaneze);
            unitOfWork.CategoryRepository.Insert(fish);
            unitOfWork.FoodRepository.Insert(carp);
            unitOfWork.FoodRepository.Insert(trout);
            unitOfWork.CategoryRepository.Insert(beverage);

            Ingredient bread = new Ingredient
            {
                Name = "BREAD",
                Category = food
            };
            Ingredient ketchup = new Ingredient { 
                Name = "KETCHUP", 
                Category = pizza
            };
            Ingredient pepperoni = new Ingredient {
                Name = "PEPPERONI", 
                Category = pizza
            };
            Ingredient corn = new Ingredient { 
                Name = "CORN", 
                Category = pizza
            };
            Ingredient sausage = new Ingredient { 
                Name = "SAUSAGE", 
                Food = capricciosa
            };
            Ingredient egg = new Ingredient { 
                Name = "EGG", 
                Food = capricciosa
            };
            Ingredient mayonnaise = new Ingredient { 
                Name = "MAYONNAISE", 
                Category = fish
            };

            unitOfWork.IngredientRepository.Insert(bread);
            unitOfWork.IngredientRepository.Insert(ketchup);
            unitOfWork.IngredientRepository.Insert(pepperoni);
            unitOfWork.IngredientRepository.Insert(corn);
            unitOfWork.IngredientRepository.Insert(sausage);
            unitOfWork.IngredientRepository.Insert(egg);
            unitOfWork.IngredientRepository.Insert(mayonnaise);

            unitOfWork.Save();

            base.Seed(context);
         }
    }
}
