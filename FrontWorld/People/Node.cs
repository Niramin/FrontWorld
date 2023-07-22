using FrontWorld.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FrontWorld.People
{
    public class Node : INode
    {
        public string name { get ; set ; }
        public int money { get; set; } 
        public Dictionary<string,IItem> needs { get ; set ; }
        public Dictionary<string, IItem> inventory { get ; set ; }
        public Subject<string> pipe { get ; set ; }
        public INetwork network { get ; set ; }

        public int min_money { get; set; } = 0;


        public Node()
        {
            needs = new Dictionary<string,IItem>();
            inventory = new Dictionary<string, IItem>();
            pipe = new Subject<string>();
        }

        public bool doTransaction()
        {
            foreach (KeyValuePair<string, IItem> item in needs)
            {
                if (item.Value.quantity == 0)
                {
                    needs.Remove(item.Key);
                }
            }
            // if needs are fulfilled,  try to buy something at random, also increase inventory price and savings since comfortable
            if (needs.Count == 0)
            {
                if (inventory.ContainsKey("land"))
                {
                    inventory["land"].sellingPrice += 10;
                }
                if (inventory.ContainsKey("food"))
                {
                    inventory["food"].sellingPrice += 1;
                }
                min_money += 50;
                INode sellingNode = network.chooseRandomNode();
                while (sellingNode.name == name)
                {
                    sellingNode = network.chooseRandomNode();
                }

                int surplus_money = money - min_money;
                foreach (KeyValuePair<string, IItem> T in sellingNode.inventory)
                {
                    string itemName = T.Key;
                    IItem item = sellingNode.inventory[itemName];
                    if (surplus_money > item.sellingPrice)
                    {
                        int quantity_to_buy = surplus_money / item.sellingPrice;
                        if (quantity_to_buy > item.quantity) quantity_to_buy = item.quantity;

                        //exchange money
                        money -= quantity_to_buy * item.sellingPrice;
                        sellingNode.money += quantity_to_buy * item.sellingPrice;

                        //exchange goods
                        IItem exchangedItem = new TradeItem(item.name,item.sellingPrice);
                        exchangedItem.quantity = quantity_to_buy;
                        
                        addItem(exchangedItem);
                        sellingNode.removeItem(exchangedItem);

                        return true;
                    }
                }
                return false;
            }

            //if not, try fulfilling need
            else {
                IItem needItem = needs[needs.Keys.First()];

                INode minSellingCountry = null;
                int minPrice = -1;
                foreach (KeyValuePair<string, INode> T in network.nodes)
                {
                    string nodeName = T.Key;
                    INode sellingNode = T.Value;
                    if (nodeName == name) continue;

                    if (sellingNode.inventory.ContainsKey(needItem.name))
                    {
                        if (minPrice == -1)
                        {
                            minSellingCountry = sellingNode;
                            minPrice = sellingNode.inventory[needItem.name].sellingPrice;
                        }
                        else {
                            if (sellingNode.inventory[needItem.name].sellingPrice < minPrice)
                            {
                                minSellingCountry = sellingNode;
                                minPrice = sellingNode.inventory[needItem.name].sellingPrice;
                            }
                        }
                    }

                }

                if (minPrice == -1)
                {
                    //no country offering the need item
                    return false;
                }

                if (money > minPrice)
                {
                    int quantity_can_buy = money / minPrice;
                    int quantity_to_buy = 0;

                    if (quantity_can_buy > needItem.quantity)
                    { 
                        quantity_to_buy = needItem.quantity;
                    }

                    if (quantity_to_buy > minSellingCountry.inventory[needItem.name].quantity)
                    {
                        quantity_to_buy = minSellingCountry.inventory[(needItem.name)].quantity;
                    }

                    //exchange money
                    money -= quantity_to_buy*minPrice;
                    minSellingCountry.money += quantity_to_buy * minPrice;

                    //exchange goods
                    IItem exhangedItem = new TradeItem(needItem.name,needItem.sellingPrice);
                    exhangedItem.quantity = quantity_to_buy;
                    
                    addItem(exhangedItem);
                    minSellingCountry.removeItem(exhangedItem);
                    
                    return true;

                }
                else
                {
                    //cannot afford to buy, in desparation reducing own inventory price reducing savings
                    if (inventory.ContainsKey("land"))
                    {
                        inventory["land"].sellingPrice -= 10;
                        if (inventory["land"].sellingPrice < 0) inventory["land"].sellingPrice = 1;
                    }
                    if (inventory.ContainsKey("food"))
                    {
                        inventory["food"].sellingPrice -= 1;
                        if (inventory["food"].sellingPrice < 0) inventory["food"].sellingPrice = 1;
                    }
                    min_money -= 50;
                    if (min_money < 0) min_money = 0;
                    return false;

                }
            
            }
            
        }

        public void addItem(IItem item)
        {
            if (needs.ContainsKey(item.name))
            {
                IItem cur_item = needs[item.name];

                if (item.quantity >= cur_item.quantity)
                {

                    if (!inventory.ContainsKey(item.name))
                    {
                        inventory[item.name] = item;
                        item.quantity = item.quantity - cur_item.quantity;
                    }
                    else
                    {
                        inventory[item.name].quantity += item.quantity - cur_item.quantity;
                    }
                    cur_item.quantity = 0;
                    needs.Remove(cur_item.name);
                }

                else
                {
                    cur_item.quantity -= item.quantity;
                }
            }

            else {
                if (inventory.ContainsKey(item.name))
                {
                    inventory[item.name].quantity += item.quantity;
                }
                else {
                    inventory[item.name] = item;
                }
            }
        }

        public void removeItem(IItem item, bool debt = false)
        {
            if (inventory.ContainsKey(item.name))
            {
                inventory[item.name].quantity -= item.quantity;
                if (inventory[item.name].quantity < 0)
                {
                    item.quantity = 0- inventory[item.name].quantity;
                    inventory[item.name].quantity = 0;
                }
               
            }

            if (item.quantity > 0)
            {
                if (needs.ContainsKey(item.name))
                {
                    needs[item.name].quantity += item.quantity;
                }
            }
        }
    }
}
