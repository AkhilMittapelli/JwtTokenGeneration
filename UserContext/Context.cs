using Learning.Models;
using Microsoft.EntityFrameworkCore;

namespace Learning.UserContext
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options): base(options) 
        {
            
        }

        public DbSet<Users> users { get; set; } 
    }
}
