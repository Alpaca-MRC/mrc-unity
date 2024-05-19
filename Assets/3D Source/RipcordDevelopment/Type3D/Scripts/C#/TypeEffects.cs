using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// /-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\
//
// 						    Type3D 1.0, Copyright © 2017, Ripcord Development
//											 TypeEffects.cs
//										   info@ripcorddev.com
//
// \-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/

//ABOUT - This script controls all the settings for the animated effect on the character prefabs.

namespace Type3D
{
    public class TypeEffects : MonoBehaviour
    {
        public List<GameObject> characters = new List<GameObject>();	//All the characters contained in the Type3D container, listed in order

        [Header ("Effect Settings")]
        public TypeGenerator.TypeEffect typeEffect;						//The animated effect that will play on each character in the Type3D container in sequence
        public TypeGenerator.LoopType loopType;							//If and how the animated effect will behave once it has affected each of the characters in the sequence

        [Range (0.1f, 10.0f)]	public float effectAmount;				//A multiplier to modify the range of motion on the animated effect.  Does not affect all effect types
        [Range (0.0f, 5.0f)]	public float effectDelay;				//The number of seconds before the effect acts on the next character in the sequence
        [Range (0.0f, 5.0f)]	public float effectFrequency;			//The number of seconds before the effect repeats on the first character in the sequence

        public bool playOnStart;										//If true, start playing the animated effect when the object first appears in the scene

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // START
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        void Start ()
        {
            if (playOnStart)
            {
                StartPlaying();
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // POPULATE THE CHARACTERS LIST
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void Populate ()
        {
            if (characters.Count == 0)                                                                                                      //If the characters list is empty...
            {
                characters = new List<GameObject>();							                                                            //...initialize the characters list

                foreach (Transform child in transform)                                                                                      //...for each child this gameObject has...
                {
                    characters.Add(child.gameObject);							                                                            //......add the child object to the characters list
                }

                typeEffect = TypeGenerator.instance.typeEffect;					                                                            //...set the type effect using the value specified in the TypeGenerator
                loopType = TypeGenerator.instance.loopType;						                                                            //...set the loop type using the value specified in the TypeGenerator
            }

            StartPlaying();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // START PLAYING ANIMATED EFFECT
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void StartPlaying ()
        {    
            if (loopType == TypeGenerator.LoopType.PlayOnce)                                                                                //If the loop type is set to PlayOnce...
            {
                PlayEffect();													                                                            //...just play the effect...
            }
            else                                                                                                                            //Otherwise...
            {
                if (loopType != TypeGenerator.LoopType.None)                                                                                //...if the loop type is not set to None...
                {
                    if (typeEffect != TypeGenerator.TypeEffect.None)                                                                        //......if a type effect has been selected, and the loop type is not set to None..
                    {
                        StartCoroutine("PlayEffectLoop");						                                                            //.........start a coroutine to continually loop and play the effect
                    }
                }
            }
        }
        
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // PLAY LOOPING ANIMATED TYPE EFFECT
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        IEnumerator PlayEffectLoop ()
        {
            while (true)
            {
                switch (loopType)
                {
                    case TypeGenerator.LoopType.OrderedLoop:

                        break;

                    case TypeGenerator.LoopType.RandomLoop:								                                                    //Randomly re-arrange the character list before playing the animated effect
                        
                        for (int i = 0; i < characters.Count; i++)
                        {
                            GameObject temp = characters[i];
                            int randomIndex = Random.Range(i, characters.Count);
                            characters[i] = characters[randomIndex];
                            characters[randomIndex] = temp;
                        }	

                        break;
                    
                    case TypeGenerator.LoopType.PingPong:								                                                    //Reverse the character list before playing the animated effect

                        characters.Reverse();
                        break;
                }			

                PlayEffect();														                                                        //Play the animated effect

                yield return new WaitForSeconds(effectFrequency);					                                                        //Wait the specified amount of time before starting the animations over again
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // PLAY ANIMATED TYPE EFFECT
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void PlayEffect ()
        {
            if (typeEffect == TypeGenerator.TypeEffect.TypeOut)                                                                             //If the TypeEffect is set to TypeOut...
            {
                for (int x = 0; x < characters.Count; x++)                                                                                  //...cycle through each character...
                {
                    characters[x].GetComponent<Renderer>().enabled = false;			                                                        //......and disable its renderer
                }
            }

            for (int x = 0; x < characters.Count; x++)
            {
                switch (typeEffect)
                {
                    case TypeGenerator.TypeEffect.Bounce:
                        iTween.PunchPosition(characters[x], iTween.Hash("Y", 1.0f * effectAmount, "Time", 0.5f, "Delay", effectDelay * x));
                        break;

                    case TypeGenerator.TypeEffect.Flip:
                        iTween.RotateAdd(characters[x], iTween.Hash("X", 360.0f, "Time", 0.5f, "Delay", effectDelay * x, "EaseType", iTween.EaseType.easeOutBack));
                        break;

                    case TypeGenerator.TypeEffect.Scale:
                        iTween.PunchScale(characters[x], iTween.Hash("Amount", Vector3.one * 1.01f * effectAmount, "Time", 0.5f, "Delay", effectDelay * x));
                        break;

                    case TypeGenerator.TypeEffect.Shake:
                        iTween.PunchRotation(characters[x], iTween.Hash("Z", 10.0f * effectAmount, "Time", 0.5f, "Delay", effectDelay * x));
                        break;

                    case TypeGenerator.TypeEffect.Spin:
                        iTween.RotateAdd(characters[x], iTween.Hash("Y", 360.0f, "Time", 0.5f, "Delay", effectDelay * x, "EaseType", iTween.EaseType.easeOutBack));
                        break;

                    case TypeGenerator.TypeEffect.SpinAndBounce:
                        iTween.RotateAdd(characters[x], iTween.Hash("Y", 360.0f, "Time", 0.75f, "Delay", effectDelay * x, "EaseType", iTween.EaseType.easeOutBack));
                        iTween.PunchPosition(characters[x], iTween.Hash("Y", 1.0f * effectAmount, "Time", 0.75f, "Delay", effectDelay * x));
                        break;

                    case TypeGenerator.TypeEffect.TypeOut:
                        StartCoroutine(EnableRenderer(characters[x], effectDelay * x) );
                        break;
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // ENABLE RENDERER - Toggle the renderer of each character
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        IEnumerator EnableRenderer (GameObject target, float delay)
        {
            yield return new WaitForSeconds(delay);										                                                    //Wait for the specified delay
            if (target.name.Substring(0, 3) != "032")                                                                                       //If the character is not a space...
            {
                target.GetComponent<Renderer>().enabled = true;							                                                    //...enable the character's renderer
            }
        }
    }
}
