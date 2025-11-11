using Microsoft.EntityFrameworkCore;
namespace Givehub.Models
{
    public class DB : DbContext
    {
        public DB(DbContextOptions<DB> options) : base(options)
        {

        }

    }
    
    
}
