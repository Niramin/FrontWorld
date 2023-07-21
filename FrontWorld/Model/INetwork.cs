using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace FrontWorld.Model
{
    public interface INetwork
    {
        public string name { get; set; }

        public Dictionary<string, INode> nodes { get; set; }

        public Subject<string> logger { get; set; }

        public void addNode(INode node);

        public void removeNode(string nodeName);

        public void increasePrice(string itemname);

        public void decreasePrice(string itemname);

        public INode chooseRandomNode();

        public bool conductTradeRound();

    }
}
