using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.Collision.ElipsoidCollision;
using TgcViewer.Utils.Shaders;
using System.Windows.Forms;
using TgcViewer.Utils._2D;

namespace AlumnoEjemplos.RenderGroup
{
    /// <summary>
    /// TP del barco pirata que lucha a ca�onazos en mar
    /// </summary>
    public class EjemploAlumno : TgcExample
    {

        BarcoProtagonista barcoProtagonista;
        Barco b1, b2, b3;

        #region DECLARACIONES DEL ESCENARIO

        TgcSkyBox skyBox;

        TgcBox piso;
        SmartTerrain terrain;
        TgcSimpleTerrain terrain2;
        Microsoft.DirectX.Direct3D.Effect efectoOlas;

        string currentHeightmap;
        string currentTexture;
        string currentHeightmap2;
        string currentTexture2;
        float currentScaleXZ = 100f;
        float currentScaleY = 1.3f;
        TgcBox lightMesh;
        TgcSprite boton1;
        TgcSprite boton2;

        #endregion

        #region TEXTO PARA EL FRAMEWORK
        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "RenderGroup";
        }

        public override string getDescription()
        {
            return "Movimiento con W, A, S, D; Disparar con 4 y 6";
        }

        #endregion

        public override void init()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            #region INICIALIZACIONES PISO

            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Texturas\\SkyBoxLostAtSeaDay\\Down.jpg");
            piso = TgcBox.fromSize(new Vector3(8500, 1, 8500), pisoTexture);

            #endregion

            //Mesh para la luz
            lightMesh = TgcBox.fromSize(new Vector3(10, 10, 10), Color.Red);
            crearHeightmaps();

            crearSkybox();
            cargarShaders();
            crearModifiers();
            crearUserVars();
            crearSprites();

            #region INICIALIZACIONES BARCO

            barcoProtagonista = ConstructorDeBarcos.ConstruirProtagonista(new Vector2(0, -930f));
            b1 = ConstructorDeBarcos.ConstruirEnemigo(new Vector2(500, 500));
            b2 = ConstructorDeBarcos.ConstruirEnemigo(new Vector2(-700, 960));
            b3 = ConstructorDeBarcos.ConstruirEnemigo(new Vector2(100, 880));

            InputManager.Add(barcoProtagonista);

            #endregion

        }


        public override void render(float elapsedTime)
        {
            InputManager.ManejarInput();

            setUsersVars();

            recargarHeightMap();

            setShadersValues();

            renderizar();
            coordenadasMouse();
        }

        public override void close()
        {
            barcoProtagonista.dispose();
            piso.dispose();
            terrain.dispose();
            efectoOlas.Dispose();
            terrain2.dispose();
            boton1.dispose();
            boton2.dispose();
        }

        #region NUEVO

        private void cargarShaders()
        {
            efectoOlas = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\shaderOlas.fx");
            terrain.Effect = efectoOlas;
            //terrain.Effect = TgcShaders.loadEffect("C:\\Users\\Julio\\Desktop\\materias\\TGC\\TgcViewer\\Examples\\Shaders\\WorkshopShaders\\Shaders\\BasicShader.fx");
            terrain.Technique = "RenderScene";
        }

        private void coordenadasMouse() //se fija si hace clic sobre un boton
        {
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            //Obtener variacion XY del mouse
            float mouseX = 0f;
            float mouseY = 0f;
            float botonX = boton1.Position.X + boton1.Texture.Width;
            float botonY = boton1.Position.Y + boton1.Texture.Height;

            float boton2X = boton2.Position.X + boton2.Texture.Width;
            float boton2Y = boton2.Position.Y + boton2.Texture.Height;

            if (d3dInput.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                mouseX = d3dInput.Xpos;// XposRelative;
                mouseY = d3dInput.Ypos;// YposRelative;

                if ((mouseX > boton1.Position.X) && (mouseX < botonX) && (mouseY > boton1.Position.Y) && (mouseY < botonY))
                {
                    MessageBox.Show("CLIC EN SPRITE CUADRADO DERECHO");
                }
                if ((mouseX > boton2.Position.X) && (mouseX < boton2X) && (mouseY > boton2.Position.Y) && (mouseY < boton2Y))
                {
                    MessageBox.Show("CLIC EN SPRITE CUADRADO IZQUIERDO");
                }
            }
        }

        public void crearHeightmaps()
        {
            currentHeightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\piedra1.jpg";
            currentTexture = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\\\texturas\\color_agua5.png";
            terrain = new SmartTerrain();
            terrain.loadHeightmap(currentHeightmap, 100f, 1.6f, new Vector3(0, 0, 0)); //150f, 1.3f, new Vector3(0, 5, 0));
            terrain.loadTexture(currentTexture);

            currentHeightmap2 = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\piedra2.jpg";
            currentTexture2 = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\piedras.png";
            terrain2 = new TgcSimpleTerrain();
            terrain2.loadHeightmap(currentHeightmap2, 30f,2.3f, new Vector3(50, 0, 30));
            terrain2.loadTexture(currentTexture2);

            GuiController.Instance.UserVars.addVar("terreno", terrain); //NO TOCAR LINEA - HERE BE DRAGONS - EL TP EXPLOTA
        }

        private void crearModifiers()
        {
            GuiController.Instance.Modifiers.addBoolean("showBoundingBox", "Bouding Box", false); //
            //modifiers para la camara
            GuiController.Instance.Modifiers.addBoolean("camaraEnBarco", "Camara 3� persona", true);//
            //modifiers para el mar
            GuiController.Instance.Modifiers.addFloat("XZ", 0.1f, 1000f, currentScaleXZ); //modifica el tama�o del terreno (mar)
            GuiController.Instance.Modifiers.addFloat("Y", 0.1f, 10f, currentScaleY); //modifica la altura de las olas
            GuiController.Instance.Modifiers.addVertex3f("LightPosition", new Vector3(-100, -100, -100), new Vector3(1000, 4000, 5000), new Vector3(-100, 140, 3000));
            GuiController.Instance.Modifiers.addFloat("Ambient", 0, 1, 0.5f);
            GuiController.Instance.Modifiers.addFloat("Diffuse", 0, 1, 0.6f);
            GuiController.Instance.Modifiers.addFloat("Specular", 0, 1, 1f);
            GuiController.Instance.Modifiers.addFloat("SpecularPower", 1, 2000, 20);
           //modifiers para la transparencia del agua
            GuiController.Instance.Modifiers.addFloat("blending", 0, 1, 0.8f);
}

        public void crearSkybox()
        {
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 2000, 0);
            skyBox.Size = new Vector3(10000, 5000, 10000);
            string texturesPath = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\celeste\\";
            //Configurar las texturas para cada una de las 6 caras
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "topax2.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "algo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "cielo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "cielo.png");
            //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "cielo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "cielo.png");
            //Configurar color  
            //skyBox.Color = Color.OrangeRed;
            skyBox.SkyEpsilon = 5f; //para que no se noten las aristas del box
            skyBox.updateValues();
        }

        private void crearSprites()
        {
            Size screenSize = GuiController.Instance.Panel3d.Size;

            boton1 = new TgcSprite();
            boton1.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\boton.png");
            Size textureSize = boton1.Texture.Size;
            boton1.Position = new Vector2(screenSize.Width - textureSize.Width  , screenSize.Height - textureSize.Height);

            boton2 = new TgcSprite();
            boton2.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\boton.png");
            textureSize = boton2.Texture.Size;
            boton2.Position = new Vector2((screenSize.Width - boton1.Texture.Size.Width) - textureSize.Width, screenSize.Height - textureSize.Height);
        }

        private void crearUserVars()
        {
            GuiController.Instance.UserVars.addVar("time", 0f);
        }

        public void recargarHeightMap()
        {
            float selectedScaleXZ = (float)GuiController.Instance.Modifiers["XZ"];
            float selectedScaleY = (float)GuiController.Instance.Modifiers["Y"];
            if (currentScaleXZ != selectedScaleXZ || currentScaleY != selectedScaleY)
            {
                //Volver a cargar el Heightmap si cambiaron los modifiers
                currentScaleXZ = selectedScaleXZ;
                currentScaleY = selectedScaleY;
                terrain.loadHeightmap(currentHeightmap, currentScaleXZ, currentScaleY, new Vector3(0, 5, 0));
            }
        }

        public void renderizar()
        {
            terrain2.render();
            skyBox.render();
            terrain.render();            
            b1.UpdateRender();
            b2.UpdateRender();
            b3.UpdateRender();
            barcoProtagonista.UpdateRender();
            lightMesh.render();
            GuiController.Instance.Drawer2D.beginDrawSprite();
            boton1.render();
            boton2.render(); 
            GuiController.Instance.Drawer2D.endDrawSprite();
        }

        public void setShadersValues()
        {
            Vector3 lightPosition = (Vector3)GuiController.Instance.Modifiers["LightPosition"];
            lightMesh.Position = lightPosition;
           
            efectoOlas.SetValue("time", (float)GuiController.Instance.UserVars.getValue("time"));
            efectoOlas.SetValue("fvLightPosition", TgcParserUtils.vector3ToFloat3Array(lightPosition));
            efectoOlas.SetValue("k_la", (float)GuiController.Instance.Modifiers["Ambient"]);
            efectoOlas.SetValue("k_ld", (float)GuiController.Instance.Modifiers["Diffuse"]);
            efectoOlas.SetValue("k_ls", (float)GuiController.Instance.Modifiers["Specular"]);
            efectoOlas.SetValue("fSpecularPower", (float)GuiController.Instance.Modifiers["SpecularPower"]);
            efectoOlas.SetValue("blendAmount", (float)GuiController.Instance.Modifiers["blending"]);
            efectoOlas.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.CurrentCamera.getPosition()));
        }

        public void setUsersVars()
        {
            //mantenemos el tiempo a nivel global con una userVar, muestra el timer y lo usan otras clases, no sacar
            GuiController.Instance.UserVars.setValue("time", ((float)GuiController.Instance.UserVars.getValue("time") + GuiController.Instance.ElapsedTime));
        }

        #endregion
    }
}
