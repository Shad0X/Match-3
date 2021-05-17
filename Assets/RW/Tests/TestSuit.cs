using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestSuit
{
    // A Test behaves as an ordinary method
    /*    [Test]
        public void TestSuitSimplePasses()
        {
            // Use the Assert class to test conditions
        }*/

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    /*[UnityTest]
    public IEnumerator TestSuitWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }*/

    [Test]
    public void GettingValueFrom2DArray()
    {
        int[,] array = new int[,] {
                                    {1},
                                    {2},
                                    {3}};

        Assert.AreEqual(1, array.GetValue(0, 0));
        Assert.AreEqual(2, array.GetValue(1, 0));
        Assert.AreEqual(3, array.GetValue(2,0));
    }

    [Test]
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


    [UnityTest]
    public IEnumerator TileIsRemoved()
    {
        GameObject gameObject = new GameObject();

        GameField gameField = gameObject.AddComponent<GameField>();
        gameField.SetTestData(1, 1, new GameObject("Field"), new GameObject("Background"));

        GameLogic gameLogic = gameObject.AddComponent<GameLogic>();

        GameObject[,] tilesOnField = new GameObject[,] { { new GameObject("Solo") } };

        gameLogic.SetTestData(gameField, tilesOnField);

        gameLogic.RemoveTiles(0, 0);//Removing the 1 Tile we got

        Assert.AreEqual(null, tilesOnField[0, 0]);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TilesFallDown()
    {
        GameObject gameObject = new GameObject();

        GameField gameField = gameObject.AddComponent<GameField>();
        gameField.SetTestData(1, 3, new GameObject("Field"), new GameObject("Background"));

        GameLogic gameLogic = gameObject.AddComponent<GameLogic>();

        GameObject[,] tilesOnField = new GameObject[1,3] {    // X >>, Y - going down = higher
                                                        { new GameObject("A"),  // 0,0
                                                        new GameObject("B"),  // 0,1
                                                        new GameObject("C")} };// 0,2

        gameLogic.SetTestData(gameField, tilesOnField);

        gameLogic.RemoveTiles(0, 2);//removing C
        
        Assert.AreEqual(null, tilesOnField[0, 0]);
        Assert.AreEqual("A", tilesOnField[0, 1].name);
        Assert.AreEqual("B", tilesOnField[0, 2].name);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TilesMatch()
    {
        GameObject gameObject = new GameObject();

        GameField gameField = gameObject.AddComponent<GameField>();
        gameField.SetTestData(3, 3, new GameObject("Field"), new GameObject("Background"));

        GameLogic gameLogic = gameObject.AddComponent<GameLogic>();

        GameObject[,] tilesOnField = new GameObject[3, 3] {                            // X >>, Y - going down = higher
                        { new GameObject("A"), new GameObject("B"), new GameObject("C")},  // 0,0   1,0    2, 0
                        { new GameObject("B"), new GameObject("A"), new GameObject("A")},  // 0,1   1, 1   2, 1
                        { new GameObject("A"), new GameObject("B"), new GameObject("B") } };// 0,2   1, 2   2, 2


        /*        GameObject[][] tilesOnField = new GameObject[3][];
                GameObject[] row1 = new GameObject[]{ new GameObject("A"), new GameObject("B"), new GameObject("C") };
                GameObject[] row2 = new GameObject[] { new GameObject("B"), new GameObject("A"), new GameObject("A") };
                GameObject[] row3 = new GameObject[] { new GameObject("A"), new GameObject("B"), new GameObject("B") };
                tilesOnField[0] = row1;
                tilesOnField[1] = row2;
                tilesOnField[3] = row3;*/


        //0,0   0,1  0,2
        //1,0   1,1  1,2
        //2,0  2,1  2,2

        gameLogic.SetTestData(gameField, tilesOnField);

        //Verifying that the data is setup as expected
/*        Assert.AreEqual("A", tilesOnField[0, 0].name);
        Assert.AreEqual("B", tilesOnField[0, 1].name);
        Assert.AreEqual("C", tilesOnField[0, 2].name);

        Assert.AreEqual("B", tilesOnField[1, 0].name);
        Assert.AreEqual("A", tilesOnField[1, 1].name);
        Assert.AreEqual("A", tilesOnField[1, 2].name);

        Assert.AreEqual("A", tilesOnField[2, 0].name);
        Assert.AreEqual("B", tilesOnField[2, 1].name);
        Assert.AreEqual("B", tilesOnField[2, 2].name);*/

        gameLogic.RemoveTiles(0, 1); //X = 0, Y = 1 .. 1st element in Row 2



        //Locations that should be empty, due to middle row disappearing due to match
        Assert.AreEqual(null, tilesOnField[0, 0]);
        Assert.AreEqual(null, tilesOnField[0, 1]);
        Assert.AreEqual(null, tilesOnField[0, 2]);

        //Expected middle row (after match)
        Assert.AreEqual(null, tilesOnField[1, 0]);
        Assert.AreEqual("B", tilesOnField[1, 1].name);
        Assert.AreEqual("C", tilesOnField[2, 1].name);

        //Expected bottom row (after match)
        Assert.AreEqual("A", tilesOnField[2, 0].name);
        Assert.AreEqual("B", tilesOnField[2, 1].name);
        Assert.AreEqual("B", tilesOnField[2, 2].name);



        //resultingField.GetValue(0,1)

        /*        //Locations that should be empty, due to middle row disappearing due to match
                Assert.AreEqual(null, tilesOnField[0, 0]);
                Assert.AreEqual(null, tilesOnField[1, 0]);
                Assert.AreEqual(null, tilesOnField[2, 0]);

                //Expected middle row (after match)
                Assert.AreEqual(null, tilesOnField[1, 0]);
                Assert.AreEqual("B", tilesOnField[1, 1].name);
                Assert.AreEqual("C", tilesOnField[2, 1].name);

                //Expected bottom row (after match)
                Assert.AreEqual("A", tilesOnField[0, 2].name);
                Assert.AreEqual("B", tilesOnField[1, 2].name);
                Assert.AreEqual("B", tilesOnField[2, 2].name);*/

        yield return null;
    }

    [Test]
    public void PredefinedArrayCheck()
    {
        GameObject[] arr = new GameObject[] { new GameObject("A"), new GameObject("B"), new GameObject("C") };

        arr[1] = null;

        Assert.AreEqual(null, arr[1]);
    }

    [Test]
    public void PredefinedArrayCheck2()
    {
        GameObject[,] arr = new GameObject[2,3] {
                                                    { new GameObject("A"), new GameObject("B"), new GameObject("C") },
                                                    { new GameObject("D"), new GameObject("E"), new GameObject("F")}
                                                };

        arr[0,1] = null;

        Assert.AreEqual(null, arr[0,1]);
    }

    [Test]
    public void DynamicArrayCheck()
    {
        GameObject[,] arr = new GameObject[2, 3];
        arr[0, 0] = new GameObject("A");
        arr[0, 1] = new GameObject("B");
        arr[0, 2] = new GameObject("C");

        arr[1, 0] = new GameObject("D");
        arr[1, 1] = new GameObject("E");
        arr[1, 2] = new GameObject("F");

        arr[0, 1] = null;

        Assert.AreEqual(null, arr[0, 1]);
    }

    [Test]
    public void DeactivatingGameObjectCheck()
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

        Assert.AreEqual(null, arr[0, 1]);
    }

}
