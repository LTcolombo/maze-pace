﻿using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Cloud.Analytics;
using AssemblyCSharp;

public class MenuController : MonoBehaviour
{

	private bool _canExit;
	
	//writing matrix
	private static int[,] LOGO_MATRIX = new int[,] {
		{0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0},
		{0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0},
		{0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 0},
		{0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0},
		{0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0},
		{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
		{1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1},
		{1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0},
		{1, 1, 0, 0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0},
		{1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0},
		{1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 1, 1}
	};


	// Use this for initialization
	void Start ()
	{
		Prefabs.Init ();
		UnityAnalytics.StartSDK ("84ec8035-1fc7-4fde-867c-3497cb4b2ace");
	
		Text bestScoreText = (Text)GameObject.Find ("Canvas/BestScoreText").GetComponent<Text> ();
		bestScoreText.text = "BEST SCORE: " + PlayerPrefs.GetInt ("highscore", 0);
	
		Camera.main.backgroundColor = new Color (0.4f, 0.4f, 0.4f);
		InitScene ();
	}
	
	void InitScene ()
	{
		_canExit = false;
	
		DOTween.CompleteAll ();
		Camera.main.DOColor (new Color (0.0f, 0.0f, 0.0f), 0.7f).OnComplete (AllowExit);
				
		//set of base colors
		ColorComponent[] colorComponents = ColorComponent.GetArray ();
		
		for (int cell_y = 0; cell_y < LOGO_MATRIX.GetLength(0); cell_y++)
			for (int cell_x = 0; cell_x< LOGO_MATRIX.GetLength(1); cell_x++) {
			
				if (LOGO_MATRIX [cell_y, cell_x] != 0) {
					int x = (-LOGO_MATRIX.GetLength (1) / 2 + cell_x) * 34;
					int y = (LOGO_MATRIX.GetLength (0) - cell_y) * 34; 
				
					float[] tileRelativePos = new float[2] {
					(float)cell_y / LOGO_MATRIX.GetLength (0),
					(float)cell_x / LOGO_MATRIX.GetLength (1)
					};
								
					float tint = 0.3f + 0.7f * (Random.Range (3.0f, 5.0f) / 5.0f);
				
					Vector3 pos = new Vector3 (x, y, 0);
					
					if (cell_x % 2 == 0)
						pos.x += Random.Range (-400, 400);
					else
						pos.y += Random.Range (-400, 400);
					
					GameObject tileInstance = (GameObject)Instantiate (Prefabs.TILE_SMALL, pos, Quaternion.identity);
					tileInstance.GetComponent<SpriteRenderer> ().color = ColorComponent.GetColorAt(tileRelativePos, colorComponents, tint);
				
					tileInstance.transform.DOMove (new Vector3 (x, y, 0), 0.5f);
				}	
			}
		
	}
	
	void AllowExit ()
	{
		_canExit = true;
	}
	
	// Update is called once per frame
	void Update ()
	{	
		if (Application.isEditor && Input.anyKey && _canExit)
			ExitToGame();
		
		if (Input.touchCount > 0) {
			bool start = true;
			for (int i = 0; i < Input.touchCount; i++) {
				if (Input.GetTouch (i).phase != TouchPhase.Ended) 
					start = false;
			}
			
			if (start && _canExit) 
				ExitToGame();
		}		
	}

	void ExitToGame() {		
		Text tipText = (Text)GameObject.Find ("Canvas/TipText").GetComponent<Text> ();
		tipText.text = "Loading...";
		_canExit = false;
		Application.LoadLevel ("GameScene");
	}
}