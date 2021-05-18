using NUnit.Framework;
using UnityEngine;

public class RandomTests {

    [Test]
    [Ignore("Just random test to see how arrays behave when setting specific element to Null")]
    public void PredefinedArrayCheck()
    {
        GameObject[] arr = new GameObject[] { new GameObject("A"), new GameObject("B"), new GameObject("C") };

        arr[1] = null;

        Assert.IsNull(arr[1]);
    }

    [Test]
    [Ignore("Just random test to see how 2D arrays behave when setting specific element to Null")]
    public void SettingArrayElementToNull()
    {
        GameObject[,] arr = new GameObject[2,3] {
                                                    { new GameObject("A"), new GameObject("B"), new GameObject("C") },
                                                    { new GameObject("D"), new GameObject("E"), new GameObject("F")}
                                                };

        arr[0,1] = null;

        Assert.IsNull(arr[0,1]);
    }

    [Test]
    [Ignore("Just random test to see if there is a difference between accessing elements via [,] and GetValue")]
    public void DynamicArrayCheck() {
        GameObject[,] arr = new GameObject[2, 3];
        arr[0, 0] = new GameObject("A");
        arr[0, 1] = new GameObject("B");
        arr[0, 2] = new GameObject("C");

        arr[1, 0] = new GameObject("D");
        arr[1, 1] = new GameObject("E");
        arr[1, 2] = new GameObject("F");

        arr[0, 1] = null;

        Assert.IsNull(arr[0, 1]);
    }

    [Test]
    [Ignore("Just random test to see if there is a difference between accessing elements via [,] and GetValue")]
    public void GettingValueFrom2DArray() {
        int[,] array = new int[,] {
                                    {1},
                                    {2},
                                    {3}};

        Assert.AreEqual(1, array.GetValue(0, 0));
        Assert.AreEqual(2, array.GetValue(1, 0));
        Assert.AreEqual(3, array.GetValue(2, 0));
    }

    [Test]
    [Ignore("Just random test to see how Array behaves when setting GameObject to null")]
    public void SettingGameObjectInArrayToNull()
    {
        GameObject[,] arr = new GameObject[2, 3];
        arr[0, 0] = new GameObject("A");
        arr[0, 1] = new GameObject("B");
        arr[0, 2] = new GameObject("C");

        arr[1, 0] = new GameObject("D");
        arr[1, 1] = new GameObject("E");
        arr[1, 2] = new GameObject("F");

        arr[0, 1].SetActive(false);
        arr[0, 1] = null;

        Assert.IsNull(arr[0, 1]);
    }

    [Test]
    [Ignore("Just random test to see how to access Array element via [,]")]
    public void GettingValueFrom2DArrayViaSquareBrackets()
    {
        int[,] array = new int[,] {
                                    {1},
                                    {2},
                                    {3}};

        Assert.AreEqual(1, array[0, 0]);
        Assert.AreEqual(2, array[1, 0]);
        Assert.AreEqual(3, array[2, 0]);
    }

    [Test]
    [Ignore("Just random test to see how to access 2D Array element via [,]")]
    public void GetValueFrom2DArray()
    {
        int[,] array = new int[,] {
                                    {1, 2},
                                    {3, 4},
                                    {5, 6}};

        Assert.AreEqual(1, array[0, 0]);
        Assert.AreEqual(4, array[1, 1]);
        Assert.AreEqual(5, array[2, 0]);
    }
}
