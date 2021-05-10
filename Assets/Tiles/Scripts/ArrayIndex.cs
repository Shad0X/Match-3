public class ArrayIndex {

    public ArrayIndex() {
        x = -1;
        y = -1;
    }

    public ArrayIndex(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public int x {
        get; private set;
    }

    public int y {
        get; private set;
    }

    public void SetValue(int x, int y) {
        this.x = x;
        this.y = y;
    }

}
