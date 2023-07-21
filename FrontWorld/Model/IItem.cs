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

        public int obtainingCost { get; set; }

        public int sellingCost { get; set;}

        public int quantitiy { get; set; }

        public INode owner { get; set; }


    }
}
