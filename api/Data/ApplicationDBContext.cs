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
    modelBuilder.Entity<Recipe_Ingredient>()
        .HasOne(ri => ri.Recipe)
        .WithMany(r => r.Recipe_Ingredients)
        .HasForeignKey(ri => ri.recipe_id);  // Usa recipe_id como clave foránea
}


    public DbSet<Menu> Menu { get; set; }
    public DbSet<Menu_Recipes> menu_recipes { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Recipe_Ingredient> Recipe_Ingredients { get; set; }

    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<User> Users { get; set; }





  }
}
