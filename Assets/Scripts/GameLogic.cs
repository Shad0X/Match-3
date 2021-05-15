using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour {

    //has a lot more than just Logic.. also takes care of entire game grid and all the functionality that affects it.. etc, etc.. 
    [SerializeField]
    private GameField gameField;

    private GameObject[,] tileGameObjects;
    private GameObject tileParentObject;//ToDo - will be used as Object Pool in the future

    [SerializeField]
    [Tooltip("1 of each Tile Type that should be used to populate the tile field")]
    private List<GameObject> tileVariations;

    void Start() {
        tileGameObjects = new GameObject[gameField.Width, gameField.Height];
        tileParentObject = new GameObject("Tiles");

        FillFieldWithTiles();
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
                        tileGameObjects[x + 1, y].transform.position = gameField.GetTileGameworldLocation(x + 1, y);

                        matches = 0;
                    }
                } else {
                    matches = 0;
                }
            }
        }
    }
    
    //ToDo - needs better name and parameter. Assumes that Users know what it does...
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
                tileGameObjects[x, y].transform.position = gameField.GetTileGameworldLocation(x, y);
            }
        }
    }

    private GameObject GenerateTileArrayPositionAt(int x, int y) {
        int tileIndex = Random.Range(0, tileVariations.Count);//0 magic nr?
        return Instantiate(tileVariations[tileIndex], tileParentObject.transform);
    }

    public void RemoveTiles(int x, int y) {//could use better name.. wont always be multiples, wont always be a single 1.. 
        //Might as well replace with the Method below ?
        if (tileGameObjects[x, y] != null) {
            RemoveMatchingTilesAroundPosition(x, y);
        }
    }

    private void RemoveTileAt(int x, int y) {
        tileGameObjects[x, y].transform.gameObject.SetActive(false);
        tileGameObjects[x, y] = null;
        //MoveAllTilesAboveThisPositionDown(x, y);
        //Done moving tiles, check for Matches on entire game field
    }

    private void HandleHorizontalMatches()
    {
        List<Vector2Int> matches = new List<Vector2Int>(); //list of all Tiles that should be removed
        
        //iterate over horizontal rows
        for (int y = 0; y < gameField.Height; y++) { //iterate over all horizontal lines
            //find all matches
            matches.AddRange(GetMatchingTilesOnHorizontalRow(y));
        }
        // keep track of their count == LIST above
        // remove ALL MATCHES @ the same time
        if (matches.Count > 0) {
            foreach (Vector2Int match in matches) {
                RemoveTileAt(match.x, match.y); 
                //ToDo - PROBLEM > This also MOVES TILES DOWN.. meaning that it might change the Grid, before all matching tiles have been removed...
                // this means that all tiles should be removed WITHOUT any movement on grid
                // only THEN we can move ALL tiles down IF NEEDED, before performing another check...
            }
            MoveAnyTilesDownWithGaps(matches);//Getting rid of any gaps created from tiles being removed..
            // perform check again, if anything changed
            HandleHorizontalMatches();
        }
    }

    private List<Vector2Int> GetMatchingTilesOnHorizontalRow(int rowIndex) {
        List<Vector2Int> tileIndexes = new List<Vector2Int>();//could use better name
        string expectedName;
        if (tileGameObjects[0, rowIndex] != null) {
            expectedName = tileGameObjects[0, rowIndex].name; //starting with name of first tile in row, IF it exists... 
        } else {
            expectedName = "null";
        }
        int matches = 0;

        for (int x = 1; x < gameField.Width ; x++) {
            if (tileGameObjects[x, rowIndex] != null
                && tileGameObjects[x, rowIndex].name.Equals(expectedName)) {
                matches += 1;
                if (x == gameField.Width - 1) {//on the last tile of row
                    if (matches >= 2) {
                        //enough matches to be removed, add to list
                        for (int match = x; match > x - 1 - matches; match--) {
                            tileIndexes.Add(new Vector2Int(match, rowIndex));
                        }
                    }
                }
            } else {
                if (matches >= 2) {
                    //enough matches to be removed, add to list
                    for (int match = x - 1; match >= x -1 - matches; match--) {
                        tileIndexes.Add(new Vector2Int(match, rowIndex));
                    }
                }
                matches = 0;
                if (tileGameObjects[x, rowIndex] != null) {
                    expectedName = tileGameObjects[x, rowIndex].name;
                } else {
                    expectedName = "null"; //ToDo - do something better about this
                }
            }
        }

        return tileIndexes;
    }


    private void MoveAnyTilesDownWithGaps(List<Vector2Int> tiles) { //needs better name
        List<Vector2Int> allTiles = tiles;
        for (int y = 0; y < gameField.Height; y++) {
            foreach (Vector2Int tile in allTiles) {
                if (tile.y == y) {
                    MoveAllTilesAboveThisPositionDown(tile.x, tile.y);
                }
            }
        }
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
        RemoveTileAt(x, y);
        MoveAllTilesAboveThisPositionDown(x, y);
        HandleHorizontalMatches();//Checks for any Matches on Field to remove them
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
