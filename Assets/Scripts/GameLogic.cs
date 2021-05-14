using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour {

    //has a lot more than just Logic.. also takes care of entire game grid and all the functionality that affects it.. etc, etc.. 
    [SerializeField]
    private GameField gameField;

    private GameObject[,] tileGameObjects;
    private GameObject tileParentObject;

    [SerializeField]
    private GameObject gameBackground;

    [SerializeField]
    [Tooltip("1 of each Tile Type that should be used to populate the tile field")]
    private List<GameObject> tileVariations;

    void Start() {
        tileGameObjects = new GameObject[gameField.Width, gameField.Height];
        tileParentObject = new GameObject("Tiles");

        SetGameBackgroundSize(gameField.Width, gameField.Height);

        FillFieldWithTiles();
    }

    private void SetGameBackgroundSize(int width, int height) {
        gameBackground.transform.localScale = new Vector3(width, height, gameBackground.transform.localScale.z);
    }


    private void FillFieldWithTiles() { //Logic - can't have more than 2 tiles of the same type in a horizontal line, so there are no accidental mathes
        FillFieldWithRandomTiles();
        UpdateTilesToPreventMatches();
    }
    
    private void UpdateTilesToPreventMatches() {//ToDo - terrible code, split it up n refactor properly...
        //iterate over all horizontal rows
        //check that current Tile doesn't have more than 1 match next to it
        int matches = 0;
        for (int y = 0; y < gameField.Height; y++) {
            matches = 0;
            for (int x = 0; x < gameField.Width - 1; x++) {
                if (tileGameObjects[x, y].name.Equals(tileGameObjects[x + 1, y].name)) {
                    matches += 1;
                    if (matches == 2) {
                        tileGameObjects[x + 1, y].SetActive(false);//disabling old Tile
                        tileGameObjects[x + 1, y] = null;//clearing out reference to it (not sure if needed... might be overwritten by line below anyway?)
                        tileGameObjects[x + 1, y] = GetRandomTileExcept(tileGameObjects[x, y].name);
                        tileGameObjects[x + 1, y].transform.position = GetTileGameworldLocation(x + 1, y);

                        matches = 0;
                    }
                } else {
                    matches = 0;
                }
            }
        }
    }
    
    private GameObject GetRandomTileExcept(string excludedTileName) {//not sure if it should use tile names or some other kind of reference...
                                                             //List<GameObject> availableTiles = tileVariations.
                                                             //NOTE - tileName might differ from the Prefab name, due to being a (Clone)... ignore that part ? 
        int tileIndex = Random.Range(0, tileVariations.Count);//0 magic nr?
        if (excludedTileName.Contains(tileVariations[tileIndex].name)) {//since the GameObject names contain (Clone) in the title...
            tileIndex = tileIndex == (tileVariations.Count - 1) ? 0 : tileIndex + 1; //not very readable
        }
        return Instantiate(tileVariations[tileIndex], tileParentObject.transform);
    }

    private void FillFieldWithRandomTiles() { //might end up with existing Matches already on field
        for (int y = 0; y < gameField.Height; y++) {
            for (int x = 0; x < gameField.Width; x++) {
                tileGameObjects[x, y] = GenerateTileArrayPositionAt(x, y);
                tileGameObjects[x, y].transform.position = GetTileGameworldLocation(x, y);
            }
        }
    }

    private Vector3 GetTileGameworldLocation(int x, int y) {//ToDo - move to Helper class. Used by Both Fields and Tiles
        //pass in Field width and height to make it Static ?
        float xOffset = 0;
        float yOffset = 0;
        
        //using offSet in case if game field size consists of unevent numbers
        if (gameField.Width % 2 == 0) {
            xOffset = 0.5f; 
        }
        if (gameField.Height % 2 == 0) {
            yOffset = 0.5f;
        }

        float xLocation = x - gameField.Width / 2 + xOffset;
        float yLocation = y - gameField.Height / 2 + yOffset;

        return new Vector3(xLocation, yLocation, gameBackground.transform.position.z); //move to parameters to make static
    }

    //ToDo - Generate a field where there are no Matches (3 or more tiles of same type in a horizontal line)
    private GameObject GenerateTileArrayPositionAt(int x, int y) {
        int tileIndex = Random.Range(0, tileVariations.Count);//0 magic nr?
        return Instantiate(tileVariations[tileIndex], tileParentObject.transform);
    }

    public void RemoveTiles(int x, int y) {//could use better name.. wont always be multiples, wont always be a single 1.. 
        if (tileGameObjects[x, y] != null) {
            RemoveMatchingTilesAroundPosition(x, y);
        }
    }

    private void RemoveTileAt(int x, int y) {
        tileGameObjects[x, y].transform.gameObject.SetActive(false);
        MoveAllTilesAboveThisPositionDown(x, y);
    }

    private void MoveAllTilesAboveThisPositionDown(int x, int y) {
        if (y + 1 >= gameField.Height) {
            return;
        }

        GameObject tileAbove = tileGameObjects[x, y + 1];
        if (tileAbove == null) {
            return;
        }

        MoveTileDownOneField(x, y + 1);
        
        MoveAllTilesAboveThisPositionDown(x, y + 1);
    }

    private void MoveTileDownOneField(int x, int y) {
        GameObject tile = tileGameObjects[x, y];
        tile.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y - 1, tile.transform.position.z);

        tileGameObjects[x, y - 1] = tile;
        tileGameObjects[x, y] = null;
    }

    private void RemoveMatchingTilesAroundPosition(int x, int y) {
        string expectedName = tileGameObjects[x, y].name;
        RemoveTileAt(x, y);

        int matchingTileCount = GetHorizontallyMatchingTileCount(x, y, expectedName);
        if (matchingTileCount >= 2) {
            RemoveHorizontallyMatchingTiles(x, y, expectedName);
        }
    }

    private int GetHorizontallyMatchingTileCount(int x, int y, string expectedName) {
        int count = 0;

        count += GetMatchingTileCountOnLeft(x, y, expectedName);
        count += GetMatchingTileCountOnRight(x, y, expectedName);

        return count;
    }

    private int GetMatchingTileCountOnLeft(int x, int y, string expectedName) {
        int count = 0;

        if (DoesTileGameObjectNameMatch(x - 1, y, expectedName)) {
            count += 1;
            count += GetMatchingTileCountOnLeft(x - 1, y, expectedName);
        }

        return count;
    }

    private int GetMatchingTileCountOnRight(int x, int y, string expectedName) {
        int count = 0;

        if (DoesTileGameObjectNameMatch(x + 1, y, expectedName)) {
            count += 1;
            count += GetMatchingTileCountOnRight(x + 1, y, expectedName);
        }

        return count;
    }

    private void RemoveHorizontallyMatchingTiles(int x, int y, string expectedName) {
        RemoveLeftTilesIfPossible(x, y, expectedName);
        RemoveRightTilesIfPossible(x, y, expectedName);
    }

    private void RemoveLeftTilesIfPossible(int x, int y, string expectedName) {
        Vector2Int leftTileIndex = new Vector2Int(x - 1, y);

        if (DoesTileGameObjectNameMatch(leftTileIndex.x, leftTileIndex.y, expectedName)) {
            RemoveTileAt(leftTileIndex.x, leftTileIndex.y);
            RemoveLeftTilesIfPossible(leftTileIndex.x, leftTileIndex.y, expectedName);
        }
    }

    private void RemoveRightTilesIfPossible(int x, int y, string expectedName) {
        Vector2Int rightTileIndex = new Vector2Int(x + 1, y);

        if (DoesTileGameObjectNameMatch(rightTileIndex.x, rightTileIndex.y, expectedName)) {
            RemoveTileAt(rightTileIndex.x, rightTileIndex.y);
            RemoveRightTilesIfPossible(rightTileIndex.x, rightTileIndex.y, expectedName);
        }
    }

    private bool DoesTileGameObjectNameMatch(int x, int y, string expectedName) {
        return IsXLocationInsideField(x)
                && tileGameObjects[x, y] != null
                && tileGameObjects[x, y].name.Equals(expectedName);
    }
    
    private bool IsXLocationInsideField(int x) {
        return x >= 0 && x < gameField.Width;
    }

}
