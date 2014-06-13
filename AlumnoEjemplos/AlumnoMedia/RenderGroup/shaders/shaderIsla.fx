
/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

float4 fogColor = float4(0.0f, 0.0f, 0.0f, 1.0f);
float fogStart = 2000;

/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT 
{
   float4 Position : POSITION0;
   float3 Normal : NORMAL0;
   float4 Color :  COLOR0;
   float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT 
{
   float4 Position : POSITION0;
   float2 Texcoord : TEXCOORD0;
   float3 Pos2 :     TEXCOORD3;		// Posicion en 2d
};

// ------------------------------------------------------------------

// vertex shader  
VS_OUTPUT vs_main( VS_INPUT Input )
{
   VS_OUTPUT Output;

    // Se establece el vértice transformado como nueva posición
    Output.Position = mul( Input.Position, matWorldViewProj);

    Output.Pos2 = Output.Position;

    //Propago las coordenadas de textura
    Output.Texcoord = Input.Texcoord;
  
    return( Output );  
   
}
// ------------------------------------------------------------------
//Pixel Shader dia
float4 ps_main( float3 Texcoord: TEXCOORD0, float3 Pos2 : TEXCOORD3) : COLOR0
{      
	//Obtener el texel de textura
    float4 fvBaseColor = tex2D( diffuseMap, Texcoord);
    return fvBaseColor;
}

//Pixel Shader  noche
float4 ps_Noche( float3 Texcoord: TEXCOORD0, float3 Pos2 : TEXCOORD3 ) : COLOR0
{      
	//Obtener el texel de textura
    float4 fvBaseColor = tex2D( diffuseMap, Texcoord);
	float4 negro =  float4(0.0f, 0.0f, 0.0f, 1.0f);
	
	//oscurecer la textura para simular la noche
	fvBaseColor =(fvBaseColor * 0.4) + ( negro * 0.6);

	//calcular el nivel de fog dependiendo de la distancia del vertice respecto de la camara
    float fogfactor = saturate(( 3000.0f - Pos2.z ) / (fogStart - 500));

    fvBaseColor = (fvBaseColor * fogfactor) + (fogColor *  (1.0 - fogfactor));
    fvBaseColor.rgb = saturate(fvBaseColor);
    return fvBaseColor;
}

//Pixel Shader congelado
float4 ps_hielo( float3 Texcoord: TEXCOORD0, float3 Pos2 : TEXCOORD3) : COLOR0
{      
	//Obtener el texel de textura
    float4 fvBaseColor = tex2D( diffuseMap, Texcoord);
    fogColor =  float4(1.0f, 1.0f, 1.0f, 1.0f);

	//saturar la textura para que sea mas blanca
    fvBaseColor.rgb = saturate(fvBaseColor * 4);
	fvBaseColor =(fvBaseColor * 0.4) + ( fogColor * 0.6);

	fvBaseColor.a = 1.0;
    return fvBaseColor;
}     

//Pixel Shader congelado
float4 ps_hieloNoche( float3 Texcoord: TEXCOORD0, float3 Pos2 : TEXCOORD3) : COLOR0
{      
	//Obtener el texel de textura
    float4 fvBaseColor = tex2D( diffuseMap, Texcoord);
    fogColor =  float4(0.0f, 0.0f, 0.0f, 1.0f);

	//oscurecer la textura 
    fvBaseColor.rgb = saturate(fvBaseColor * 4);
	fvBaseColor =(fvBaseColor * 0.1) + ( fogColor * 0.9);

	fvBaseColor.a = 1.0;
    return fvBaseColor;
}  
// ------------------------------------------------------------------
technique RenderScene
{
   pass Pass_0

   {
          AlphaBlendEnable =TRUE;
          DestBlend= INVSRCALPHA;
          SrcBlend= SRCALPHA;
		  VertexShader = compile vs_2_0 vs_main();
		  PixelShader = compile ps_2_0 ps_main();
   }

}

technique RenderSceneNoche
{
   pass Pass_0

   {
          AlphaBlendEnable =TRUE;
          DestBlend= INVSRCALPHA;
          SrcBlend= SRCALPHA;
		  VertexShader = compile vs_2_0 vs_main();
		  PixelShader = compile ps_2_0 ps_Noche();
   }

}
technique RenderSceneNocheCongelada
{
   pass Pass_0

   {
          AlphaBlendEnable =TRUE;
          DestBlend= INVSRCALPHA;
          SrcBlend= SRCALPHA;
		  VertexShader = compile vs_2_0 vs_main();
		  PixelShader = compile ps_2_0 ps_hieloNoche();
   }

}
technique RenderSceneCongelada
{
   pass Pass_0

   {
          AlphaBlendEnable =TRUE;
          DestBlend= INVSRCALPHA;
          SrcBlend= SRCALPHA;
		  VertexShader = compile vs_2_0 vs_main();
		  PixelShader = compile ps_2_0 ps_hielo();
   }

}