
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

//Textura utilizada para EnvironmentMap
texture texCubeMap;
samplerCUBE cubeMap = sampler_state
{
	Texture = (texCubeMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

float time = 0;
float blendAmount = 0.65;//nivel de translucides entre 0 y 1 , cero translucido, 1 opaco
bool llueve = false;

//variables para la iluminacion
float3 fvLightPosition = float3( -100.00, 100.00, -100.00 );
float3 fvEyePosition = float3( 0.00, 0.00, -100.00 );
float k_la = 0.3;							// luz ambiente global
float k_ld = 0.6;							// luz difusa
float k_ls = 1.0;							// luz specular
float fSpecularPower = 16.84;				// exponente de la luz specular

float4 fogColor = float4(0.2f, 0.9f, 1.0f, 1.0f);
float fogStart = 2000;
float blurStart = 2000;
float blendStart = 2000;

float blur_intensity; 
bool camara3p = true;
float reflection = 0.4;
float delta = 150.0;

float4 blur(float2 Texcoord: TEXCOORD0)
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
   float3 WorldPosition : TEXCOORD4;
   float3 WorldNormal	: TEXCOORD5;
};

// ------------------------------------------------------------------
float3 superficie(float x, float z)
{
  float y;
  float frecuencia = 10;
  float ola   = frecuencia   * sin(x/5 - time  ) *  frecuencia   * cos(z/2 - time  );
  float olita;

      if (llueve)
        olita = (frecuencia/2) * cos(x - time*8) * (frecuencia/2  )* sin(z - time*8);
      else
        olita = (frecuencia/2.5  ) * cos(x - time*3) * (frecuencia/2.5)* sin(z - time*3);  //mar agitado


  y = ola + olita + 90;
  return float3(x,y,z);
}

float3 getNormal(float3 posicionActual)
{


  float3 posicionN = superficie(posicionActual.x, posicionActual.z + delta);
  float3 posicionE = superficie(posicionActual.x + delta, posicionActual.z);

  float3 vector1 = posicionN - posicionActual;

  float3 vector2 = posicionE - posicionActual;

  return cross(posicionN, posicionE);

}

// vertex shader que anima la posicion de los vertices 
VS_OUTPUT vs_main( VS_INPUT Input )
{
   VS_OUTPUT Output;

   // Calculo la posicion real (en world space)
   float4 pos_real = mul(Input.Position,matWorld);

   // Y la propago usando las coordenadas de texturas 2 (*)
   Output.Pos = float3(pos_real.x,pos_real.y,pos_real.z);

    // Se aplica una transformación variable y periódica
    //Input.Position.y = Input.Position.y  * (cos(time) + 1.2) ;
  float3 pos = superficie(Input.Position.x, Input.Position.z);
  Input.Position.y = pos.y;

      Input.Normal =normalize(getNormal(Input.Position.xyz));
  float3 normal = normalize(Input.Normal);   

    // Se establece el vértice transformado como nueva posición
    Output.Position = mul( Input.Position, matWorldViewProj);

	//Posicion pasada a World-Space
	Output.WorldPosition = mul(Input.Position, matWorld).xyz;
	//Pasar normal a World-Space
	Output.WorldNormal =  mul(Input.Normal, matInverseTransposeWorld).xyz;

    Output.Pos2 = Output.Position;
    Output.fogfactor = saturate(Output.Position.z);

    Input.Texcoord.y  +=  Input.Texcoord.y  + time / 8;

    //Propago las coordenadas de textura
    Output.Texcoord = Input.Texcoord;
   
    // Transformo la normal y la normalizo (si la escala no es uniforme usar la inversa Traspta)
    Output.Norm = normalize(mul(Input.Normal,matWorld));

    return( Output );  
   
}

// ------------------------------------------------------------------

//Pixel Shader blend
float4 ps_main( float3 Texcoord: TEXCOORD0, float3 N:TEXCOORD1, float3 Pos: TEXCOORD2, float3 Pos2 : TEXCOORD3, float fogfactor : FOG, float3 WorldPosition : TEXCOORD4, float3 WorldNormal	: TEXCOORD5) : COLOR0
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

   fogfactor = saturate(( 3000.0f - Pos2.z ) / (fogStart)); // (fogEnd - z) /(fogEnd - fogStart)
   float blendfactor = saturate((5000.0f - Pos2.z ) / ( blendStart));

	float4 RGBColor = 0;

	//Normalizar vectores
	float3 Nn = normalize(WorldNormal);
	//Obtener texel de CubeMap
	float3 Vn = normalize(fvEyePosition - WorldPosition);
	float3 R = reflect(Vn, Nn);
    float3 reflectionColor = texCUBE(cubeMap, R).rgb;

    if(camara3p) {
      
            fvBaseColor = float4( (fvBaseColor.xyz * (1-reflection)) + (reflectionColor * reflection),1.0f);// blur(Texcoord); 
            fvBaseColor = (fvBaseColor *  fogfactor) + (fogColor * (1.0 -  fogfactor));

            RGBColor.rgb = saturate(fvBaseColor*(saturate(k_la+ld)) + le);

            RGBColor.a = blendfactor; //1.0 -  fogfactor;
    }
    else     {
		    float4 MezclaTex =  float4( (fvBaseColor.xyz * (1-reflection)) + (reflectionColor * reflection), 1.0f);
            RGBColor.rgb = saturate(MezclaTex*(saturate(k_la+ld)) + le);
			RGBColor.a = blendAmount; //aplica la transparencia estatica
            }
       return RGBColor;
}

//Pixel Shader de noche
float4 ps_Noche( float3 Texcoord: TEXCOORD0, float3 N:TEXCOORD1, float3 Pos: TEXCOORD2, float3 Pos2 : TEXCOORD3, float fogfactor : FOG, float3 WorldPosition : TEXCOORD4, float3 WorldNormal	: TEXCOORD5) : COLOR0
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
	ks = pow(ks,20);
	le += ks*k_ls;

	//Obtener el texel de textura
    float4 fvBaseColor= tex2D( diffuseMap, Texcoord);

   fogfactor = saturate(( 3000.0f - Pos2.z ) / (fogStart)); // (fogEnd - z) /(fogEnd - fogStart)
   float blendfactor = saturate((5000.0f - Pos2.z ) / ( blendStart));

	float4 RGBColor = 0;

	//Normalizar vectores
	float3 Nn = normalize(WorldNormal);
	//Obtener texel de CubeMap
	float3 Vn = normalize(fvEyePosition - WorldPosition);
	float3 R = reflect(Vn, Nn);
    float3 reflectionColor = texCUBE(cubeMap, R).rgb;

    fogColor = float4(0.0f, 0.0f, 0.0f, 1.0f);

    if(camara3p) {
      
            fvBaseColor = float4( (fvBaseColor.xyz * (1-reflection)) + (reflectionColor * reflection),1.0f);
            fvBaseColor = (fvBaseColor *  fogfactor) + (fogColor * (1.0 -  fogfactor));

            RGBColor.rgb = saturate(fvBaseColor*(saturate(k_la+ld)) + le);

            RGBColor.a = blendfactor;
    }
    else     {
		    float4 MezclaTex =  float4( (fvBaseColor.xyz * (1-reflection)) + (reflectionColor * reflection), 1.0f);
            RGBColor.rgb = saturate(MezclaTex*(saturate(k_la+ld)) + le);
			RGBColor.a = blendAmount; //aplica la transparencia estatica
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