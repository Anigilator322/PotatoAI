using UnityEngine;

public class CircleData : MonoBehaviour
{
    Texture2D circleCoordinatesTexture;
    Texture2D circleColorTexture;
    public Material circleMaterial;
    public int circleCount = 20;
    public float intensity = 0.4f;

    void Start()
    {
        // Create the texture
        circleCoordinatesTexture = new Texture2D(circleCount, 1, TextureFormat.RGBAFloat, false);
        circleColorTexture = new Texture2D(circleCount, 1, TextureFormat.RGBAFloat, false);

        for (int i = 0; i < circleCount; i++)
        {
            // Example circle data
            float x = Random.value; // X position
            float y = Random.value; // Y position
            float radius = 0.1f; // Radius

            Color randomColor = new Color(Random.value, Random.value, Random.value, intensity);

            circleCoordinatesTexture.SetPixel(i, 0, new Color(x, y, radius, intensity));
            circleColorTexture.SetPixel(i, 0, randomColor);

            //ComputeBuffer coordinatesBuffer = new ComputeBuffer(circleCount, sizeof(float) * 4);
        }

        circleCoordinatesTexture.Apply();
        circleColorTexture.Apply();

        // Pass texture to the material
        circleMaterial.SetTexture("_CoordinatesData", circleCoordinatesTexture);
        circleMaterial.SetTexture("_ColorData", circleColorTexture);
        circleMaterial.SetFloat("_CircleCount", circleCount);
        circleMaterial.SetVector("_SoilScale", new Vector2(25.8f/7.7f, 1f));
        //circleMaterial.SetBuffer()
    }
}