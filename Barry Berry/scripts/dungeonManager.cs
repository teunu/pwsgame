using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*----------------------------------------------------------------------------/
 * -
 * We've put thought and effort into this. Please mind the human hours when you
 * blatandly copy the code.
 * 
 * With love, Nick Elferink, Sven Schuiten, Teun Hoiting.
 * <3
 ----------------------------------------------------------------------------*/

public class dungeonManager : MonoBehaviour {

	//<Summary>
	//The key to the randomness!
	//</Summary>
	public string manualDungeonSeed;

	//<Summary>
	//The seed's numerical value! Since multiple strings can return the same hash code, devide the string into some pieces and multiply them.
	//</Summary>
	public int numericalSeed;

	//<Summary>
	//This is the amount of rooms this particular dungeon will have.
	//</Summary>
	public int AmountOfRooms;

	//<Summary>
	//The list of rooms themselves. This is used as a reference to the other rooms.
	//</Summary>
	public List<roomManager> rooms;

	//<Summary>
	//Both these values will make sure the dungeon does not overextend.
	//Note:This will be split into negative and positive numbers. Example:
	//If it is set to 10. this means that it won go below -5 and not above 5 
	//</Summary>
	public int maxGridSizeOnX;
	public int maxGridSizeOnY;

	//<Summary>
	//This is the set path the dungeon will take so we know for sure we end up at the end. This ignores AmountOfRooms.
	//</Summary>
	public List<int> Path;

	public GameObject _RoomBase;

	//<Summary>
	//Will Check whether or not this room is in a  legitemate position. So not over the maxGridSize and not on another Room
	//</Summary>
	private bool newRoomLegitemate(Vector3 newRoomPos) {
		//TODO: Write out the algorithm to check whether or not the room is legitemate
		return true;
	}

	void Start(){
		ConvertSeedToInt ();
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.B)) {
			DebugGiveAllRooms ();
		}
	}

	public void ConvertSeedToInt(string seed = null){
		if (seed == null) {
			seed = manualDungeonSeed;
		}
		//Debug.Log ("Barry is converting the seed into an integer! the next values will be the hash parts!");
		numericalSeed = 1;

		foreach (char ch in seed) {
			numericalSeed = numericalSeed + ch.GetHashCode();
			//Debug.Log ("the current hash part is: " + ch.GetHashCode() + "... Making the new seed: " + numericalSeed);
		}
	}

	public void createNewRoom(GameObject caller){
		Vector3 newRoomPosition = new Vector3();
		int PathDir = 0;
		//PathDir is the integeral value of which direction the new room will spawn.
		// 0 is west,
		// 1 is north,
		// 2 is east,
		// 3 is south.

		if (Path.Count > 0) {
			//At least creates the path that has been set. Even if AmountOfRooms is set to 0, the path will still be there.
			if (PathDir > 3) {
				//Check if a errorical number has been given, before doing the checks. We can still fix this by using modulo!
				PathDir = PathDir % 4;
				Debug.LogError ("It looks like either one of the devs or the code itself put a wrong path direction! p/ Barry fixed it by using modulo!");
			}

			if (PathDir == 0) {
				newRoomPosition = caller.transform.position + new Vector3 (-11, 0);				
			} else if (PathDir == 1) {
				newRoomPosition = caller.transform.position + new Vector3 (0, 11);				
			} else if (PathDir == 2) {
				newRoomPosition = caller.transform.position + new Vector3 (11, 0);				
			} else if (PathDir == 3) {
				newRoomPosition = caller.transform.position + new Vector3 (0, -11);				
			} else {
				Debug.LogError ("Despite Barry's best efforts, the new room's path direction is errorous and he will stop it spawning!");
				return;
			}

			Path.RemoveAt (0);
		} 

		else if (rooms.Count < AmountOfRooms) {
			Random.InitState (numericalSeed);
			PathDir = 1;//Mathf.RoundToInt (Random.Range (0, 4));
			//
			// TODO: Make sure these paths are genuinely random
			//
			if (PathDir == 0) {
				newRoomPosition = caller.transform.position + new Vector3 (-11, 0);				
			} else if (PathDir == 1) {
				newRoomPosition = caller.transform.position + new Vector3 (0, 11);				
			} else if (PathDir == 2) {
				newRoomPosition = caller.transform.position + new Vector3 (11, 0);				
			} else if (PathDir == 3) {
				newRoomPosition = caller.transform.position + new Vector3 (0, -11);				
			}

		}

		else {
			return;
		}

		//Create the new the Room!
		var newRoom = Instantiate(_RoomBase, newRoomPosition, new Quaternion(), transform);

		//Name the new Room!
		newRoom.gameObject.name = "Room " + rooms.Count;
	}

	public void DebugGiveAllRooms(){

		Debug.Log ("DebugGiveAllRooms() got triggered! Barry will now log all " + rooms.Count +" rooms in this dungeon!");
			int i = 0;
			foreach (roomManager Room in rooms) {
				i++;
				Debug.Log ("Barry logged room number " + i + "! Its position on the room grid is: [" + (Room.transform.position.x / 11) + ", " + (Room.transform.position.y / 11) + "]");
			}
	}
}
