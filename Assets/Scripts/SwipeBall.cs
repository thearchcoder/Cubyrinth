using UnityEngine;
using UnityEngine.InputSystem;
public enum BallAxis
{
	XPositive,
	XNegative,
	YPositive,
	YNegative,
	ZPositive,
	ZNegative
}
public class SwipeBall : MonoBehaviour
{
	[SerializeField] private BallAxis m_Face = BallAxis.ZPositive;
	[SerializeField] private float m_GravityMultiplier = 9.81f;
	private Rigidbody m_Rigidbody;
	void Start()
	{
		m_Rigidbody = GetComponent<Rigidbody>();
		if (m_Rigidbody == null)
		{
			m_Rigidbody = gameObject.AddComponent<Rigidbody>();
		}
		m_Rigidbody.useGravity = false;

		GameObject maze_walls = GameObject.FindGameObjectWithTag("MazeOutsideWalls");
		if (maze_walls != null)
		{
			Renderer[] renderers = maze_walls.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in renderers)
			{
				renderer.enabled = false;
			}
		}

		if (Accelerometer.current != null)
		{
			InputSystem.EnableDevice(Accelerometer.current);
		}
	}
	void FixedUpdate()
	{
		if (Accelerometer.current != null)
		{
			Vector3 accel = Accelerometer.current.acceleration.ReadValue();
			Vector3 gravity = GetGravityDirection(accel) * m_GravityMultiplier;
			m_Rigidbody.AddForce(gravity, ForceMode.Acceleration);
			ConstrainVelocity();
		}
	}
	Vector3 GetGravityDirection(Vector3 accel)
	{
		switch (m_Face)
		{
			case BallAxis.XPositive:
				return new Vector3(0f, -accel.y, accel.x);
			case BallAxis.XNegative:
				return new Vector3(0f, -accel.y, -accel.x);
			case BallAxis.YPositive:
				return new Vector3(-accel.x, 0f, -accel.y);
			case BallAxis.YNegative:
				return new Vector3(-accel.x, 0f, accel.y);
			case BallAxis.ZPositive:
				return new Vector3(-accel.x, -accel.y, 0f);
			case BallAxis.ZNegative:
				return new Vector3(accel.x, -accel.y, 0f);
			default:
				return new Vector3(-accel.x, -accel.y, 0f);
		}
	}
	void ConstrainVelocity()
	{
		Vector3 velocity = m_Rigidbody.linearVelocity;
		switch (m_Face)
		{
			case BallAxis.XPositive:
			case BallAxis.XNegative:
				velocity.x = 0f;
				break;
			case BallAxis.YPositive:
			case BallAxis.YNegative:
				velocity.y = 0f;
				break;
			case BallAxis.ZPositive:
			case BallAxis.ZNegative:
				velocity.z = 0f;
				break;
		}
		m_Rigidbody.linearVelocity = velocity;
	}
}
