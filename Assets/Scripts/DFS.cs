using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DFS : PathController
{
    private Stack<NodePath> stack = new Stack<NodePath>();

    public DFS(Node[,] grid, Vector2Int startPosition) : base(grid)
    {
        stack.Push(new NodePath()
        {
            Node = grid[startPosition.y, startPosition.x],
            Parent = null
        });
    }

    public override GridStatus Execute()
    {
        var status = new GridStatus();

        if (stack.Count == 0)
        {
            status.Finished = true;
            return status;
        }

        NodePath path = null;
        while (stack.Count > 0 && (path == null || path.Node.Visited))
        {
            path = stack.Pop();
        }

        if (path == null)
        {
            status.Finished = true;
            return status;
        }

        path.Node.Visited = true;

        var connections = GetConnectedNodes(path.Node);

        var target = connections.FirstOrDefault(c => c.Type == NodeType.TARGET);
        //TARGET FOUND
        if (target != null)
        {
            status.Finished = true;
            status.SolvedPath = GeneratePath(new NodePath()
            {
                Node = target,
                Parent = path
            });
            return status;
        }
        else
        {
            foreach (var connectedNode in connections)
            {
                if (!connectedNode.Visited && connectedNode.Type == NodeType.EMPTY)
                    stack.Push(new NodePath()
                    {
                        Node = connectedNode,
                        Parent = path
                    });
            }
        }
        return status;
    }

    public LinkedList<Node> GeneratePath(NodePath node)
    {
        var path = new LinkedList<Node>();
        path.AddFirst(node.Node);

        while (node.Parent != null)
        {
            node = node.Parent;
            path.AddLast(node.Node);
        }

        return path;
    }

    private List<Node> GetConnectedNodes(Node node)
    {
        var list = new List<Node>();

        var up = GetNode(node.Position - Vector2Int.up);
        var down = GetNode(node.Position - Vector2Int.down);
        var left = GetNode(node.Position + Vector2Int.left);
        var right = GetNode(node.Position + Vector2Int.right);

        if (up != null && !up.Visited)
            list.Add(up);
        if (down != null && !down.Visited)
            list.Add(down);
        if (left != null && !left.Visited)
            list.Add(left);
        if (right != null && !right.Visited)
            list.Add(right);

        return list;
    }

    private Node GetNode(Vector2Int pos)
    {
        if (pos.y < 0 || pos.y >= grid.GetLength(0))
            return null;
        if (pos.x < 0 || pos.x >= grid.GetLength(1))
            return null;

        return grid[pos.y, pos.x];
    }
}
