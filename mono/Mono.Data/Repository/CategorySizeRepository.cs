using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using Mono.Model;

namespace Mono.Data
{
    public class CategorySizeRepository : GenericRepository<CategorySize>
    {
        public CategorySizeRepository()
            : base()
        {

        }
        public CategorySizeRepository(MonoDbContext context)
            : base(context)
        {

        }
        public virtual List<CategorySize> SizeValues(int sizeType)
        {
            return Get(s => s.Type == sizeType, q => q.OrderBy(s => s.Order), "").ToList();
        }

        public virtual String SizeValuesString(int sizeType)
        {
            return string.Join(", ", SizeValues(sizeType).Select(s => s.Value).ToArray());
        }

        private List<int> SizeTypes()
        {
            return Get().Select(s => s.Type).Distinct().OrderBy(s => s).ToList();
        }

        public class TypeSelectList
        {
            public int ID { get; set; }
            public string SizeValuesString { get; set; }
        }

        public virtual List<TypeSelectList> SizeValuesSelectList()
        {
            List<TypeSelectList> selectList = new List<TypeSelectList>();

            var sizeTypes = SizeTypes();
            foreach (var sizeType in sizeTypes)
            {
                selectList.Add(new TypeSelectList { ID = sizeType, SizeValuesString = SizeValuesString(sizeType) });
            }

            return selectList;
        }
    }
}