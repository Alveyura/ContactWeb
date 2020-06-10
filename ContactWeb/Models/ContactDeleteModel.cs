using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactWeb.Models
{
    public class ContactDeleteModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
