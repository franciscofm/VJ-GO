using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : Level {
	[Header("Tutorial")]
	public List<Phrase> StartingDialog;
	List<Phrase> MidleDialog;
	List<Phrase> FinishDialog;

	protected override void Start2 () {
		chat.Init (StartingDialog);
	}
}
