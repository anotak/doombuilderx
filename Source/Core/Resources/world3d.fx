// 3D world rendering shader
// Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com

// Vertex input data
struct VertexData
{
    float3 pos		: POSITION;
	float4 color	: COLOR0;
    float2 uv		: TEXCOORD0;
};

// Pixel input data
struct PixelData
{
    float4 pos		: POSITION;
    float4 color	: COLOR0;
    float2 uv		: TEXCOORD0;
};

// Modulation color
float4 modulatecolor;

// Highlight color
float4 highlightcolor;

// Matrix for final transformation
float4x4 worldviewproj;

// Texture input
texture texture1;

// Filter settings
dword minfiltersettings;
dword magfiltersettings;
dword mipfiltersettings;
float maxanisotropysetting;

// Texture sampler settings
sampler2D texturesamp = sampler_state
{
    Texture = <texture1>;
    MagFilter = magfiltersettings;
    MinFilter = minfiltersettings;
    MipFilter = mipfiltersettings;
	MipMapLodBias = 0.0f;
	MaxAnisotropy = maxanisotropysetting;
};

// Vertex shader
PixelData vs_main(VertexData vd)
{
    PixelData pd;
    
    // Fill pixel data input
    pd.pos = mul(float4(vd.pos, 1.0f), worldviewproj);
    pd.color = vd.color;
    pd.uv = vd.uv;
    
    // Return result
    return pd;
}

// Normal pixel shader
float4 ps_main(PixelData pd) : COLOR
{
	float4 tcolor = tex2D(texturesamp, pd.uv);
	
	// Blend texture color, vertex color and modulation color
    return tcolor * pd.color * modulatecolor;
}

// Full-bright pixel shader
float4 ps_fullbright(PixelData pd) : COLOR
{
	float4 tcolor = tex2D(texturesamp, pd.uv);
	tcolor.a *= pd.color.a;
	
	// Blend texture color and modulation color
    return tcolor * modulatecolor;
}

// Normal pixel shader with highlight
float4 ps_main_highlight(PixelData pd) : COLOR
{
	float4 tcolor = tex2D(texturesamp, pd.uv);
	
	// Blend texture color, vertex color and modulation color
	float4 ncolor = tcolor * pd.color * modulatecolor;
	float4 hcolor = float4(highlightcolor.rgb, ncolor.a);
	
    //return lerp(ncolor, hcolor, highlightcolor.a);
    return float4(hcolor.rgb * highlightcolor.a + (ncolor.rgb - 0.4f * highlightcolor.a), tcolor.a);
}

// Full-bright pixel shader with highlight
float4 ps_fullbright_highlight(PixelData pd) : COLOR
{
	float4 tcolor = tex2D(texturesamp, pd.uv);
	
	// Blend texture color and modulation color
	float4 ncolor = tcolor * modulatecolor;
	float4 hcolor = float4(highlightcolor.rgb, ncolor.a);
	
    //return lerp(ncolor, hcolor, highlightcolor.a);
    return float4(hcolor.rgb * highlightcolor.a + (ncolor.rgb - 0.4f * highlightcolor.a), tcolor.a);
}

// Technique for shader model 2.0
technique SM20
{
	// Normal
    pass p0
    {
	    VertexShader = compile vs_2_0 vs_main();
	    PixelShader = compile ps_2_0 ps_main();
    }
    
    // Full brightness mode
    pass p1
    {
	    VertexShader = compile vs_2_0 vs_main();
	    PixelShader = compile ps_2_0 ps_fullbright();
    }
	
	// Normal with highlight
    pass p2
    {
	    VertexShader = compile vs_2_0 vs_main();
	    PixelShader = compile ps_2_0 ps_main_highlight();
    }
    
    // Full brightness mode with highlight
    pass p3
    {
	    VertexShader = compile vs_2_0 vs_main();
	    PixelShader = compile ps_2_0 ps_fullbright_highlight();
    }
}
