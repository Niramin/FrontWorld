using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace FrontWorld.Model
{
    public interface INode
    {
        public string name { get; set; }

        public int money { get; set; }

        public Queue<IItem> needs { get; set; }

        public Dictionary<string, IItem> inventory { get; set; }

        public Subject<string> pipe { get; set; }

        public bool trybuyNextNeed();

        public INetwork network { get; set; }
    }
}
