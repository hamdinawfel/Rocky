using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;

namespace Rocky_DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(RockyDbContext db) : base(db)
        {
        }

        public void Update(Product obj)
        {
            var objFromDb = base.FirstOrDefault(x => x.Id == obj.Id);

            if(objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.ShortDescription = obj.ShortDescription;
                objFromDb.Description = obj.Description;
                objFromDb.ImageUrl = obj.ImageUrl;
                objFromDb.Price = obj.Price;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.Category = obj.Category;
                
            }
        }
    }
}
