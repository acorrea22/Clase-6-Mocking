using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Tresana.Data.Entities
{
    public class User 
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Mail { get; set; }
        public string Address { get; set; }
        public virtual IEnumerable<Task> Tasks { get; set; }

        public User()
        {

        }
    }
}
