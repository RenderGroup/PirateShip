
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

//Textura para calar el difuse map
texture texCalar;
sampler2D calar = sampler_state
{
	Texture = (texCalar);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

float blendStart = 2000;

float calado = 0.1;//nivel de calado, valores entre 0.0 (sin calar) y 1.0 (calado maximo, depende de la textura texCalar)

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

    // Se establece el v�rtice transformado como nueva posici�n
    Output.Position = mul( Input.Position, matWorldViewProj);

    Output.Pos2 = Output.Position;

    //Propago las coordenadas de textura
    Output.Texcoord = Input.Texcoord;
  
    return( Output );  
   
}

// ------------------------------------------------------------------

//Pixel Shader 
float4 ps_main( float3 Texcoord: TEXCOORD0, float3 Pos2 : TEXCOORD3) : COLOR0
{      
	//Obtener el texel de textura
    float4 fvBaseColor = tex2D( diffuseMap, Texcoord);
	float4 calarColor = tex2D( calar, Texcoord);

    float blendfactor = saturate(( 3000.0f - Pos2.z ) / (blendStart - 500));

	if ((calarColor.r > 0.7) && (calarColor.g < calado) && (calarColor.b < calado))
	{
		   fvBaseColor.a = 0.0f;//descarta el pixel y no se ve por pantalla
    }
    else
    {
           fvBaseColor.a = blendfactor;
	}
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

