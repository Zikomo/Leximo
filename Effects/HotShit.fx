// TODO: add effect parameters here.
uniform extern texture ScreenTexture;
texture mask;
float4 color = float4(0,0.5,1,0.5);
bool use1337 = false;
sampler ScreenS = sampler_state
{
	Texture = <ScreenTexture>;	
};

sampler Mask = sampler_state
{
	Texture = <mask>;
};




float4 PixelShaderFunction(float2 texCoord: TEXCOORD0) : COLOR0
{
    // TODO: add your pixel shader code here.
	float4 colorMask = tex2D(Mask, texCoord);
	float4 colorScreen = tex2D(ScreenS, texCoord);
	float4 hotShit = colorMask * colorScreen + color/8;
	if (use1337)
	{
		float lum = hotShit.r * 0.3 + hotShit.g * 0.59 + hotShit.b * 0.11;
		float tvLookFactor = abs(sin(texCoord.y * 360));
		hotShit = float4(0, lum*tvLookFactor, 0, hotShit.a);
	}
    return hotShit;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
