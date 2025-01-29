using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractManager : MonoBehaviour
{
	private static InteractManager instance;
	public static InteractManager Instance => instance;

	public TetrominoData data { get; private set; }
	public Vector3Int[] cells { get; private set; }
	private Vector3 originalPosition;
	private Vector3 originalDoorPosition;
	private Vector3 originalDoorScale;
	private bool isRinging = false;
	private bool isKnocking = false;
	private float knockDuration = 0.5f;

	[SerializeField] private GameObject phone;
	[SerializeField] private float shakeIntensity = 0.05f;
	
	[SerializeField] private GameObject door;
	[SerializeField] private float knockRepetitions = 3;
	[SerializeField] private float knockPauseDuration  = 1f;
	[SerializeField] private float knockScaleIntensity = 5f;
	[SerializeField] private float shakeDuration = 0.5f;

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
		originalDoorPosition = door.transform.position;
		originalDoorScale = door.transform.localScale;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			Interact();
		}

		if (phone.transform.position != originalPosition && !isRinging)
		{
			isRinging = true;
			StartCoroutine(RingPhone());
		}
		
		if (door.transform.position != originalDoorPosition && !isKnocking)
		{
			isKnocking = true;
			StartCoroutine(KnockDoor()); // Start door knock animation
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

		isRinging = false;
	}

	private IEnumerator KnockDoor()
	{

		while (true) // Keeps knocking indefinitely
		{
			for (int i = 0; i < knockRepetitions; i++)
			{
				yield return StartCoroutine(PerformSingleKnock());
				yield return new WaitForSeconds(0.2f); // Small delay between knocks
			}

			yield return new WaitForSeconds(knockPauseDuration); // Pause before the next knocking sequence
		}
	}

	private IEnumerator PerformSingleKnock()
	{
		float timePassed = 0f;
		bool growing = true;

		while (timePassed < knockDuration)
		{
			float scaleChange = (growing ? knockScaleIntensity : -knockScaleIntensity) * Time.deltaTime * 4;
			door.transform.localScale += new Vector3(scaleChange, scaleChange, 0);

			if (door.transform.localScale.x >= originalDoorScale.x + knockScaleIntensity)
				growing = false;
			else if (door.transform.localScale.x <= originalDoorScale.x)
				growing = true;

			timePassed += Time.deltaTime;
			yield return null;
		}

		// Reset door scale
		door.transform.localScale = originalDoorScale;
	}


	public void Interact()
	{
		FindAnyObjectByType<CameraHandler>().IncrementZoom();
	}
}
