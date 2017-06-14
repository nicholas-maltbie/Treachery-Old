using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
/*
public class Player : NetworkBehaviour {

	public enum PlayerType {Explorer, Tratior, Hero, Madman};

	public static GUIStyle PLAYER_STYLE = new GUIStyle();
	private static int FONT_SIZE = -1;

	private MultiplayerCharacterController characterController;
	
	public bool hasPercentMessage, isShowingText, coolingDown;
	private float showTime = 0;
	[SyncVar]
	public string percentText = "", cooldownText;
	public const int NUMBER_STATS = 4, NUMBER_ITEMS = 5;
	public static int SKILL_WIDTH = Screen.width / 8, SKILL_HEIGHT = Screen.height / 12, NUMBER_WIDTH = Screen.width/15, NUMBER_HEIGHT = Screen.height / 12;
	public enum Skill {STRENGTH, DEXTERITY, INTELLIGENCE, SANITY};

	public Texture notSelectedBox, selectedBox;
	public Texture[] numbers = new Texture[8], skills = new Texture[4];
	public GameObject backpack, hand;
	public float timeOut = 3.5f, cooldown, cooldownProgress;
	[SyncVar]
	public float percent;
	[SyncVar]
	public float dexterity, strength, intelligence, sanity;
	private float baseDex, baseStr, baseInt, baseSan;
	[SyncVar]
	private int selected;
	private int lastSelected;
	private Item[] items = new Item[NUMBER_ITEMS];
	[SyncVar]
	private GameObject serverHeld;
	private GameObject held;
	[SyncVar]
	public GameObject interuptableAction;

	private PlayerType type = PlayerType.Explorer;

	public bool CanBeInterupted()
	{
		return interuptableAction != null;
	}

	public void InteruptAction()
	{
		interuptableAction.GetComponent<InteruptableAction> ().InteruptAction ();
	}

	[ClientRpc]
	public void RpcSetPercentComplete(float percent)
	{
		this.percent = percent;
	}

	[ClientRpc]
	public void RpcSetPercentText(string text)
	{
		percentText = text;
	}

	[ClientRpc]
	public void RpcSetPercentMessage(bool display)
	{
		hasPercentMessage = display;
	}

	public PlayerType GetPlayerType()
	{
		return type;
	}

	public void SetPlayerType(PlayerType playerType)
	{
		type = playerType;
	}

	public int GetSkillDisplay(Skill skill)
	{
		int value = (int)CheckSkill (skill);
		if (value <= 0)
			value = 1;
		if (value > 8)
			value = 8;
		return value - 1;
	}

	public void viewItem()
	{
		showTime = 0;
		isShowingText = true;
	}

	public float CheckSkill(Skill skill)
	{
		switch(skill)
		{
		case Skill.STRENGTH:
			return strength;
		case Skill.DEXTERITY:
			return dexterity;
		case Skill.INTELLIGENCE:
			return intelligence;
		case Skill.SANITY:
			return sanity;
		}
		return 0;
	}

	[ClientRpc]
	public void RpcSetActionCooldown(string message, float time)
	{
		SetActionCooldown(message, time);
	}

	public void SetActionCooldown(string message, float time)
	{
		if (!hasPercentMessage) {
			hasPercentMessage = true;
		}
		characterController.setInteract (false);
		characterController.setExamine (false);
		cooldown = time;
		cooldownText = message;
		cooldownProgress = 0;
		coolingDown = true;
		percent = 0;
		percentText = message + "; cooldown: " + ((int)time * 10.0 / 10) + " sec";
	}

	[ServerCallback]
	public void AdjustSkill(Skill skill, float value)
	{
		switch(skill)
		{
		case Skill.STRENGTH:
			strength += value;
			break;
		case Skill.DEXTERITY:
			dexterity += value;
			break;
		case Skill.INTELLIGENCE:
			intelligence += value;
			break;
		case Skill.SANITY:
			sanity += value;
			break;
		}
	}
	
	[Command]
	public void CmdAdjustSkill(Skill skill, float value)
	{
		AdjustSkill(skill, value);
	}

	void OnGUI()
	{
		if (isLocalPlayer) {
			float boxDims = Screen.width*.4f/items.Length;
			float iconDims = boxDims * .8f;

			if (FONT_SIZE == -1) {
				FONT_SIZE = 12;
				while(PLAYER_STYLE.CalcHeight(new GUIContent("TestString"), 1000) < boxDims * .2f)
				{
					PLAYER_STYLE.fontSize = FONT_SIZE++;
				}
				PLAYER_STYLE.fontSize = FONT_SIZE--;
				PLAYER_STYLE.normal.textColor = Color.white;
				PLAYER_STYLE.wordWrap = true;
			}

			for(int i = 0; i < items.Length; i++)
			{
				if(items[i] != null)
				{
					GUI.DrawTexture(new Rect(Screen.width/2  - boxDims/2 + boxDims * (items.Length/2-i) + boxDims *.1f, Screen.height - boxDims + boxDims *.1f,
					                         iconDims, iconDims), items[i].icon);
				}
				Texture box = notSelectedBox;
				if(i == selected)
					box = selectedBox;
				GUI.DrawTexture(new Rect(Screen.width/2  - boxDims/2 + boxDims * (items.Length/2-i), Screen.height - boxDims, boxDims, boxDims), box);
			}
			for(int i = 0; i < NUMBER_STATS; i++)
			{
				GUI.DrawTexture(new Rect(10, SKILL_HEIGHT * i + 10 * (i+1), SKILL_WIDTH, SKILL_HEIGHT), skills[i]);
				GUI.DrawTexture(new Rect(20 + SKILL_WIDTH, SKILL_HEIGHT * i + 10 * (i+1), NUMBER_WIDTH, NUMBER_HEIGHT), numbers[GetSkillDisplay((Skill) i)]);
			}
			if(held != null && isShowingText)
			{
				Color oldColor = GUI.contentColor;
				GUI.color = Color.white;

				GUI.DrawTexture(new Rect(Screen.width/2 - boxDims/2 - boxDims * (items.Length/2 + 2) + boxDims * .1f, Screen.height - boxDims * 2.1f,
				                         boxDims, boxDims), held.GetComponent<Item>().icon);
				GUI.Label(new Rect(Screen.width / 2 - boxDims/2 - boxDims * (items.Length/2 + 2) + boxDims * .2f, Screen.height - boxDims * 2.6f,
				                   boxDims * (3), boxDims * .8f), held.GetComponent<Item>().name, PLAYER_STYLE);
				GUI.Label(new Rect(Screen.width / 2 - boxDims/2 - boxDims * (items.Length/2) + boxDims * .2f, Screen.height - boxDims * 2.6f,
				                   boxDims * (items.Length + 1), boxDims * .8f), held.GetComponent<Item>().flavorText, PLAYER_STYLE);
				GUI.Label(new Rect(Screen.width / 2 - boxDims/2 - boxDims * (items.Length/2 + 1) + boxDims * .2f, Screen.height - boxDims * 2.1f,
				                   boxDims * (items.Length + 2), boxDims * 1.2f), held.GetComponent<Item>().description, PLAYER_STYLE);
				
				GUI.color = oldColor;
			}
			
			if (hasPercentMessage) {
				float heightMod = 2.9f;
				if(!isShowingText)
				{
					heightMod = 1.4f;
				}
				float width = Player.PLAYER_STYLE.CalcSize(new GUIContent(percentText)).x;
				Color oldColor = Color.white;
				GUI.color = Color.green;
				GUI.Box(new Rect (Screen.width / 2 - width * .55f, Screen.height - boxDims * heightMod,
				                  width * 1.1f * percent/100, boxDims * .3f), "");
				GUI.color = Color.white;
				GUI.contentColor = Color.white;
				GUI.Label (new Rect (Screen.width / 2 - width/2, Screen.height - boxDims * heightMod,
				                     width, boxDims * .4f), percentText, Player.PLAYER_STYLE);
			}
		}
	}

	[ClientRpc]
	public void RpcDropItem(GameObject item)
	{
		DropItem (item);
	}

	public void DropItem(GameObject item)
	{
		item.GetComponent<Item>().DropItem (gameObject);
		item.transform.position = hand.transform.position;
		item.transform.parent = null;
		for (int i = 0; i < items.Length; i++) {
			if(items[i] == item.GetComponent<Item>())
			{
				items[i] = null;
				if (i == selected) {
					held = null;
					HoldItem(null);
				}
			}
		}
	}

	public void HoldItem(GameObject item)
	{
		if (held != null) {
			held.transform.position = backpack.transform.position;
			held.transform.eulerAngles = gameObject.transform.eulerAngles;
			held.SendMessage("PutInBag");
		}
		if (item != null) {
			held = item;
			held.transform.position = hand.transform.position;
			held.transform.eulerAngles = hand.transform.eulerAngles;
			held.SendMessage ("HoldItem");
			if (isLocalPlayer) {
				showTime = 0;
				isShowingText = true;
			}
		} else {
			isShowingText = false;
		}
		held = item;
	}

	[ClientRpc]
	public void RpcHoldItem(GameObject item)
	{
		HoldItem (item);
	}

	[Command]
	public void CmdHoldItem(int index)
	{
		if (items [index] != null)
			serverHeld = items [index].gameObject;
		else
			serverHeld = null;
		RpcHoldItem (serverHeld);
	}

	[Command]
	public void CmdDropItem(int index)
	{
		if (selected == index) {
			serverHeld = null;
		}
		RpcDropItem(items[index].gameObject);
	}

	public void PickupItem(GameObject item)
	{
		PickupItem (item, -1);
	}

	public void PickupItem(GameObject item, int index)
	{
		if (index == -1)
			index = AddItem (item.GetComponent<Item> ());
		else
			items [index] = item.GetComponent<Item>();
		if (isLocalPlayer && index == selected) {
			CmdHoldItem(index);
		}
		item.transform.position = backpack.transform.position;
		item.transform.eulerAngles = gameObject.transform.eulerAngles;
		item.transform.parent = gameObject.transform;
	}

	[ClientRpc]
	public void RpcPickupItem(GameObject item)
	{
		PickupItem (item);
	}

	public int AddItem(Item item)
	{
		for(int i = items.Length - 1; i >= 0; i--)
		{
			if(items[i] == null)
			{
				items[i] = item;
				return i;
			}
		}
		return -1;
	}

	[Command]
	public void CmdSetSelected(int selected)
	{
		this.selected = selected;
	}

	public bool HasOpenSpace()
	{
		foreach (Item i in items) {
			if(i == null)
				return true;
		}
		return false;
	}

	// Use this for initialization
	void Start () {
		selected = items.Length - 1;
		lastSelected = selected;
		characterController = GetComponent<MultiplayerCharacterController> ();
		baseSan = sanity;
		baseDex = dexterity;
		baseInt = intelligence;
		baseStr = strength;
	}

	public bool HasItem(Item item)
	{
		for (int i = 0; i < items.Length; i++) {
			if(items[i] == item)
				return true;
		}
		return false;
	}

	[ClientCallback]
	void Update () {
		if (isLocalPlayer) {

			if(coolingDown)
			{
				cooldownProgress += Time.deltaTime;
				coolingDown = true;
				percent = cooldownProgress / cooldown * 100;
				percentText = cooldownText + "; cooldown: " + ((int)((cooldown - cooldownProgress) * 10))/10.0f + " sec";
				if(cooldownProgress >= cooldown)
				{
					coolingDown = false;
					hasPercentMessage = false;
					characterController.setInteract (true);
					characterController.setExamine (true);
				}
			}

			if(isShowingText)
			{
				showTime += Time.deltaTime;
				if(showTime > timeOut)
					isShowingText = false;
			}
			float selectChange = Input.GetAxis ("Mouse ScrollWheel");
			if (selectChange > 0) {
				selected = (selected + 1) % items.Length;
			} else if (selectChange < 0) {
				selected = (selected + items.Length - 1) % items.Length;
			}

			for(int number = 1; number <= items.Length; number++)
			{
				if(Input.GetKeyDown("" + number))
					selected = items.Length - number;
			}

			if(lastSelected != selected)
			{
				CmdHoldItem(selected);
			}
			lastSelected = selected;

			CmdSetSelected(selected);

			if(Input.GetButtonDown("Drop"))
			{
				if (items [selected] != null) {
					CmdDropItem(selected);
				}
			}

			if(Input.GetButtonDown("Examine") && characterController.isAllowedToExamine())
			{
				if (items [selected] != null && items[selected].GetComponent<Examinable>() != null) {
					items[selected].GetComponent<Examinable>().Examine();
				}
			}
		}

		if (!isServer && held != serverHeld) {
			HoldItem(serverHeld);
			held = serverHeld;
		}

		for(int i = 0; i < items.Length; i++)
		{
			if(items[i] != null)
			{
				if(held != null && items[i] == held.GetComponent<Item>())
					items[i].gameObject.transform.position = hand.transform.position;
				else
					items[i].gameObject.transform.position = backpack.transform.position;
			}
		}
	}
}
*/