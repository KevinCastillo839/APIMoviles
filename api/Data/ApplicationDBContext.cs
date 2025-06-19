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
            modelBuilder.Entity<Weekly_Menu>()
              .HasOne(wm => wm.Menu)
              .WithMany()
              .HasForeignKey(wm => wm.menu_id);

            modelBuilder.Entity<Weekly_Menu>()
             .HasOne(wm => wm.Weekly_Menu_Table)
             .WithMany(wmt => wmt.Weekly_Menus)
             .HasForeignKey(wm => wm.menu_table_id);

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
                .WithMany()
                .HasForeignKey(wmt => wmt.user_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Menu>()
         .Property(m => m.user_id) // La propiedad del modelo
         .HasColumnName("user_id");


            // Configurar la relación entre Menu y User
            modelBuilder.Entity<Menu>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.user_id);
            // Relación basada en user_id

            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.HasNoKey(); // Define que esta entidad no tiene clave primaria
            });
            base.OnModelCreating(modelBuilder);
        }


        public DbSet<Menu> Menu { get; set; }
        public DbSet<Menu_Recipes> menu_recipes { get; set; }
        public DbSet<Weekly_Menu> weekly_menu { get; set; }
        public DbSet<Weekly_Menu_Table> Weekly_Menu_Table { get; set; }



        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Recipe_Ingredient> Recipe_Ingredients { get; set; }

        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<User_Allergy> User_Allergies { get; set; }
        public DbSet<Preference> user_preferences { get; set; }

        public DbSet<Recipe_Allergy> Recipe_Allergies { get; set; }

        public DbSet<Dietary_Goal> Dietary_Goals { get; set; }
        public DbSet<Dietary_Restriction> Dietary_Restrictions { get; set; }
        public DbSet<User_Dietary_Restriction> User_Dietary_Restrictions { get; set; }
        public DbSet<User_Dietary_Goal> User_Dietary_Goals { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<Unit_Measurement> unit_measurement { get; set; }


    }
}
