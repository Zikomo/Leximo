float4x4 World;
float4x4 View;
float4x4 Projection;

// TODO: add effect parameters here.
uniform extern texture ScreenTexture;
sampler ScreenS = sampler_state
{
	Texture = <ScreenTexture>;	
};





float4 PixelShaderFunction(float2 texCoord: TEXCOORD0) : COLOR0
{
    // TODO: add your pixel shader code here.
    float4 color = tex2D(ScreenS, texCoord);
	float lum = color.r * 0.3 + color.g * 0.59 + color.b * 0.11;
	float tvLookFactor = abs(sin(texCoord.y * 360));
    return float4(0, lum*tvLookFactor, 0, 1);
}

technique Technique1
{
    pass P0
    {
        // TODO: set renderstates here.

        //VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
