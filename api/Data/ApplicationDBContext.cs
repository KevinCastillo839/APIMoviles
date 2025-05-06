using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
  public class ApplicationDBContext : DbContext
  {
    public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Relación: Recipe_Ingredient → Recipe
        modelBuilder.Entity<Recipe_Ingredient>()
            .HasOne(ri => ri.Recipe)
            .WithMany(r => r.Recipe_Ingredients)
            .HasForeignKey(ri => ri.recipe_id);

        // Relación: User_Allergy → Allergy
        modelBuilder.Entity<User_Allergy>()
            .HasOne(ua => ua.Allergy)
            .WithMany()
            .HasForeignKey(ua => ua.allergy_id);

        // Relación: Preference → User
        modelBuilder.Entity<Preference>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.user_id);

        // ✅ Relación: Menu_Recipes → Menu
        modelBuilder.Entity<Menu_Recipes>()
            .HasOne(mr => mr.Menu)
            .WithMany(m => m.MenuRecipes)
            .HasForeignKey(mr => mr.menu_id);

        // ✅ Relación: Menu_Recipes → Recipe
        modelBuilder.Entity<Menu_Recipes>()
            .HasOne(mr => mr.Recipe)
            .WithMany()
            .HasForeignKey(mr => mr.recipe_id);

        base.OnModelCreating(modelBuilder);
    }
    public DbSet<Menu> Menu { get; set; }
    public DbSet<Menu_Recipes> menu_recipes { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Recipe_Ingredient> Recipe_Ingredients { get; set; }

    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<User> Users { get; set; }

    public DbSet<Allergy> Allergies { get; set; }
    public DbSet<User_Allergy> User_Allergies { get; set; }
    public DbSet<Preference> user_preferences { get; set; }

    public DbSet<ShoppingList> ShoppingLists { get; set; }

    public DbSet<Unit_Measurement> Unit_Measurements { get; set; }
  }
}
