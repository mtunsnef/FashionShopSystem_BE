using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure;
using FashionShopSystem.Service.DTOs.ApiResponseDto;
using FashionShopSystem.Infrastructure.Repositories;
using FashionShopSystem.Service.DTOs;


namespace FashionShopSystem.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepo _productRepo;
        private readonly string _webRootPath;
        public ProductService(IProductRepo productRepo, string webRootPath)
        {
            _productRepo = productRepo;
            _webRootPath = webRootPath;
        }
        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync(
    int? categoryid,
    string? keyword,
    string? sort,
    string? brand,
    decimal? price)
        {
            var product = await _productRepo.GetAllAsync();

            if (categoryid.HasValue)
            {
                product = product.Where(p => p.CategoryId == categoryid.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                product = product.Where(p =>
                    (!string.IsNullOrEmpty(p.ProductName) &&
                     p.ProductName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    ||
                    (!string.IsNullOrEmpty(p.Brand) &&
                     p.Brand.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            decimal? minPrice = null;
            decimal? maxPrice = null;

            switch (price)
            {
                case 1:
                    minPrice = 0; maxPrice = 50000;
                    break;
                case 2:
                    minPrice = 50000; maxPrice = 200000;
                    break;
                case 3:
                    minPrice = 200000; maxPrice = 300000;
                    break;
                case 4:
                    minPrice = 300000; maxPrice = 400000;
                    break;
                case 5:
                    minPrice = 400000; maxPrice = 500000;
                    break;
                case 6:
                    minPrice = 500000; maxPrice = null;
                    break;
            }

            if (minPrice.HasValue)
            {
                product = product.Where(p => p.Price.HasValue && p.Price.Value >= minPrice.Value).ToList();
            }

            if (maxPrice.HasValue)
            {
                product = product.Where(p => p.Price.HasValue && p.Price.Value <= maxPrice.Value).ToList();
            }

            // Lọc theo thương hiệu
            if (!string.IsNullOrWhiteSpace(brand))
            {
                product = product.Where(p => p.Brand != null &&
                                             p.Brand.Contains(brand, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Sắp xếp
            if (!string.IsNullOrWhiteSpace(sort))
            {
                if (sort.Equals("asc", StringComparison.OrdinalIgnoreCase))
                {
                    product = product.OrderBy(p => p.Price).ToList();
                }
                else if (sort.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    product = product.OrderByDescending(p => p.Price).ToList();
                }
            }

            // Map sang DTO
            var listProduct = product.Select(p => new ProductResponseDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                ImageUrl = p.ImageUrl,
                CreatedAt = p.CreatedAt,
                Brand = p.Brand,
                Stock = p.Stock,
                IsActive = p.IsActive,
                Category = new CategoryResponseDto
                {
                    CategoryId = p.Category?.CategoryId ?? 0,
                    CategoryName = p.Category?.CategoryName,
                    Description = p.Category?.Description
                }
            });

            return listProduct;
        }


        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(int? categoryId, string? brand, decimal? minPrice, decimal? maxPrice, string? keyword)
        {
            var products = await _productRepo.GetFilteredProductsAsync(categoryId, brand, minPrice, maxPrice, keyword);

            return products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Brand = p.Brand,
                ImageUrl = p.ImageUrl,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.CategoryName
            });
        }


        public async Task<ProductResponseDto> GetProductByIdAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            return new ProductResponseDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl,
                CreatedAt = product.CreatedAt,
                Brand = product.Brand,
                Stock = product.Stock,
                IsActive = product.IsActive,
                Category = new CategoryResponseDto
                {
                    CategoryId = product.Category.CategoryId,
                    CategoryName = product.Category?.CategoryName,
                    Description = product.Category?.Description
                }
            };
        }
        public async Task<ApiResponseDto<Product>> CreateProduct(CreateProductDto dto)
        {
            string? imagePath = null;

            if (dto.ImageUrl != null && dto.ImageUrl.Length > 0)
            {
                var imageFolder = Path.Combine(_webRootPath, "images");

                Console.WriteLine("🛠 Lưu ảnh vào: " + imageFolder);

                if (!Directory.Exists(imageFolder))
                {
                    Directory.CreateDirectory(imageFolder);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ImageUrl.FileName);
                var filePath = Path.Combine(imageFolder, fileName);

                Console.WriteLine("📄 File sẽ được lưu tại: " + filePath);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageUrl.CopyToAsync(stream);
                }

                // Đường dẫn để client truy cập
                imagePath = "/images/" + fileName;
            }
            var product = new Product
            {
                ProductName = dto.ProductName,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                ImageUrl = imagePath,
                CreatedAt = DateTime.UtcNow,
                Brand = dto.Brand,
                Stock = dto.Stock,
            };
            await _productRepo.AddAsync(product);
            return new ApiResponseDto<Product>(true, product, 200, "Create product success");
        }
        public async Task<ApiResponseDto<ProductResponseDto>> UpdateProduct(UpdateProductDto dto)
        {
            var product = await _productRepo.GetByIdAsync(dto.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found.");
            }

            string? imagePath = null;

            // Nếu người dùng gửi ảnh mới, xử lý upload
            if (dto.ImageUrl != null && dto.ImageUrl.Length > 0)
            {
                var imageFolder = Path.Combine(_webRootPath, "images");

                Console.WriteLine("🛠 Lưu ảnh vào: " + imageFolder);

                if (!Directory.Exists(imageFolder))
                {
                    Directory.CreateDirectory(imageFolder);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ImageUrl.FileName);
                var filePath = Path.Combine(imageFolder, fileName);

                Console.WriteLine("📄 File sẽ được lưu tại: " + filePath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageUrl.CopyToAsync(stream);
                }

                // Đường dẫn để lưu vào DB (client truy cập được)
                imagePath = "/images/" + fileName;
            }

            // Cập nhật thông tin sản phẩm
            product.ProductName = dto.ProductName;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;
            product.Brand = dto.Brand;
            product.Stock = dto.Stock;

            // Nếu có ảnh mới thì cập nhật ảnh
            if (!string.IsNullOrEmpty(imagePath))
            {
                product.ImageUrl = imagePath;
            }

            await _productRepo.UpdateAsync(product);

            var productDto = new ProductResponseDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl,
                CreatedAt = product.CreatedAt,
                Brand = product.Brand,
                Stock = product.Stock,
            };

            return new ApiResponseDto<ProductResponseDto>(true, productDto, 200, "Update product success");
        }
        public async Task<ApiResponseDto<ProductResponseDto>> deteletProduct(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            await _productRepo.DeleteAsync(product);
            var Data = new ProductResponseDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl,
                CreatedAt = product.CreatedAt,
                Brand = product.Brand,
                Stock = product.Stock,
            };
            return new ApiResponseDto<ProductResponseDto>(true, Data, 200);
        }
        public async Task<List<string>> getAllBrand() { 
        var brand = _productRepo.GetAllAsync().Result
            .Select(p => p.Brand)
            .Where(b => !string.IsNullOrEmpty(b))
            .Distinct()
            .ToList();
            return brand;
        }

    }
}
