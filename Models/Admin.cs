using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminNamespace
{
    public class Admin
    {
        public Guid Id { get; set; }
        public string username { get; set; }

        public string Password { get; set; }
    }
}
