using UnityEngine;

public class TileArrayIndex : MonoBehaviour {

    private Vector2Int index;
   
    private void Awake() {
        index = new Vector2Int(-1, -1);
    }

    public int x {
        get { return index.x; }
    }

    public int y {
        get { return index.y; }
    }

    public void SetValue(int x, int y) {
        index = new Vector2Int(x, y);
    }
   
}
