void Wind_float(float2 uv, float time, float amplitude, float frequency, out float3 Out) {
    // Add spatial variation using UV
    float spatialVariation = sin(uv.x * 10.0 + time * 0.5) * cos(uv.y * 10.0 + time * 0.7);

    // Add some noise for organic movement
    float noise = sin(uv.x * 20.0 + uv.y * 20.0 + time * 1.2);

    // Calculate the base wind oscillation
    float baseOscillationX = amplitude * cos(time * frequency + spatialVariation + noise);
    float baseOscillationZ = amplitude * sin(time * frequency + spatialVariation + noise);

    // Output the final wind displacement
    Out = uv.y * float3(baseOscillationX, 0, baseOscillationZ);
}