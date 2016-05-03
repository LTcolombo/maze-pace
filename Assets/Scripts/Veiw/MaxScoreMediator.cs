using UnityEngine;
using UnityEngine.UI;
using Model;
using Notifications;

namespace View {
	public class MaxScoreMediator : MonoBehaviour {
		
		public string prefix = "MAX SCORE: ";
		public string format = "F0";
		
		Text _target;
		int _previousValue;
		
		void Start(){
			_target = GetComponent<Text> ();
			_previousValue = 0;
			MazePaceNotifications.GAME_UPDATED.Add(OnGameStateUpdated);
		}
		
		// Update is called once per frame
		void OnGameStateUpdated () {
			GameModel state = GameModel.Instance ();
			if (_previousValue == state.maxScore)
				return;
				
			_previousValue = state.maxScore;
			_target.text = prefix + state.maxScore.ToString (format);
		}
		
		void OnDestroy(){
			MazePaceNotifications.GAME_UPDATED.Remove(OnGameStateUpdated);
		}
	}
}
