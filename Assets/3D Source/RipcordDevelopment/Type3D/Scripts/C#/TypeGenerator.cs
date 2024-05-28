using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

// /-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\
//
// 						    Type3D 1.0, Copyright © 2017, Ripcord Development
//											TypeGenerator.cs
//										   info@ripcorddev.com
//
// \-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/

//ABOUT - This script takes a string of characters and converts it into 3D objects representing each one.
//		- This script can modify the size and spacing between letters along with the alignment, materials, as well as add animated effects to each one

namespace Type3D
{
    public class TypeGenerator : MonoBehaviour
    {
        public static TypeGenerator instance;

        public List<Font3D> font3DList;							//A list of all the 3D font sets available to the TypeGenerator
        public int font3DIndex;									//The index of the currently visible Type3D font from the font list

        public string inputString;								//The string of characters that will get converted into Type3D objects

        public List<GameObject> type3DList;						//A list of the Type3D characters currently in the scene
        GameObject type3DContainer;								//The parent object that will contain all the generated Type3D objects
        float heightOffset = 2.0f;								//The height offset for positioning the Type3D container within the viewport

        [Header ("Display Settings")]
        public float kerning;									//The amount of space between each Type3D character
        public enum Alignment
        {
            Left,												//The Type3D characters will all be positioned left of the Type3D container pivot
            Center, 											//The Type3D characters will be equally distributed left and right of the Type3D container pivot
            Right												//The Type3D characters will all be positioned right of the Type3D container pivot
        }				
        public Alignment alignment;								//The position of the Type3D characters relative to the Type3D container

        public enum TypeEffect
        {
            None, 						                        //The Type3D characters will not have an animated effect
            Bounce, 					                        //Each Type3D character will move up and down a bit on its Y axis
            Flip, 						                        //Each Type3D character will rotate 360 degrees on its X-axis
            Scale, 						                        //Each Type3D character will scale up and back down
            Shake, 						                        //Each Type3D character will rotate back and forth a little bit on its Z-axis
            Spin,						                        //Each Type3D character will rotate 360 degrees on its Y-axis
            SpinAndBounce,				                        //Each Type3D character will rotate 360 degrees on its Y-axis while moving up then down on the same axis
            TypeOut						                        //Each Type3D character will switch from invisible to visible as if being typed out
        }
        public TypeEffect typeEffect;

        public enum LoopType
        {
            None, 						                        //No animated effect will play unless specifically called through script
            PlayOnce, 					                        //The animated effect will play once only when the object first spawns
            OrderedLoop, 				                        //The animated effect will loop through all characters starting at the beginning of the list all the way to the end
            RandomLoop, 				                        //The animated effect will loop through all characters in a random order.  Once it plays on each character it will start over with another random order
            PingPong					                        //The animated effect will loop through each character from left to right, and then back from right to left
        }
        public LoopType loopType;

        [Header ("- - - - - - - - - - UI Components - - - - - - - - - -")]
        [Header ("Input Field")]
        public bool useInputField;
        public InputField inputField;

        [Header ("Type Style")]
        public bool useStyleList;
        public Dropdown styleList;

        [Header ("Type Alignment")]
        public bool useTypeAlignment;
        public Button[] alignmentButtons;

        [Header ("Type Size")]
        public bool useTypeSizeSlider;
        public Slider typeSizeSlider;

        [Header ("Type Kerning")]
        public bool useKerningSlider;
        public Slider kerningSlider;

        [Header ("Type Material")]
        public bool useMaterialSlider;
        public Slider materialSlider;

        [Header ("Type Effect")]
        public bool useEffectList;
        public Dropdown effectList;

        [Header ("Effect Loop Type")]
        public bool useLoopTypeList;
        public Dropdown loopTypeList;

        [Header ("Effect Amount")]
        public bool useEffectAmount;
        public Slider effectAmountSlider;

        [Header ("Effect Delay")]
        public bool useEffectDelay;
        public Slider effectDelaySlider;

        [Header ("Effect Frequency")]
        public bool useEffectFrequency;
        public Slider effectFrequencySlider;

        [Header ("Effect Order")]
        public bool useEffectOrder;
        public Toggle effectOrderToggle;

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // AWAKE
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        void Awake ()
        {
            if (!instance)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }

            //Populate type style dropdown list
            if (useStyleList && styleList)
            {
                styleList.ClearOptions();

                List<string> fontNames = new List<string>();
                for (int x = 0; x < font3DList.Count; x++)
                {
                    fontNames.Add(font3DList[x].fontName);
                }
                styleList.AddOptions(fontNames);
            }

            //Populate type effect dropdown list
            if (useEffectList && effectList)
            {
                effectList.ClearOptions();

                string[] enumNames = Enum.GetNames(typeof(TypeEffect));
                List<string> listOptions = new List<string>(enumNames);
                effectList.AddOptions(listOptions);
            }

            //Populate effect loop type dropdown list
            if (useLoopTypeList && loopTypeList)
            {
                loopTypeList.ClearOptions();

                string[] enumNames = Enum.GetNames(typeof(LoopType));
                List<string> listOptions = new List<string>(enumNames);
                loopTypeList.AddOptions(listOptions);
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // GENERATE Type3D OBJECTS
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void GenerateType ()
        {
            //Clear the old Type3D objects - - - - - - - - - - - - - - - - - - - -
            type3DList.Clear();																                                                //Clear the list of any existing Type3D objects

            if (type3DContainer)                                                                                                            //If there is an existing Type3D container...
            {
                Destroy(type3DContainer);													                                                //...destroy it
            }

            //Prepare the new Type3D objects - - - - - - - - - - - - - - - - - - - -
            char[] inputCharacters = inputString.ToCharArray();								                                                //Create a list of all the characters in the input string

            for (int x = 0; x < inputCharacters.Length; x++)                                                                                //Cycle through each character in the input string...
            {
                short unicode = Convert.ToInt16(inputCharacters[x]);						                                                //...grab the unicode value of each character...
                if (font3DList[font3DIndex].characters[unicode] != null)                                                                    //...if the character exists in the Font3D list...
                {
                    type3DList.Add(font3DList[font3DIndex].characters[unicode]);			                                                //......use the unicode value to look up and add the 3D font character to the Font3D list
                }
                else                                                                                                                        //...otherwise...
                {
                    type3DList.Add(font3DList[font3DIndex].replacementCharacter);			                                                //......add the replacement character to the Font3D list instead
                }
            }

            type3DContainer = new GameObject();												                                                //Create a new container object for the Type3D characters
            type3DContainer.name = "NewType3D";												                                                //Set a name for the new container object
            type3DContainer.transform.position = new Vector3(0.0f, heightOffset, 0.0f);		                                                //Position the new container object
            type3DContainer.AddComponent<TypeEffects>();									                                                //Add the TypeEffects component to the new container object

            //Spawn the new Type3D characters - - - - - - - - - - - - - - - - - - - -
            for (int y = 0; y < type3DList.Count; y++)
            {
                GameObject newCharacter = (GameObject)Instantiate(type3DList[y], transform.position, transform.rotation);                   //...spawn the next character in the type list...
                newCharacter.transform.parent = type3DContainer.transform;					                                                //...make the new character object a child of the Type3D container...
                newCharacter.transform.localPosition = Vector3.zero;						                                                //...reset the local position of the new character object...
                newCharacter.transform.localEulerAngles = new Vector3(0.0f, 180.0f, 0.0f);                                                  //...reset the local rotatino of the new character object...

                type3DList[y] = newCharacter;												                                                //...update the character prefab in the Type3D list with the new character
            }

            if (useMaterialSlider && materialSlider)                                                                                        //If the TypeGenerator is using the Material slider...
            {
                SetMaterial();																                                                //...set the material for the Type3D object
            }

            if (useKerningSlider && kerningSlider)                                                                                          //If the TypeGenerator is using the Kerning slider...
            {
                kerning = kerningSlider.value;												                                                //...set the kerning value for the Type3D object
            }

            SetKerning();																	                                                //Set the kerning for the new Type3D characters
            SetAlignment( (int)alignment);													                                                //Set the alignment for the new Type3D characters

            if (typeEffect != TypeEffect.None)                                                                                              //If a type effect has been specified...
            {
                
                if (useEffectAmount && effectAmountSlider)                                                                                  //...if the TypeGenerator is using the EffectAmount slider...
                {
                    type3DContainer.GetComponent<TypeEffects>().effectAmount = effectAmountSlider.value;				                    //......set the effect amount value in the Type3D object
                }

                if (useEffectDelay && effectDelaySlider)                                                                                    //...if the TypeGenerator is using the EffectDelay slider...
                {
                    type3DContainer.GetComponent<TypeEffects>().effectDelay = effectDelaySlider.value;					                    //......set the effect delay value in the Type3D object
                }

                if (useEffectFrequency && effectFrequencySlider)                                                                            //...if the TypeGenerator is using the EffectFrequency slider...
                {
                    type3DContainer.GetComponent<TypeEffects>().effectFrequency = effectFrequencySlider.value;			                    //......set the effect frequency value in the Type3D object
                }

                type3DContainer.GetComponent<TypeEffects>().Populate();						                                                //...populate the characters list on the Type3D object and start running the animated effect
            }

            if (useTypeSizeSlider && typeSizeSlider)                                                                                        //If the TypeGenerator is using the TypeSize slider...
            {
                SetTypeSize();																                                                //...set the type size
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // SET INPUT STRING - Set the value for the input string then generate 3d type
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void SetInputString ()
        {
            if (useInputField && inputField)                                                                                                //If the TypeGenerator is using the InputField..
            {
                inputString = inputField.text;												                                                //...set the inputString value using the inputField's text

                if (inputString.Length > 0)                                                                                                 //...if the input string isn't empty...
                {
                    GenerateType();															                                                //......then Generate the new Type3D objects
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // SET TYPE FONT STYLE
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void SetTypeStyle ()
        {
            if (useStyleList && styleList)                                                                                                  //If the TypeGenerator is using the type StyleList...
            {
                font3DIndex = (int)styleList.value;											                                                //...set the type index using the value from the StyleList
            }

            if (useMaterialSlider && materialSlider)                                                                                        //If the TypeGenerator is using the MaterialSlider...
            {
                materialSlider.maxValue = font3DList[font3DIndex].materials.Length - 1;		                                                //...set the type material using the value from the MaterialSlider
            }

            if (type3DContainer)                                                                                                            //If there is an existing Type3D container in the scene...
            {
                GenerateType();																                                                //...then Generate the new Type3D objects
            }																
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // SET TYPE ALIGNMENT
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void SetAlignment (int alignmentIndex)
        {
            alignment = (Alignment)alignmentIndex;

            if (type3DContainer)
            {
                //Align the new Type 3D objects - - - - - - - - - - - - - - - - - - - -
                type3DContainer.transform.DetachChildren();														                            //Detach all children from the type 3D container

                float extentsMin = type3DList[0].transform.position.x - type3DList[0].GetComponent<Renderer>().bounds.extents.x;
                float extentsMax = type3DList[type3DList.Count - 1].GetComponent<Renderer>().bounds.max.x;
                float center = extentsMax - extentsMin;

                float alignmentOffset = 0.0f;																	                            //Reset the alignment offset

                switch (alignment)
                {
                    case Alignment.Left:

                        alignmentOffset = extentsMin;																                        //Set the alignment offset to the far left of the Type3D characters
                        break;

                    case Alignment.Center:

                        alignmentOffset = extentsMin + center / 2.0f;												                        //Set the alignment offset to the center of the Type3D characters
                        break;

                    case Alignment.Right:

                        alignmentOffset = extentsMax;																                        //Set the alignment offset to the far right of the Type3D characters
                        break;
                }

                type3DContainer.transform.position = new Vector3(alignmentOffset, type3DContainer.transform.position.y, type3DContainer.transform.position.z);

                for (int x = 0; x < type3DList.Count; x++)                                                                                  //Cycle through each Type3D character...
                {
                    type3DList[x].transform.parent = type3DContainer.transform;									                            //...and make it a child of the Type3D container
                }

                type3DContainer.transform.position = new Vector3(0.0f, heightOffset, 0.0f);						                            //Move the Type3D container up so that it's not hidden behind the UI
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // SET TYPE SIZE - Adjust the scale of the Type3D container
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void SetTypeSize ()
        {
            if (type3DContainer)                                                                                                            //If there is an existing Type3D container in the scene...
            {
                type3DContainer.transform.localScale = Vector3.one * typeSizeSlider.value;			                                        //...set the Type3D container's scale to the value of the type size slider
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // SET TYPE KERNING
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void SetKerning ()
        {
            if (useKerningSlider && kerningSlider)                                                                                          //If the TypeGenerator is using the KerningSlider
            {
                kerning = kerningSlider.value;															                                    //...set the kerning value based on the KerningSlider value
            }

            float xOffset = -type3DList[0].GetComponent<Renderer>().bounds.extents.x;					                                    //Reset the x offset

            for (int x = 0; x < type3DList.Count; x++)                                                                                      //Cycle through each character of the Tpye3D object...
            {
                type3DList[x].transform.localPosition = type3DList[0].transform.localPosition;			                                    //...reset the position of each object based off the position of the first character...
                float centerOffset = type3DList[x].GetComponent<Renderer>().bounds.extents.x;			                                    //...create an offset that is half the width of the character...

                type3DList[x].transform.position += new Vector3(xOffset + centerOffset, 0.0f, 0.0f);	                                    //...set the new position using the x offset...

                xOffset += kerning + type3DList[x].GetComponent<Renderer>().bounds.size.x;				                                    //...update the xOffset with the kerning value and the width of the previous letter
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // SET TYPE MATERIAL
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void SetMaterial ()
        {
            if (type3DContainer)
            {
                foreach (Transform child in type3DContainer.transform)
                {
                    child.GetComponent<Renderer>().material = font3DList[font3DIndex].materials[(int)materialSlider.value];
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // SET ANIMATED TYPE EFFECT
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void SetTypeEffect ()
        {
            if (useEffectList && effectList)                                                                                                //If the TypeGenerator us using the EffectsList...
            {
                typeEffect = (TypeEffect)effectList.value;													                                //...set the type effect using the value from the EffectsList
            }

            if (type3DContainer)                                                                                                            //If there is an existing Type3D container in the scene...
            {
                type3DContainer.GetComponent<TypeEffects>().typeEffect = typeEffect;						                                //...set the containers type effect value...
                GenerateType();																				                                //...then Generate the new Type3D objects
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // SET ANIMATED EFFECT LOOP TYPE
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void SetLoopType ()
        {
            if (useLoopTypeList && loopTypeList)                                                                                            //If the TypeGenerator us using the LoopTypeList...
            {
                loopType = (LoopType)loopTypeList.value;													                                //...set the loop type using the value from the LoopTypeList
            }

            if (type3DContainer)                                                                                                            //If there is an existing Type3D container in the scene...
            {
                type3DContainer.GetComponent<TypeEffects>().loopType = loopType;							                                //...set the containers loop type value...
                GenerateType();																				                                //...then Generate the new Type3D objects
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // SET TYPE EFFECT AMOUNT
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void SetEffectAmount ()
        {
            if (useEffectAmount && effectAmountSlider)                                                                                      //If the TypeGenerator is using the EffectAmount slider
            {
                if (type3DContainer)                                                                                                        //...if there is an existing Type3D container in the scene...
                {
                    type3DContainer.GetComponent<TypeEffects>().effectAmount = effectAmountSlider.value;	                                //......set the effect amount value based on the EffectAmount slider
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // SET TYPE EFFECT DELAY
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void SetEffectDelay ()
        {
            if (useEffectDelay && effectDelaySlider)                                                                                        //If the TypeGenerator is using the EffectDelay slider
            {
                if (type3DContainer)                                                                                                        //...if there is an existing Type3D container in the scene...
                {
                    type3DContainer.GetComponent<TypeEffects>().effectDelay = effectDelaySlider.value;		                                //......set the effect delay value based on the EffectDelay slider
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // SET TYPE EFFECT FREQUENCY
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void SetEffectFrequency ()
        {
            if (useEffectFrequency && effectFrequencySlider)                                                                                //If the TypeGenerator is using the EffectFrequency slider
            {
                if (type3DContainer)                                                                                                        //...if there is an existing Type3D container in the scene...
                {
                    type3DContainer.GetComponent<TypeEffects>().effectFrequency = effectFrequencySlider.value;	                            //......set the effect frequency value based on the EffectFrequency slider
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // SAVE Type3D OBJECT
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public void SaveType3D ()
        {
            #if UNITY_EDITOR
            if (!Directory.Exists("Assets/Resources/Type3D"))                                                                               //If the Type3D resources folder does not exist...
            {
                Debug.LogWarning("<color=#ffff44><b>TypeGenerator.cs</b></color> - Resources/Type3D not found, creating new folder");
                Directory.CreateDirectory("Assets/Resources/Type3D");							                                            //...create the Type3D resources folder
            }
            if (!type3DContainer)                                                                                                           //If there is no Type3D container in the scene...
            {
                return;                                                                                                                     //...quit the function
            }

            string localPath = "Assets/Resources/Type3D/" + "NewType3D" + ".prefab";                                                        //Create a path to save the container as a new prefab...

            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);                                                                   //Make sure the new prefab's file name is unique in case one with the same name already exists

            string fileName = localPath.Substring(localPath.LastIndexOf("/") + 1);                                                          //Create a reference to the prefab's file name by removing the file path...
            fileName = fileName.Substring(0, fileName.LastIndexOf("."));                                                                    //Remove the file extension from the name reference...

            bool prefabSuccess;
            PrefabUtility.SaveAsPrefabAsset(type3DContainer, localPath, out prefabSuccess);                                                 //Create a new prefab using the Type3D container and save it to the local path
            if (prefabSuccess == true)
            {
                Debug.Log("<color=#ffffff><b>TypeGenerator.cs</b></color> - Prefab saved successfully");
            }
            else
            {
                Debug.LogError("<color=#ff4444><b>TypeGenerator.cs</b></color> - Prefab failed to save" + prefabSuccess);
            }

            GameObject newType3D = Resources.Load("Type3D/" + fileName) as GameObject;									                    //Create a reference to the Type3D container that was just saved...
            
            newType3D.GetComponent<TypeEffects>().playOnStart = true;											                            //Set the Type3D container's playOnStart value to true
            
            foreach (Transform child in newType3D.transform)                                                                                //Cycle through each character in the saved Type3D container...
            {
                Vector3 resetPosition = child.transform.localPosition;                                                                      //...create a reference to the character's local position...
                child.transform.localPosition = new Vector3(resetPosition.x, 0.0f, resetPosition.z);			                            //...reset the y position of the character...
                child.transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);														        //...reset the rotation of the character...
                child.name = child.name.Substring(0, child.name.Length - 7);									                            //...clean up the name of the character object...

                iTween[] iTweens = child.GetComponents<iTween>();												                            //...create a list of all the iTween components on the character...
                if (iTweens.Length > 0)                                                                                                     //...if there are any iTween components on the character...
                {
                    foreach (iTween component in iTweens)                                                                                   //......cycle through each iTween component...
                    {
                        DestroyImmediate(component, true);														                            //.........and remove it from the character
                    }
                }
            }
            #endif
        }
    }
}
