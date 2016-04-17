using UnityEngine;
using System.Collections;

public class pickup_Blossom : MonoBehaviour {

    public float floatSpeed = 3;
    public float amplitude = 1;
    private Vector3 startPos;
    private float floatOffset;

    void Start()
    {
        startPos = transform.position;
        floatOffset = Random.Range(0f, 90f);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            collider.GetComponent<PlatformerCharacter2D_Origami>().RaiseBlossemCounter(1);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.position = startPos + Vector3.up * + amplitude * Mathf.Sin(floatSpeed * Time.time + floatOffset);
    }
}
