using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using Mono.Model;

namespace Mono.Data
{
    public class OrderRepository : GenericRepository<Order> 
    {
        public OrderRepository()
            : base()
        {

        }
        public OrderRepository(MonoDbContext context)
            : base(context)
        {

        }
    }
}