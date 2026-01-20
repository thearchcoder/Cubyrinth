using UnityEngine;

public class WinningArea : MonoBehaviour
{
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
			Destroy(other.gameObject);
			ballsRemaining--;

			if (ballsRemaining <= 0)
			{
				Debug.Log("You won!");
			}
		}
	}

	public static void ResetBallCount()
	{
		totalBalls = -1;
		ballsRemaining = 0;
	}
}
