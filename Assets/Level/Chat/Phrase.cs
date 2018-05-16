using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Phrase", menuName = "Chat/Phrase")]
public class Phrase : ScriptableObject {
	public Position position;
	public Sprite image;
	public int id;
	public bool inverted;
	public string text;
}
public enum Position { Left, Right }
