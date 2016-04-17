using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class intro_button : MonoBehaviour {

	//Outside your functions where you want to set all your declarations...
	public GameObject[] pages; //store all your images in here at design time
	public GameObject displayObject; //The current image thats visible
	public Button nextImg; //Button to view next image
	public Button prevImg; //Button to view previous image
	public int i = 0; //Will control where in the array you are
	public GameObject nextButton;
	public GameObject backButton;

	public void BtnNext () {
		if(i + 1 < pages.Length){
			pages[i].SetActive(false);
			i++;
		}
			

		if(i == pages.Length){
			nextButton.SetActive(false);
		}
			if (i == 1){
				backButton.SetActive(true);
			}
		}





	public void BtnPrev () {
		if(i - 1 >= 0){
			pages[i].SetActive(false);
			i--;
		}

		if(i == 0){
				backButton.SetActive(false);
			}

		if(i == pages.Length - 1){
			nextButton.SetActive(true);
		}

			
		}

		// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		//displayImage.sprite = pages[i];
		pages[i].SetActive(true);
		//if(i - 1 > 0){
		//pages[i-1].SetActive(false);
			
	}
}


