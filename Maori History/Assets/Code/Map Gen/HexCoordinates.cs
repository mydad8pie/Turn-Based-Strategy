
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct HexCoordinates
{
    [SerializeField]
    private int x, z;
    public int X
    {
        get
        {
            return x;
        }
    }
    public int Z
    {
        get
        {
            return z;
        }
    }
    public HexCoordinates(int x, int z)
    {
        this.x = x;
        this.z = z;

    }
    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x - z / 2, z);
    }
    public int Y{
        get{
            return -X - Z;
        }
    }
    public override string ToString()
    {
        return "(" + X.ToString() + ", " + Y.ToString() + "," + Z.ToString() + ")";
    }
    public string ToStringOnSeparateLines()
    {
        return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
    }
     


     public static HexCoordinates FromPosition(Vector3 position){
        float x = position.x / (HexMetrics.innerRadius * 2f);
        float y = -x;

        float offset = position.z / (HexMetrics.outerRadius * 3f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y); 

        if (iX + iY + iZ !=0){
           float dx = Mathf.Abs(x - iX);
           float dy = Mathf.Abs(y - iY);
           float dz = Mathf.Abs(-x - y - iZ);

           if (dx > dy && dx > dz){
               iX = -iY - iZ;
           } else if (dz > dy){
               iZ = -iX - iY;
           }
        }
        return new HexCoordinates(iX, iZ);
    }


}

