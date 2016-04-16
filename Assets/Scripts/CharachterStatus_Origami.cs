using UnityEngine;
using System.Collections;

public class CharachterStatus_Origami : MonoBehaviour {

    public bool o_Fox = false;
    public bool o_Fish = false;
    public bool o_Fly = false;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Item")
            Destroy(other.gameObject);
    }
}
