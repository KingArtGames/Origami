using UnityEngine;
using System.Collections;

public class AirCurrent : MonoBehaviour {
    private PlatformerCharacter2D_Origami platformerCharacter2D_Origami;
    //private enum BirdStat;

    void OnTriggerStay2D(Collider2D other)
    {
        platformerCharacter2D_Origami = other.GetComponent<PlatformerCharacter2D_Origami>();
        //BirdStat = platformerCharacter2D_Origami.CharacterShapes
           // GetComponent<AreaEffector2D>().enabled= true;
    }
}
