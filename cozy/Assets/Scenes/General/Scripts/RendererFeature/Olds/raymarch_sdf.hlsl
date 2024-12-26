float sdSphere(float3 p, float3 sp, float s)
{
    return length(p - sp) - s;
}

float sdBox(float3 p, float3 bp, float3 b)
{
    float3 q = abs(p - bp) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}