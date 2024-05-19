using UnityEngine;
using System.Collections;

// /-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\
//
// 						  Ripcord Tools, Copyright © 2017, Ripcord Development
//											 PlatformChecker.cs
//												 v1.1.1
//										   info@ripcorddev.com
//
// \-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/

//ABOUT - This script enables or disables the attached object based on the selected build platforms

namespace Ripcord.Common {
	public class PlatformChecker : MonoBehaviour {

		public enum Action {Enable, Disable}			//The action to take when the active platform matches one from the list
		public Action action;

		public RuntimePlatform[] platforms;				//A list of platforms that will either enable or disable the attached object
		bool listedPlatform;							//If true, the application platform is one of the ones from the platforms list
		
		void Awake () {
		
			for (int x = 0; x < platforms.Length; x++) {					//Cycle through the list of platforms...
				if (Application.platform == platforms[x]) {					//...if the applications platform matches a runtime platform from the list...

					listedPlatform = true;
				}
			}
			switch (action) {

				case Action.Disable:										//......if the action is set to Disable...
					gameObject.SetActive(!listedPlatform);					//.........disable the attached object...
					break;

				case Action.Enable:											//......if the action is set to Enbable...
					gameObject.SetActive(listedPlatform);					//.........enable the attached object
					break;
			}
		}
	}
}
