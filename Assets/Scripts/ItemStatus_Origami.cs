using UnityEngine;
using System.Collections;

public class ItemStatus_Origami : MonoBehaviour {

    public enum ItemEnum { Fox=0, Fish=1, Fly=2 };
    private CharachterStatus_Origami charachterStatus_Origami;
    public ItemEnum ItemPick;
    private bool ItemGive;

    void OnTriggerEnter2D(Collider2D other)
    {
        charachterStatus_Origami = other.GetComponent<CharachterStatus_Origami>();
        if (ItemPick == ItemEnum.Fox)
            charachterStatus_Origami.o_Fox=true;
        if (ItemPick == ItemEnum.Fish)
            charachterStatus_Origami.o_Fish = true;
        if (ItemPick == ItemEnum.Fly)
            charachterStatus_Origami.o_Fly = true;
        Debug.Log(ItemPick);
    }
}
