using UnityEngine;

[CreateAssetMenu(fileName = "Phrase", menuName = "Custom/Phrase")]
public class Phrase : ScriptableObject {
	public Position position;
	public Sprite image;
	public int id;
	public bool inverted;

	public Translation[] translations = new Translation[] { new Translation("English", ""), new Translation("Español","") };
}
public enum Position { Left, Right }
