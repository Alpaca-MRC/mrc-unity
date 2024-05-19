using UnityEngine;
using System.Collections;

namespace Type3D
{
    public class Score3D : MonoBehaviour
    {
        public Mesh[] numbers;						//The mesh objects that will represent each of the numbers from 0-9
        public GameObject[] scoreDigits;			//The GameObjects that display up each individual character of the score
        public bool animateOnUpdate;				//If true, the score container will play a TypeEffect each time it is updated

        [HideInInspector]	public int score;		//The score value that will be converted to Type3D characters

        public void UpdateScore (int newScore)
        {
            score += newScore;																                                                //Add the new score value to the current score

            string scoreString = score.ToString();											                                                //Convert the score to a string

            if (scoreString.Length >= scoreDigits.Length)                                                                                   //If the length of the score string is greater than the number of score digits in the display...
            {
                for (int x = 0; x < scoreDigits.Length; x++)                                                                                //...cycle through each of the digits in the display...
                {
                    scoreDigits[x].GetComponent<MeshFilter>().mesh = numbers[9];			                                                //......and set it's value to 9 (the result is a maxed out score display)
                }
            }
            else                                                                                                                            //Otherwise...
            {
                for (int x = 0; x < scoreString.Length; x++)                                                                                //...cycle through the each character of the score string
                {
                    int digitValue = System.Convert.ToInt32(scoreString.Substring(scoreString.Length - 1 - x, 1));
                    scoreDigits[x].GetComponent<MeshFilter>().mesh = numbers[digitValue];                                                   //......and set each digit to display the corresponding value from the string
                }
            }

            if (animateOnUpdate && this.GetComponent<TypeEffects>() )                                                                       //If the score display is set to animate on update and the display has a TypeEffects component...
            {
                this.GetComponent<TypeEffects>().PlayEffect();								                                                //...play the type effect on the score display
            }
        }
    }
}
