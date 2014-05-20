using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using Mono.Model;

namespace Mono.Data
{
    public class UserRepository : GenericRepository<MyUser> 
    {
        public UserRepository()
            : base()
        {

        }

        public UserRepository(MonoDbContext context)
            : base(context)
        {

        }
        public virtual bool IsAdmin(MyUser user)
        {
            return (user.Roles.FirstOrDefault(f => f.Role.Name == "admin") != null);
        }
    }
}