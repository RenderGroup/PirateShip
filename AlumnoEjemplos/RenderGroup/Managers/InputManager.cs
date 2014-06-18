using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Input;
using TgcViewer;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.RenderGroup
{
    //clase que se encarga de sensar input y manda eventos a los interesados
    //la idea de esta clase es que no necesariamente todo el input va a ser para el barco protagonista
    //por ejemplo, si queremos hacer un mapa en 2D que siga al barco, este mapa tambien recibe input

    class InputManager
    {
        static List<ReceptorInput> interesados;

        static public TgcD3dInput d3dInput;

        public static void Cargar() 
        {
            interesados = new List<ReceptorInput>();

            d3dInput = GuiController.Instance.D3dInput;
        }

        public static void Agregar(params ReceptorInput[] i)
        {
            i.ToList().ForEach(interesado => interesados.Add(interesado));
        }

        public static void ManejarInput() 
        {
            if (d3dInput.keyDown(Key.W))
                foreach (ReceptorInput i in interesados) { i.W_apretado(); }            

            if (d3dInput.keyDown(Key.S))
                foreach (ReceptorInput i in interesados) { i.S_apretado(); }

            if (d3dInput.keyDown(Key.D))
                foreach (ReceptorInput i in interesados) { i.D_apretado(); }

            if (d3dInput.keyDown(Key.A))
                foreach (ReceptorInput i in interesados) { i.A_apretado(); }

            if (d3dInput.keyDown(Key.P))
                foreach (ReceptorInput i in interesados) { i.P_apretado(); }

            if (d3dInput.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
                foreach (ReceptorInput i in interesados) { i.ClickIzquierdo(); }

            if (d3dInput.buttonDown(TgcD3dInput.MouseButtons.BUTTON_RIGHT))
                foreach (ReceptorInput i in interesados) { i.ClickDerecho(); }
        }

        static public void DisposeReceptoresInput() 
        {
            interesados = null; //asigna null para que el garbage collector junte la coleccion
        }
    }
}

