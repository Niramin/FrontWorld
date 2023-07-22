using FrontWorld.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace FrontWorld.People
{
    public class Network : INetwork
    {
        public string name { get ; set ; }
        public Dictionary<string, INode> nodes { get ; set ; }
        public Subject<string> logger { get ; set ; }
        public bool freshStart { get => false; set => clearFiles(value); }

        private int _timeCount;
        private Dictionary<string,string> _files;
        public Dictionary< string,bool> _initialized;
        private List<string> _nodeNames = new List<string>();
        public Network() { 
            nodes = new Dictionary<string, INode>();
            logger = new Subject<string>();
            _timeCount = 0;
            _files = new Dictionary<string,string>();
            _initialized = new Dictionary<string,bool>();
            _files.Add("money", "C:\\Users\\Shashwat Ratna\\Desktop\\Learn\\InfoDump\\NodeMoney.csv");
            _files.Add("land", "C:\\Users\\Shashwat Ratna\\Desktop\\Learn\\InfoDump\\NodeLand.csv");

            _initialized["money"] = false;
            _initialized["land"] = false;

        }

        public void addNode(INode node)
        {
            node.network = this;
            nodes.Add(node.name, node);
            _nodeNames.Add(node.name);
        }

        public INode chooseRandomNode()
        {
            Random random = new Random();
            return nodes.ElementAt(random.Next(0,nodes.Count)).Value;
        }

        public bool conductTradeRound()
        {
            _timeCount++;
            bool b = chooseRandomNode().doTransaction();
            WriteStatusToFile("money");
            WriteStatusToFile("land");


            return b;
        }

        public void decreasePrice(string itemname)
        {
            throw new NotImplementedException();
        }

        public void increasePrice(string itemname)
        {
            throw new NotImplementedException();
        }

        public void removeNode(string nodeName)
        {
            nodes.Remove(nodeName);
            _nodeNames.Remove(nodeName);
        }

        public Dictionary<string, int> reportNodesMoney()
        {
            Dictionary<string,int> monies = new Dictionary<string,int>();

            foreach (KeyValuePair<string, INode> node in nodes)
            {
                monies[node.Key] = node.Value.money;
            }

            return monies;
        }

        private void InitializeFile(string file) 
        {
            string fileName = _files[file];
            File.WriteAllText(fileName, string.Empty);
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                string initialLine = "Trade Round";
                foreach (string nodeName in _nodeNames)
                {
                    initialLine += $",{nodeName}";
                }

                writer.WriteLine(initialLine);
            }
            _initialized[file] = true;
        }

        private void WriteStatusToFile(string fileName)
        {
            if (_initialized[fileName] == false) InitializeFile(fileName);

            using (StreamWriter writer = File.AppendText(_files[fileName]))
            {
                string initialLine = $"{_timeCount}";
                foreach (string nodeName in _nodeNames)
                {
                    switch (fileName)
                    {
                        case "land":
                            int value = 0;
                            if (nodes[nodeName].inventory.ContainsKey("land"))
                            {
                                value = nodes[nodeName].inventory["land"].quantity;
                            }
                            initialLine += $",{value}";
                            break;

                        case "money":
                            int val = nodes[nodeName].money;
                            foreach (KeyValuePair<string, IItem> t in nodes[nodeName].inventory)
                            {
                                val += (t.Value.sellingPrice) * (t.Value.quantity);
                            }

                            foreach (KeyValuePair<string, IItem> t in nodes[nodeName].needs)
                            {
                                val -= (t.Value.sellingPrice) * (t.Value.quantity);
                            }
                            initialLine += $",{val}";
                            break;
                    }
                    
                }

                writer.WriteLine(initialLine);
            }
        }

        private void clearFiles(bool b)
        {
            if (b)
            {
                _timeCount = 0;
                foreach (KeyValuePair<string, string> t in _files)
                {
                    _initialized[t.Key] = false;

                    File.WriteAllText(_files[t.Key], string.Empty);
                }
            }
        }

    }

}
