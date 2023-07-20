using FrontWorld.ExchangeItem;
using FrontWorld.Node;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace FrontWorld.Network
{

    public class WorldNetwork
    {
        public Dictionary<string,Country> countries { get; set; }

        public Subject<string> logger { get; set; }

        public WorldNetwork()
        {

            countries = new Dictionary<string, Country>();
            logger = new Subject<string>();

            logger.Subscribe(WriteToFile);

            //Pakistan
            Country pak = new Country(1, "Pakistan");
            pak.localcurrency = 500;

            Item rice = new Item("rice");
            rice.country = "Pakistan";
            rice.price = 2;
            rice.quantity = 200;


            Item metal = new Item("metal");
            metal.country = "Pakistan";
            metal.price = 5;
            metal.quantity = 100;

            pak.inventory["rice"] = rice;
            pak.needs["metal"] = metal;

            //India
            Country ind = new Country(2, "India");
            ind.localcurrency = 600;

            Item metalin = new Item("metal");
            metalin.country = "India";
            metalin.price = 3;
            metalin.quantity = 100;


            Item ricein = new Item("rice");
            ricein.country = "India";
            ricein.price = 3;
            ricein.quantity = 50;

            ind.inventory["metal"] = metalin;
            ind.needs["rice"] = ricein;


            addNode(ind);
            addNode(pak);
            ind.BuyItem(ricein);

            logStatus();

            pak.BuyItem(metal);

            logStatus();


            /*
            pak.BuyItem(metal);
            logStatus();
            ind.BuyItem(ricein);
            logStatus();

            */

            







        }

        private void WriteToFile(string message)
        {
            string filePath = "C:\\Users\\Shashwat Ratna\\Desktop\\Learn\\InfoDump\\log";
            using (StreamWriter w = File.AppendText(filePath))
            {
                w.WriteLine(message);
            }
        }

        public void addNode(Country country)
        {
            countries[country.name] = country;
            country.pipe.Subscribe(logger);
            country.worldNetwork = this;
            
        }

        private void post(string message)
        {
            logger.OnNext($"{message}\n");
        }

        private void logStatus()
        {
            foreach (KeyValuePair<string, Country> T in countries)
            {
                post($"{T.Key} has {T.Value.localcurrency} now!");
            }
        }
    }
}
