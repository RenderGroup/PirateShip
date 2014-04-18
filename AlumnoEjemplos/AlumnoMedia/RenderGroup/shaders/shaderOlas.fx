// ---------------------------------------------------------
// Ejemplo shader Minimo:
// ---------------------------------------------------------

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

float time = 0;

float blendAmount=0.7;//nivel de translucides entre 0 y 1 , cero translucido, 1 opaco




/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT 
{
   float4 Position : POSITION0;
   float4 Color :  COLOR0;
  // float4 Normal : NORMAL0;
   float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT 
{
   float4 Position : POSITION0;
   float4 Color :  COLOR0;
  
   float2 Texcoord : TEXCOORD0;
};



//Vertex Shader
VS_OUTPUT vs_main( VS_INPUT Input )
{
   VS_OUTPUT Output;
   //Proyectar posicion
  // Output.Position = mul( Input.Position, matWorldViewProj);


    // Change the position vector to be 4 units for proper matrix calculations.
    Input.Position.w = 1.0f;
  
  // Calculate the position of the vertex against the world, view, and projection matrices.
  //  Output.Position = mul(Input.Position, matWorld);
   // Output.Position = mul(Output.Position, matWorldView);
    Output.Position = mul(Input.Position, matWorldViewProj);


   
   //Propago las coordenadas de textura
   Output.Texcoord = Input.Texcoord;

   //Propago el color x vertice
   Output.Color = Input.Color;

   return( Output );
   
}


// Ejemplo de un vertex shader que anima la posicion de los vertices 
// ------------------------------------------------------------------
VS_OUTPUT vs_main2( VS_INPUT Input )
{
   VS_OUTPUT Output;


   // Animar posicion
    //Input.Position.x += sin(time)*30*sign(Input.Position.x);
    //Input.Position.y += cos(time)*30*sign(Input.Position.y-20);
    //Input.Position.z += sin(time)*30*sign(Input.Position.z);
     

     Input.Position.y = Input.Position .y * time;

   //Proyectar posicion
   Output.Position = mul( Input.Position, matWorldViewProj);
   
   //Propago las coordenadas de textura
   Output.Texcoord = Input.Texcoord;

	// Animar color
   Input.Color.r = abs(sin(time));
   Input.Color.g = abs(cos(time));
   
   //Propago el color x vertice
   Output.Color = Input.Color;

   return( Output );
   
}


// Ejemplo de un vertex shader que anima la posicion de los vertices 
// ------------------------------------------------------------------
VS_OUTPUT vs_main3( VS_INPUT Input )
{
   VS_OUTPUT Output;

    // Se aplica una transformación variable y periódica

    Input.Position.y = Input.Position.y  * (cos(time) + 1.2) ;

    // Se establece el vértice transformado como nueva posición

    Output.Position = mul( Input.Position, matWorldViewProj);



      Input.Texcoord.y  +=  Input.Texcoord.y  * abs(cos(time/5)) + 1.2  ;

   //Input.Texcoord.y = Input.Texcoord.y  * abs(cos(time/10)) + 1.2 ; //mueve la textura
  

 //Propago las coordenadas de textura

    Output.Texcoord = Input.Texcoord;

   //Propago el color x vertice
   Output.Color = Input.Color;

   return( Output );  
   
}

// Ejemplo de un vertex shader que anima la posicion de los vertices 
// ------------------------------------------------------------------
VS_OUTPUT vs_main4( VS_INPUT Input )
{
   VS_OUTPUT Output;


    // Se aplica una transformación variable y periódica

 Input.Position.y = Input.Position.y  *(cos(time) + 1.2 );

   if(Input.Position.x > 0)
  {
     Input.Position.x -= Input.Position.x * cos(time) + 10.2  ;
  }

  if(Input.Position.x <= 0)
   {
      Input.Position.x += Input.Position.x * cos(time) + 10.2 ; 
  }


    // Se aplica establece el vértice transformado como posición

    Output.Position = mul( Input.Position, matWorldViewProj);
   
   // Input.Texcoord.x = Input.Texcoord.x * 0.2; //mueve la textura
  
 //Propago las coordenadas de textura
  // Output.Texcoord = mul( Input.Texcoord, matWorldViewProj);

    Output.Texcoord = Input.Texcoord.x ;

   //Propago el color x vertice
   Output.Color = Input.Color;

   return( Output );  
   
}

//Pixel Shader
float4 ps_main( float2 Texcoord: TEXCOORD0, float4 Color:COLOR0) : COLOR0
{      
	// Obtener el texel de textura
	// diffuseMap es el sampler, Texcoord son las coordenadas interpoladas
	float4 fvBaseColor = tex2D( diffuseMap, Texcoord );
	// combino color y textura
	// en este ejemplo combino un 80% el color de la textura y un 20%el del vertice
	//return 0.8*fvBaseColor + 0.2*Color;
       return fvBaseColor;
        //return (Color.r * 0.222 + Color.g * 0.707 + Color.b * 0.071);
}



//Pixel Shader blend
float4 ps_main2( float2 Texcoord: TEXCOORD0, float4 Color:COLOR0) : COLOR0
{      
      float4 fvBaseColor = tex2D( diffuseMap, Texcoord );
      float4 color; 
      color = 0.8*fvBaseColor + 0.2*Color;
      color.a= blendAmount;
      return color;

}


// ------------------------------------------------------------------
technique RenderScene
{
   pass Pass_0

   {
          AlphaBlendEnable =TRUE;
          DestBlend= INVSRCALPHA;
          SrcBlend= SRCALPHA;
	  VertexShader = compile vs_2_0 vs_main3();
	  PixelShader = compile ps_2_0 ps_main2();
   }

}
technique RenderScene2
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_main4();
	  PixelShader = compile ps_2_0 ps_main();
   }

}
