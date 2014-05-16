
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
bool rayo = false;
float blendAmount = 0.65;//nivel de translucides entre 0 y 1 , cero translucido, 1 opaco
bool llueve = false;

//variables para la iluminacion
float3 fvLightPosition = float3( 0.00, 400.00, 0.00 );
float3 fvEyePosition;
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

//Factor de reflexion del cielo en el agua
float reflection;

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
   float3 Reflexion: TEXCOORD6;
};

// ------------------------------------------------------------------

float3 superficie(float x, float z)
{
  float y;
  float frecuencia = 10;
  float ola   = frecuencia   * sin(x/5 - time  ) *  frecuencia   * cos(z/5 - time  );
  float olita;

      if (llueve)
        olita = (frecuencia/2.5) * cos(x - time*8) * (frecuencia/2  )* sin(z - time*8);
      else
        olita = (frecuencia/3  ) * cos(x - time*3) * (frecuencia/2.5)* sin(z - time*3);  //mar agitado


  y = ola + olita + 90;
  return float3(x,y,z);
}

float3 getNormal(float3 posicionActual)
{

  float delta = 1;
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

  float3 pos = superficie(Input.Position.x, Input.Position.z);
  //Input.Position.y = Input.Position.x  * (cos(time) + 1.2) ;
  Input.Position.y = pos.y;

  Input.Normal = getNormal(Input.Position.xyz);
  float3 normal = normalize(Input.Normal);

  // Se establece el vértice transformado como nueva posición
  Output.Position = mul(Input.Position, matWorldViewProj);
	//Posicion pasada a World-Space
	Output.WorldPosition = mul(Input.Position, matWorld).xyz;

//#########################################################################################

  float3 v = normalize(Input.Position.xyz - fvEyePosition.xyz);
  // vector reflejado
  Output.Reflexion = reflect(v,normal);

//#########################################################################################

	//Pasar normal a World-Space
	Output.WorldNormal = mul(Input.Normal, matInverseTransposeWorld).xyz;

    Output.Pos2 = Output.Position;
    Output.fogfactor = saturate(Output.Position.z);

  Input.Texcoord.y  +=  Input.Texcoord.y  * abs(cos(time/6)) + 1.2;

    //Propago las coordenadas de textura
    Output.Texcoord = Input.Texcoord;
   
    // Transformo la normal y la normalizo (si la escala no es uniforme usar la inversa Traspta)
    Output.Norm = normalize(mul(Input.Normal,matWorld));
    
    return( Output );  
   
}

/*//Input del Vertex Shader
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
   float3 Norm :     TEXCOORD1;     // Normales
   float3 Pos :      TEXCOORD2;   // Posicion real 3d
   float3 Pos2 :     TEXCOORD3;   // Posicion en 2d
   float fogfactor:  FOG;
   float3 WorldPosition : TEXCOORD4;
   float3 WorldNormal : TEXCOORD5;
   float3 Reflexion
};*/

// ------------------------------------------------------------------

//Pixel Shader blend
float4 ps_main(	VS_OUTPUT In) : COLOR0
{      
	float ld = 0;		// luz difusa
	float le = 0;		// luz specular
	float3 N = normalize(In.WorldNormal);

	// 1- calcula la luz diffusa           K_ld*diffuseColor*(N dot L)
	float3 L = normalize(fvLightPosition - In.WorldPosition);
	ld += k_ld * saturate(dot(N, L));
	
	// 2- calcula la reflexion specular    K_ls*specularColor*(R dot V)´´shininess
	float3 V = normalize(In.Pos - fvEyePosition);
	float RxV = saturate(dot(In.Reflexion, V));
	le += k_ls * pow(RxV,fSpecularPower);

	//Obtener el texel de textura
      float4 fvBaseColor= tex2D( diffuseMap, In.Texcoord);

  In.fogfactor = saturate(( 3000.0f - In.Pos2.z ) / (fogStart));
  //	float	blurfactor = saturate(( 3000.0f - Pos2.z ) / (blurStart));
	float	blendfactor = saturate(( 3000.0f - In.Pos2.z ) / (blendStart));

	float4 RGBColor = 0;

	//Obtener texel de CubeMap
  float3 reflectionColor = texCUBE(cubeMap, In.Reflexion).rgb;

        if (rayo)
        {
           RGBColor = fvBaseColor;
           RGBColor.rgb = saturate(RGBColor*(saturate(k_la+ld)) + le);
           RGBColor.rgb = RGBColor.rgb*5;
		    RGBColor.a = blendAmount; //aplica la transparencia estatica
        }
        else
        {
           if(camara3p) {
      
                    RGBColor =  fvBaseColor;//blur(In.Texcoord); 
                    RGBColor = (RGBColor *  In.fogfactor) + (fogColor * (1.0 -  In.fogfactor));
                    fvBaseColor = (RGBColor * (1.0 - In.fogfactor)) + (fvBaseColor * In.fogfactor);
                    RGBColor.rgb = saturate(fvBaseColor*(saturate(k_la+ld)) + le);
                    RGBColor.a = blendfactor;//aplica la transparencia dinamica
           }
           else     {
		            float4 MezclaTex = float4( (fvBaseColor.xyz * (1-reflection)) + (reflectionColor * reflection), 1.0f);	
                    RGBColor.rgb = saturate(MezclaTex*(saturate(k_la+ld)) + le);
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

