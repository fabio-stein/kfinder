using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OldUIController : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile filledTile;
    public Tile emptyTile;
    public Tile sourceTile;
    public Tile targetTile;
    public Tile wallTile;

    private Queue<Node> queue = new Queue<Node>();
    private Queue<Vector3Int> resultQueue = new Queue<Vector3Int>();
    private bool found = false;
    void Start()
    {
        FillEmptyCells();
        SetSourceAndTarget();

        //Add the first block to the seraching queue
        var sourcePosition = FindTile(sourceTile);
        queue.Enqueue(new Node(sourcePosition, null));
    }

    private void FillEmptyCells()
    {
        for (int y = 0; y < tilemap.cellBounds.size.y; y++)
        {
            for (int x = 0; x < tilemap.cellBounds.size.x; x++)
            {
                var xPos = tilemap.cellBounds.xMin + x;
                var yPos = tilemap.cellBounds.yMin + y;
                tilemap.SetTile(new Vector3Int(xPos, yPos, 0), emptyTile);
            }
        }
    }

    private void Test()
    {
        for (int y = 0; y < tilemap.cellBounds.size.y; y++)
        {
            for (int x = 0; x < tilemap.cellBounds.size.x; x++)
            {
                var xPos = tilemap.cellBounds.xMin + x;
                var yPos = tilemap.cellBounds.yMin + y;
                tilemap.SetTile(new Vector3Int(xPos, yPos, 0), tilemap.GetTile(new Vector3Int(xPos, yPos, 0)));
            }
        }
    }

    public void SetSourceAndTarget()
    {
        var wallDistance = 5;
        var leftPoint = new Vector3Int(tilemap.cellBounds.xMin + wallDistance, 0, 0);
        var rightPoint = new Vector3Int(tilemap.cellBounds.xMax - wallDistance, 0, 0);
        tilemap.SetTile(leftPoint, sourceTile);
        tilemap.SetTile(rightPoint, targetTile);
    }

    public Vector3Int FindTile(Tile tile)
    {
        for (int y = 0; y < tilemap.cellBounds.size.y; y++)
        {
            for (int x = 0; x < tilemap.cellBounds.size.x; x++)
            {
                var xPos = tilemap.cellBounds.xMin + x;
                var yPos = tilemap.cellBounds.yMin + y;
                var pos = new Vector3Int(xPos, yPos, 0);
                var foundTile = tilemap.GetTile(pos);
                if (foundTile == tile)
                    return pos;
            }
        }
        return Vector3Int.zero;
    }

    bool first = true;
    bool init = false;
    void Update()
    {
        Test();

        var mouse = 0;
        if (Input.GetMouseButton(0))
            mouse = 1;
        if (Input.GetMouseButton(1))
            mouse = 2;

        if (mouse > 0)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var tpos = tilemap.WorldToCell(pos);
            if (mouse == 1)
                tilemap.SetTile(tpos, wallTile);
            else
                tilemap.SetTile(tpos, emptyTile);
        }
        if (Input.GetMouseButtonDown(2))
            init = true;

        if (!init)
            return;

        if (TickUtil.Tick())
        {
            if (found)
            {
                if (resultQueue.Count == 0)
                {
                    return;
                }

                var tpos = resultQueue.Dequeue();
                tilemap.SetTile(tpos, targetTile);
                return;
            }

            var node = queue.Dequeue();
            if (!first)
            {
                while (tilemap.GetTile(node.pos) == filledTile || tilemap.GetTile(node.pos) == wallTile || tilemap.GetTile(node.pos) == null || tilemap.GetTile(node.pos) == sourceTile)
                {
                    if (queue.Count == 0)
                        return;
                    node = queue.Dequeue();
                }
            }

            if (tilemap.GetTile(node.pos) == targetTile)//FOUND
            {
                found = true;
                var parent = node.parent;
                var count = 0;
                while (parent != null)
                {
                    resultQueue.Enqueue(parent.pos);
                    parent = parent.parent;
                    count++;
                }
                Debug.LogWarning("TOTAL: " + count);
                return;
            }

            if (node.pos.x > tilemap.cellBounds.xMin)
            {
                var targetPos = new Vector3Int(node.pos.x - 1, node.pos.y, 0);
                var target = tilemap.GetTile(targetPos);
                if (target != filledTile && target != wallTile)
                    queue.Enqueue(new Node(targetPos, node));
            }
            if (node.pos.x < tilemap.cellBounds.xMax)
            {
                var targetPos = new Vector3Int(node.pos.x + 1, node.pos.y, 0);
                var target = tilemap.GetTile(targetPos);
                if (target != filledTile && target != wallTile)
                    queue.Enqueue(new Node(targetPos, node));
            }
            if (node.pos.y > tilemap.cellBounds.yMin)
            {
                var targetPos = new Vector3Int(node.pos.x, node.pos.y - 1, 0);
                var target = tilemap.GetTile(targetPos);
                if (target != filledTile && target != wallTile)
                    queue.Enqueue(new Node(targetPos, node));
            }
            if (node.pos.y < tilemap.cellBounds.yMax)
            {
                var targetPos = new Vector3Int(node.pos.x, node.pos.y + 1, 0);
                var target = tilemap.GetTile(targetPos);
                if (target != filledTile && target != wallTile)
                    queue.Enqueue(new Node(targetPos, node));
            }

            tilemap.SetTile(node.pos, filledTile);

            if (first)
            {
                tilemap.SetTile(node.pos, sourceTile);
                first = false;
            }

        }
    }

    public class Node
    {
        public Vector3Int pos { get; set; }
        public Node parent { get; set; }

        public Node(Vector3Int pos, Node parent)
        {
            this.pos = pos;
            this.parent = parent;
        }
    }
}
