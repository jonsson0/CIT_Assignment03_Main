using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment03
{
    internal class Response
    {
        public string Status { get; set; }
        public string Body { get; set; }

        public Response(string status, string body)
        {
            Status = status;
            Body = body;
        }

        public void addToStatus(string text)
        {
            Status = Status + ", " + text;
        }
    }
}