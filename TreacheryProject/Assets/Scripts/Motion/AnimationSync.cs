using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Synca an animtor across a network.
/// </summary>
public class AnimationSync : NetworkBehaviour {

	public Animator animatorSync;

	private AnimatorControllerParameter[] parameters;
	private Dictionary<int, int> intPrevValues = new Dictionary<int, int>();
	private Dictionary<int, bool> boolPrevValues = new Dictionary<int, bool>();
	private Dictionary<int, float> floatPrevValues = new Dictionary<int, float>();

	void Start() {
		int count = animatorSync.parameterCount;
		parameters = new AnimatorControllerParameter[count];
		for (int i = 0; i < count; i++) {
			parameters [i] = animatorSync.GetParameter (i);
			if (parameters [i].type == AnimatorControllerParameterType.Int) {
				intPrevValues [i] = animatorSync.GetInteger (parameters [i].name);
			}
			else if (parameters [i].type == AnimatorControllerParameterType.Bool) {
				boolPrevValues [i] = animatorSync.GetBool (parameters [i].name);
			}
			else if (parameters [i].type == AnimatorControllerParameterType.Float) {
				floatPrevValues [i] = animatorSync.GetFloat (parameters [i].name);
			}
		}
	}

	[Command]
	void CmdSetAnimatorInt(string paramName, int value) {
		RpcSetAnimatorInt (paramName, value);
	}

	[ClientRpc]
	void RpcSetAnimatorInt(string paramName, int value) {
		if (!isLocalPlayer) {
			animatorSync.SetInteger (paramName, value);
		}
	}

	[Command]
	void CmdSetAnimatorBool(string paramName, bool value) {
		RpcSetAnimatorBool (paramName, value);
	}

	[ClientRpc]
	void RpcSetAnimatorBool(string paramName, bool value) {
		if (!isLocalPlayer) {
			animatorSync.SetBool (paramName, value);
		}
	}

	[Command]
	void CmdSetAnimatorFloat(string paramName, float value) {
		RpcSetAnimatorFloat (paramName, value);
	}

	[ClientRpc]
	void RpcSetAnimatorFloat(string paramName, float value) {
		if (!isLocalPlayer) {
			animatorSync.SetFloat (paramName, value);
		}
	}

	// Update is called once per frame
	void Update () {
		if (isLocalPlayer) {
			for (int i = 0; i < parameters.Length; i++) {
				AnimatorControllerParameterType type = animatorSync.GetParameter (i).type;
				if (type == AnimatorControllerParameterType.Int) {
					int value = animatorSync.GetInteger (parameters [i].name);
					if (intPrevValues [i] != value) {
						CmdSetAnimatorInt (parameters [i].name, value);
						intPrevValues [i] = value;
					}
				} else if (type == AnimatorControllerParameterType.Bool) {
					bool value = animatorSync.GetBool (parameters [i].name);
					if (boolPrevValues[i] != value) {
						CmdSetAnimatorBool (parameters [i].name, value);
						boolPrevValues [i] = value;
					}
				} else if (type == AnimatorControllerParameterType.Float) {
					float value = animatorSync.GetFloat (parameters [i].name);
					if (floatPrevValues[i] != value) {
						CmdSetAnimatorFloat (parameters [i].name, value);
						floatPrevValues [i] = value;
					}
				}


			}
		}
	}
}
