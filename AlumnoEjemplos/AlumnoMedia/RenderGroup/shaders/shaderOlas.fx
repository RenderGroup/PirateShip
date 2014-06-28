
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
bool llueve = false;

//variables para la iluminacion
float3 fvLightPosition = float3( -100.00, 140.00, 3000.00 );
float3 fvEyePosition = float3( 0.00, 0.00, -100.00 );
float k_la = 0.3;							// luz ambiente global
float k_ld = 0.6;							// luz difusa
float k_ls = 1.0;							// luz specular
float fSpecularPower = 16.84;				// exponente de la luz specular


float4 fogColor = float4(0.2f, 0.9f, 1.0f, 1.0f);
float fogStart = 2000;
float blendStart = 2000;
float blur_intensity; 

float reflection = 0.4;
float delta = 150.0;

float sangre = 8;

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
  float ola = frecuencia * sin(x/5 - time) * frecuencia * cos(z/5 - time);
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
  float3 posicionN = superficie(posicionActual.x, posicionActual.z + delta);
  float3 posicionE = superficie(posicionActual.x + delta, posicionActual.z);

  float3 vector1 = posicionN - posicionActual;
  float3 vector2 = posicionE - posicionActual;

 // return cross(vector1, vector2);
  return cross(posicionN, posicionE);
}

// vertex shader que anima la posicion de los vertices 
VS_OUTPUT vs_main( VS_INPUT Input )
{
   VS_OUTPUT Output;

   // Calculo la posicion real (en world space)
   float4 pos_real = mul(Input.Position,matWorld);

   // Y la propago usando las coordenadas de texturas 2
   Output.Pos = float3(pos_real.x,pos_real.y,pos_real.z);

    // Se aplica una transformación variable y periódica
  float3 pos = superficie(Input.Position.x, Input.Position.z);
  
  Input.Position.y = pos.y;
  Input.Normal =normalize(getNormal(Input.Position.xyz));  

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

// vertex shader
VS_OUTPUT vs_hielo( VS_INPUT Input )
{
   VS_OUTPUT Output;

   // Calculo la posicion real (en world space)
   float4 pos_real = mul(Input.Position,matWorld);

   // Y la propago usando las coordenadas de texturas 2 (*)
   Output.Pos = float3(pos_real.x,pos_real.y,pos_real.z);

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

	// calcula la luz diffusa
	float3 LD = normalize(fvLightPosition-float3(Pos.x,Pos.y,Pos.z));
	ld += saturate(dot(N, LD))*k_ld;
	
	// calcula la reflexion specular
	float3 D = normalize(float3(Pos.x,Pos.y,Pos.z)-fvEyePosition);
	float ks = saturate(dot(reflect(LD,N), D));
	ks = pow(ks,fSpecularPower);
	le += ks*k_ls;

	//Obtener el texel de textura
    float4 fvBaseColor= tex2D( diffuseMap, Texcoord);
	
	fvBaseColor.r= fvBaseColor.r * sangre;
	fvBaseColor.g= fvBaseColor.g / sangre;
	fvBaseColor.b= fvBaseColor.b / sangre;

	//calcular los factores de fog y alpha blending que actuan en profundidad
    fogfactor = saturate(( 3000.0f - Pos2.z ) / (fogStart)); // (fogEnd - z) /(fogEnd - fogStart)
    float blendfactor = saturate((5000.0f - Pos2.z ) / ( blendStart));

	float4 RGBColor = 0;

	//Normalizar vectores
	float3 Nn = normalize(WorldNormal);
	//Obtener texel de CubeMap
	float3 Vn = normalize(fvEyePosition - WorldPosition);
	float3 R = reflect(Vn, Nn);
    float3 reflectionColor = texCUBE(cubeMap, R).rgb;

	//reflectionColor.r= reflectionColor.r * sangre / 4;
	reflectionColor.g= reflectionColor.g / sangre;
	reflectionColor.b= reflectionColor.b / sangre;
      
    //agregar la reflexion del cielo y del fog al diffuse map
    fvBaseColor = float4( (fvBaseColor.xyz * (1-reflection)) + (reflectionColor * reflection),1.0f);
    fvBaseColor = (fvBaseColor *  fogfactor) + (fogColor * (1.0 -  fogfactor));

    RGBColor.rgb = saturate(fvBaseColor*(saturate(k_la+ld)) + le);
    RGBColor.a = blendfactor;

    return RGBColor;
}

//Pixel Shader de noche
float4 ps_Noche( float3 Texcoord: TEXCOORD0, float3 N:TEXCOORD1, float3 Pos: TEXCOORD2, float3 Pos2 : TEXCOORD3, float fogfactor : FOG, float3 WorldPosition : TEXCOORD4, float3 WorldNormal	: TEXCOORD5) : COLOR0
{      
	float ld = 0;		// luz difusa
	float le = 0;		// luz specular
	N = normalize(N);

	//calcula la luz diffusa
	float3 LD = normalize(fvLightPosition-float3(Pos.x,Pos.y,Pos.z));
	ld += saturate(dot(N, LD))*k_ld;
	
	//calcula la reflexion specular
	float3 D = normalize(float3(Pos.x,Pos.y,Pos.z)-fvEyePosition);
	float ks = saturate(dot(reflect(LD,N), D));
	ks = pow(ks,20);
	le += ks*k_ls;

	//Obtener el texel de textura
    float4 fvBaseColor= tex2D( diffuseMap, Texcoord);
	fvBaseColor.r= fvBaseColor.r * sangre / 2;
	fvBaseColor.g= fvBaseColor.g / sangre;
	fvBaseColor.b= fvBaseColor.b / sangre;

	//calcular el factor de fog y de alpha blending dependiendo de la posicion del vertice con la camara
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
    
	//unir las texturas del cielo, del diffuse map y sumar el color del fog  
    fvBaseColor = float4( (fvBaseColor.xyz * (1-reflection)) + (reflectionColor * reflection),1.0f);
    fvBaseColor = (fvBaseColor *  fogfactor) + (fogColor * (1.0 -  fogfactor));

    RGBColor.rgb = saturate(fvBaseColor*(saturate(k_la+ld)) + le);
    RGBColor.a = blendfactor;
    return RGBColor;
}

//Pixel Shader congelado
float4 ps_hielo( float3 Texcoord: TEXCOORD0, float3 N:TEXCOORD1, float3 Pos: TEXCOORD2, float3 Pos2 : TEXCOORD3, float fogfactor : FOG, float3 WorldPosition : TEXCOORD4, float3 WorldNormal	: TEXCOORD5) : COLOR0
{      
	float ld = 0;		// luz difusa
	float le = 0;		// luz specular
	N = normalize(N);

	// calcula la luz diffusa
	float3 LD = normalize(fvLightPosition-float3(Pos.x,Pos.y,Pos.z));
	ld += saturate(dot(N, LD))*k_ld;
	
	// calcula la reflexion specular
	float3 D = normalize(float3(Pos.x,Pos.y,Pos.z)-fvEyePosition);
	float ks = saturate(dot(reflect(LD,N), D));
	ks = pow(ks,fSpecularPower);
	le += ks*k_ls;

	//Normalizar vectores
	float3 Nn = normalize(WorldNormal);
	//Obtener texel de CubeMap
	float3 Vn = normalize(fvEyePosition - WorldPosition);
	float3 R = reflect(Vn, Nn);
    float3 reflectionColor = texCUBE(cubeMap, R).rgb;
		//reflectionColor.r= reflectionColor.r * sangre / 4;
	reflectionColor.g= reflectionColor.g / sangre;
	reflectionColor.b= reflectionColor.b / sangre;

    float blendfactor = saturate((5000.0f - Pos2.z ) / ( blendStart));

	//Obtener el texel de textura
    float4 fvBaseColor= tex2D( diffuseMap, Texcoord);
	float4 RGBColor = 0;

    //saturar las texturas para perder detalles y simular nieve
	reflectionColor.rgb = saturate(reflectionColor * 2);
    fvBaseColor.rgb = saturate(fvBaseColor * 2);

	//unir las texturas de diffuse  + cubemap
    fvBaseColor = float4( (fvBaseColor.xyz * 0.7) + (reflectionColor * 0.3),1.0f);

    RGBColor.rgb = saturate(fvBaseColor*(saturate(1.0 + ld)) + le);
    RGBColor.a = blendfactor;

    return RGBColor;
}

float4 ps_hieloNoche( float3 Texcoord: TEXCOORD0, float3 N:TEXCOORD1, float3 Pos: TEXCOORD2, float3 Pos2 : TEXCOORD3, float fogfactor : FOG, float3 WorldPosition : TEXCOORD4, float3 WorldNormal	: TEXCOORD5) : COLOR0
{      
	float ld = 0;		// luz difusa
	float le = 0;		// luz specular
	N = normalize(N);

	// calcula la luz diffusa
	float3 LD = normalize(fvLightPosition-float3(Pos.x,Pos.y,Pos.z));
	ld += saturate(dot(N, LD))*k_ld;
	
	// calcula la reflexion specular
	float3 D = normalize(float3(Pos.x,Pos.y,Pos.z)-fvEyePosition);
	float ks = saturate(dot(reflect(LD,N), D));
	ks = pow(ks,fSpecularPower);
	le += ks*k_ls;

    //Normalizar vectores
	float3 Nn = normalize(WorldNormal);

	//Obtener texel de CubeMap
	float3 Vn = normalize(fvEyePosition - WorldPosition);
	float3 R = reflect(Vn, Nn);
    float3 reflectionColor = texCUBE(cubeMap, R).rgb;


	//Obtener el texel de textura
    float4 fvBaseColor= tex2D( diffuseMap, Texcoord);
				//reflectionColor.r= reflectionColor.r * sangre / 4;
	fvBaseColor.g= fvBaseColor.g / sangre;
	fvBaseColor.b= fvBaseColor.b / sangre;

	float4 RGBColor = 0;

	//calcular el nivel de fog dependiendo de la distancia del vertice con la camara
    fogfactor = saturate(( 3000.0f - Pos2.z ) / (2500)); // (fogEnd - z) /(fogEnd - fogStart)
    fogColor =  float4(0.0f, 0.0f, 0.05f, 1.0f); //azul oscuro

	//saturar las texturas para perder detalles y simular nieve
    fvBaseColor.rgb = saturate(fvBaseColor * 2);
	reflectionColor.rgb = saturate(reflectionColor * 4); 

	//unir la textura del diffuse, del cubemap y agregar el fog
	fvBaseColor = float4( (fvBaseColor.xyz * 0.8) + (reflectionColor * 0.2),1.0f);
	fvBaseColor =(fvBaseColor * 0.2) + ( fogColor * 0.8);
    fvBaseColor = (fvBaseColor *  fogfactor) + (fogColor * (1.0 -  fogfactor));

	float blendfactor = saturate((5000.0f - Pos2.z ) / ( blendStart));
    RGBColor.rgb = saturate(fvBaseColor);
    RGBColor.a = blendfactor;

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

technique RenderSceneNocheCongelada
{
   pass Pass_0

   {
          AlphaBlendEnable =TRUE;
          DestBlend= INVSRCALPHA;
          SrcBlend= SRCALPHA;
		  VertexShader = compile vs_2_0 vs_hielo();
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
		  VertexShader = compile vs_2_0 vs_hielo();
		  PixelShader = compile ps_2_0 ps_hielo();
   }

}