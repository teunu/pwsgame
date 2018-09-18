using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*----------------------------------------------------------------------------/
 * Errorical Errors, Barry fixes the space-time continuum. (God blesses Barry)
 * We've put thought and effort into this. Please mind the human hours when you
 * blatandly copy the code.
 * 
 * With love, Nick Elferink, Sven Schuiten, Teun Hoiting.
 * <3
 ----------------------------------------------------------------------------*/

public class roomManager : MonoBehaviour {

	//<Summary>
	//What position on the room grid this is. To make sure there isnt a room there already.
	//</Summary>
	int gridXPos;
	int gridyPos;

	private dungeonManager _dungeonManager;
	private roomManager _creator;

	void Start(){
		if (GetComponentInParent<dungeonManager> ()) {
			_dungeonManager = GetComponentInParent<dungeonManager> ();
		} else {
			Debug.LogError("Barry did not find a dungeonmanager attached to this room's parents! Delete the room, lest it will throw errors!");
			Destroy(this);
		}
		onCreation();
	}
		

	void onCreation(){
		_dungeonManager.rooms.Add(this);
		_dungeonManager.createNewRoom(this.gameObject);
	}


}
