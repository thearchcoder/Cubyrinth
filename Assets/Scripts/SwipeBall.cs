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
	[SerializeField] private float m_DragForceMultiplier = 0.006f;
	private Rigidbody m_Rigidbody;
	private bool m_IsDragging;
	private Vector2 m_LastPosition;
	void Start()
	{
		m_Rigidbody = GetComponent<Rigidbody>();
		if (m_Rigidbody == null)
		{
			m_Rigidbody = gameObject.AddComponent<Rigidbody>();
		}

		GameObject maze_walls = GameObject.FindGameObjectWithTag("MazeOutsideWalls");
		if (maze_walls != null)
		{
			Renderer[] renderers = maze_walls.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in renderers)
			{
				renderer.enabled = false;
			}
		}
	}

	void Update()
	{
		if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
		{
			HandleTouch();
		}
		else
		{
			HandleMouse();
		}
	}
	void HandleTouch()
	{
		var touch = Touchscreen.current.primaryTouch;
		if (touch.press.wasPressedThisFrame)
		{
			m_IsDragging = true;
			m_LastPosition = touch.position.ReadValue();
		}
		else if (m_IsDragging)
		{
			Vector2 current_position = touch.position.ReadValue();
			ApplyDragForce(current_position);
		}
		if (touch.press.wasReleasedThisFrame)
		{
			m_IsDragging = false;
			m_Rigidbody.linearVelocity = Vector3.zero;
		}
	}
	void HandleMouse()
	{
		if (Mouse.current == null) return;
		if (Mouse.current.leftButton.wasPressedThisFrame)
		{
			m_IsDragging = true;
			m_LastPosition = Mouse.current.position.ReadValue();
		}
		else if (Mouse.current.leftButton.isPressed && m_IsDragging)
		{
			Vector2 current_position = Mouse.current.position.ReadValue();
			ApplyDragForce(current_position);
		}
		else if (Mouse.current.leftButton.wasReleasedThisFrame)
		{
			m_IsDragging = false;
			m_Rigidbody.linearVelocity = Vector3.zero;
		}
	}
	Vector3 GetForceDirection(Vector2 delta)
	{
		switch (m_Face)
		{
			case BallAxis.XPositive:
				return new Vector3(0f, delta.y, -delta.x);
			case BallAxis.XNegative:
				return new Vector3(0f, delta.y, delta.x);
			case BallAxis.YPositive:
				return new Vector3(delta.x, 0f, delta.y);
			case BallAxis.YNegative:
				return new Vector3(delta.x, 0f, -delta.y);
			case BallAxis.ZPositive:
				return new Vector3(delta.x, delta.y, 0f);
			case BallAxis.ZNegative:
				return new Vector3(-delta.x, delta.y, 0f);
			default:
				return new Vector3(delta.x, delta.y, 0f);
		}
	}
	void ApplyDragForce(Vector2 current_position)
	{
		Vector2 delta = current_position - m_LastPosition;
		Vector3 force = GetForceDirection(delta) * m_DragForceMultiplier;

		m_Rigidbody.linearVelocity = force;
		m_LastPosition = current_position;
		ConstrainVelocity();
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
