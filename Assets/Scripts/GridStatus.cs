using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class GridStatus
    {
        public bool Finished { get; set; } = false;
        public LinkedList<Node> SolvedPath { get; set; }
    }
}
