using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontWorld.Model
{
    public interface IItem
    {
        public string name { get; set; }

        public int obtainingPrice { get; set; }

        public int sellingPrice { get; set;}

        public int quantity { get; set; }

        public INode owner { get; set; }


    }
}
