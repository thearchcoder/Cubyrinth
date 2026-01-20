using UnityEngine;

public class WinningArea : MonoBehaviour
{
	public Color requiredColor;
	private static int totalBalls = -1;
	private static int ballsRemaining = 0;

	void Start()
	{
		if (totalBalls == -1)
		{
			GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
			totalBalls = balls.Length;
			ballsRemaining = totalBalls;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Ball"))
		{
			BallColor ballColor = other.GetComponent<BallColor>();
			if (ballColor != null)
			{
				if (ColorsMatch(ballColor.color, requiredColor))
				{
					// Instead of destroying immediately, start the smooth entry animation
					SwipeBall swipeBall = other.GetComponent<SwipeBall>();
					if (swipeBall != null)
					{
						swipeBall.StartEnteringHole(transform.position);
					}

					ballsRemaining--;

					if (ballsRemaining <= 0)
					{
						Debug.Log("You won!");
					}
				}
			}
		}
	}

	bool ColorsMatch(Color a, Color b)
	{
		// Check if colors are approximately equal (with small tolerance for floating point comparison)
		float tolerance = 0.01f;
		return Mathf.Abs(a.r - b.r) < tolerance &&
		       Mathf.Abs(a.g - b.g) < tolerance &&
		       Mathf.Abs(a.b - b.b) < tolerance;
	}

	public static void ResetBallCount()
	{
		totalBalls = -1;
		ballsRemaining = 0;
	}
}
