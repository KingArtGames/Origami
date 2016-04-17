using UnityEngine;
using System.Collections;

public class ItemStatus_Origami : MonoBehaviour {

    public float floatSpeed = 3;
    public float amplitude = 1;

    private Vector3 startPos;
    private float floatOffset;

    public CharacterShapes ItemPick;
    private bool ItemGive;

    void Start()
    {
        startPos = transform.position;
        floatOffset = Random.Range(0f, 90f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            CharachterStatus_Origami charStat = other.GetComponent<CharachterStatus_Origami>();

            switch (ItemPick)
            {
                case CharacterShapes.Fox:
                    charStat.o_Fox = true;
                    break;
                case CharacterShapes.Fish:
                    charStat.o_Fish = true;
                    break;
                case CharacterShapes.Bird:
                    charStat.o_Bird = true;
                    break;
                case CharacterShapes.none:
                    break;
                default:
                    break;
            }

            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.position = startPos + Vector3.up * +amplitude * Mathf.Sin(floatSpeed * Time.time + floatOffset);
    }
}
