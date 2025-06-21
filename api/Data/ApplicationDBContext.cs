using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.ShoppingList;
using api.Models;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }
<<<<<<< HEAD

=======
    
>>>>>>> origin/main
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Weekly_Menu → Menu
            modelBuilder.Entity<Weekly_Menu>()
                .HasOne(wm => wm.Menu)
                .WithMany()
                .HasForeignKey(wm => wm.menu_id);

            // Weekly_Menu → Weekly_Menu_Table
            modelBuilder.Entity<Weekly_Menu>()
                .HasOne(wm => wm.Weekly_Menu_Table)
                .WithMany(wmt => wmt.Weekly_Menus)
                .HasForeignKey(wm => wm.menu_table_id)
                .OnDelete(DeleteBehavior.Cascade);

            // Weekly_Menu_Table → User
            modelBuilder.Entity<Weekly_Menu_Table>()
                .HasOne(wmt => wmt.User)
                .WithMany()
                .HasForeignKey(wmt => wmt.user_id)
                .OnDelete(DeleteBehavior.Cascade);

            // Recipe_Ingredient → Recipe
            modelBuilder.Entity<Recipe_Ingredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.Recipe_Ingredients)
                .HasForeignKey(ri => ri.RecipeId); //recipe_id

            // User_Allergy → Allergy
            modelBuilder.Entity<User_Allergy>()
                .HasOne(ua => ua.Allergy)
                .WithMany()
                .HasForeignKey(ua => ua.allergy_id);

            // User_Allergy → User
            modelBuilder.Entity<User_Allergy>()
                .HasOne(ua => ua.User)
                .WithMany()
                .HasForeignKey(ua => ua.user_id);

<<<<<<< HEAD
            // Preference → User
=======
            modelBuilder.Entity<Recipe>(entity =>
                {
                    // Mapea la propiedad user_id a la columna user_id (ya es igual, pero aseguras)
                    entity.Property(e => e.user_id).HasColumnName("user_id");

                    // Configura la relación con User
                    entity.HasOne(r => r.User)
                        .WithMany() // si no tienes colección de recetas en User, sino cambia por .WithMany(u => u.Recipes)
                        .HasForeignKey(r => r.user_id)
                        .IsRequired(false); // permite nulo
                });

>>>>>>> origin/main
            modelBuilder.Entity<Preference>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.user_id);

<<<<<<< HEAD
            // Menu_Recipes → Menu
            modelBuilder.Entity<Menu_Recipes>()
                .HasOne(mr => mr.Menu)
                .WithMany(m => m.Menu_Recipes)
                .HasForeignKey(mr => mr.menu_id);

            // Menu_Recipes → Recipe
            modelBuilder.Entity<Menu_Recipes>()
                .HasOne(mr => mr.Recipe)
                .WithMany()
                .HasForeignKey(mr => mr.recipe_id);

            // Menu → User
=======

            modelBuilder.Entity<Weekly_Menu>()
    .HasOne(wm => wm.Weekly_Menu_Table)
    .WithMany(wmt => wmt.Weekly_Menus)
    .HasForeignKey(wm => wm.menu_table_id)
    .OnDelete(DeleteBehavior.Cascade);  // Puedes usar .Cascade o lo que consideres

            modelBuilder.Entity<Weekly_Menu_Table>()
                .HasOne(wmt => wmt.User)
                .WithMany()
                .HasForeignKey(wmt => wmt.user_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Menu>()
         .Property(m => m.user_id) // La propiedad del modelo
         .HasColumnName("user_id");


            // Configurar la relación entre Menu y User
>>>>>>> origin/main
            modelBuilder.Entity<Menu>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.user_id);

            
            modelBuilder.Entity<Menu>()
                .Property(m => m.user_id)
                .HasColumnName("user_id");

            // ShoppingList → Recipe
            modelBuilder.Entity<ShoppingList>()
                .HasOne(sl => sl.Recipe)
                .WithMany()
                .HasForeignKey(sl => sl.recipe_id)
                .OnDelete(DeleteBehavior.Cascade);

<<<<<<< HEAD

            //Correct mapping for Unit_Measurement
            modelBuilder.Entity<Unit_Measurement>()
                .ToTable("unit_measurement");

            modelBuilder.Entity<ShoppingList>()
                .ToTable("shopping_list");

=======
            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.HasNoKey(); // Define que esta entidad no tiene clave primaria
            });
>>>>>>> origin/main
            base.OnModelCreating(modelBuilder);

           
            modelBuilder.Entity<SimpleShoppingListItem>().HasNoKey();

            base.OnModelCreating(modelBuilder);
                    }

        // DbSet properties
        public DbSet<Menu> Menu { get; set; }
        public DbSet<Menu_Recipes> menu_recipes { get; set; }
        public DbSet<Weekly_Menu> weekly_menu { get; set; }
        public DbSet<Weekly_Menu_Table> Weekly_Menu_Table { get; set; }
<<<<<<< HEAD
=======



>>>>>>> origin/main
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Recipe_Ingredient> Recipe_Ingredients { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<User_Allergy> User_Allergies { get; set; }
        public DbSet<Preference> user_preferences { get; set; }
        public DbSet<Recipe_Allergy> Recipe_Allergies { get; set; }
<<<<<<< HEAD
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<Unit_Measurement> Unit_Measurements { get; set; }
        public DbSet<SimpleShoppingListItem> SimpleShoppingListItems { get; set; }
    
        
=======

        public DbSet<Dietary_Goal> Dietary_Goals { get; set; }
        public DbSet<Dietary_Restriction> Dietary_Restrictions { get; set; }
        public DbSet<User_Dietary_Restriction> User_Dietary_Restrictions { get; set; }
        public DbSet<User_Dietary_Goal> User_Dietary_Goals { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<Unit_Measurement> unit_measurement { get; set; }


>>>>>>> origin/main
    }
}

