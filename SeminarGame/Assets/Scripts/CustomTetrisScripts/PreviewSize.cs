using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSize : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;
	[SerializeField] private Sprite phone;
	[SerializeField] private Sprite door;
	[SerializeField] private Sprite fish;
	[SerializeField] private Sprite fishfood;
	[SerializeField] private Sprite box;
	
	[SerializeField] private Vector3 phoneVector;
	[SerializeField] private Vector3 doorVector;
	[SerializeField] private Vector3 fishVector;
	[SerializeField] private Vector3 foodVector;
	[SerializeField] private Vector3 boxVector;
	private Vector3 vector3;
	
	// Start is called before the first frame update
	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		vector3 = transform.localScale;
	}

	// Update is called once per frame
	void Update()
	{
		if (spriteRenderer.sprite == phone)
		{
			transform.localScale = phoneVector;
		} else if (spriteRenderer.sprite == door)
		{
			transform.localScale = doorVector;
		} else if (spriteRenderer.sprite == fish)
		{
			transform.localScale = fishVector;
		} else if (spriteRenderer.sprite == fishfood)
		{
			transform.localScale = foodVector;
		} else if (spriteRenderer.sprite == box)
		{
			transform.localScale = boxVector;
		} else
		{
			transform.localScale = vector3;
		}
	}
}
