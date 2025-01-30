using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fish : MonoBehaviour
{
	[SerializeField] private GameObject fishfood;
	private void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject == fishfood)
		{
			Destroy(other.gameObject);
		}
	}
}
