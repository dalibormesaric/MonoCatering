using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using Mono.Model;

namespace Mono.Data
{
    public class OfferRepository : GenericRepository<Offer> 
    {
        public OfferRepository()
            : base()
        {

        }
        public OfferRepository(MonoDbContext context)
            : base(context)
        {

        }
    }
}