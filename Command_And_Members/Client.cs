using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CliientApp
{
    public class Client
    {
        public int Id { get; set; }
        public string Login { get; set; }    
        public string Password { get; set; }    
        public string IPEndPoint { get; set; }
    }
}
