using UnityEngine;

public class TileArrayIndex : MonoBehaviour {

    private ArrayIndex index;
   
    private void Awake() {
        index = new ArrayIndex();
    }

    public int x {
        get { return index.x; }
    }

    public int y {
        get { return index.y; }
    }

    public void SetValue(int x, int y) {
        index.SetValue(x, y);
    }
   
}
