using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// /-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\
//
// 						  Ripcord Tools, Copyright © 2017, Ripcord Development
//											  SceneLoader.cs
//												 v1.1.1
//										   info@ripcorddev.com
//
// \-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/

//ABOUT - This script reads the list of scenes from the build settings
//		- Calling the NextScene or PreviousScene functions allow this script to load the next or previous scene from the list of scenes in the build settings

namespace Ripcord.Common {
	public class SceneLoader : MonoBehaviour {

		int sceneCount;				//A short reference to the number of scenes in the build settings list of scenes
		int sceneIndex;				//The index of the current scene in the build settings list of scenes

		public bool useHotKeys;

		void Start () {
		
			sceneCount = SceneManager.sceneCountInBuildSettings;		//Grab the number of scenes in the scene list in the Build Settings
			sceneIndex = SceneManager.GetActiveScene().buildIndex;		//Grab the index of the current scene
		}

		void Update () {

			if (useHotKeys) {
				if (Input.GetKeyDown(KeyCode.Alpha1) ) {
					PreviousScene();
				}
				if (Input.GetKeyDown(KeyCode.Alpha2) ) {
					NextScene();
				}
			}
		}

		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		// LOAD NEXT SCENE
		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		public void NextScene () {

			int newSceneIndex = sceneIndex;

			if (newSceneIndex < sceneCount - 1) {
				newSceneIndex++;
			}
			else {
				newSceneIndex = 0;	
			}

			SceneManager.LoadScene(newSceneIndex);

		}

		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		// LOAD PREVIOUS SCENE
		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		public void PreviousScene () {

			int newSceneIndex = sceneIndex;

			if (newSceneIndex > 0) {
				newSceneIndex--;
			}
			else {
				newSceneIndex = sceneCount - 1;
			}

			SceneManager.LoadScene(newSceneIndex);
		}
	}
}