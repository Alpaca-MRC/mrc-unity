using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// /-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\
//
// 						Ripcord Tools, Copyright © 2017, Ripcord Development
//										   DisplaySliderValue.cs
//												 v1.0.1
//										   info@ripcorddev.com
//
// \-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/

//ABOUT - This script displays the value of the attached slider using the specified text object.

namespace Ripcord.Common {
	public class DisplaySliderValue : MonoBehaviour {

		public Text label;

		void Start () {

			SetValue();
		}

		public void SetValue () {

			Slider slider = this.GetComponent<Slider>();

			if (slider.wholeNumbers) {
				label.text = slider.value.ToString();
			}
			else {
				label.text = slider.value.ToString("F2");
			}
		}
	}
}
