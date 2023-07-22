using FrontWorld.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontWorld.People
{
    public class TradeItem : IItem
    {
        public string name { get ; set ; }
        public int obtainingPrice { get ; set ; }
        public int sellingPrice { get; set; }
        public int quantity { get; set; }
        public INode owner { get ; set ; }

        public TradeItem(string name, int sellingPrice)
        {
            this.name = name;
            this.sellingPrice = sellingPrice;
        }
    }
}
