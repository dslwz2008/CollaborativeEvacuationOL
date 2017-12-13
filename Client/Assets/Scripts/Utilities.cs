using UnityEngine;
using System;
using System.Collections;

public class Utilities {

    public static int[] Shuffle(int[] intArray)
    {
        int[] shuffledArray = new int[intArray.Length];
        int rndNo;

        System.Random rnd = new System.Random();
        for (int i = intArray.Length; i >= 1; i--)
        {
            rndNo = rnd.Next(1, i + 1) - 1;
            shuffledArray[i - 1] = intArray[rndNo];
            intArray[rndNo] = intArray[i - 1];
        }
        return shuffledArray;
    }

    public static Transform GetChildTransformWithName(GameObject go, string name)
    {
        foreach (Transform child in go.transform)
        {
            if (child.gameObject.name == name)
            {
                return child;
            }
        }
        return null;
    }

    public static GameObject GetChildObjectWithName(GameObject go, string name)
    {
        foreach (Transform child in go.transform)
        {
            if (child.gameObject.name == name)
            {
                return child.gameObject;
            }
        }
        return null;
    }
    
}
