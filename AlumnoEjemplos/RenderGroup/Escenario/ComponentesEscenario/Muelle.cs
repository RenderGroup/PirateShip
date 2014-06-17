using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Shaders;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;

namespace AlumnoEjemplos.RenderGroup
{
    class Muelle : IUpdateRender
    {

        Microsoft.DirectX.Direct3D.Effect efecto;
        List<TgcMesh> mesh;

        public Muelle()
        {          
            crearMeshes();
            cargarShaders();
        }
        
        public void render()
        {
            for (int i = 0; i < 13; i++)
                mesh[i].render();
        }

        public void update() { }

        public void dispose()
        {
            for (int i = 0; i < 13; i++)
            {
                mesh[i].dispose();
            }
        }

        public void crearMeshes()
        {
            //Crear loader
            TgcSceneLoader loader = new TgcSceneLoader();
            //Cargar mesh
            TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\meshes\\muelle-TgcScene.xml");
            mesh = scene.Meshes;
            efecto = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\shaderMuelle.fx");

            for (int i = 1; i < 13; i++)
            {
                mesh[i].Scale = new Vector3(80.5f, 80.5f, 60.5f);
                mesh[i].Position = new Vector3(mesh[i].Position.X + 3900f, mesh[i].Position.Y, mesh[i].Position.Z - 3000);
            }

            mesh[0].Scale = new Vector3(80.5f, 60.5f, 60.5f);
            mesh[0].Position = new Vector3(mesh[0].Position.X + 3900f, mesh[0].Position.Y + 100, mesh[0].Position.Z - 3000);
        }

        private void cargarShaders()
        {
            efecto = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\shaders\\shaderMuelle.fx");
            for (int i = 0; i < 13; i++)
            {
                mesh[i].Effect = efecto;
                mesh[i].Technique = "RenderScene";
            }
        }

        public void tecnicas(string technique)
        {
            for (int i = 0; i < 13; i++)
            {
                mesh[i].Technique = technique;
            }
        }
    }
}
