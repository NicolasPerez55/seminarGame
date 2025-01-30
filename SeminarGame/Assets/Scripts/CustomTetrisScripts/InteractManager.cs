using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
	private Vector3 originalNotificationPosition;
	private Vector3 originalBoxPosition;
	private Vector3 originalFishPosition;
	private Vector3 originalFishFoodPosition;
	
	private bool isRinging = false;
	private bool isKnocking = false;
	private float knockDuration = 0.5f;
	[SerializeField] private float disappearTime = 3f;
	private float fishFoodTrigger = 0;
	private Dictionary<GameObject, Coroutine> activeDisappearCoroutines = new Dictionary<GameObject, Coroutine>();


	[SerializeField] private GameObject phone;
	[SerializeField] private float shakeIntensity = 0.05f;
	
	[SerializeField] private GameObject door;
	[SerializeField] private float knockRepetitions = 3;
	[SerializeField] private float knockPauseDuration  = 1f;
	[SerializeField] private float knockScaleIntensity = 5f;
	[SerializeField] private float shakeDuration = 0.5f;
	
	[SerializeField] private GameObject notification;
	
	[SerializeField] private GameObject box;
	
	[SerializeField] private GameObject fish;
	[SerializeField] private GameObject fishFood;

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
		originalNotificationPosition = notification.transform.position;
		originalBoxPosition = box.transform.position;
		originalFishPosition = fish.transform.position;
		originalFishFoodPosition = fishFood.transform.position;
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
		
		if (Input.GetMouseButtonDown(0)) // Left click
		{
			DetectClick();
		}
		
		CheckObjectMovement(phone, originalPosition);
		CheckObjectMovement(door, originalDoorPosition);
		CheckObjectMovement(notification, originalNotificationPosition);
		CheckObjectMovement(box, originalBoxPosition);
		CheckObjectMovement(fish, originalFishPosition);
		CheckObjectMovement(fishFood, originalFishFoodPosition);
	}
	
	private void CheckObjectMovement(GameObject obj, Vector3 originalPos)
	{
		if (obj.transform.position != originalPos)
		{
			// Start coroutine only if one is NOT already running for this object
			//if (!activeDisappearCoroutines.ContainsKey(obj))
			//{
				Coroutine disappearCoroutine = StartCoroutine(StartDisappearTimer(obj, originalPos));
				activeDisappearCoroutines[obj] = disappearCoroutine;
			//}
		}
	}
	
	private void DetectClick()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			Debug.Log(hit.collider.gameObject.name);
			if (hit.collider.gameObject == phone)
			{
				HandlePhoneInteraction();
			} else if (hit.collider.gameObject == door)
			{
				HandleDoorInteraction();
			} else if (hit.collider.gameObject == notification)
			{
				HandleNotificationInteraction();
			} else if (hit.collider.gameObject == box)
			{
				HandleBoxInteraction();
			}
		}
	}
	
	private void HandlePhoneInteraction()
	{
		phone.GetComponent<SpriteRenderer>().enabled = false;
		phone.GetComponent<Collider>().enabled = false;
		
		Interact();
	}
	
	private void HandleDoorInteraction()
	{
		door.GetComponent<SpriteRenderer>().enabled = false;
		door.GetComponent<Collider>().enabled = false;
		
		Interact();
	}
	
	private void HandleNotificationInteraction()
	{
		notification.GetComponent<SpriteRenderer>().enabled = false;
		notification.GetComponent<Collider>().enabled = false;
		
		Interact();
	}
	
	private void HandleBoxInteraction()
	{
		box.GetComponent<SpriteRenderer>().enabled = false;
		box.GetComponent<Collider>().enabled = false;
		
		Interact();
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

	private IEnumerator StartDisappearTimer(GameObject obj, Vector3 originalPos)
	{
		yield return new WaitForSeconds(disappearTime);
		float fadeDuration = 2f;
		float elapsedTime = 0f;
		SpriteRenderer objRenderer = obj.GetComponent<SpriteRenderer>();
		Color originalColor = objRenderer.material.color;

		while (elapsedTime < fadeDuration)
		{
			float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
			objRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		
		if (obj == phone)
		{
			obj.GetComponent<SpriteRenderer>().enabled = false;
			obj.GetComponent<Collider>().enabled = false;
		}
		
		obj.transform.position = originalPos;
		objRenderer.material.color = originalColor;
		
		//activeDisappearCoroutines.Remove(obj);
	}

	public void Interact()
	{
		FindAnyObjectByType<CameraHandler>().IncrementZoom();
	}
}
