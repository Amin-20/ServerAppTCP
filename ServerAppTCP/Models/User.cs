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
        public EndPoint endPoint { get; set; } 
    }
}
