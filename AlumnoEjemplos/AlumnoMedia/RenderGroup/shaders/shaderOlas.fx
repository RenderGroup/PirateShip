
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


//Textura para perlin noise
texture texPerlin;
sampler2D perlin = sampler_state
{
	Texture = (texPerlin);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

// Textura para heightmap
texture aux_Tex;
sampler2D heightMap =
sampler_state
{
   Texture = (aux_Tex);
   ADDRESSU = MIRROR;
   ADDRESSV = MIRROR;
   MINFILTER = LINEAR;
   MAGFILTER = LINEAR;
   MIPFILTER = LINEAR;
};

// enviroment map
texture  g_txCubeMap;
samplerCUBE g_samCubeMap = 
sampler_state
{
    Texture = <g_txCubeMap>;

   MINFILTER = LINEAR;
   MAGFILTER = LINEAR;
   MIPFILTER = LINEAR;
};

float time = 0;
bool rayo = false;
float blendAmount = 0.65;//nivel de translucides entre 0 y 1 , cero translucido, 1 opaco

//variables para la iluminacion
float3 fvLightPosition = float3( -100.00, 100.00, -100.00 );
float3 fvEyePosition = float3( 0.00, 0.00, -100.00 );
float k_la = 0.3;							// luz ambiente global
float k_ld = 0.9;							// luz difusa
float k_ls = 0.4;							// luz specular
float fSpecularPower = 16.84;				// exponente de la luz specular

float4 fogColor = float4(0.2f, 0.9f, 1.0f, 1.0f);

float blur_intensity; 
bool camara3p = true;

float4 blur(float3 Texcoord: TEXCOORD0)
{
	//Obtener color de textura
	float4 color = tex2D( diffuseMap, Texcoord );
	
	//Tomar samples adicionales de texels vecinos y sumarlos (formamos una cruz)
	color += tex2D( diffuseMap, float2(Texcoord.x + blur_intensity, Texcoord.y));
	color += tex2D( diffuseMap, float2(Texcoord.x - blur_intensity, Texcoord.y));
	color += tex2D( diffuseMap, float2(Texcoord.x, Texcoord.y + blur_intensity));
	color += tex2D( diffuseMap, float2(Texcoord.x, Texcoord.y - blur_intensity));
	
	//Promediar todos
	color = color / 5;

      return color;
}


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
   float3 Norm :     TEXCOORD1;			// Normales
   float3 Pos :      TEXCOORD2;		// Posicion real 3d
   float3 Pos2 :     TEXCOORD3;		// Posicion en 2d
   float fogfactor:  FOG;
};

// ------------------------------------------------------------------

// vertex shader que anima la posicion de los vertices 
VS_OUTPUT vs_main( VS_INPUT Input )
{
   VS_OUTPUT Output;

   // Calculo la posicion real (en world space)
   float4 pos_real = mul(Input.Position,matWorld);

   // Y la propago usando las coordenadas de texturas 2 (*)
   Output.Pos = float3(pos_real.x,pos_real.y,pos_real.z);

    // Se aplica una transformación variable y periódica
    Input.Position.y = Input.Position.y  * (cos(time) + 1.2) ;
   
    // Se establece el vértice transformado como nueva posición
    Output.Position = mul( Input.Position, matWorldViewProj);
    

    Output.Pos2 = Output.Position;
    Output.fogfactor = saturate(Output.Position.z);


    Input.Texcoord.y  +=  Input.Texcoord.y  * abs(cos(time/5)) + 1.2;

    //Propago las coordenadas de textura
    Output.Texcoord = Input.Texcoord;
   
    // Transformo la normal y la normalizo (si la escala no es uniforme usar la inversa Traspta)
    Output.Norm = normalize(mul(Input.Normal,matWorld));
    
    return( Output );  
   
}

// ------------------------------------------------------------------

//Pixel Shader blend
float4 ps_main( float3 Texcoord: TEXCOORD0, float3 N:TEXCOORD1, float3 Pos: TEXCOORD2, float3 Pos2 : TEXCOORD3, float fogfactor : FOG) : COLOR0
{      
	float ld = 0;		// luz difusa
	float le = 0;		// luz specular
	N = normalize(N);

	// 1- calcula la luz diffusa
	float3 LD = normalize(fvLightPosition-float3(Pos.x,Pos.y,Pos.z));
	ld += saturate(dot(N, LD))*k_ld;
	
	// 2- calcula la reflexion specular
	float3 D = normalize(float3(Pos.x,Pos.y,Pos.z)-fvEyePosition);
	float ks = saturate(dot(reflect(LD,N), D));
	ks = pow(ks,fSpecularPower);
	le += ks*k_ls;

	//Obtener el texel de textura
        float4 fvBaseColor= tex2D( diffuseMap, Texcoord);

        fogfactor = saturate(( 3000.0f - Pos2.z ) / (2000.0f));

      	//suma luz diffusa, ambiente y especular
	float4 RGBColor = 0;
        //RGBColor.rgb = saturate(fvBaseColor*(saturate(k_la+ld)) + le);

        if (rayo)
        {
           RGBColor = fvBaseColor;
           RGBColor.rgb = saturate(RGBColor*(saturate(k_la+ld)) + le);
           RGBColor.rgb = RGBColor.rgb*5;

        }
        else
        {
           if(camara3p) {
      
                    RGBColor =  blur(Texcoord); 
                    RGBColor = (RGBColor * fogfactor) + (fogColor * (1.0 - fogfactor));
                    fvBaseColor = (RGBColor * (1.0 - fogfactor)) + (fvBaseColor * fogfactor);
                    RGBColor.rgb = saturate(fvBaseColor*(saturate(k_la+ld)) + le);
                    RGBColor.a = fogfactor;//aplica la transparencia dinamica

           }
           else     {
                    RGBColor.rgb = saturate(fvBaseColor*(saturate(k_la+ld)) + le);
                    RGBColor.a = blendAmount; //aplica la transparencia estatica
                    }
          
       }
       return RGBColor;
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

