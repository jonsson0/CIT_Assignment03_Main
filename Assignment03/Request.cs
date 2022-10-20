using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIT_Assignment03
{
    class Request
    {
        public string? Method { get; set; }
        public string? Path { get; set; }
        public string? Date { get; set; }
        public string? Body { get; set; }

        /*  public Request(string method, string path, DateTime date, string body)
          {
              this.method = method;
              this.path = path;
              this.date = date;
              this.body = body;
          }
        */
    }
}