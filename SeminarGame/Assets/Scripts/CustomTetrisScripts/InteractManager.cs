using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractManager : MonoBehaviour
{
	private static InteractManager instance;
	public static InteractManager Instance
	{
		get { return instance; }
	}
	
	public TetrominoData data { get; private set; }
	public Vector3Int[] cells { get; private set; }
	private Vector3 originalPosition;
	private bool isRinging = false;
	private float shakeIntensity = 0.05f;
	private float shakeDuration = 0.5f;
	
	[SerializeField] private GameObject phone;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{
		originalPosition = phone.transform.position;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			Interact();
		}
		
		// Check if the phone is not in its original position and is not already ringing
		if (phone.transform.position != originalPosition && !isRinging)
		{
			isRinging = true;
			StartCoroutine(RingPhone());
		}
	}
	
	private IEnumerator RingPhone()
	{
		float timePassed = 0f;

		while (timePassed < shakeDuration)
		{
			Vector3 randomShake = new Vector3(
				Random.Range(-shakeIntensity, shakeIntensity),
				Random.Range(-shakeIntensity, shakeIntensity),
				0f
			);

			phone.transform.position += randomShake;
			timePassed += Time.deltaTime;

			yield return null;
		}

		// Return to the original position after shaking
		//transform.position = originalPosition;
		isRinging = false;
	}

	public void Interact()
	{
		FindAnyObjectByType<CameraHandler>().IncrementZoom();
	}
}
