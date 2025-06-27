using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FashionShopSystem.Domain.Models;

public partial class FashionShopContext : DbContext
{
	public FashionShopContext()
	{
	}

	public FashionShopContext(DbContextOptions<FashionShopContext> options)
		: base(options)
	{
	}

	public virtual DbSet<Category> Categories { get; set; }

	public virtual DbSet<Favorite> Favorites { get; set; }

	public virtual DbSet<Order> Orders { get; set; }

	public virtual DbSet<OrderDetail> OrderDetails { get; set; }

	public virtual DbSet<Product> Products { get; set; }

	public virtual DbSet<User> Users { get; set; }

	private string GetConnectionString()
	{
		IConfiguration configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", true, true).Build();
		return configuration["ConnectionStrings:DefaultConnectionString"];
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlServer(GetConnectionString());
	}
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Category>(entity =>
		{
			entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B02A3D48B");

			entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
			entity.Property(e => e.CategoryName).HasMaxLength(100);
			entity.Property(e => e.Description).HasMaxLength(255);
		});

		modelBuilder.Entity<Favorite>(entity =>
		{
			entity.HasKey(e => e.FavoriteId).HasName("PK__Favorite__CE74FAF51721623D");

			entity.Property(e => e.FavoriteId).HasColumnName("FavoriteID");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime");
			entity.Property(e => e.ProductId).HasColumnName("ProductID");
			entity.Property(e => e.UserId).HasColumnName("UserID");

			entity.HasOne(d => d.Product).WithMany(p => p.Favorites)
				.HasForeignKey(d => d.ProductId)
				.HasConstraintName("FK__Favorites__Produ__398D8EEE");

			entity.HasOne(d => d.User).WithMany(p => p.Favorites)
				.HasForeignKey(d => d.UserId)
				.HasConstraintName("FK__Favorites__UserI__38996AB5");
		});

		modelBuilder.Entity<Order>(entity =>
		{
			entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAFDFCE8314");

			entity.Property(e => e.OrderId).HasColumnName("OrderID");
			entity.Property(e => e.DeliveryStatus).HasMaxLength(50);
			entity.Property(e => e.Email).HasMaxLength(100);
			entity.Property(e => e.OrderDate)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime");
			entity.Property(e => e.PaymentStatus).HasMaxLength(50);
			entity.Property(e => e.ShippingAddress).HasMaxLength(255);
			entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
			entity.Property(e => e.UserId).HasColumnName("UserID");

			entity.HasOne(d => d.User).WithMany(p => p.Orders)
				.HasForeignKey(d => d.UserId)
				.HasConstraintName("FK__Orders__UserID__30F848ED");
		});

		modelBuilder.Entity<OrderDetail>(entity =>
		{
			entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30CD377DBD7");

			entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
			entity.Property(e => e.OrderId).HasColumnName("OrderID");
			entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
			entity.Property(e => e.ProductId).HasColumnName("ProductID");

			entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
				.HasForeignKey(d => d.OrderId)
				.HasConstraintName("FK__OrderDeta__Order__33D4B598");

			entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
				.HasForeignKey(d => d.ProductId)
				.HasConstraintName("FK__OrderDeta__Produ__34C8D9D1");
		});

		modelBuilder.Entity<Product>(entity =>
		{
			entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED42117DC1");

			entity.Property(e => e.ProductId).HasColumnName("ProductID");
			entity.Property(e => e.Brand).HasMaxLength(100);
			entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime");
			entity.Property(e => e.ImageUrl).HasMaxLength(255);
			entity.Property(e => e.IsActive).HasDefaultValue(true);
			entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
			entity.Property(e => e.ProductName).HasMaxLength(200);

			entity.HasOne(d => d.Category).WithMany(p => p.Products)
				.HasForeignKey(d => d.CategoryId)
				.HasConstraintName("FK__Products__Catego__2D27B809");
		});

		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACBF5BAE74");

			entity.HasIndex(e => e.Email, "UQ__Users__A9D10534262F4EA8").IsUnique();

			entity.Property(e => e.UserId).HasColumnName("UserID");
			entity.Property(e => e.Address).HasMaxLength(255);
			entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime");
			entity.Property(e => e.Email).HasMaxLength(100);
			entity.Property(e => e.FullName).HasMaxLength(100);
			entity.Property(e => e.IsActive).HasDefaultValue(true);
			entity.Property(e => e.PasswordHash).HasMaxLength(255);
			entity.Property(e => e.Phone).HasMaxLength(20);
			entity.Property(e => e.Role).HasMaxLength(20);
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
