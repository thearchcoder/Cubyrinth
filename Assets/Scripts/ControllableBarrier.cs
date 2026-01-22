using UnityEngine;

public class ControllableBarrier : MonoBehaviour
{
	private bool isOpen = false;
	private Collider barrierCollider;
	private Renderer barrierRenderer;
	private Color closedColor;
	private Color openColor;
	private float transitionSpeed = 5.0f;
	private float currentAlpha = 1.0f;
	private float targetAlpha = 1.0f;

	void Start()
	{
		barrierCollider = GetComponent<Collider>();
		barrierRenderer = GetComponent<Renderer>();

		if (barrierRenderer != null)
		{
			closedColor = barrierRenderer.material.color;
			Color mazeBgColor = new Color(0.392f, 0.208f, 0.0f);
			openColor = closedColor * 0.3f + mazeBgColor * 0.7f;

			Material mat = barrierRenderer.material;
			mat.SetFloat("_Mode", 3);
			mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			mat.SetInt("_ZWrite", 0);
			mat.DisableKeyword("_ALPHATEST_ON");
			mat.EnableKeyword("_ALPHABLEND_ON");
			mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			mat.renderQueue = 3000;
		}
	}

	void Update()
	{
		targetAlpha = isOpen ? 0.2f : 1.0f;
		currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * transitionSpeed);

		if (barrierRenderer != null)
		{
			Color targetColor = isOpen ? openColor : closedColor;
			Color c = Color.Lerp(barrierRenderer.material.color, targetColor, Time.deltaTime * transitionSpeed);
			c.a = currentAlpha;
			barrierRenderer.material.color = c;
		}
	}

	public void Open()
	{
		isOpen = true;
		if (barrierCollider != null)
		{
			barrierCollider.enabled = false;
		}
	}

	public void Close()
	{
		isOpen = false;
		if (barrierCollider != null)
		{
			barrierCollider.enabled = true;
		}
	}
}
