using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class PathController
    {
        public readonly Node[,] grid;
        public PathController(Node[,] grid)
        {
            this.grid = grid;
        }

        public abstract GridStatus Execute();
    }
}
