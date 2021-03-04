using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile filledTile;
    public Tile emptyTile;
    public Tile sourceTile;
    public Tile targetTile;
    public Tile wallTile;
    public GameObject Panel;
    public Text textMode;
    public static bool isBfs = true;

    private Node[,] grid;
    private int ySize;
    private int xSize;
    private int yMin;
    private int xMin;

    private PathController controller;
    private bool started = false;
    private GridStatus status = new GridStatus();

    void Start()
    {
        textMode.text = (isBfs) ? "Current Mode: BFS" : "Current Mode: DFS";
        ySize = tilemap.cellBounds.size.y;
        xSize = tilemap.cellBounds.size.x;
        yMin = tilemap.cellBounds.yMin;
        xMin = tilemap.cellBounds.xMin;

        InitializeGrid();
        var sourcePosition = SetupSourceAndTarget();

        if(isBfs)
            controller = new BFS(grid, sourcePosition);
        else
            controller = new DFS(grid, sourcePosition);
    }

    private void InitializeGrid()
    {
        grid = new Node[ySize, xSize];

        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                grid[y, x] = new Node()
                {
                    Type = NodeType.EMPTY,
                    Position = new Vector2Int(x, y)
                };
            }
        }
    }

    private Vector2Int SetupSourceAndTarget()
    {
        var distance = 5;
        var midY = ySize / 2;

        var sourcePosition = new Vector2Int(distance, midY);
        grid[sourcePosition.y, sourcePosition.x] = new Node()
        {
            Type = NodeType.SOURCE,
            Position = sourcePosition
        };

        var targetPosition = new Vector2Int(xSize - distance, midY);
        grid[targetPosition.y, targetPosition.x] = new Node()
        {
            Type = NodeType.TARGET,
            Position = targetPosition
        };

        return sourcePosition;
    }

    private void RenderGrid()
    {
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                var tileX = xMin + x;
                var tileY = yMin + y;
                var tile = NodeToTile(grid[y, x]);
                tilemap.SetTile(new Vector3Int(tileX, tileY, 0), tile);
            }
        }
    }

    private Tile NodeToTile(Node node)
    {
        switch (node.Type)
        {
            case NodeType.SOURCE: return sourceTile;
            case NodeType.TARGET: return targetTile;
            case NodeType.WALL: return wallTile;
            case NodeType.EMPTY: return (node.Visited) ? filledTile : emptyTile;
            default: throw new System.Exception("Unknown NodeType");
        }
    }

    void Update()
    {

        InputHandler();

        if (started)
        {
            if (!TickUtil.Tick())
                return;

            if (!status.Finished)
            {
                status = controller.Execute();
                grid = controller.grid;
            }
            else
            {
                SolutionUtil.Execute(status.SolvedPath, grid);
            }
        }

        RenderGrid();
    }

    private void InputHandler()
    {
        var mouse = 0;
        if (Input.GetMouseButton(0))
            mouse = 1;
        if (Input.GetMouseButton(1))
            mouse = 2;

        if (mouse > 0)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var tpos = tilemap.WorldToCell(pos);
            tpos = tpos + new Vector3Int(tilemap.cellBounds.xMin * -1, tilemap.cellBounds.yMin * -1, 0);
            if (mouse == 1)
                grid[tpos.y, tpos.x] = new Node()
                {
                    Type = NodeType.WALL,
                    Position = new Vector2Int(tpos.x, tpos.y)
                };
            else
                grid[tpos.y, tpos.x] = new Node()
                {
                    Type = NodeType.EMPTY,
                    Position = new Vector2Int(tpos.x, tpos.y)
                };
        }
        if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.Space))
            started = true;

        if (Input.GetKeyDown(KeyCode.U))
        {
            Panel.SetActive(!Panel.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            isBfs = !isBfs;
            Restart();
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
