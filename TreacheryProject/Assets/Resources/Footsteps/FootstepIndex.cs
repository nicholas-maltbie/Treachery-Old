using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class FootstepIndex : MonoBehaviour {

	public Dictionary<string, Dictionary<string, List<AudioClip>>> materialSounds = 
		new Dictionary<string, Dictionary<string, List<AudioClip>>>();
	public AudioClip[] clips;
	public string defaultMat;
	public TextAsset keyFile;

	// Use this for initialization
	void Start () {

		Dictionary<string, AudioClip> clipIndex = new Dictionary<string, AudioClip>(); 
		foreach (AudioClip c in clips) {
			clipIndex [c.name] = c;
		}

		XmlDocument footstepKey = new XmlDocument ();
		//Load XML document
		footstepKey.LoadXml(keyFile.text); 
		XmlNode footNode = footstepKey.SelectSingleNode("FootSound");

		//Loop over materials
		foreach (XmlNode mat in footNode.SelectNodes("Material")) {
			//Get material folder and name
			string matName = (string) mat.Attributes["materialName"].Value;
			//Initialize dictionary
			Dictionary<string, List<AudioClip>> matSounds = new Dictionary<string, List<AudioClip>>();

			//Lop over actions
			foreach (XmlNode action in mat.SelectNodes("Action")) {
				string actionName = (string)action.Attributes["actionName"].Value;
				//Load all clips for individual actions
				List<AudioClip> actionSounds = new List<AudioClip>();

				//Loop over clilps
				foreach(XmlNode clip in action.SelectNodes("Clip")) {
					//Get and load audio clip
					string fileName = clip.InnerText;
					AudioClip loaded = clipIndex[fileName];
					actionSounds.Add(loaded);
				}
				//Save loaded clips
				matSounds.Add(actionName, actionSounds);
			}
			//Save loaded sounds
			materialSounds.Add(matName, matSounds);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
