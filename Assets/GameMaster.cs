using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour {

	private GameObject canvas;
	private GameObject script;
	private GameObject textObject;
	private Text text;
	private Text scoreCounter;
	private Text secondText;
	private GameObject gameOverlay;

	// Use this for initialization
	void Start () {
		
		gameOverlay = GameObject.Find("GameOverlay");
		scoreCounter = GameObject.Find("GameOverlay/Canvas/Text").GetComponent<Text>();
		scoreCounter.text = "";
		scoreCounter.color = Color.white;
		gameOverlay.SetActive(false);
		secondText = GameObject.Find("Start Menu/Canvas/SecondText").GetComponent<Text>();
		secondText.text = "";
				
		textObject = GameObject.Find("Start Menu/Canvas/Text");
		text = textObject.GetComponent<Text>();
		text.color = Color.white;
		text.fontSize = 18;
		text.text = "";
		
		canvas = GameObject.Find("Start Menu/Canvas");
		script = GameObject.Find("SecondGameMaster");
		script.SetActive(false);
		
		
	}

	public void OnExit(){
		Application.Quit();
	}

	//When start button is clicked
	public void OnStartClick(){
	if(script.activeSelf){
			script.SetActive(false);
		}

		text.text = "";
		CollisionDetector.wasCollision = false;
		GameController.score = 0;
		script.SetActive(true);
		canvas.SetActive(false);
		gameOverlay.SetActive(true);
	}	
	
	// Update is called once per frame
	void Update () {
		//If the script colided with the moving wall
		scoreCounter.text = "Score : " + GameController.score;
		if(CollisionDetector.wasCollision){
			int score = GameController.score;
			script.SetActive(false);
			text.text = ("GAME OVER");
			secondText.text =("Score : " + score);
			gameOverlay.SetActive(false);
			canvas.SetActive(true);
		}
	}
}
