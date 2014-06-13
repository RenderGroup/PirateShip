
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

//variables para la iluminacion
float3 fvLightPosition = float3( -100.00, 140.00, 3000.00 );
float3 fvEyePosition = float3( 0.00, 0.00, -100.00 );
float k_la = 0.5;							// luz ambiente global
float k_ld = 0.6;							// luz difusa
float k_ls = 1.0;							// luz specular
float fSpecularPower = 16.84;				// exponente de la luz specular

float reflection; //Factor de reflexion del cielo en el agua
float time = 0;
float blendAmount = 0.65;  //Factor de translucides

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
   float3 Norm :     TEXCOORD1;		// Normales
   float3 Pos :      TEXCOORD2;		// Posicion real 3d
   float3 Pos2 :     TEXCOORD3;		// Posicion en 2d
   float3 WorldPosition : TEXCOORD4;
   float3 WorldNormal	: TEXCOORD5;
};

// ------------------------------------------------------------------

VS_OUTPUT vs_main( VS_INPUT Input )
{
   VS_OUTPUT Output;

   // Calculo la posicion real (en world space)
   float4 pos_real = mul(Input.Position,matWorld);

   // Y la propago usando las coordenadas de texturas 2 (*)
   Output.Pos = float3(pos_real.x,pos_real.y,pos_real.z);

    // Se establece el v�rtice transformado como nueva posici�n
    Output.Position = mul( Input.Position, matWorldViewProj);
    
	//Posicion pasada a World-Space
	Output.WorldPosition = mul(Input.Position, matWorld).xyz;
	//Pasar normal a World-Space
	Output.WorldNormal = mul(Input.Normal, matInverseTransposeWorld).xyz;

    Output.Pos2 = Output.Position;

	//cambia las coordenadas de textura para simular la caida del agua
    Input.Texcoord.y  +=  Input.Texcoord.y  + time / 3;

    //Propago las coordenadas de textura
    Output.Texcoord = Input.Texcoord;

    // Transformo la normal y la normalizo (si la escala no es uniforme usar la inversa Traspta)
    Output.Norm = normalize(mul(Input.Normal,matWorld));
    
    return( Output );  
   
}

VS_OUTPUT vs_hielo( VS_INPUT Input )
{
   VS_OUTPUT Output;

   // Calculo la posicion real (en world space)
   float4 pos_real = mul(Input.Position,matWorld);

   // Y la propago usando las coordenadas de texturas 2 (*)
   Output.Pos = float3(pos_real.x,pos_real.y,pos_real.z);

    // Se establece el v�rtice transformado como nueva posici�n
    Output.Position = mul( Input.Position, matWorldViewProj);
    
	//Posicion pasada a World-Space
	Output.WorldPosition = mul(Input.Position, matWorld).xyz;

	//Pasar normal a World-Space
	Output.WorldNormal = mul(Input.Normal, matInverseTransposeWorld).xyz;

    Output.Pos2 = Output.Position;

    //Propago las coordenadas de textura
    Output.Texcoord = Input.Texcoord;

    // Transformo la normal y la normalizo (si la escala no es uniforme usar la inversa Traspta)
    Output.Norm = normalize(mul(Input.Normal,matWorld));
    
    return( Output );  
   
}
// ------------------------------------------------------------------

//Pixel Shader blend
float4 ps_main( float3 Texcoord: TEXCOORD0, float3 N:TEXCOORD1, float3 Pos: TEXCOORD2, float3 Pos2 : TEXCOORD3, float3 WorldPosition : TEXCOORD4, float3 WorldNormal	: TEXCOORD5) : COLOR0
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

	float4 RGBColor = 0;

	//Normalizar vectores
	float3 Nn = normalize(WorldNormal);

	//Obtener texel de CubeMap
	float3 Vn = normalize(fvEyePosition - WorldPosition);
	float3 R = reflect(Vn, Nn);
    float3 reflectionColor = texCUBE(cubeMap, R).rgb;

	//agregar el reflejo del cielo a la textura del agua
	float4 MezclaTex = float4( (fvBaseColor.xyz * (1-reflection)) + (reflectionColor * reflection), 1.0f);	
   
    RGBColor.rgb = saturate(MezclaTex*(saturate(k_la+ld)) + le);
	RGBColor.a = blendAmount;

    return RGBColor;
}

//Pixel Shader 
float4 ps_hielo( float3 Texcoord: TEXCOORD0, float3 N:TEXCOORD1, float3 Pos: TEXCOORD2, float3 Pos2 : TEXCOORD3, float3 WorldPosition : TEXCOORD4, float3 WorldNormal	: TEXCOORD5) : COLOR0
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

	float4 RGBColor = 0;
    
	//saturar la textura para simular hielo
    RGBColor.rgb = saturate(fvBaseColor*2);

	RGBColor.a = 1.0;
    return RGBColor;
}

//Pixel Shader 
float4 ps_hieloNoche( float3 Texcoord: TEXCOORD0, float3 N:TEXCOORD1, float3 Pos: TEXCOORD2, float3 Pos2 : TEXCOORD3, float3 WorldPosition : TEXCOORD4, float3 WorldNormal	: TEXCOORD5) : COLOR0
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

	//agregar el reflejo del cielo a la textura del agua
	float4 MezclaTex = float4( (fvBaseColor.xyz * reflection) + (reflectionColor * (1-reflection)), 1.0f);	 
	fvBaseColor.rgb = saturate(MezclaTex*(saturate(k_la+ld)) + le);

	//Saturar y oscurecer la textura para simular la noche
	float4 fogColor =  float4(0.0f, 0.0f, 0.0f, 1.0f);
    fvBaseColor.rgb = saturate(fvBaseColor.rgb * 2);
	fvBaseColor =(fvBaseColor * 0.1) + ( fogColor * 0.9);

	fvBaseColor.a = 1.0;
    return fvBaseColor;
}


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
		  PixelShader = compile ps_2_0 ps_main();
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

