using System.Collections.Generic;
using UnityEngine;

public static class RandomUtility
{
    #region index

    /// <summary>
    /// Just Random.Range(0, arrayLength)
    /// </summary>
    /// <param name="arrayLength">Return an int from 0 to this length</param>
    /// <returns></returns>
    public static int NormalRandom(int arrayLength)
    {
        return Random.Range(0, arrayLength);
    }

    /// <summary>
    /// Set Random.seed always with different numbers
    /// </summary>
    /// <param name="arrayLength">Return an int from 0 to this length</param>
    /// <returns></returns>
    public static int SuperRandom(int arrayLength)
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        return Random.Range(0, arrayLength);
    }

    /// <summary>
    /// Return random values, but try to not return always the same value. So try to return every value in the array
    /// </summary>
    /// <param name="returnLength">Return is an array of this length</param>
    /// <param name="arrayLength">For every value to return, get an int from 0 to this length</param>
    /// <returns></returns>
    public static int[] NotAlwaysSame(int returnLength, int arrayLength)
    {
        int[] needToFindNTimes = new int[arrayLength];

        //find random for N times
        int[] result = new int[returnLength];
        for (int i = 0; i < returnLength; i++)
        {
            int[] foundsInThisCycle = new int[returnLength];
            while (true)
            {
                //select random number and add to founds in this cycle
                int random = Random.Range(0, arrayLength);
                foundsInThisCycle[random]++;

                //if found N times, add 1 to number of necessary times and move to next
                if (needToFindNTimes[random] < foundsInThisCycle[random])
                {
                    needToFindNTimes[random]++;
                    result[i] = random;
                    break;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Return random values, but can't return same value two times
    /// </summary>
    /// <param name="returnLength">Return is an array of this length. NB if arrayLength is lower, this value is ignored</param>
    /// <param name="arrayLength">For every value to return, get an int from 0 to this length</param>
    /// <returns></returns>
    public static int[] CantReturnTheSame(int returnLength, int arrayLength)
    {
        //create a list of indexes
        List<int> possibles = new List<int>();
        for (int i = 0; i < arrayLength; i++)
            possibles.Add(i);

        //find random for N times
        returnLength = Mathf.Min(returnLength, arrayLength);
        int[] result = new int[returnLength];
        for (int i = 0; i < returnLength; i++)
        {
            int random = Random.Range(0, possibles.Count);
            result[i] = possibles[random];

            //remove from possible indexes, to not find it again
            possibles.RemoveAt(random);
        }

        return result;
    }

    #endregion

    #region value

    /// <summary>
    /// Just Random.Range(0, arrayLength)
    /// </summary>
    /// <param name="array">Return a value inside this array</param>
    /// <returns></returns>
    public static T NormalRandom<T>(T[] array)
    {
        int random = NormalRandom(array.Length);
        return array[random];
    }

    /// <summary>
    /// Set Random.seed always with different numbers
    /// </summary>
    /// <param name="array">Return a value inside this array</param>
    /// <returns></returns>
    public static T SuperRandom<T>(T[] array)
    {
        int random = SuperRandom(array.Length);
        return array[random];
    }

    /// <summary>
    /// Return random values, but try to not return always the same value. So try to return every value in the array
    /// </summary>
    /// <param name="returnLength">Return is an array of this length</param>
    /// <param name="array">For every value to return, get a value from this array</param>
    /// <returns></returns>
    public static T[] NotAlwaysSame<T>(int returnLength, T[] array)
    {
        int[] random = NotAlwaysSame(returnLength, array.Length);
        T[] result = new T[random.Length];

        for (int i = 0; i < random.Length; i++)
            result[i] = array[random[i]];

        return result;
    }

    /// <summary>
    /// Return random values, but can't return same value two times
    /// </summary>
    /// <param name="returnLength">Return is an array of this length. NB if arrayLength is lower, this value is ignored</param>
    /// <param name="array">For every value to return, get a value from this array</param>
    /// <returns></returns>
    public static T[] CantReturnTheSame<T>(int returnLength, T[] array)
    {
        int[] random = CantReturnTheSame(returnLength, array.Length);
        T[] result = new T[random.Length];

        for (int i = 0; i < random.Length; i++)
            result[i] = array[random[i]];

        return result;
    }

    #endregion
}
