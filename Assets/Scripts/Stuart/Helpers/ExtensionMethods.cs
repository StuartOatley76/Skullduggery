using System;
using System.Linq;

public class EnumExtensions
{

    /// <summary>
    /// Generic extension method for Enums to return a random value.
    /// Call with EnumExtensions.GetRandomEntry<EnumName>();
    /// </summary>
    /// <typeparam name="T">Type of Enum</typeparam>
    /// <returns></returns>
    public static T GetRandomEntry<T>() where T : Enum {
        Enum[] allEntries = Enum.GetValues(typeof(T)).OfType<Enum>().ToArray();
        Shuffle(allEntries);
        return (T)allEntries[0];

    }

    /// <summary>
    /// Gets an array of distinct enum values for the given enum type.
    /// </summary>
    /// <typeparam name="T">Type of enum</typeparam>
    /// <param name="numberOfEntries">Size of array. If bigger than the number of entries will return an array with each of the entries in a random order</param>
    /// <returns></returns>
    public static T[] GetDistinctRandomEntries<T>(int numberOfEntries) where T : Enum {
        Enum[] allEntries = Enum.GetValues(typeof(T)) //Gets an array of all the values in the enum
            .OfType<Enum>().ToArray();
        Shuffle(allEntries);
        if(numberOfEntries - 1 >= allEntries.Length) {
            T[] toReturn = new T[allEntries.Length];
            for(int i = 0; i < allEntries.Length; i++) {
                toReturn[i] = (T)allEntries[i];
            }
            return toReturn;
        }

        T[] results = new T[numberOfEntries];
        for(int i = 0; i < numberOfEntries; i++) {
            results[i] = (T)allEntries[i];
        }
        return results;

    }

    /// <summary>
    /// Shuffles the arrays of enums supplied from 
    /// </summary>
    /// <param name="array"></param>
    private static void Shuffle(Enum[] array) {
            for (int i = 0; i < array.Length - 1; i++) {
                int pos = UnityEngine.Random.Range(i, array.Length);

                Enum temp = array[pos];
                array[pos] = array[i];
                array[i] = temp;

            }
        }

}
