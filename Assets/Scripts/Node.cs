using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Node
    {
        public NodeType Type { get; set; }
        public bool Visited { get; set; } = false;
        public Vector2Int Position { get; set; }
    }
}
