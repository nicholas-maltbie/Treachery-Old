using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MessageBoard : NetworkBehaviour {

	public class MessageList : SyncListStruct<PlayerMessage>{}
	private MessageList messageList = new MessageList();

	public struct PlayerMessage
	{
		public float percent;
		public string message;
		public bool timed;
		public float fullTime;
		public float elapsed;
		public PlayerMessage(string message, float percent, bool timed = false, float fullTime = 0f) {
			this.percent = percent;
			this.message = message;
			this.timed = timed;
			this.fullTime = fullTime;
			this.elapsed = 0f;
		}
	}

	public PlayerMessage AddPercentMessage(string message, float percent) {
		PlayerMessage m = new PlayerMessage (message, percent);
		AddMessage (m);
		return m;
	}

	private void AddMessage(PlayerMessage message) {
		messageList.Add (message);
	}

	public PlayerMessage AddTimedMessage(string message, float maxtime) {
		PlayerMessage m = new PlayerMessage (message, 1, true, maxtime);
		AddMessage (m);
		return m;
	}

	public void RemoveMessage(PlayerMessage message) {
		messageList.Remove (message);
	}

	void OnGUI() {
		int message = 0;
		foreach (PlayerMessage playerM in messageList) {
			DisplayMessage (playerM.message, playerM.percent, message);
			message += 1;
		}
	}

	private void DisplayMessage(string message, float percent, int pos) {
		if (percent <= .10f)
			percent = .10f;
		Vector2 textSize = GUI.skin.label.CalcSize (new GUIContent (message));
		float width = textSize.x;
		float height = textSize.y;
		Color oldColor = Color.white;
		GUI.color = Color.green;
		GUI.Box (new Rect (Screen.width / 2 - width * 0.55f, Screen.height - height * (2 + pos * 1.25f),
			width * 1.1f * percent, height * 1f), "");
		GUI.color = Color.white;
		GUI.contentColor = Color.white;
		GUI.Label (new Rect (Screen.width / 2 - width / 2, Screen.height - height * (2 + pos * 1.25f),
			width, height), message);
	}
	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer) {
			print ("updating");
			for (int i = 0; i < messageList.Count; i++) {
				PlayerMessage m = messageList [i];
				if (m.timed) {
					m.elapsed += Time.deltaTime;
					m.percent = 1 - (m.elapsed / m.fullTime);
				}
				messageList [i] = m;
			}
			int index = 0;
			while (index < messageList.Count) {
				if (messageList [index].timed && messageList [index].elapsed >= messageList [index].fullTime) {
					messageList.RemoveAt (index);
				} else {
					index++;
				}
			}
		}
	}
}
