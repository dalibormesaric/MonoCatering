using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using Mono.Model;

namespace Mono.Data
{
    public class RestaurantRepository : GenericRepository<Restaurant> 
    {
        public RestaurantRepository()
            : base()
        {

        }

        public RestaurantRepository(MonoDbContext context)
            : base(context)
        {

        }
    }
}