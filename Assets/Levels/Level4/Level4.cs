using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level4 : Level {
	public List<Phrase> startingDialog;

	protected override void Start2 ()
	{
		base.Start2 ();
		chat.Init (startingDialog);
	}
}
