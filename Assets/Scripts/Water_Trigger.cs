using UnityEngine;
using System.Collections;

public class Water_Trigger : MonoBehaviour {

	void OnTriggerEnter (Collider other)
    {
        Debug.Log(other);
    }
}
