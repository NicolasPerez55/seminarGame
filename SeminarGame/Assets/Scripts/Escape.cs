using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Escape : MonoBehaviour
{
	[Header("CanvasItems")]
	[SerializeField] private GameObject escapeCanvas;
	[SerializeField] private GameObject escapeButton;
	[SerializeField] private Image escapeTV;


	[Header("Player")]
	[SerializeField] private Transform player;
	[SerializeField] private GameObject cameraController;
	[SerializeField] private float scaleSpeed = 0.5f; // Adjust this value as needed


	private bool _isEscaping = false;
	private float _timer = 0f;
	private float _maxTimer = 3f;
	void Start()
	{
		escapeCanvas.SetActive(false);
		escapeButton.SetActive(false);
		escapeTV.enabled = false;    
	}

	void Update()
	{
		if (_isEscaping)
		{
			Cursor.lockState = CursorLockMode.None;
			escapeTV.transform.localScale -= Vector3.one * (scaleSpeed * Time.deltaTime);
			if (escapeTV.transform.localScale.x <= 1.1f)
			{
				escapeTV.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
			}
			_timer += Time.deltaTime;
			if (_timer >= _maxTimer)
			{
				EscapeSuccess();
			}
		}
	}

	void OnMouseOver()
	{
		if (Input.GetMouseButtonDown(0) && !_isEscaping)
		{
			if (Vector3.Distance(player.position, transform.position) < 15)
			{
				TryEscape();
			}
		}
	}

	void TryEscape()
	{
		escapeCanvas.SetActive(true);
		escapeTV.enabled = true;
		_isEscaping = true;
	}

	void EscapeSuccess()
	{
		escapeButton.SetActive(true);
		cameraController.GetComponent<ExamplePlayer>().enabled = false;
	}
	
	public void QuitGame()
	{
		Application.Quit();
	}
}
