using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class NodePath
    {
        public Node Node { get; set; }
        public NodePath Parent { get; set; }
    }
}
