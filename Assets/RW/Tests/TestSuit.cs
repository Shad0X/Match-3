using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestSuit {

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

        Assert.IsNull(tilesOnField[0, 0]);

        yield return null;
    }

    [UnityTest]
    public IEnumerator MatchingTilesAreRemoved()
    {
        GameObject gameObject = new GameObject();

        GameField gameField = gameObject.AddComponent<GameField>();
        gameField.SetTestData(3, 3, new GameObject("Field"), new GameObject("Background"));

        GameLogic gameLogic = gameObject.AddComponent<GameLogic>();

        GameObject[,] tilesOnField = new GameObject[3, 3] {                            // X >>, Y - going down = higher
                        { new GameObject("A"), new GameObject("B"), new GameObject("C")},  // 0,0   1,0    2, 0
                        { new GameObject("B"), new GameObject("A"), new GameObject("A")},  // 0,1   1, 1   2, 1
                        { new GameObject("A"), new GameObject("B"), new GameObject("B")}};// 0,2   1, 2   2, 2


        // arrays:
        // 0,0  0,1  0,2
        // 1,0  1,1  1,2
        // 2,0  2,1  2,2

        // inGame field:
        // 0,0  1,0  2,0
        // 0,1  1,1  2,1
        // 0,2  1,2  2,2

        gameLogic.SetTestData(gameField, tilesOnField);
        
        gameLogic.RemoveTiles(1, 0); //removing 1st element 'B' of center row

        //Locations that should be empty, due to middle row disappearing because of 3 matching A,A,A
        Assert.IsNull(tilesOnField[0, 0]);
        Assert.IsNull(tilesOnField[0, 1]);
        Assert.IsNull(tilesOnField[0, 2]);

        //Expected middle row (after match)
        Assert.IsNull(tilesOnField[1, 0]);
        Assert.AreEqual("B", tilesOnField[1, 1].name);
        Assert.AreEqual("C", tilesOnField[1, 2].name);

        //Expected bottom row (after match)
        Assert.AreEqual("A", tilesOnField[2, 0].name);
        Assert.AreEqual("B", tilesOnField[2, 1].name);
        Assert.AreEqual("B", tilesOnField[2, 2].name);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TilesFallDown()
    {
        GameObject gameObject = new GameObject();

        GameField gameField = gameObject.AddComponent<GameField>();
        gameField.SetTestData(1, 3, new GameObject("Field"), new GameObject("Background"));

        GameLogic gameLogic = gameObject.AddComponent<GameLogic>();

        GameObject[,] tilesOnField = new GameObject[3, 2] { 
                                                        { new GameObject("A"), new GameObject("B")},
                                                        { new GameObject("C"), new GameObject("D")},
                                                        { new GameObject("E"), new GameObject("F")}};

        gameLogic.SetTestData(gameField, tilesOnField);

        gameLogic.RemoveTiles(1, 0);//removing C
        gameLogic.RemoveTiles(2, 1);//removing F
        gameLogic.RemoveTiles(2, 1);//removing D that should have fallen down in place of F after it was removed from method call above

        //Top row should be empty
        Assert.IsNull(tilesOnField[0, 0]);
        Assert.IsNull(tilesOnField[0, 1]);

        //Center row
        Assert.AreEqual("A", tilesOnField[1, 0].name);
        Assert.IsNull(tilesOnField[1, 1]);

        //Bottom row
        Assert.AreEqual("E", tilesOnField[2, 0].name);
        Assert.AreEqual("B", tilesOnField[2, 1].name);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TryingToRemoveNonExistingTileThrowsException()
    {
        GameObject gameObject = new GameObject();

        GameField gameField = gameObject.AddComponent<GameField>();
        gameField.SetTestData(1, 3, new GameObject("Field"), new GameObject("Background"));

        GameLogic gameLogic = gameObject.AddComponent<GameLogic>();

        GameObject[,] tilesOnField = new GameObject[,] {};

        gameLogic.SetTestData(gameField, tilesOnField);

        Assert.That(() => gameLogic.RemoveTiles(0, 0), Throws.TypeOf<IndexOutOfRangeException>());

        yield return null;
    }
}
