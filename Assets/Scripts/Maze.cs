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
	private int[,] Maze;
	private List<Vector3> pathMazes = new List<Vector3>();
	private Stack<Vector2> _tiletoTry = new Stack<Vector2>();
	private List<Vector2> offsets = new List<Vector2> { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
	private System.Random rnd = new System.Random();
	private int _width, _height;
	private Vector2 _currentTile;
	public Vector2 CurrentTile
	{
		get { return _currentTile; }
		private set
		{
			if (value.x < 1 || value.x >= this.width - 1 || value.y < 1 || value.y >= this.height - 1)
			{
				throw new ArgumentException("CurrentTile must be within the one tile border all around the maze");
			}
			if (value.x % 2 == 1 || value.y % 2 == 1)
			{ _currentTile = value; }
			else
			{
				throw new ArgumentException("The current square must not be both on an even X-axis and an even Y-axis, to ensure we can get walls around all tunnels");
			}
		}
	}
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
		Camera.main.orthographic = true;
		Camera.main.orthographicSize = 0.6f;
		GenerateMaze();
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
		CurrentTile = Vector2.one;
		_tiletoTry.Push(CurrentTile);
		Maze = CreateMaze();
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
					ptype.transform.localScale = new Vector3(cell_size, cell_size, cell_size);
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
	public int[,] CreateMaze()
	{
		List<Vector2> neighbors;
		while (_tiletoTry.Count > 0)
		{
			Maze[(int)CurrentTile.x, (int)CurrentTile.y] = 0;
			neighbors = GetValidNeighbors(CurrentTile);
			if (neighbors.Count > 0)
			{
				_tiletoTry.Push(CurrentTile);
				CurrentTile = neighbors[rnd.Next(neighbors.Count)];
			}
			else
			{
				CurrentTile = _tiletoTry.Pop();
			}
		}
		return Maze;
	}
	private List<Vector2> GetValidNeighbors(Vector2 centerTile)
	{
		List<Vector2> validNeighbors = new List<Vector2>();
		foreach (var offset in offsets)
		{
			Vector2 toCheck = new Vector2(centerTile.x + offset.x, centerTile.y + offset.y);
			if (toCheck.x % 2 == 1 || toCheck.y % 2 == 1)
			{
				if (Maze[(int)toCheck.x, (int)toCheck.y] == 1 && HasThreeWallsIntact(toCheck))
				{
					validNeighbors.Add(toCheck);
				}
			}
		}
		return validNeighbors;
	}
	private bool HasThreeWallsIntact(Vector2 Vector2ToCheck)
	{
		int intactWallCounter = 0;
		foreach (var offset in offsets)
		{
			Vector2 neighborToCheck = new Vector2(Vector2ToCheck.x + offset.x, Vector2ToCheck.y + offset.y);
			if (IsInside(neighborToCheck) && Maze[(int)neighborToCheck.x, (int)neighborToCheck.y] == 1)
			{
				intactWallCounter++;
			}
		}
		return intactWallCounter == 3;
	}
	private bool IsInside(Vector2 p)
	{
		return p.x >= 0 && p.y >= 0 && p.x < width && p.y < height;
	}
}
