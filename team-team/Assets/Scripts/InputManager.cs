﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

Script para abstrair a relação entre as componentes do jogo(movimentação dos jogadores, por exemplo) e
os controladores. Isso é positivo para que cada componente não precise se preocupar com problemas comuns do tipo trocar controle por teclado, controle desconectado etc.

Autores: Krauss,
*/


// a controller scheme is defined as the pair controller mode and index
[System.Serializable]
public struct ControllerScheme
{
    public ControllerMode mode;
    public uint index;
}

//controller mode can be either keyboard or joystick
//we define a maximum of two keyboards and 4 joysticks, this beeing implemented below and in the input settings
[System.Serializable]
public enum ControllerMode{Keyboard, Joystick};

public class InputManager : MonoBehaviour
{
    
    //singleton
    static private InputManager Instance{ get; set;} 

    //updated each frame, only queried once
    private static string[] joystickNames;

    //the axis we have defined in the project settings. Anything besides this will result in an error
    /*
    Defined axis:
        - HorizontalL, VerticalL: the two main axis for movement. Follows the left joystick on controllers, and the asdf or direction-keys on a keyboard
        - [TO-DO]HorizontalR, VerticalR: the same, but they track the right joystick, and nothing on a keyboard
    
    */
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            //destroy second instance, ie this
            GameObject.Destroy(this);
        }

        joystickNames = Input.GetJoystickNames();
        
    }
    
   
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        joystickNames = Input.GetJoystickNames();
        
        // Debug.Log(Input.GetJoystickNames().Length);
        // for (uint i = 0; i < joystickNames.Length; i++)
        // {
        //     ControllerScheme cs = new ControllerScheme();
        //     cs.mode = ControllerMode.Joystick;
        //     cs.index = i;
        //     if(IsControllerConnected(cs))
        //     {
        //         Debug.Log(GetIdString(cs) + " h:" + GetAxis(cs, "HorizontalL") + " v:" + GetAxis(cs, "VerticalL"));
        //     }
        // }
        
    }

    /* a idéia é que essas funções funcionem exatamente como as equivalentes delas na classe
    Input do Unity, mas que já tratem e abstraiam as diferenças entre controladores e teclados, e, de alguma maneira,
    tratem a situação de quando este controle estiver desconectado

    */

    //retorna identicamente à Input.GetAxis, mas somente usando o controle especificado
    //também retorna false se o controle estiver desconectado
    public static float GetAxis(ControllerScheme controller, string axisName)
    {
        if(IsControllerConnected(controller))
        {
            string str = axisName + GetIdString(controller);
            return Input.GetAxis(str);
        }
        else return 0.0f;
    }

    public static bool GetKeyDown(ControllerScheme controller, string axisName)
    {
        if(IsControllerConnected(controller))
        {
            //um hardcode temporário:
            if(controller.mode == ControllerMode.Joystick)
            {
                if(axisName == "Action1")
                {
                    switch(controller.index)
                    {
                        //quadrado ou R1 num PS4
                        case 0:
                            return Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick1Button5);
                        case 1:
                            return Input.GetKeyDown(KeyCode.Joystick2Button0) || Input.GetKeyDown(KeyCode.Joystick2Button5);
                        case 2:
                            return Input.GetKeyDown(KeyCode.Joystick3Button0) || Input.GetKeyDown(KeyCode.Joystick3Button5);
                        case 3:
                            return Input.GetKeyDown(KeyCode.Joystick4Button0) || Input.GetKeyDown(KeyCode.Joystick4Button5);
                        default:
                            return false;
                    }
                }
                else if(axisName == "Action2")
                {
                    switch(controller.index)
                    {
                        //X ou L1 num PS4
                        case 0:
                            return Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Joystick1Button4);
                        case 1:
                            return Input.GetKeyDown(KeyCode.Joystick2Button1) || Input.GetKeyDown(KeyCode.Joystick2Button4);
                        case 2:
                            return Input.GetKeyDown(KeyCode.Joystick3Button1) || Input.GetKeyDown(KeyCode.Joystick3Button4);
                        case 3:
                            return Input.GetKeyDown(KeyCode.Joystick4Button1) || Input.GetKeyDown(KeyCode.Joystick4Button4);
                        default:
                            return false;
                    }
                }
                else if (axisName == "Start")
                {
                    switch (controller.index)
                    {
                        //Start em ambos os controles 
                        //O: A princípio, não deve dar problema, mas o controle do ps4 tambem reconhece R2 como start. No do xbox não deve ter problema
                        case 0:
                            return Input.GetKeyDown(KeyCode.Joystick1Button7) || Input.GetKeyDown(KeyCode.Joystick1Button9);
                        case 1:
                            return Input.GetKeyDown(KeyCode.Joystick2Button7) || Input.GetKeyDown(KeyCode.Joystick2Button9);
                        case 2:
                            return Input.GetKeyDown(KeyCode.Joystick3Button7) || Input.GetKeyDown(KeyCode.Joystick3Button9);
                        case 3:
                            return Input.GetKeyDown(KeyCode.Joystick4Button7) || Input.GetKeyDown(KeyCode.Joystick4Button9);
                        default:
                            return false;
                    }
                }
                else 
                {
                    Debug.LogWarning("Comando estranho passado para a gambiarra dos controles");
                    return false; //não deveria nunca cair aqui
                }
            }
            else
            {
                string str = axisName + GetIdString(controller);
                return Input.GetAxis(str) > 0;
            }
        }
        else return false;
    }

    //for a given controller scheme, tells if it is connected or not
    public static bool IsControllerConnected(ControllerScheme controller)
    {
        if(controller.mode == ControllerMode.Keyboard)
        {
            //teclado está sempre conectado(será?)
            return true;
        }
        else
        {
            //se o joystick com esse índice estiver conectado,
            //(podemos testar se está conectado checando o nome retornado na string[] Input.GetJoystickNames)
            if(controller.index < joystickNames.Length && joystickNames[controller.index].Length > 0)
            {
                return true;
            }
            else return false;
        }
    }

    private static string GetIdString(ControllerScheme controller)
    {
        //constrói a string identificadora para um determinado controle. Isso só vai sentido se for seguido este mesmo padrão o input settings
        return controller.mode.ToString() + controller.index.ToString(); 
    }


}
