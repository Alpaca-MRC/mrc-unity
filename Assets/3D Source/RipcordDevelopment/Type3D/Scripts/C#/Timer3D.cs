using UnityEngine;
using System.Collections;
using System;

// /-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\
//
// 						    Type3D 1.0, Copyright © 2017, Ripcord Development
//											   Timer3D.cs
//										   info@ripcorddev.com
//
// \-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/

//ABOUT - This script converts a float-based timer into a collection of 3D characters representing the time

namespace Type3D
{
    public class Timer3D : MonoBehaviour
    {
        public Mesh[] numbers;						//The mesh objects that will represent each of the numbers from 0-9
        public GameObject[] timerDigits;			//The GameObjects that display up each individual character of the timer

        float timer;								//The number of seconds that have passed since the timer was last reset

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // UPDATE
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        void Update ()
        {
            if (timer < 120.0f)                                                                                                             //For demo purposes this timer will reset back to 0 after every 120 seconds
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0.0f;
            }

            UpdateTimer(timer);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // UPDATE Type3D TIMER - Using the given time value, update the Type3D timer
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void UpdateTimer (float time)
        {
            int minutes = (int)Mathf.Floor(time / 60);
            string seconds = Mathf.Floor(time % 60).ToString("00");
            string hundredths = Mathf.Floor((time * 100) % 100).ToString("00");
                
            timerDigits[4].GetComponent<MeshFilter>().mesh = numbers[minutes];
            timerDigits[3].GetComponent<MeshFilter>().mesh = numbers[Convert.ToInt32(seconds.Substring(0, 1))];
            timerDigits[2].GetComponent<MeshFilter>().mesh = numbers[Convert.ToInt32(seconds.Substring(1, 1))];
            timerDigits[1].GetComponent<MeshFilter>().mesh = numbers[Convert.ToInt32(hundredths.Substring(0, 1))];
            timerDigits[0].GetComponent<MeshFilter>().mesh = numbers[Convert.ToInt32(hundredths.Substring(1, 1))];
        }
    }
}
