float sdSphere(float3 p, float s)
{
    return length(p - float3(0, 0, 0)) - s;
}

float sdBox(float3 p, float3 bp, float3 b)
{
    float3 q = abs(p - bp) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}