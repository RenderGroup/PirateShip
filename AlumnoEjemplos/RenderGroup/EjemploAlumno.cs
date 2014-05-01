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
        Microsoft.DirectX.Direct3D.Effect efectoLuz;
        Microsoft.DirectX.Direct3D.Effect efectoOlas;

        string currentHeightmap;
        string currentTexture;
        float currentScaleXZ = 100f;
        float currentScaleY = 1.3f;

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

            /*
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 0, 0);
            skyBox.Size = new Vector3(9000, 9000, 9000);
            string texturesPath = GuiController.Instance.AlumnoEjemplosMediaDir + "Rendergroup\\Texturas\\SkyBoxLostAtSeaDay\\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "Up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "Down.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "Left.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "Right.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "Back.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "Front.jpg");
            skyBox.updateValues();
            */

            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\Texturas\\SkyBoxLostAtSeaDay\\Down.jpg");
            piso = TgcBox.fromSize(new Vector3(8500, 1, 8500), pisoTexture);

            #endregion

            crearHeightmaps();

            crearSkybox();

            cargarShaders();

            crearModifiers();

            crearUserVars();

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
        }

        public override void close()
        {
            barcoProtagonista.dispose();
            piso.dispose();
            terrain.dispose();
            efectoLuz.Dispose();
            efectoOlas.Dispose();
            
        }

        #region NUEVO

        private void cargarShaders()
        {
            efectoLuz = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\PhongShading.fx");
            efectoOlas = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\shaderOlas.fx");
            terrain.Effect = efectoOlas;
            //terrain.Effect = TgcShaders.loadEffect("C:\\Users\\Julio\\Desktop\\materias\\TGC\\TgcViewer\\Examples\\Shaders\\WorkshopShaders\\Shaders\\BasicShader.fx");
            terrain.Technique = "RenderScene";
        }

        public void crearHeightmaps()
        {
            currentHeightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\piedra1.jpg";
            currentTexture = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\\\texturas\\color_agua5.png";
            terrain = new SmartTerrain();
            terrain.loadHeightmap(currentHeightmap, 100f, 1.6f, new Vector3(0, 0, 0)); //150f, 1.3f, new Vector3(0, 5, 0));
            terrain.loadTexture(currentTexture);

            currentHeightmap = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\\\texturas\\piedra1.jpg";
            currentTexture = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\\\texturas\\color_agua5.png";
            terrain = new SmartTerrain();
            terrain.loadHeightmap(currentHeightmap, 150f, 5f, new Vector3(0, 0, 0));
            terrain.loadTexture(currentTexture);

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
            //modifiers para el shader de iluminacion dinamica(del barco)
            GuiController.Instance.Modifiers.addVertex3f("LightPosition", new Vector3(-100, -100, -100), new Vector3(1000, 4000, 1000), new Vector3(50, 4000, 0));
            GuiController.Instance.Modifiers.addFloat("Ambient", 0, 1, 0.5f);
            GuiController.Instance.Modifiers.addFloat("Diffuse", 0, 1, 0.6f);
            GuiController.Instance.Modifiers.addFloat("Specular", 0, 1, 0.5f);
            GuiController.Instance.Modifiers.addFloat("SpecularPower", 1, 2000, 100);
        }

        public void crearSkybox()
        {
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 3990, 0);
            skyBox.Size = new Vector3(10000, 10000, 10000);
            string texturesPath = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\celeste\\";
            //Configurar las texturas para cada una de las 6 caras
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "cielo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "algo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "cielo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "cielo.png");
            //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "cielo.png");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "cielo.png");
            //Configurar color  
            //skyBox.Color = Color.OrangeRed;
            skyBox.SkyEpsilon = 50f; //para que no se noten las aristas del box
            skyBox.updateValues();
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
            skyBox.render();
            terrain.render();            
            b1.UpdateRender();
            b2.UpdateRender();
            b3.UpdateRender();
            barcoProtagonista.UpdateRender();
        }

        public void setShadersValues()
        {
            Vector3 lightPosition = (Vector3)GuiController.Instance.Modifiers["LightPosition"];
            //Cargar variables de shader para la iluminacion dinamica
            efectoLuz.SetValue("fvLightPosition", TgcParserUtils.vector3ToFloat3Array(lightPosition));
            efectoLuz.SetValue("k_la", (float)GuiController.Instance.Modifiers["Ambient"]);
            efectoLuz.SetValue("k_ld", (float)GuiController.Instance.Modifiers["Diffuse"]);
            efectoLuz.SetValue("k_ls", (float)GuiController.Instance.Modifiers["Specular"]);
            efectoLuz.SetValue("fSpecularPower", (float)GuiController.Instance.Modifiers["SpecularPower"]);
            //Cargar variables de shader para el mar
            efectoOlas.SetValue("time", (float)GuiController.Instance.UserVars.getValue("time"));
        }

        public void setUsersVars()
        {
            //mantenemos el tiempo a nivel global con una userVar, muestra el timer y lo usan otras clases, no sacar
            GuiController.Instance.UserVars.setValue("time", ((float)GuiController.Instance.UserVars.getValue("time") + GuiController.Instance.ElapsedTime));
        }

        #endregion
    }
}
