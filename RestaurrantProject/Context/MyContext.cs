using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestaurrantProject.Models;

namespace RestaurrantProject.Context
{
    public class MyContext : IdentityDbContext<ApplicationUser>
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Restaurant;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True");
        //}

        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var seedDate = new DateTime(2025, 6, 14, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Item>(entity =>
            {

                entity.HasQueryFilter(p => !p.IsDeleted);


                entity.HasOne(p => p.category)
                      .WithMany(c => c.items)
                      .HasForeignKey(p => p.CategoryID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {

                entity.HasQueryFilter(c => !c.IsDeleted);
            });

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Pizza",
                    CreatedAt = seedDate,
                },
                new Category
                {
                    Id = 2,
                    Name = "Sandwiches",
                    CreatedAt = seedDate,
                },
                new Category
                {
                    Id = 3,
                    Name = "Meals",
                    CreatedAt = seedDate,
                }
            );

            // Seed Items
            modelBuilder.Entity<Item>().HasData(
                // 🍕 Pizza Items (Category 1)
                new Item { Id = 1, Name = "Margherita Pizza", Description = "Classic pizza with tomato sauce and mozzarella", Price = 95.00m, IsAvailable = true, DailyOrderCount = 0, CategoryID = 1, CreatedAt = seedDate },
                new Item { Id = 2, Name = "Pepperoni Pizza", Description = "Pepperoni slices over cheesy crust", Price = 110.00m, IsAvailable = true, DailyOrderCount = 0, CategoryID = 1, CreatedAt = seedDate },
                new Item { Id = 3, Name = "BBQ Chicken Pizza", Description = "Grilled chicken with BBQ sauce", Price = 120.00m, IsAvailable = true, DailyOrderCount = 0, CategoryID = 1, CreatedAt = seedDate },
                new Item { Id = 4, Name = "Four Cheese Pizza", Description = "Blend of mozzarella, cheddar, parmesan, and blue cheese", Price = 130.00m, IsAvailable = true, DailyOrderCount = 0, CategoryID = 1, CreatedAt = seedDate },
                new Item { Id = 5, Name = "Veggie Pizza", Description = "Loaded with fresh vegetables", Price = 100.00m, IsAvailable = true, DailyOrderCount = 0, CategoryID = 1, CreatedAt = seedDate },

                // 🥪 Sandwich Items (Category 2)
                new Item { Id = 6, Name = "Beef Burger", Description = "Juicy beef patty with cheese and lettuce", Price = 90.00m, IsAvailable = true, DailyOrderCount = 0, CategoryID = 2, CreatedAt = seedDate },
                new Item { Id = 7, Name = "Chicken Burger", Description = "Crispy chicken fillet with mayo", Price = 85.00m, IsAvailable = true, DailyOrderCount = 0, CategoryID = 2, CreatedAt = seedDate },
                new Item { Id = 8, Name = "Club Sandwich", Description = "Triple-decker sandwich with turkey and egg", Price = 95.00m, IsAvailable = true, DailyOrderCount = 0, CategoryID = 2, CreatedAt = seedDate },
                new Item { Id = 9, Name = "Tuna Sandwich", Description = "Tuna salad with lettuce and tomato", Price = 80.00m, IsAvailable = true, DailyOrderCount = 0, CategoryID = 2, CreatedAt = seedDate },
                new Item { Id = 10, Name = "Falafel Sandwich", Description = "Traditional Egyptian falafel with tahini", Price = 50.00m, IsAvailable = true, DailyOrderCount = 0, CategoryID = 2, CreatedAt = seedDate },

                // 🍗 Meal Items (Category 3)
                new Item { Id = 11, Name = "Grilled Chicken Meal", Description = "Half grilled chicken with rice and salad", Price = 150.00m, IsAvailable = true, DailyOrderCount = 0, CategoryID = 3, CreatedAt = seedDate },
                new Item { Id = 12, Name = "Beef Steak Meal", Description = "Tender beef steak with mashed potatoes", Price = 180.00m, IsAvailable = true, DailyOrderCount = 0, CategoryID = 3, CreatedAt = seedDate },
                new Item { Id = 13, Name = "Kofta Meal", Description = "Grilled kofta with rice and salad", Price = 140.00m, IsAvailable = true, DailyOrderCount = 0, CategoryID = 3, CreatedAt = seedDate },
                new Item { Id = 14, Name = "Fried Chicken Meal", Description = "Crispy fried chicken pieces with fries", Price = 130.00m, IsAvailable = true, DailyOrderCount = 0, CategoryID = 3, CreatedAt = seedDate },
                new Item { Id = 15, Name = "Fish Fillet Meal", Description = "Breaded fish fillet with tartar sauce", Price = 160.00m, IsAvailable = true, DailyOrderCount = 0, CategoryID = 3, CreatedAt = seedDate }
            );


        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = null;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }




    }
}
