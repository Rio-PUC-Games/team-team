﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelection : MonoBehaviour
{
    public CharacterSelectionData PlayerData;
    public Transform[] PlayerPanels;
    public Sprite[] CharSprites;
    public float controllerCooldown = 0.3f;
    public Button readyButton;
    public Button returnButton;

    public int numPlayers = 0;
    private bool[] playerReady;
    private GameObject[] Arrows;
    private GameObject[] pressKeyText;
    private Image[] playerPortrait;
    private Image[] selectedMask;
    private Image[] playerText;
    private PlayerInput[] playerInput;
    private float currentCooldown = 0.0f;

    public string eventStringUIMove = "event:/Menu/Opção";
    public string[] eventStringUIChars;

    // Start is called before the first frame update
    void Start()
    {
        PlayerData = ScriptableObject.CreateInstance<CharacterSelectionData>();
        playerReady = new bool[PlayerPanels.Length];
        playerInput = new PlayerInput[PlayerPanels.Length];

        PlayerData.PlayerController = new int[4];
        PlayerData.CharSelected = new int[4];
        PlayerData.PlayerIndex = new int[4];

        Arrows = new GameObject[PlayerPanels.Length];
        pressKeyText = new GameObject[PlayerPanels.Length];

        playerPortrait = new Image[PlayerPanels.Length];
        selectedMask = new Image[PlayerPanels.Length];
        playerText = new Image[PlayerPanels.Length];


        //K: hardcoded - nomes dos eventos dos sons de cada personagem quando selecionados
        //ordem: fofo, sereno, vaquinha, edgy
        eventStringUIChars = new string[]{"event:/Menu/Personagem1", "event:/Menu/Personagem 2", "event:/Menu/Personagem 4", "event:/Menu/Personagem 3"};

        readyButton.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(false);

        for (int i = 0; i < 4; i++)
        {
            PlayerData.PlayerController[i] = -1; //põe o valor default (nenhum player controlando) em cada mago
        }

        for (int i = 0; i < PlayerPanels.Length; i++)
        {
            playerPortrait[i] = PlayerPanels[i].GetChild(0).gameObject.GetComponent<Image>();
            selectedMask[i] = PlayerPanels[i].GetChild(1).gameObject.GetComponent<Image>();
            playerText[i] = PlayerPanels[i].GetChild(4).gameObject.GetComponent<Image>();
            pressKeyText[i] = PlayerPanels[i].GetChild(2).gameObject;
            Arrows[i] = PlayerPanels[i].GetChild(3).gameObject;
            PlayerData.CharSelected[i] = i;
            playerPortrait[i].sprite = CharSprites[PlayerData.CharSelected[i]];
            playerPortrait[i].gameObject.SetActive(false);
            selectedMask[i].gameObject.SetActive(false);
            playerText[i].gameObject.SetActive(false);
            playerReady[i] = false;
            playerInput[i] = PlayerPanels[i].GetComponent<PlayerInput>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //O: Se o número de jogadores não for máximo, verifica se há um controle conectado
        if(numPlayers < 4)
        {
            DetectDevice();
        }
        if (currentCooldown <= 0)
        {
            for (int i = 0; i < numPlayers; i++)
            {
                if (!playerReady[i])
                {
                    if (InputManager.GetAxis(playerInput[i].controllerScheme, "VerticalL") > 0)
                    {
                        ChangeCharUp(i);
                        FMODUnity.RuntimeManager.PlayOneShot(eventStringUIMove);
                    }
                    else if (InputManager.GetAxis(playerInput[i].controllerScheme, "VerticalL") < 0)
                    {
                        ChangeCharDown(i);
                        FMODUnity.RuntimeManager.PlayOneShot(eventStringUIMove);
                    }                        
                    else if (InputManager.GetKeyDown(playerInput[i].controllerScheme, "Action1"))
                    {
                        Confirm(i);
                        FMODUnity.RuntimeManager.PlayOneShot(eventStringUIChars[PlayerData.CharSelected[i]]);
                    }
                }
                else
                {
                    if (InputManager.GetKeyDown(playerInput[i].controllerScheme, "Action2") && playerReady[i])
                        ReturnSelection(i);
                }
            }
        }
        else
            currentCooldown -= Time.deltaTime;
    }

    public void DetectDevice()
    {
        /*O: Para o número de inputs detectados, verifica se o controle em questão já está sendo usado, 
        impedindo que dois jogadres controlem o mesmo painel de seleção*/
        for (int i = 0; i < playerInput.Length; i++)
        {
            // bool controllerAlreadyInUse = false;

            // for(int j = 0; j < numPlayers; j++)
            // {
            //     if(InputManager.IsControllerConnected(playerInput[j].controllerScheme) == true)
            //     {
            //         controllerAlreadyInUse = true;
            //     }
            // }

            // if(controllerAlreadyInUse)
            // {
            //     continue;
            // }

            //O: Quando o jogador pressiona o botão "A" ele está livre para escolher seu personagem
            if(i >= numPlayers && InputManager.GetKeyDown(playerInput[i].controllerScheme, "Start"))
            {
                IncreasePlayers(i);
            }
        }


    }
    public void IncreasePlayers(int controllerNumber)
    {
        currentCooldown = controllerCooldown;

        if(numPlayers < 4)
        {
            //O: Aumenta o número de jogadores após inicializados os controles e libera para selecionar um personagem
            PlayerData.PlayerController[numPlayers] = controllerNumber;
            PlayerData.PlayerIndex[numPlayers] = numPlayers + 1;

            pressKeyText[numPlayers].SetActive(false);
            playerPortrait[numPlayers].gameObject.SetActive(true);
            playerText[numPlayers].gameObject.SetActive(true);


            numPlayers++;
        }
    }

    //O: As duas funções abaixo alteram a imagem do personagem selecionado ao mover o joystick para cima. Caso chegue à ultima imagem, retorna para a primeira
    public void ChangeCharUp(int player)
    {
        currentCooldown = controllerCooldown;

        PlayerData.CharSelected[player]++;

        if (PlayerData.CharSelected[player] > CharSprites.Length-1)
        {
            PlayerData.CharSelected[player] = 0;
        }

        playerPortrait[player].sprite = CharSprites[PlayerData.CharSelected[player]];
    }
    public void ChangeCharDown(int player)
    {
        currentCooldown = controllerCooldown;

        PlayerData.CharSelected[player]--;

        if (PlayerData.CharSelected[player] < 0)
        {
            PlayerData.CharSelected[player] = CharSprites.Length - 1;
        }

        playerPortrait[player].sprite = CharSprites[PlayerData.CharSelected[player]];
    }

    //O: Confirma a seleção do jogador e declara que ele está pronto
    public void Confirm(int player)
    {
        currentCooldown = controllerCooldown;
            
        if (CharAlreadySelected(player))
            return;

        playerReady[player] = true;
        Arrows[player].SetActive(false);
        selectedMask[player].gameObject.SetActive(true);

        if (CheckIfPlayersReady())
        {
            readyButton.gameObject.SetActive(true);
            returnButton.gameObject.SetActive(true);
        }
    }

    public void ReturnSelection(int player)
    {
        currentCooldown = controllerCooldown;

        playerReady[player] = false;
        Arrows[player].SetActive(true);
        selectedMask[player].gameObject.SetActive(false);

        if(!CheckIfPlayersReady())
        {
            readyButton.gameObject.SetActive(false);
            returnButton.gameObject.SetActive(false);
        }
    }

    public void Voltar()
    {
        for(int i = 0; i < PlayerPanels.Length; i++)
        {
            playerReady[i] = false;
            Arrows[i].SetActive(true);
            readyButton.gameObject.SetActive(false);
            returnButton.gameObject.SetActive(false);
            playerPortrait[i].gameObject.SetActive(false);
            pressKeyText[i].SetActive(true);
            selectedMask[i].gameObject.SetActive(false);
            playerText[i].gameObject.SetActive(false);
            numPlayers = 0;
        }
    }

    //O: Verifica se o personagem que o jogador tenta selecionar já foi escolhido por outro jogador
    public bool CharAlreadySelected(int player)
    {
        for (int i = 0; i < PlayerPanels.Length; i++)
        {
            if (playerReady[i] == false || PlayerData.CharSelected[i] != PlayerData.CharSelected[player])
                continue;

            Debug.Log("Persongagem ja selecionado");
            return true;
        }
        return false;
    }

    //O: Verifica se todos os jogadores estão prontos para iniciar o jogo
    public bool CheckIfPlayersReady()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            if (!playerReady[i])
                return false;
        }

        if (numPlayers > 1)
            return true;
        else
            return false;
    }
}
