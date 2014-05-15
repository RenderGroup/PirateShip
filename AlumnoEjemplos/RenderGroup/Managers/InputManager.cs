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
        static List<IReceptorInput> interesados = new List<IReceptorInput>();

        static public TgcD3dInput d3dInput = GuiController.Instance.D3dInput;


        public static void Add(IReceptorInput i)
        {
            interesados.Add(i);
        }

        public static void ManejarInput() 
        {
            if (d3dInput.keyDown(Key.W))
                foreach (IReceptorInput i in interesados) { i.W_apretado(); }            

            if (d3dInput.keyDown(Key.S))
                foreach (IReceptorInput i in interesados) { i.S_apretado(); }

            if (d3dInput.keyDown(Key.D))
                foreach (IReceptorInput i in interesados) { i.D_apretado(); }

            if (d3dInput.keyDown(Key.A))
                foreach (IReceptorInput i in interesados) { i.A_apretado(); }

            if (d3dInput.keyDown(Key.P))
                foreach (IReceptorInput i in interesados) { i.P_apretado(); }
        }

    }
}

