using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIT_Assignment03
{
    class Category
    {
        public int cid { get; set; }
        public string name { get; set; }

        public Category(int cid, string name)
        {
            this.cid = cid;
            this.name = name;
        }


    }
}