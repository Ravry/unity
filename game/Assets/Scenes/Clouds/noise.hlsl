float rand(float3 co)
{
    return frac(sin(dot(co.xyz ,float3(12.9898,78.233,45.5432))) * 43758.5453);
}

float3 randomOnUnitSphere(float3 seed) {
    float u = rand(seed) * 2 - 1;
    float theta = rand(seed.zxy) * 2 * 3.14159265;
    float sqrtClamp = sqrt(1 - u * u);
    return float3(sqrtClamp * cos(theta), sqrtClamp * sin(theta), u);
}

float hash(float n) {
    return frac(sin(n) * 43758.5453123);
}

float noise(float3 position) {
    float x = position.x;
    float y = position.y;
    float z = position.z;

    // Grid cell coordinates
    float3 p = floor(float3(x, y, z));
    float3 f = frac(float3(x, y, z));

    // Hash coordinates of the corners
    float n000 = hash(p.x + p.y * 57.0 + p.z * 113.0);
    float n100 = hash(p.x + 1.0 + p.y * 57.0 + p.z * 113.0);
    float n010 = hash(p.x + (p.y + 1.0) * 57.0 + p.z * 113.0);
    float n110 = hash(p.x + 1.0 + (p.y + 1.0) * 57.0 + p.z * 113.0);
    float n001 = hash(p.x + p.y * 57.0 + (p.z + 1.0) * 113.0);
    float n101 = hash(p.x + 1.0 + p.y * 57.0 + (p.z + 1.0) * 113.0);
    float n011 = hash(p.x + (p.y + 1.0) * 57.0 + (p.z + 1.0) * 113.0);
    float n111 = hash(p.x + 1.0 + (p.y + 1.0) * 57.0 + (p.z + 1.0) * 113.0);

    // Smooth interpolation (fade function)
    float3 u = f * f * (3.0 - 2.0 * f);

    // Interpolate between corners
    float lerpXY0 = lerp(lerp(n000, n100, u.x), lerp(n010, n110, u.x), u.y);
    float lerpXY1 = lerp(lerp(n001, n101, u.x), lerp(n011, n111, u.x), u.y);
    float result = lerp(lerpXY0, lerpXY1, u.z);
    return result;
}
