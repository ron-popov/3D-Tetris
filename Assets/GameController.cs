using System;﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	private const int requiredFramesClosed = 30;
	
	
	//Variables
	public static int score = 0;
	public static float squareSize;
	public static int squares_per_line = 3;
	private float totalSquares;
	private float movingWallSpeed = startingWallSpeed;
	private GameObject template;
	private List<GameObject> list = null;
	private GameObject player;
	private Rigidbody playerRigidbody;
	private Vector3 templateLoc;
	private Vector3 templateScale;
	
	//Realsense variables
	public SenseInput input;
	public float hmin, hmax;
	public float vmin, vmax;
	
	private Texture2D depthImage=null;
	private static bool visualizeImage=false;
	public  static bool visualizeHand=true;
	public  static bool visualizeFace=true;
	private PXCMSizeI32 depthSize;
	private PXCMSizeI32 colorSize;
	private bool rotate=false;

	void OnEnable(){
		//Initializing stuff
		
		depthSize.width=depthSize.height=0;
		colorSize.width=colorSize.height=0;
		rotate=false;
		
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
			//Debug.Log("ERROR , square_per_line cannot be 0");
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
		input = GameObject.Find("RealSense Input Manager").GetComponent<SenseInput>();
		input.OnHandData+=OnHandData;		
		input.OnDepthImage+=OnDepthImage;
		input.OnColorImage+=OnColorImage;
		input.OnFaceData+=OnFaceData;
	}
		
	
	// Update is called once per frame
	void Update () {
		if(squares_per_line == 0){
			return;
		}
		
		//Moves every tile a bit forward every frame
		for(int i = 0 ; i < list.Count ; i++){
			if(list[i].transform.position.z > GameObject.Find("Camera").transform.position.z){
				Rigidbody temp = list[i].GetComponent<Rigidbody>();
				temp.position = new Vector3(temp.position.x , temp.position.y , temp.position.z - movingWallSpeed);
			}
		}
		
		//Allowes the user to toggle the guildlines by pressing p
		if(Input.GetKeyDown(KeyCode.P)){
			//TODO : Will toggle the guildlines
		}
		
		// Tradditional controls
		
		//Moves the player according to keyboard input
		/* float speed = squareSize;		
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
		}*/
		
		
		
		
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
	
	
	
	/*
	
	8888888b.                    888  .d8888b.                                           .d8888b.                    888                     888 888          
	888   Y88b                   888 d88P  Y88b                                         d88P  Y88b                   888                     888 888          
	888    888                   888 Y88b.                                              888    888                   888                     888 888          
	888   d88P  .d88b.   8888b.  888  "Y888b.    .d88b.  88888b.  .d8888b   .d88b.      888         .d88b.  88888b.  888888 888d888  .d88b.  888 888 .d8888b  
	8888888P"  d8P  Y8b     "88b 888     "Y88b. d8P  Y8b 888 "88b 88K      d8P  Y8b     888        d88""88b 888 "88b 888    888P"   d88""88b 888 888 88K      
	888 T88b   88888888 .d888888 888       "888 88888888 888  888 "Y8888b. 88888888     888    888 888  888 888  888 888    888     888  888 888 888 "Y8888b. 
	888  T88b  Y8b.     888  888 888 Y88b  d88P Y8b.     888  888      X88 Y8b.         Y88b  d88P Y88..88P 888  888 Y88b.  888     Y88..88P 888 888      X88 
	888   T88b  "Y8888  "Y888888 888  "Y8888P"   "Y8888  888  888  88888P'  "Y8888       "Y8888P"   "Y88P"  888  888  "Y888 888      "Y88P"  888 888  88888P' 
	
	*/
	
	void OnFaceData(PXCMFaceData.LandmarksData data){
		
	}
	
	void OnColorImage(PXCMImage image) {
		colorSize.width=image.info.width;
		colorSize.height=image.info.height;
	}
	
	void OnDepthImage(PXCMImage image) {
		//Debug.Log("Depth");
		/* Save depth size for later use */
		depthSize.width=image.info.width;
		depthSize.height=image.info.height;
		if (!visualizeImage) return;
		if (!rotate) return;

		if (depthImage==null) {
			/* If not allocated, allocate Texture2D */
			depthSize.width=image.info.width;
			depthSize.height=image.info.height;
			depthImage=new Texture2D((int)depthSize.width, (int)depthSize.height, TextureFormat.ARGB32, false);
			
			/* Associate the Texture2D with the cube */
			GetComponent<Renderer>().material.mainTexture=depthImage;
			GetComponent<Renderer>().material.mainTextureScale=new Vector2(-1,-1);
		}
		
		/* Retrieve the image data in Texture2D */
		PXCMImage.ImageData data;
		image.AcquireAccess(PXCMImage.Access.ACCESS_READ,PXCMImage.PixelFormat.PIXEL_FORMAT_RGB32,out data);
		data.ToTexture2D(0, depthImage);
		image.ReleaseAccess(data);
		
		/* Display on the Cube */
		depthImage.Apply();
	}
		

	int framesClosed = 0;
	int lastLoc = 0;
	void OnHandData(PXCMHandData.IHand ihand) {
		if (!visualizeHand){ 
			return;
		}
		
		if(ihand == null){
			return;
		}
		
		if (depthSize.width==0 || depthSize.height==0){
			return;
		}
        
		//Hand location
		PXCMPointF32 xy = ihand.QueryMassCenterImage();
		xy.x=xy.x/depthSize.width;
		xy.y=xy.y/depthSize.height;
		float xLoc = (1-xy.x);
		float yLoc = (xy.y);
		
		int openess = ihand.QueryOpenness();
		if(GameObject.Find("Start Menu").activeSelf){
			bool isRight = xLoc >= 0.5;
			bool isLeft = xLoc < 0.5;
			int loc = 0;
			Debug.Log(framesClosed + ":" + openess);
			
			
			/* Loc :
			0 - None
			1 - Right
			2 - Left */
			
			if(isRight){
				loc = 1;
			} else if (isLeft){
				loc = 2;
			} else {
				loc = 0;
			}
			
			
			bool isClosed = openess < 40;
			
			if(isClosed && loc == lastLoc){
				framesClosed++;
			} else {
				framesClosed = 0;
				GameObject.Find("Start Menu/Canvas/Button").GetComponent<Image>().color = Color.white;
				GameObject.Find("Start Menu/Canvas/Button (1)").GetComponent<Image>().color = Color.white;
			}
			
			if(framesClosed >= requiredFramesClosed){
				if(loc == 1){
					GameObject.Find("RealGameMaster").GetComponent<GameMaster>().OnStartClick();
					//Debug.Log("Start");
					GameObject.Find("Start Menu/Canvas/Button").GetComponent<Image>().color = Color.green;
				} else if (loc == 2){
					GameObject.Find("RealGameMaster").GetComponent<GameMaster>().OnExit();
					//Debug.Log("Exit");
					GameObject.Find("Start Menu/Canvas/Button (1)").GetComponent<Image>().color = Color.green;
				} else {
					Debug.Log("ERROR , LOC 0");
				}
			}
			
			lastLoc = loc;
		}
		
		
		//Location stuff
		float xRange = Math.Abs(GameObject.Find("Walls/Plane (1)").transform.position.x - GameObject.Find("Walls/Plane (2)").transform.position.x);
		float yRange = Math.Abs(GameObject.Find("Walls/Plane (4)").transform.position.y - GameObject.Find("Floor").transform.position.y);
		Vector3 newLoc = new Vector3(GameObject.Find("Walls/Plane (2)").transform.position.x + ((xRange * xLoc)) , GameObject.Find("Plane (4)").transform.position.y - ((yRange * yLoc)) , player.transform.position.z);
		newLoc = new Vector3(newLoc.x - newLoc.x % (xRange / squares_per_line) , newLoc.y - newLoc.y % (yRange / squares_per_line) + squareSize/2, newLoc.z);
		player.transform.position = newLoc;
	}
	
}