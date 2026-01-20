using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum MazeAxis {
	XPositive,
	XNegative,
	YPositive,
	YNegative,
	ZPositive,
	ZNegative
}

public class MazeGenerator : MonoBehaviour {
	public int width, height;
	public MazeAxis axis = MazeAxis.ZPositive;
	public Material brick;
	public int levelId = 0;
	public int totalBallsCount = 2;
	public Vector3 holesSpawnOffset = Vector3.zero;
	public Vector3 ballsSpawnOffset = Vector3.zero;
	private Color[] ballColors = new Color[] {
		new Color(1.0f, 0.4f, 0.4f),
		new Color(0.4f, 0.6f, 1.0f)
	};
	private int[,] Maze;
	private List<Vector3> pathMazes = new List<Vector3>();
	private Stack<Vector2> _tiletoTry = new Stack<Vector2>();
	private List<Vector2> offsets = new List<Vector2> { new Vector2(0, 2), new Vector2(0, -2), new Vector2(2, 0), new Vector2(-2, 0) };
	private System.Random rnd;
	private Vector3 winningPosition;
	private static MazeGenerator instance;
	public static MazeGenerator Instance
	{
		get
		{
			return instance;
		}
	}
	void Awake()
	{
		instance = this;
	}
	void Start()
	{
		rnd = new System.Random(levelId);
		GenerateMaze();
		PositionBalls();
		CreateWinningAreas();
	}
	Quaternion GetRotationFromAxis()
	{
		switch (axis)
		{
			case MazeAxis.XPositive: return Quaternion.Euler(0, 90, 0);
			case MazeAxis.XNegative: return Quaternion.Euler(0, -90, 0);
			case MazeAxis.YPositive: return Quaternion.Euler(-90, 0, 0);
			case MazeAxis.YNegative: return Quaternion.Euler(90, 0, 0);
			case MazeAxis.ZPositive: return Quaternion.identity;
			case MazeAxis.ZNegative: return Quaternion.Euler(0, 180, 0);
			default: return Quaternion.identity;
		}
	}
	void GenerateMaze()
	{
		Maze = new int[width, height];
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				Maze[x, y] = 1;
			}
		}
		CreateMaze();
		float cell_size = 1.0f / Mathf.Max(width, height);
		float offset_x = -0.45f;
		float offset_y = -0.45f;
		Quaternion rotation = GetRotationFromAxis();
		GameObject ptype = null;
		for (int i = 0; i <= Maze.GetUpperBound(0); i++)
		{
			for (int j = 0; j <= Maze.GetUpperBound(1); j++)
			{
				Vector3 local_pos = new Vector3(offset_x + i * cell_size, offset_y + j * cell_size, 0.45f);
				Vector3 rotated_pos = rotation * local_pos;
				if (Maze[i, j] == 1)
				{
					ptype = GameObject.CreatePrimitive(PrimitiveType.Cube);
					ptype.transform.localScale = new Vector3(cell_size, cell_size, cell_size * 3.0f);
					ptype.transform.rotation = rotation;
					ptype.transform.position = rotated_pos;
					if (brick != null)
					{
						ptype.GetComponent<Renderer>().material = brick;
					}
					ptype.transform.parent = transform;
				}
				else if (Maze[i, j] == 0)
				{
					pathMazes.Add(rotated_pos);
				}
			}
		}
	}
	void CreateMaze()
	{
		Vector2 start = Vector2.one;
		_tiletoTry.Push(start);
		Maze[(int)start.x, (int)start.y] = 0;
		while (_tiletoTry.Count > 0)
		{
			Vector2 current = _tiletoTry.Peek();
			List<Vector2> neighbors = GetValidNeighbors(current);
			if (neighbors.Count > 0)
			{
				Vector2 chosen = neighbors[0];
				Vector2 wall = new Vector2((current.x + chosen.x) / 2, (current.y + chosen.y) / 2);
				Maze[(int)wall.x, (int)wall.y] = 0;
				Maze[(int)chosen.x, (int)chosen.y] = 0;
				_tiletoTry.Push(chosen);
			}
			else
			{
				_tiletoTry.Pop();
			}
		}
	}
	private List<Vector2> GetValidNeighbors(Vector2 centerTile)
	{
		List<Vector2> validNeighbors = new List<Vector2>();
		foreach (var offset in offsets)
		{
			Vector2 toCheck = new Vector2(centerTile.x + offset.x, centerTile.y + offset.y);
			if (IsInside(toCheck) && Maze[(int)toCheck.x, (int)toCheck.y] == 1)
			{
				validNeighbors.Add(toCheck);
			}
		}
		Shuffle(validNeighbors);
		return validNeighbors;
	}
	private void Shuffle(List<Vector2> list)
	{
		for (int i = list.Count - 1; i > 0; i--)
		{
			int j = rnd.Next(i + 1);
			Vector2 temp = list[i];
			list[i] = list[j];
			list[j] = temp;
		}
	}
	private bool IsInside(Vector2 p)
	{
		return p.x >= 0 && p.y >= 0 && p.x < width && p.y < height;
	}

	void CreateWinningAreas()
	{
		if (pathMazes.Count == 0) return;

		float cell_size = 1.0f / Mathf.Max(width, height);
		float offset_x = -0.45f;
		float offset_y = -0.45f;
		Quaternion rotation = GetRotationFromAxis();

		Vector3[] targetPositions = new Vector3[5];
		targetPositions[0] = rotation * new Vector3(0f, 0f, 0.45f);
		targetPositions[1] = rotation * new Vector3(offset_x, offset_y + (height - 1) * cell_size, 0.45f);
		targetPositions[2] = rotation * new Vector3(offset_x, offset_y, 0.45f);
		targetPositions[3] = rotation * new Vector3(offset_x + (width - 1) * cell_size, offset_y + (height - 1) * cell_size, 0.45f);
		targetPositions[4] = rotation * new Vector3(offset_x + (width - 1) * cell_size, offset_y, 0.45f);

		// Create 2 winning areas for 2 balls
		List<Vector3> usedWinningPositions = new List<Vector3>();
		float minWinningAreaDistance = 0.15f;

		for (int i = 0; i < 2; i++)
		{
			int chosenIndex = rnd.Next(5);
			Vector3 targetPos = targetPositions[chosenIndex];

			float minDist = float.MaxValue;
			Vector3 winningPosition = pathMazes[0];

			foreach (Vector3 pathPos in pathMazes)
			{
				// Check if too close to existing winning areas
				bool tooClose = false;
				foreach (Vector3 usedPos in usedWinningPositions)
				{
					if (Vector3.Distance(pathPos, usedPos) < minWinningAreaDistance)
					{
						tooClose = true;
						break;
					}
				}

				if (!tooClose)
				{
					float dist = Vector3.Distance(pathPos, targetPos);
					if (dist < minDist)
					{
						minDist = dist;
						winningPosition = pathPos;
					}
				}
			}

			usedWinningPositions.Add(winningPosition);

			if (i == 0)
			{
				this.winningPosition = winningPosition;
			}

			GameObject winArea = new GameObject("WinningArea_" + i);
			winArea.tag = "WinningArea";
			winArea.transform.position = winningPosition + holesSpawnOffset;
			winArea.transform.rotation = rotation;
			winArea.transform.parent = transform;

			float scaledSize = cell_size * 0.55f;
			Color areaColor = ballColors[i];
			Color grayedColor = areaColor * 0.7f;

			GameObject innerSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			innerSphere.name = "InnerSphere";
			innerSphere.transform.parent = winArea.transform;
			innerSphere.transform.localPosition = Vector3.zero;
			innerSphere.transform.localScale = new Vector3(scaledSize, scaledSize, scaledSize);

			Material innerMaterial = new Material(Shader.Find("Unlit/Color"));
			innerMaterial.color = grayedColor * 0.6f;
			innerSphere.GetComponent<Renderer>().material = innerMaterial;

			Destroy(innerSphere.GetComponent<SphereCollider>());

			GameObject outerRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			outerRing.name = "OuterRing";
			outerRing.transform.parent = winArea.transform;
			outerRing.transform.localPosition = Vector3.zero;

			Vector3 cylinderRotation = Vector3.zero;
			switch (axis)
			{
				case MazeAxis.XPositive:
				case MazeAxis.XNegative:
					cylinderRotation = new Vector3(0, 0, 90);
					break;
				case MazeAxis.YPositive:
				case MazeAxis.YNegative:
					cylinderRotation = new Vector3(0, 0, 0);
					break;
				case MazeAxis.ZPositive:
				case MazeAxis.ZNegative:
					cylinderRotation = new Vector3(90, 0, 0);
					break;
			}
			outerRing.transform.localEulerAngles = cylinderRotation;

			float ringDiameter = scaledSize * 1.2f;
			float ringThickness = scaledSize * 0.18f;
			outerRing.transform.localScale = new Vector3(ringDiameter, ringThickness, ringDiameter);

			Material outerMaterial = new Material(Shader.Find("Unlit/Color"));
			outerMaterial.color = grayedColor;
			outerRing.GetComponent<Renderer>().material = outerMaterial;

			Destroy(outerRing.GetComponent<CapsuleCollider>());
			SphereCollider triggerCollider = winArea.AddComponent<SphereCollider>();
			triggerCollider.radius = scaledSize * 0.6f;
			triggerCollider.isTrigger = true;

			// Add WinningArea component and set its color
			WinningArea winAreaComponent = winArea.AddComponent<WinningArea>();
			winAreaComponent.requiredColor = areaColor;
		}
	}

	Dictionary<Vector3, int> CalculatePathDistances()
	{
		Dictionary<Vector3, int> distances = new Dictionary<Vector3, int>();
		Queue<Vector3> queue = new Queue<Vector3>();

		queue.Enqueue(winningPosition);
		distances[winningPosition] = 0;

		float cell_size = 1.0f / Mathf.Max(width, height);
		float maxNeighborDist = cell_size * 1.5f;

		while (queue.Count > 0)
		{
			Vector3 current = queue.Dequeue();
			int currentDist = distances[current];

			foreach (Vector3 pathPos in pathMazes)
			{
				if (!distances.ContainsKey(pathPos))
				{
					float dist = Vector3.Distance(current, pathPos);
					if (dist <= maxNeighborDist)
					{
						distances[pathPos] = currentDist + 1;
						queue.Enqueue(pathPos);
					}
				}
			}
		}

		return distances;
	}

	void PositionBalls()
	{
		GameObject[] existingBalls = GameObject.FindGameObjectsWithTag("Ball");
		if (existingBalls.Length == 0 || pathMazes.Count < 2) return;

		GameObject ball1 = existingBalls[0];

		for (int i = existingBalls.Length - 1; i >= 1; i--)
		{
			Destroy(existingBalls[i]);
		}

		List<GameObject> balls = new List<GameObject>();
		balls.Add(ball1);

		for (int i = 1; i < totalBallsCount; i++)
		{
			GameObject newBall = Instantiate(ball1);
			newBall.name = "Ball" + (i + 1);
			balls.Add(newBall);
		}

		Dictionary<Vector3, int> pathDistances = CalculatePathDistances();

		List<Vector3> sortedPositions = new List<Vector3>(pathMazes);
		sortedPositions.Sort((a, b) => {
			int distA = pathDistances.ContainsKey(a) ? pathDistances[a] : 0;
			int distB = pathDistances.ContainsKey(b) ? pathDistances[b] : 0;
			return distB.CompareTo(distA);
		});

		List<Vector3> usedPositions = new List<Vector3>();
		float minBallDistance = 0.1f;

		for (int i = 0; i < balls.Count; i++)
		{
			Vector3 chosenPos = Vector3.zero;
			bool foundPosition = false;

			foreach (Vector3 candidatePos in sortedPositions)
			{
				bool tooClose = false;
				foreach (Vector3 usedPos in usedPositions)
				{
					if (Vector3.Distance(candidatePos, usedPos) < minBallDistance)
					{
						tooClose = true;
						break;
					}
				}

				if (!tooClose)
				{
					chosenPos = candidatePos;
					foundPosition = true;
					break;
				}
			}

			if (foundPosition)
			{
				balls[i].transform.position = chosenPos + ballsSpawnOffset;
				usedPositions.Add(chosenPos);

				Color ballColor = ballColors[i % ballColors.Length];
				Renderer ballRenderer = balls[i].GetComponent<Renderer>();
				if (ballRenderer != null)
				{
					ballRenderer.material.color = ballColor;
				}

				BallColor ballColorComponent = balls[i].GetComponent<BallColor>();
				if (ballColorComponent == null)
				{
					ballColorComponent = balls[i].AddComponent<BallColor>();
				}
				ballColorComponent.color = ballColor;
			}
		}
	}
}
