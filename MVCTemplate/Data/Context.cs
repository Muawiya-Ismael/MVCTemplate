using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVCTemplate.Models;

namespace MVCTemplate.Data
{
    public class Context: IdentityDbContext

    {  
        public Context(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UploadedFile> UploadedFile { get; set; }
    }
}
