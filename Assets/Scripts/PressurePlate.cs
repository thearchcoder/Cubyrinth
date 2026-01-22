using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
	[Header("Plate Settings")]
	public bool requiresWeight = true;
	public Color activatedColor = new Color(0.2f, 0.8f, 0.2f);
	public Color deactivatedColor = new Color(0.5f, 0.5f, 0.5f);

	[Header("Events")]
	public UnityEvent onActivated = new UnityEvent();
	public UnityEvent onDeactivated = new UnityEvent();

	private bool isActivated = false;
	private int ballsOnPlate = 0;
	private Renderer outerRenderer;
	private Renderer innerRenderer;
	private Color outerBaseColor;
	private Color innerBaseColor;

	void Start()
	{
		Transform outerSquare = transform.Find("OuterSquare");
		Transform innerSquare = transform.Find("InnerSquare");

		if (outerSquare != null)
		{
			outerRenderer = outerSquare.GetComponent<Renderer>();
			if (outerRenderer != null)
			{
				outerBaseColor = outerRenderer.material.color;
			}
		}

		if (innerSquare != null)
		{
			innerRenderer = innerSquare.GetComponent<Renderer>();
			if (innerRenderer != null)
			{
				innerBaseColor = innerRenderer.material.color;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Ball"))
		{
			ballsOnPlate++;
			if (!isActivated)
			{
				Activate();
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Ball"))
		{
			ballsOnPlate--;
			if (ballsOnPlate <= 0 && isActivated && requiresWeight)
			{
				ballsOnPlate = 0;
				Deactivate();
			}
		}
	}

	void Activate()
	{
		isActivated = true;
		if (outerRenderer != null)
		{
			outerRenderer.material.color = outerBaseColor * 1.5f;
		}
		if (innerRenderer != null)
		{
			innerRenderer.material.color = innerBaseColor * 1.5f;
		}
		onActivated.Invoke();
		Debug.Log("Pressure plate activated!");
	}

	void Deactivate()
	{
		isActivated = false;
		if (outerRenderer != null)
		{
			outerRenderer.material.color = outerBaseColor;
		}
		if (innerRenderer != null)
		{
			innerRenderer.material.color = innerBaseColor;
		}
		onDeactivated.Invoke();
		Debug.Log("Pressure plate deactivated!");
	}

	public bool IsActivated()
	{
		return isActivated;
	}
}
