using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Piece : MonoBehaviour
{
	public Board board { get; private set; }
	public TetrominoData data { get; private set; }
	public Vector3Int[] cells { get; private set; }
	public Vector3Int position { get; private set; }
	public int rotationIndex { get; private set; }

	public float stepDelay = 1f;
	public float moveDelay = 0.1f;
	public float lockDelay = 0.5f;

	private float stepTime;
	private float moveTime;
	private float lockTime;

	public CameraHandler cameraHandler;

	int trailIndex;
	public TrailDrop trail;
	public TrailDrop[] trailPool;

	float inputTimer;
	public float inputTapDelay;
	public float inputHoldDelay;
	bool holding;
	
	[SerializeField] private GameObject phone; 
	[SerializeField] private Vector3 phoneOffset;
	private Vector3 phonePosition;
	
	[SerializeField] private GameObject door;
	[SerializeField] private Vector3 doorOffset;

    [SerializeField] private GameObject notification;
    [SerializeField] private Vector3 notificationOffset;

    [SerializeField] private GameObject box;
    [SerializeField] private Vector3 boxOffset;

    [SerializeField] private GameObject fish;
    [SerializeField] private Vector3 fishOffset;

    [SerializeField] private GameObject fishFood;
    [SerializeField] private Vector3 foodOffset;


    public void Start()
	{
		phonePosition = phone.transform.position;
	}
	public void Initialize(Board board, Vector3Int position, TetrominoData data)
	{
		cells = null;

		this.data = data;
		this.board = board;
		this.position = position;

		rotationIndex = 0;
		stepTime = Time.time + stepDelay;
		moveTime = Time.time + moveDelay;
		lockTime = 0f;

		if (cells == null) {
			cells = new Vector3Int[data.cells.Length];
		}

		for (int i = 0; i < cells.Length; i++) {
			cells[i] = (Vector3Int)data.cells[i];
		}

        ChangeTrailSize();
    }

	private void Update()
	{
		board.Clear(this);

		// We use a timer to allow the player to make adjustments to the piece
		// before it locks in place
		lockTime += Time.deltaTime;

		// Handle rotation
		if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Z)) {
			Rotate(-1);
		} else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
			Rotate(1);
		}

		// Handle hard drop
		if (Input.GetKeyDown(KeyCode.Space)) {
			HardDrop();

			cameraHandler.Shake(Vector2.down * 6);
		}

		//Store piece.
		// if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.C))
		// {
		// 	board.StorePiece();
		// }

		// Allow the player to hold movement keys but only after a move delay
		// so it does not move too fast
		if (Time.time > moveTime) {
			HandleMoveInputs();
		}

		// Advance the piece to the next row every x seconds
		if (Time.time > stepTime) {
			Step();
		}

        trail.transform.position = position + GetTetrominoCenter();

		board.Set(this);
		
		if (Vector3.Distance(phone.transform.position, phonePosition) > 0.5f)
		{
			phone.transform.position = phonePosition;
		}
	}

	private void HandleMoveInputs()
	{
		// Soft drop movement
		if (Mathf.RoundToInt(Input.GetAxisRaw("Vertical")) < 0)
		{
			if (Move(Vector2Int.down)) {
				// Update the step time to prevent double movement
				stepTime = Time.time + stepDelay;

				cameraHandler.Shake(Vector2.down * 1);
			}
		}

		if (inputTimer > 0) inputTimer -= Time.deltaTime;

		int input = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
		if (input != 0)
		{
			if (inputTimer <= 0)
			{
				Move(Vector2Int.right * input);

				cameraHandler.Shake(Vector2.right * input * 2.5f);

				if (holding) inputTimer = inputHoldDelay;
				else inputTimer = inputTapDelay;

				holding = true;
			}
		}
		else
		{
			holding = false;
			inputTimer = 0;
		}
	}

	private void Step()
	{
		stepTime = Time.time + stepDelay;

		// Step down to the next row
		Move(Vector2Int.down);

		// Once the piece has been inactive for too long it becomes locked
		if (lockTime >= lockDelay) {
			Lock();
		}
	}

	private void HardDrop()
    {
		Vector3 oldPos = position + GetTetrominoCenter();

        //trail.Clear();
        //trail.emitting = true;

        while (Move(Vector2Int.down))
        {
            continue;
        }

        //Ensure trail catches up with dropped tetro.
        trail.transform.position = position + GetTetrominoCenter();

        trail.GoToLine(oldPos, trail.transform.position);

        Lock();
	}

	private void Lock()
	{
		board.Set(this);
		int clearedLines = board.ClearLines();

		for (int i = 0; i < clearedLines; i++)
		{
			trail.Move(Vector3.down);
		}

		//Change pooled trail.
		SwitchTrail();

		board.SpawnPiece();

		//Reset 'new' trail and set their corresponding size and position to the new piece.
		trail.transform.position = position + GetTetrominoCenter();
    }

	private bool Move(Vector2Int translation)
	{
		Vector3Int newPosition = position;
		newPosition.x += translation.x;
		newPosition.y += translation.y;

		bool valid = board.IsValidPosition(this, newPosition);

		// Only save the movement if the new position is valid
		if (valid)
		{
			position = newPosition;
			moveTime = Time.time + moveDelay;
			lockTime = 0f; // reset
			
			if (data.tetromino == Tetromino.Phone)
			{
				phone.transform.position = newPosition + phoneOffset;
				phonePosition = newPosition + phoneOffset;
			} else if (data.tetromino == Tetromino.Door)
			{
				door.transform.position = newPosition + doorOffset;
            }
            else if (data.tetromino == Tetromino.Not)
            {
                notification.transform.position = newPosition + notificationOffset;
            }
            else if (data.tetromino == Tetromino.Box)
            {
                box.transform.position = newPosition + boxOffset;
            }
            else if (data.tetromino == Tetromino.Fish)
            {
                fish.transform.position = newPosition + fishOffset;
            }
            else if (data.tetromino == Tetromino.FishFood)
            {
                fishFood.transform.position = newPosition + foodOffset;
            }
		}

		return valid;
	}

	private void Rotate(int direction)
	{
		if (data.tetromino == Tetromino.Phone || data.tetromino == Tetromino.Door || data.tetromino == Tetromino.Not || data.tetromino == Tetromino.Fish || data.tetromino == Tetromino.Box || data.tetromino == Tetromino.FishFood) return;
		
		// Store the current rotation in case the rotation fails
		// and we need to revert
		int originalRotation = rotationIndex;

		// Get the maximum number of rotations based on the current tetromino type
		int maxRotation = (data.tetromino == Tetromino.Phone) ? 6 : 4;

		// Rotate using the correct wrap logic for the current tetromino
		rotationIndex = Wrap(rotationIndex + direction, 0, maxRotation);

        // Apply the rotation matrix for the piece
        ApplyRotationMatrix(direction);

		// Revert the rotation if the wall kick tests fail
		if (!TestWallKicks(rotationIndex, direction))
		{
			rotationIndex = originalRotation;
			ApplyRotationMatrix(-direction);
        }

        //Update the trail;
        ChangeTrailSize();
    }


	private void ApplyRotationMatrix(int direction)
	{
		float[] matrix = Data.RotationMatrix;

		// Rotate all of the cells using the rotation matrix
		for (int i = 0; i < cells.Length; i++)
		{
			Vector3 cell = cells[i];

			int x, y;

			switch (data.tetromino)
			{
				case Tetromino.I:
				case Tetromino.O:
					// "I" and "O" are rotated from an offset center point
					cell.x -= 0.5f;
					cell.y -= 0.5f;
					x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
					y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
					break;

				default:
					x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
					y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
					break;
			}

			cells[i] = new Vector3Int(x, y, 0);
		}
	}

	private bool TestWallKicks(int rotationIndex, int rotationDirection)
	{
		int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

		for (int i = 0; i < data.wallKicks.GetLength(1); i++)
		{
			Vector2Int translation = data.wallKicks[wallKickIndex, i];

			if (Move(translation)) {
				return true;
			}
		}

		return false;
	}

	private int GetWallKickIndex(int rotationIndex, int rotationDirection)
	{
		int wallKickIndex = rotationIndex * 2;

		if (rotationDirection < 0) {
			wallKickIndex--;
		}

		return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
	}

	private int Wrap(int input, int min, int max)
	{
		if (input < min) {
			return max - (min - input) % (max - min);
		} else {
			return min + (input - min) % (max - min);
		}
	}

	void SwitchTrail()
	{
		trailIndex = (trailIndex + 1) % trailPool.Length;

		trail = trailPool[trailIndex];
	}

	Vector3 GetTetrominoCenter()
	{
		return data.middle[rotationIndex];
    }

	void ChangeTrailSize()
	{
		if (rotationIndex % 2 == 0)
		{
			trail.Resize(data.width);
		}
		else
        {
            trail.Resize(data.height);
        }
	}
}
