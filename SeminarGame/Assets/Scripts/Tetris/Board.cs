using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }

    public QueueHandler queue;
    public StorageHandler storage;

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

    public GameObject particleClearLine;

    public int spawnpiece;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }


    [Header("Points")]
    [SerializeField] private int pointsPerLine = 100;
    [SerializeField] private TMPro.TextMeshProUGUI pointsText;
    private int totalPoints;
    [HideInInspector] public int TotalPoints
    {
        get => totalPoints;
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++) {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        if (queue == null)
        {
            Debug.LogError("Attach a queue component to the board.");

            return;
        }

        if (storage != null) storage.used = false;

        for (int i = queue.queuedPieces.Count(); i < queue.queueLength+1; ++i)
        {
            int random = Random.Range(0, tetrominoes.Length);
            queue.queuedPieces.Add(tetrominoes[random]);

            //Useful for debugging specific pieces.
            //queue.queuedPieces.Add(tetrominoes[spawnpiece]);
        }

        activePiece.Initialize(this, spawnPosition, queue.queuedPieces.FirstOrDefault());

        queue.queuedPieces.Remove(queue.queuedPieces.FirstOrDefault());
        queue.UpdateDisplay();

        if (IsValidPosition(activePiece, spawnPosition)) {
            Set(activePiece);
        } else {
            GameOver();
        }
    }

    public void StorePiece()
    {
        if (storage == null)
        {
            Debug.LogError("Attach a storage component to the board.");

            return;
        }

        if (storage.used) return;

        TetrominoData oldStoredPiece = storage.piece;

        storage.AddToStorage(activePiece.data);
        storage.UpdateDisplay();

        if (oldStoredPiece.tetromino != Tetromino.Empty)
        {
            activePiece.Initialize(this, spawnPosition, oldStoredPiece);
        }
        else
        {
            SpawnPiece();
        }

        storage.used = true;
    }

    public void GameOver()
    {
        tilemap.ClearAllTiles();

        totalPoints = 0;
        pointsText.text = totalPoints.ToString();
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (tilemap.HasTile(tilePosition)) {
                return false;
            }
        }

        return true;
    }

    public int ClearLines()
    {
        int totalLines = 0;
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        // Clear from bottom to top
        while (row < bounds.yMax)
        {
            // Only advance to the next row if the current is not cleared
            // because the tiles above will fall down when a row is cleared
            if (IsLineFull(row)) {
                totalLines++;
                LineClear(row);

                Transform prefab = Instantiate(particleClearLine).transform;
                prefab.position = new Vector3(0, row + totalLines - .5f);
            } else {
                row++;
            }
            UpdatePoints(totalLines);
        }

        return totalLines;
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!tilemap.HasTile(position)) {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        // Clear all tiles in the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        // Shift every row above down one
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }

    public void UpdatePoints(int linesCleared)
    {
        if (linesCleared == 0) return;
        int clearedPoints = (int)(pointsPerLine * Mathf.Pow(2, linesCleared - 1));
        totalPoints += clearedPoints;
        pointsText.text = totalPoints.ToString();
    }

}
