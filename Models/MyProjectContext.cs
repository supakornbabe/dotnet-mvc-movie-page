using System;
using Microsoft.EntityFrameworkCore;

namespace mvcTest.Models {
    public class MyProjectContext : DbContext {
        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data source=database/db.db");
        }
        public DbSet<MovieModel> Movie { get; set; }
    }
}