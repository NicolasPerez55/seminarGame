using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Power : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0)) // Left click
		{
			DetectClick();
		}
	}
	
	private void DetectClick()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			if (hit.collider.gameObject == this.gameObject)
			{
				SceneManager.LoadScene(1);
			} 
		}
	}
}
