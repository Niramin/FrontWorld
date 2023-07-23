using FrontWorld.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace FrontWorld.People
{
    public class World
    {
        public INetwork network { get; set; }

        public Subject<string> logger { get; set; }
        public World() { 
        

            network = new Network();

            logger = new Subject<string>();

            //create 10 rich nodes
            for (int i = 1; i <= 30; i++)
            {
                INode node = new Node();
                node.name = $"R{i}";
                node.money = 1500;

                //adding Land
                IItem landitem = new TradeItem("land",500);
                landitem.quantity = 10;

                node.addItem(landitem);
                network.addNode(node);
                               
            }

            //create 50 middle nodes
            for (int i = 1; i <= 50; i++)
            {
                INode node = new Node();
                node.name = $"M{i}";
                node.money = 500;

                //adding Land
                IItem landitem = new TradeItem("land", 500);
                landitem.quantity = 5;

                node.addItem(landitem);
                network.addNode(node);


            }

            //create 60 poor nodes
            for (int i = 1; i <= 50; i++)
            {
                INode node = new Node();
                node.name = $"P{i}";
                node.money = 100;

                //adding Land
                IItem landitem = new TradeItem("land", 500);
                landitem.quantity = 1;

                node.addItem(landitem);
                network.addNode(node);


            }

            regenerateFood();
            regenerateHunger();

        }

        //add food item to everyones needs'
        public void regenerateHunger()
        {
            foreach (KeyValuePair<string, INode> node in network.nodes)
            {
                IItem fooditem = new TradeItem("food", 10);
                fooditem.quantity = 10;

                node.Value.removeItem(fooditem);

            }
        }

        // add some food item to every node
        public void regenerateFood()
        {
            Random random = new Random();
            foreach (KeyValuePair<string,INode> node in network.nodes)
            {
                IItem fooditem = new TradeItem("food",10);
                fooditem.quantity = random.Next(5,20);

                node.Value.addItem(fooditem);

            }
        }

        // increase value of land
        public void increaseLandPrice()
        {
            foreach (KeyValuePair<string, INode> node in network.nodes)
            {
                INode _node = node.Value;
                if (_node.inventory.ContainsKey("land"))
                {
                    _node.inventory["land"].sellingPrice += 10;
                }

            }
        }


        // remove money from nodes as land tax
        public void levyLandTax()
        {
            foreach (KeyValuePair<string, INode> node in network.nodes)
            {
                INode _node = node.Value;
                if (_node.inventory.ContainsKey("land"))
                {
                    if (_node.inventory["land"].quantity > 7)
                    {
                        int landValue = _node.inventory["land"].sellingPrice * _node.inventory["land"].quantity;
                        int taxedAmount = landValue * 20 / 100;

                        _node.money -= taxedAmount;
                        if (_node.money < 0) _node.money = 0;
                    }
                    
                }

            }

        }


        public void conductCycle(int count, int reinit = 0)
        {
            network.freshStart=true;
            int p = 1;
            for (int i = 1; i <= count; i++)
            {
                network.conductTradeRound();
                logger.OnNext($"{i}/{count}");
                p++;
                if (p == reinit)
                {
                    regenerateFood();
                    regenerateHunger();
                    increaseLandPrice();
                    levyLandTax();
                    p = 1;
                }

               
            }
        }
    }
}
