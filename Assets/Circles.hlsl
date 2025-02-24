
void CircleShaderGraph_float(UnityTexture2D coordinatesData, UnityTexture2D colorData, float2 uv, float circleCount, UnitySamplerState circleDataSampler, float2 uvScale, float dataTexturesWidth, out float4 result)
{
    float j = 0;
	float EPSILON = 0.0001;
    float outlineWidth = EPSILON * 7 * length(uvScale);
    
	// Iterate over circles
    [loop]
    for (int i = 0; i < circleCount; i++)
    {
        float t = (i + 0.5) / dataTexturesWidth; // UV X for current circle
       
	    // Sample circle coordinates and color from the textures
        float4 circleData = coordinatesData.Sample(circleDataSampler, float2(t, 0));
        float4 circleColor = colorData.Sample(circleDataSampler, float2(t, 0));

        float2 circlePos = circleData.xy; // Circle center (normalized)
        float radius = circleData.z; // Circle radius
        bool drawOutline = circleData.w;
        
        float2 diff = uv - circlePos;
        diff.x *= uvScale.x;
        diff.y *= uvScale.y;
	
        float dist = length(diff);
        
        if (drawOutline && (abs(dist - radius) < outlineWidth))
        {
            result = float4(0.43, 0.34, 0.3, 0.9);
            return;
        }
        
        if (dist < radius)
        {
            result += circleColor;
            j++;
        }
    }
    
    result /= j;

    if(j == 1)
        return;
    
    j--;
    result.a = saturate(result.a * (1 + (j / (j + 2))));
}