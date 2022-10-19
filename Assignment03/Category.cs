using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment03
{
    class Category
    {
        public int Cid { get; set; }
        public string Name { get; set; }

        public Category(int cid, string name)
        {
            this.Cid = cid;
            this.Name = name;
        }


    }
}