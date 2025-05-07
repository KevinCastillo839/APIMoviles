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

    modelBuilder.Entity<User_Allergy>()
                .HasOne(ua => ua.User_Preference)
                .WithMany(up => up.User_Allergies)
                .HasForeignKey(ua => ua.user_preferences_id);

<<<<<<< Updated upstream
    modelBuilder.Entity<User_Allergy>()
        .HasOne(ua => ua.Allergy)
=======
            modelBuilder.Entity<Recipe_Ingredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.Recipe_Ingredients)
                .HasForeignKey(ri => ri.recipe_id);  // Usa recipe_id como clave foránea

            modelBuilder.Entity<Menu_Recipes>()
                 .HasOne(mr => mr.Menu)
                 .WithMany(m => m.Menu_Recipes)
                 .HasForeignKey(mr => mr.menu_id); // usa snake_case

            modelBuilder.Entity<Menu_Recipes>()
                .HasOne(mr => mr.Recipe)
                .WithMany()
                .HasForeignKey(mr => mr.recipe_id);

            modelBuilder.Entity<User_Allergy>()
                .HasOne(ua => ua.Allergy)
                .WithMany()
                .HasForeignKey(ua => ua.allergy_id);

            modelBuilder.Entity<User_Allergy>()
                .HasOne(ua => ua.User)
                .WithMany()
                .HasForeignKey(ua => ua.user_id);

            modelBuilder.Entity<Preference>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.user_id);


                modelBuilder.Entity<Weekly_Menu>()
        .HasOne(wm => wm.Weekly_Menu_Table)
        .WithMany(wmt => wmt.Weekly_Menus)
        .HasForeignKey(wm => wm.menu_table_id)
        .OnDelete(DeleteBehavior.Cascade);  // Puedes usar .Cascade o lo que consideres

    modelBuilder.Entity<Weekly_Menu_Table>()
        .HasOne(wmt => wmt.User)
>>>>>>> Stashed changes
        .WithMany()
        .HasForeignKey(ua => ua.allergy_id);

    modelBuilder.Entity<Preference>()
        .HasOne(p => p.User)
        .WithMany()
        .HasForeignKey(p => p.user_id);

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





  }
}
