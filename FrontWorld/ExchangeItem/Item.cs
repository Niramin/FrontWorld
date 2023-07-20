using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontWorld.ExchangeItem
{
    public class Item
    {
        public string name { get; set; }

        public string country { get; set; }

        public int price { get; set; }

        public int quantity { get; set; }

        public Item(string name)
        {
            this.name = name;
        }

    }
}
