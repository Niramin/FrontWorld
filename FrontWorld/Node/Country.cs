using FrontWorld.ExchangeItem;
using FrontWorld.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace FrontWorld.Node
{
    public class Country
    {
        public WorldNetwork worldNetwork { get; set; }
        public string name { get; set; }
        public int localcurrency { get; set; }

        public int id { get; set; }

        public Dictionary<string, Item> inventory { get; set; }

        public Dictionary<string, Item> needs { get; set; }

        public Subject<string> pipe;

        public List<Item> offers { get; set; }

        public Country(int id, string name)
        {
            this.id = id;
            this.name = name;
            inventory = new Dictionary<string, Item>();
            needs = new Dictionary<string, Item>();
            pipe = new Subject<string>();
            offers = new List<Item>();
           

        }


        private void post(string message)
        {
            
            pipe.OnNext($"{message}\n");
        }

        public bool BuyItem(Item item)
        {
            post($"{name} has asked for {item.name} at the internaltional market!");
            int minPrice = -1;
            Country sellingCountry = null;
            foreach (KeyValuePair<string, Country> T in worldNetwork.countries)
            {
                string countryName = T.Key;
                Country country = T.Value;

                if (name == countryName) continue;

                if (country.inventory.ContainsKey(item.name) && country.inventory[item.name].quantity > 0)
                {
                    if (sellingCountry == null)
                    {
                        minPrice = country.inventory[item.name].price;
                        sellingCountry = country;
                    }

                    if (minPrice > country.inventory[item.name].price)
                    {
                        minPrice = country.inventory[item.name].price;
                        sellingCountry = country;
                    }
                }

            }

            if (sellingCountry == null) {

                post($"No one is selling {item.name}");
                return false;
            }

            int _quantityCanBuy = (int)(localcurrency / minPrice);

            if (_quantityCanBuy > 0)
            {
                if (_quantityCanBuy < item.quantity)
                {
                    //cannot buy more than needed
                    post($"{name} buys the {_quantityCanBuy} units of {item.name} from {sellingCountry.name}");

                    int moneyexchanged = _quantityCanBuy * minPrice;

                    localcurrency -= moneyexchanged;
                    sellingCountry.localcurrency += moneyexchanged;

                    if (!inventory.ContainsKey(item.name))
                    {
                        inventory[item.name] = new Item(item.name);
                        item.quantity = 0; item.price = minPrice;
                        item.country = name;
                    }
                    inventory[item.name].quantity += _quantityCanBuy;
                    sellingCountry.inventory[item.name].quantity -= _quantityCanBuy;

                    needs[item.name].quantity -= _quantityCanBuy;


                }
                else
                {
                    //can buy more than needed
                    int moneyexchanged = minPrice * sellingCountry.inventory[item.name].quantity;

                     post($"{name} buys the {sellingCountry.inventory[item.name].quantity} units of {item.name} from {sellingCountry.name}");

                    localcurrency -= moneyexchanged;
                    sellingCountry.localcurrency += moneyexchanged;

                    if (!inventory.ContainsKey(item.name))
                    {
                        inventory[item.name] = new Item(item.name);
                        item.quantity = 0; item.price = minPrice;
                        item.country = name;
                    }
                    inventory[item.name].quantity += sellingCountry.inventory[item.name].quantity;
                    sellingCountry.inventory[item.name].quantity = 0;

                    needs[item.name].quantity -= sellingCountry.inventory[item.name].quantity;

                }

                return true;
            }


            post($"{name} cannot afford to buy {item.name}");
            return false;
           
           

        }


       

        

    }
}
