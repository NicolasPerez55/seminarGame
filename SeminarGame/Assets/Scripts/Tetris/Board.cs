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

	private float elapsedTime;
	[SerializeField] private int nextSpecialPieceTime = 60; // First special piece appears at 1 min
	private int originalNextSpecialPieceTime;
	private int specialPieceIndex = 7; // Start at 8th tetromino (index 7)
	private List<int> specialPieces = new List<int> { 7, 8, 9, 10, 11, 12 }; // Indices of the special pieces

	public GameObject particleClearLine;

	public int debugSpawnPiece = -1;

	private soundManager sounds;
	private float soundCooldown = 0.1f;

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
	[SerializeField] private TMPro.TextMeshProUGUI pointsText, highScoreText;
	private int totalPoints;
	[HideInInspector] public int TotalPoints
	{
		get => totalPoints;
	}
	private int highScore;
	[HideInInspector] public int HighScore
	{
		get => highScore;
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
		
		originalNextSpecialPieceTime = nextSpecialPieceTime;
		sounds = FindAnyObjectByType<soundManager>();
	}

	void Update()
	{
		elapsedTime += Time.deltaTime;

		// Check if it's time to release a special piece
		if (specialPieces.Count > 0 && elapsedTime >= nextSpecialPieceTime)
		{
			nextSpecialPieceTime += originalNextSpecialPieceTime; // Next special piece appears in 1 more minute
			queue.queuedPieces.Add(tetrominoes[specialPieces[0]]); // Add the special piece
			specialPieces.RemoveAt(0); // Remove it from the list so it doesn't appear again
		}
		if (soundCooldown >= 0)
        {
			soundCooldown -= Time.deltaTime;
        }
		if (soundCooldown < 0)
        {
			soundCooldown = 0;
        }
	}

	public void SpawnPiece()
	{
		if (queue == null)
		{
			Debug.LogError("Attach a queue component to the board.");

			return;
		}

		if (storage != null) storage.used = false;

		// Ensure the queue is filled with the first 7 tetrominoes
		while (queue.queuedPieces.Count < queue.queueLength + 1)
		{
			//Useful for debugging specific pieces.
			if (debugSpawnPiece < 0)
			{
				int random = Random.Range(0, 7);
				queue.queuedPieces.Add(tetrominoes[random]);
			}
			else
			{
				queue.queuedPieces.Add(tetrominoes[debugSpawnPiece]);
			}
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

	// public void StorePiece()
	// {
	//     if (storage == null)
	//     {
	//         Debug.LogError("Attach a storage component to the board.");

	//         return;
	//     }

	//     if (storage.used) return;

	//     TetrominoData oldStoredPiece = storage.piece;

	//     storage.AddToStorage(activePiece.data);
	//     storage.UpdateDisplay();

	//     if (oldStoredPiece.tetromino != Tetromino.Empty)
	//     {
	//         activePiece.Initialize(this, spawnPosition, oldStoredPiece);
	//     }
	//     else
	//     {
	//         SpawnPiece();
	//     }

	//     storage.used = true;
	// }

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
		sounds.lineClear();
		soundCooldown = 0.1f;
	}

	public void UpdatePoints(int linesCleared)
	{
		if (linesCleared == 0) return;
		int clearedPoints = (int)(pointsPerLine * Mathf.Pow(2, linesCleared - 1));
		totalPoints += clearedPoints;
		pointsText.text = totalPoints.ToString();

		SetHighScore();
	}

	public void SetHighScore()
	{
		if (totalPoints > highScore)
		{
			highScore = totalPoints;
			highScoreText.text = "High Score: " + highScore.ToString();
		}
		else return;
	}

	public void playPieceSettleSound()
    {
		if (soundCooldown <= 0)
        {
			sounds.pieceSettles();
			soundCooldown = 0.1f;
		}
		
    }

	public void playInstaDropSound()
    {
		if (soundCooldown <= 0)
		{
			sounds.instaDrop();
			soundCooldown = 0.1f;
		}
	}

}
