using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerAppTCP.Models
{
    public class User
    {
        public string Name { get; set; }
        public string IpAdress { get; set; }
        public int Port { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
