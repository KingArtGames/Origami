using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Main : MonoBehaviour {

    [SerializeField] GameObject FoxUnlock;
    [SerializeField] GameObject FishUnlock;
    [SerializeField] GameObject BirdUnlock;
    [SerializeField] GameObject BlossumCounter;
    [SerializeField] GameObject PlayerCharacter;
    [SerializeField] Sprite InActiveUnlockIcon;
    [SerializeField] Sprite ActiveUnlockIcon;

    private CharachterStatus_Origami charStats;
    private PlatformerCharacter2D_Origami charClass;
    private GameObject foxCheckMark;
    private GameObject fishCheckMark;
    private GameObject birdCheckMark;

    private Image foxPic;
    private Image fishPic;
    private Image birdPic;

    private Text blossomCountText;

    private Slider wingCoolDownBar;

    // Use this for initialization
    void Start () {
        charStats = PlayerCharacter.GetComponent<CharachterStatus_Origami>();
        charClass = PlayerCharacter.GetComponent<PlatformerCharacter2D_Origami>();

        foxCheckMark = FoxUnlock.transform.Find("FoxCheckMark").gameObject;
        fishCheckMark = FishUnlock.transform.Find("FishCheckMark").gameObject;
        birdCheckMark = BirdUnlock.transform.Find("BirdCheckMark").gameObject;

        foxPic = FoxUnlock.GetComponent<Image>();
        fishPic = FishUnlock.GetComponent<Image>();
        birdPic = BirdUnlock.GetComponent<Image>();

        blossomCountText = BlossumCounter.GetComponent<Text>();

        wingCoolDownBar = transform.Find("Slider").GetComponent<Slider>();
    }
	
	// Update is called once per frame
	void Update () {

	}

    void FixedUpdate()
    {
        foxCheckMark.SetActive(charStats.o_Fox);
        fishCheckMark.SetActive(charStats.o_Fish);
        birdCheckMark.SetActive(charStats.o_Bird);

        CharacterShapes currShape = charClass.GetCurrentCharacterShape();
        foxPic.sprite = currShape == CharacterShapes.Fox ? ActiveUnlockIcon : InActiveUnlockIcon;
        fishPic.sprite = currShape == CharacterShapes.Fish ? ActiveUnlockIcon : InActiveUnlockIcon;
        birdPic.sprite = currShape == CharacterShapes.Bird? ActiveUnlockIcon : InActiveUnlockIcon;

        blossomCountText.text = charClass.GetBlossemCount().ToString();
        
        wingCoolDownBar.value=(1-(charClass.GetCurrentWingCooldownTime() / charClass.GetWingCooldownTime()));
        wingCoolDownBar.gameObject.SetActive(currShape == CharacterShapes.Bird);

    }
}
