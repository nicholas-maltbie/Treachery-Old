using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

/**
 * Character foot sounds, sounds when the charactr's foot does stuff.
 * Supported events - 
 * 	Lift - When the player jumps
 *  Land - When the player lands
 *  FootDown - Whenever a foot hits the ground
 * 
 * Each of these sounds can have a different sound based on the material.
 */
public class FootSounds : MonoBehaviour {
	
	public TextAsset keyFile;
	/**
	 * Dctionary formatted as follows:
	 * Material Name -> Action Name -> Audio Clip
	 */
	private Dictionary<string, Dictionary<string, List<AudioClip>>> materialSounds = 
		new Dictionary<string, Dictionary<string, List<AudioClip>>>();

	void Start() {
		XmlDocument footstepKey = new XmlDocument ();
		//Load XML document
		try { 
			footstepKey.LoadXml(keyFile.text); 
			XmlNode footNode = footstepKey.SelectSingleNode("FootSound");
			//Get footstep folder
			string mainFolder = (string) footNode.Attributes["directory"].Value;

			//Loop over materials
			foreach (XmlNode mat in footNode.SelectNodes("Material")) {
				//Get material folder and name
				string matName = (string) mat.Attributes["materialName"].Value;
				string materialFolder = (string) mat.Attributes["directory"].Value;
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
						string path = mainFolder + "/" + materialFolder + "/" + fileName;
						AudioClip loaded = Resources.Load<AudioClip>(path);
						actionSounds.Add(loaded);
					}
					//Save loaded clips
					matSounds.Add(actionName, actionSounds);
				}
				//Save loaded sounds
				materialSounds.Add(matName, matSounds);
			}
		}
		catch (System.IO.FileNotFoundException) {
			Debug.Log ("Could not find xml key document");
		}
		
	}

	void Lift() {

	}

	void Land() {

	}

	public void FootDown(Vector3 footPosition) {

	}
}
