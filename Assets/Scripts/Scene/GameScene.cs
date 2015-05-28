﻿using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class GameScene : MonoBehaviour
{
	
	//container for maze and player
	private GameObject _container;
	private GameObject _maze;
	private GameObject _player;
	
	//textfields for game state UI
	private Text scoreText;
	private Text movesText;
		
	//view scripts
	private MazeView _mazeView;
	private PlayerView _playerView;
		
	//current maze data
	private MazeData _mazeData;
	
	//current game state
	private int _score;
	private uint _movesLeft;
	private uint _movesLeftCritical;
	private bool _activated;
	private bool _stuck;
	
	//score to add after previous interation, depending on bonus moves
	private uint _increaseValue;

	//scores to to takeoff on ueach update when stuck. approx should drain all score in 1 second
	private int _reduceValue;
	
	// Use this for initialization
	void Start ()
	{
		DOTween.Init (false, true, LogBehaviour.ErrorsOnly);
				
		_container = GameObject.Find ("GameContainer");
		
		var mazeObject = (GameObject)Instantiate (Resources.Load ("Prefabs/Maze"));
		mazeObject.transform.parent = _container.transform;
		_mazeView = mazeObject.GetComponent<MazeView> ();
				
		var playerObject = (GameObject)Instantiate (Resources.Load ("Prefabs/Player"));
		playerObject.transform.parent = _container.transform;
		_playerView = playerObject.GetComponent<PlayerView> ();
		_playerView.onStepComplete += OnPlayerStepComplete;
				
		scoreText = (Text)GameObject.Find ("Canvas/ScoreText").GetComponent<Text> ();
		movesText = (Text)GameObject.Find ("Canvas/MovesText").GetComponent<Text> ();
				
		_movesLeft = 0;
		_score = 0;
		Next ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_stuck) {
			scoreText.color = new Color (1.0f, 0.0f, 0.0f);
			if (_score < _reduceValue)
				Application.LoadLevel ("MenuScene");
			else {
				_score -= _reduceValue;
				scoreText.text = "SCORE: " + _score;
			}
		} else
			if (_increaseValue == 0)
			scoreText.color = new Color (0.761f, 0.761f, 0.668f);
		else {
			if (_increaseValue > 0) {
				scoreText.color = new Color (0.0f, 0.8f, 0.0f);

				if (_increaseValue > _movesLeft)
					_increaseValue = _movesLeft;
			} else {
				scoreText.color = new Color (1.0f, 0.0f, 0.0f);	
				
				if (_increaseValue < _movesLeft)
					_increaseValue = _movesLeft;
			}
			
			_score += (int)_increaseValue;
			_movesLeft -= _increaseValue;
			scoreText.text = "SCORE: " + _score;
			movesText.text = "MOVES: " + _movesLeft;
		}
	}
	
	private void OnPlayerStepComplete ()
	{
		if (!_activated)
			Activate ();
	
		NodeData node = _mazeData.GetNode (_playerView.cellX, _playerView.cellY);
		_score += node.score;
		node.score = 0;
		
		scoreText.text = "SCORE: " + _score;
		movesText.text = "MOVES: " + _movesLeft;
		
		if (node.HasFlag (NodeData.SPECIALS_EXIT)) {
			movesText.color = new Color (0.761f, 0.761f, 0.668f);
			Next ();
			return;
		}
		
		_movesLeft--;
		if (_movesLeft == 0) {
			Application.LoadLevel ("MenuScene");
			return;
		} 
		
		if (_movesLeft < _movesLeftCritical)
			movesText.color = new Color (1.0f, 0.0f, 0.0f);
		
		if (!node.HasWall (_playerView.directionIdx)) {
			_mazeView.DesaturateTileAt (_playerView.cellX, _playerView.cellY);
			_playerView.Next ();
			//_mazeView.ShowObjects (true);
			_stuck = false;
		} else {
			//_mazeView.ShowObjects (false);
			_stuck = true;
			
			_reduceValue = (int)((float)_score * Time.deltaTime);
			if (_reduceValue < 1)
				_reduceValue = 1;
		}
	}
		
	private void Next ()
	{
		_activated = false;
	
		if (_movesLeft > 0) {
			_increaseValue = (uint)((float)_movesLeft * Time.deltaTime * 2);
			if (_increaseValue == 0)
				_increaseValue = 1;
		} else
			_increaseValue = 0;
	
		_mazeData = new MazeData(getNextMazeConfig(), _playerView.cellX, _playerView.cellY);
		
		ScoreDecorator.Apply(_mazeData);
		//SpeedUpDecorator.Apply(_mazeData);
		RotatorDecorator.Apply(_mazeData);
		
		_container.transform.position = new Vector2 (
			-(_mazeData.config.width - 1) * MazeView.NODE_SIZE / 2, 
			-(_mazeData.config.height - 1) * MazeView.NODE_SIZE / 2
		);
		_mazeView.UpdateMazeData (_mazeData);
		
		//_playerView.InvokeAutostartIn (1);
	}

//move to model
	MazeConfig getNextMazeConfig ()
	{
		MazeConfig config = new MazeConfig();
		config.width = 8;
		config.height = 8;
		
		config.minScore = 1;
		config.maxScore = 4;
		
		config.speedUpsCount = 2;
		config.rotatorsCount = 6;
		
		return config;
	}
	
	private void Activate ()
	{
		_increaseValue = 0;
		_score += (int)_movesLeft;
		_movesLeft = _mazeData.movesQuota;
		_movesLeftCritical = _movesLeft/10;
		_activated = true;
	}
}
