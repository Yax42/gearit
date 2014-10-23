sampler TextureSampler : register(s0);
float2   ViewportSize;
float4x4 ScrollMatrix;

void SpriteVertexShader(inout float4 color : COLOR0, inout float2 texCoord : TEXCOORD0, inout float4 position : POSITION0)
{
   position.xy -= 0.5;   // Half pixel offset for correct texel centering.
   // Viewport adjustment.
   position.xy = position.xy / ViewportSize;
   position.xy *= float2(2, -2);
   position.xy -= float2(1, -1);
	// Transform our texture coordinates to account for camera
	texCoord = mul(float4(texCoord.xy, 0, 1), ScrollMatrix).xy;
}

technique SpriteBatch
{
    pass
    {
        VertexShader = compile vs_2_0 SpriteVertexShader();
    }
}


