using UnityEngine;
using System.Collections;

// /-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\
//
// 						    Type3D 1.0, Copyright © 2017, Ripcord Development
//											    Font3D.cs
//										   info@ripcorddev.com
//
// \-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/-\-/

//ABOUT - This script contains all the data for a Type3D font.

namespace Type3D
{
    public class Font3D : MonoBehaviour
    {
        public string fontName;						//The written name of the font as it will appear in the TypeGenerator
        public GameObject[] characters;				//A list of all the character prefabs associated with the font.  The index of each character should line up to its Unicode Decimal Value
        public GameObject replacementCharacter;		//If an unsupported character is entered into the TypeGenerator's inputString, it will be replaced with this character instead
        public Material[] materials;				//A list of all the materials that can be applied to this 3D font through the TypeGenerator
    }
}
