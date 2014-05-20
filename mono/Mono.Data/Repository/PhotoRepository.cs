using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using Mono.Model;

namespace Mono.Data
{
    public class PhotoRepository : GenericRepository<Photo> 
    {
        public PhotoRepository()
            : base()
        {

        }
        public PhotoRepository(MonoDbContext context)
            : base(context)
        {

        }
    }
}