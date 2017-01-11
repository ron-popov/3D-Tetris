using System;﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	//Constant values
	private const float defaultSize = 10f;
	private const float playerScaleMultiplier = 0.8f;
	private const float playerStartingZValue = -7f;
	private const float playerZScale = 1f;
	private const float wallStartingZValue = 15f;
	private const float wallAccelerationPerRoundMultiplier = 1.5f;
	private const int maxScoreAcceleration = 6;
	private const float startingWallSpeed = 0.05f;
	
	
	//Variables
	public static int score = 0;
	public float squareSize;
	public static int squares_per_line = 3;
	private float totalSquares;
	private float movingWallSpeed = startingWallSpeed;
	private GameObject template;
	private List<GameObject> list = null;
	private GameObject player;
	private Rigidbody playerRigidbody;
	private Vector3 templateLoc;
	private Vector3 templateScale;

	void OnEnable(){
		//Initializing stuff
		score = 0;
		player = GameObject.Find("Player");
		playerRigidbody = player.GetComponent<Rigidbody>();
		GameObject.Find("Movable").transform.position = new Vector3(0 , 5f , 0f);	
		if(list != null){
			foreach(GameObject g in list){
				if(g.name != "0"){
					Destroy(g);					
				}
			}
			list = null;
		}
		list = new List<GameObject>();	
		movingWallSpeed = startingWallSpeed;
		score = 0;
		totalSquares = (float) (Math.Pow(squares_per_line , 2));
		squareSize = defaultSize / squares_per_line;
		template = GameObject.Find("Movable/0");
		playerRigidbody.MovePosition(new Vector3(-1 * (defaultSize / 2f) + squareSize / 2f , defaultSize / 2f - squareSize / 2f + defaultSize / 2 , 5f));
		template.transform.position = new Vector3(-1 * (defaultSize / 2f) + squareSize / 2f , defaultSize / 2f - squareSize / 2f + defaultSize / 2 , 5f);
		template.transform.localScale = new Vector3(10f / squares_per_line , 10f / squares_per_line , 10f / squares_per_line);
		list.Add(template);
		
		//Make sure the squares per line isn't zero
		if(squares_per_line == 0){
			Debug.Log("ERROR , square_per_line cannot be 0");
			return;
		}
				
		//Creating the rest of the tiles
		for(int i = 1 ; i < totalSquares ; i++){
			string name = i.ToString();
			GameObject g = GameObject.Instantiate(template);
			g.name = name;
			g.transform.localScale = template.transform.localScale;
			g.transform.parent = GameObject.Find("Movable").transform;
			g.transform.rotation = template.transform.rotation;
			float x = template.transform.position.x;
			float y = template.transform.position.y;
			x += (float) (squareSize * (i % squares_per_line));
			y -= (float) (squareSize * ((int) (i / Math.Sqrt(totalSquares))));
			g.transform.position = new Vector3(x , y , 5f);
			list.Add(g);
		}
		
		//Setting up the player start values
		System.Random rand = new System.Random();
		int startPosition = rand.Next((int) Math.Pow(squares_per_line , 2));
		player.transform.localScale = list[startPosition].transform.localScale;
		player.transform.localScale = new Vector3(player.transform.localScale.x * playerScaleMultiplier , player.transform.localScale.y * playerScaleMultiplier , playerZScale);
		playerRigidbody.position = (list[startPosition].transform.TransformPoint(Vector3.zero));
		playerRigidbody.position = new Vector3(playerRigidbody.position.x , playerRigidbody.position.y , playerStartingZValue);
		
		//Choosing the random tile to move
		int target = rand.Next((int) Math.Pow(squares_per_line , 2));
		list[target].SetActive(false);
	}

	void Start () {
		//Initialize first variables
	}
		
	
	// Update is called once per frame
	void Update () {
		if(squares_per_line == 0){
			return;
		}
		
		//Moves every tile a bit forward every frame
		for(int i = 0 ; i < list.Count ; i++){
			if(list[i].transform.position.z > GameObject.Find("Camera").transform.position.z){
				//FIXME : Don't use transform.position for movement , use the rigidbody.position
				Rigidbody temp = list[i].GetComponent<Rigidbody>();
				//temp.AddForce(Vector3.forward * movingWallSpeed * -1);
				temp.position = new Vector3(temp.position.x , temp.position.y , temp.position.z - movingWallSpeed);
			}
		}
		
		//Allowes the user to toggle the guildlines by pressing p
		if(Input.GetKeyDown(KeyCode.P)){
			//TODO : Will toggle the guildlines
		}
		
		//Moves the player according to keyboard input
		float speed = squareSize;
		if(Input.GetKeyDown(KeyCode.UpArrow)){
			if(! (player.transform.position.y + speed > GameObject.Find("Plane (4)").transform.position.y)){
				player.transform.position = new Vector3(player.transform.position.x , player.transform.position.y + speed , player.transform.position.z);
			}
		} else if (Input.GetKeyDown(KeyCode.DownArrow)){
			if(! (player.transform.position.y - speed < GameObject.Find("Floor").transform.position.y)){
				player.transform.position = new Vector3(player.transform.position.x , player.transform.position.y - speed , player.transform.position.z);
			}
		} else if (Input.GetKeyDown(KeyCode.LeftArrow)){
			if(! (player.transform.position.x - speed < GameObject.Find("Plane (2)").transform.position.x)){
				player.transform.position = new Vector3(player.transform.position.x - speed , player.transform.position.y , player.transform.position.z);
			}
		} else if (Input.GetKeyDown(KeyCode.RightArrow)){
			if(! (player.transform.position.x + speed > GameObject.Find("Plane (1)").transform.position.x)){
				player.transform.position = new Vector3(player.transform.position.x + speed , player.transform.position.y , player.transform.position.z);			
			}
		}
		
		
		//If the player succeeded to pass
		if(RoundOverDetector.wasRoundOver && ! CollisionDetector.wasCollision){
			score++;
			
			//Returns all of the walls to the starting position
			for(int i = 0 ; i < list.Count ; i++){
				list[i].transform.position = new Vector3(list[i].transform.position.x , list[i].transform.position.y , wallStartingZValue);
				list[i].SetActive(true);
			}
			
			System.Random rand = new System.Random();
			int randomNumber = rand.Next((int) Math.Pow(squares_per_line , 2));
			list[randomNumber].SetActive(false);
			
			
			if(!(score >= maxScoreAcceleration)){
				movingWallSpeed *= wallAccelerationPerRoundMultiplier;
			}
			
			RoundOverDetector.wasRoundOver = false;	
		}
	}
}