using Tresana.Data.Entities;
using System.Data.Entity;

namespace Tresana.Data.DataAccess
{
    public class TresanaContext : DbContext
    {
        public TresanaContext():base("name=TresanaContext") { }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
    }
}
