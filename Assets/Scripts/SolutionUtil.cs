using Assets.Scripts;
using System;
using System.Collections.Generic;

internal class SolutionUtil
{
    public static bool Execute(LinkedList<Node> solvedPath, Node[,] Grid)
    {
        if (solvedPath == null || solvedPath.Count == 0)
            return true;

        var node = solvedPath.Last.Value;
        solvedPath.RemoveLast();

        Grid[node.Position.y, node.Position.x] = new Node()
        {
            Position = node.Position,
            Type = NodeType.TARGET
        };

        return false;   
    }
}