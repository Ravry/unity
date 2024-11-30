using UnityEngine;

public class Perlin : MonoBehaviour
{
    private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);

    private static Vector2 GradientVector(int ix, int iy)
    {
        int seed = ix * 73856093 ^ iy * 19349663;
        Random.InitState(seed);
        float angle = Random.Range(0, 360f);
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    private static Vector3 GradientVector(int ix, int iy, int iz)
    {
        int seed = ix * 73856093 ^ iy * 19349663 ^ iz * 40006501;
        Random.InitState(seed);

        float theta = Random.Range(0f, Mathf.PI * 2);
        float phi = Mathf.Acos(Random.Range(-1f, 1f));
        float x = Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = Mathf.Sin(phi) * Mathf.Sin(theta);
        float z = Mathf.Cos(phi);
        return new Vector3(x, y, z).normalized;
    }


    public static float Noise2D(float x, float y)
    {
        int x0 = Mathf.FloorToInt(x);
        int x1 = x0 + 1;
        int y0 = Mathf.FloorToInt(y);
        int y1 = y0 + 1;

        Vector2 g00 = GradientVector(x0, y0);
        Vector2 g10 = GradientVector(x1, y0);
        Vector2 g01 = GradientVector(x0, y1);
        Vector2 g11 = GradientVector(x1, y1);

        Vector2 d00 = new Vector2(x - x0, y - y0);
        Vector2 d10 = new Vector2(x - x1, y - y0);
        Vector2 d01 = new Vector2(x - x0, y - y1);
        Vector2 d11 = new Vector2(x - x1, y - y1);

        float dot00 = Vector2.Dot(g00, d00);
        float dot10 = Vector2.Dot(g10, d10);
        float dot01 = Vector2.Dot(g01, d01);
        float dot11 = Vector2.Dot(g11, d11);

        float u = Fade(x - x0);
        float v = Fade(y - y0);

        float nx0 = Mathf.Lerp(dot00, dot10, u);
        float nx1 = Mathf.Lerp(dot01, dot11, u);
        return Mathf.Lerp(nx0, nx1, v);
    }

    public static float Noise3D(float x, float y, float z)
    {
        int x0 = Mathf.FloorToInt(x);
        int x1 = x0 + 1;
        int y0 = Mathf.FloorToInt(y); 
        int y1 = y0 + 1;
        int z0 = Mathf.FloorToInt(z);
        int z1 = z0 + 1;
        
        Vector3 g000 = GradientVector(x0, y0, z0);
        Vector3 g100 = GradientVector(x1, y0, z0);
        Vector3 g001 = GradientVector(x0, y0, z1);
        Vector3 g101 = GradientVector(x1, y0, z1);
        
        Vector3 g010 = GradientVector(x0, y1, z0);
        Vector3 g110 = GradientVector(x1, y1, z0);
        Vector3 g011 = GradientVector(x0, y1, z1);
        Vector3 g111 = GradientVector(x1, y1, z1);
        
        Vector3 d000 = new Vector3(x - x0, y - y0, z - z0);
        Vector3 d100 = new Vector3(x - x1, y - y0, z - z0);
        Vector3 d001 = new Vector3(x - x0, y - y0, z - z1);;
        Vector3 d101 = new Vector3(x - x1, y - y0, z - z1);;

        Vector3 d010 = new Vector3(x - x0, y - y1, z - z0);;
        Vector3 d110 = new Vector3(x - x1, y - y1, z - z0);;
        Vector3 d011 = new Vector3(x - x0, y - y1, z - z1);;
        Vector3 d111 = new Vector3(x - x1, y - y1, z - z1);;

        float dot000 = Vector3.Dot(g000, d000);
        float dot100 = Vector3.Dot(g100, d100);
        float dot001 = Vector3.Dot(g001, d001);
        float dot101 = Vector3.Dot(g101, d101);
        
        float dot010 = Vector3.Dot(g010, d010);
        float dot110 = Vector3.Dot(g110, d110);
        float dot011 = Vector3.Dot(g011, d011);
        float dot111 = Vector3.Dot(g111, d111);
                
        
        float u = Fade(x - x0);
        float v = Fade(y - y0);
        float w = Fade(z - z0);

        float nx0 = Mathf.Lerp(dot000, dot100, u);
        float nx1 = Mathf.Lerp(dot001, dot101, u);
        float nx2 = Mathf.Lerp(dot010, dot110, u);
        float nx3 = Mathf.Lerp(dot011, dot111, u);

        float nz0 = Mathf.Lerp(nx0, nx1, w);
        float nz1 = Mathf.Lerp(nx2, nx3, w);

        float ny0 = Mathf.Lerp(nz0, nz1, v);
        return ny0;
    }
}