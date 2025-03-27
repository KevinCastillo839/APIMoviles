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


    public DbSet<Menu> Menu { get; set; }
    public DbSet<Menu_Recipes> menu_recipes { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<User> Users { get; set; }
  
    public DbSet<Allergy> Allergies { get; set; }
    public DbSet<Preference> user_preferences { get; set; }





  }
}
