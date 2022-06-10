using WebAPIJwtTokenAuthentication.Data;
using WebAPIJwtTokenAuthentication.Entities;

namespace WebAPIJwtTokenAuthentication.BackgroundService.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public void Save()
        {
            var product = _context.Products.ToList();
            foreach (var item in product)
            {
                ProductTwo productTwo = new ProductTwo()

                {
                    Name = item.Name,
                    Price = item.Price,
                };
                _context.Add(productTwo);
                _context.SaveChanges();
            }
        }
    }
}
