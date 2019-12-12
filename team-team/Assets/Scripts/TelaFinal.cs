﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TelaFinal : MonoBehaviour
{
    private GameObject[] characterPanels;

    //a ordem aqui tem que seguir a mesma ordem que tiver sido usada na tela de seleção de personagens
    [SerializeField] private List<Sprite> sprites_vitoria;
    [SerializeField] private List<Sprite> sprites_derrota;
    
    int playersQtd;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    void Initialize()
    {
         if(PersistentInfo.Instance == null)
        {
            playersQtd = 4;
        }
        else
        {
            playersQtd = PersistentInfo.Instance.playersQtd;
        }


        GameObject parent = transform.Find("PainelPersonagens").gameObject;
        int childCount = parent.transform.childCount;
        characterPanels = new GameObject[childCount];
        for(int i = 0; i < childCount; i++)
        {
            characterPanels[i] =  parent.transform.GetChild(i).gameObject;
            characterPanels[i].SetActive(i < playersQtd); //disabla painel se player não está no jogo
        }
    }
    
    //função para ser chamada quando o jogo termina e a telafinal deva ser acionada
    public void ActivateTelaFinal()
    {
        Initialize();
        this.gameObject.SetActive(true);
        CharacterSelectionData csd = PersistentInfo.Instance.PlayerData;

        //ativa ou desativa paineis de acordo com o numero de jogadores jogando
        for(int i = 0; i < 4; i++)
        {
            characterPanels[i].SetActive(playersQtd > i);
        }
        //o primeiro painel é do player vitorioso
        
        List<int> orderedIndex = new List<int>();
        for(int i = 0; i < playersQtd; i++)
        {
            orderedIndex.Add(i);
        }
        orderedIndex.Sort((i,j) => GameController.Instance.teamPoints[j].CompareTo(GameController.Instance.teamPoints[i]));

        for(int i = 0; i < playersQtd; i++)
        {
            characterPanels[i].GetComponentInChildren<Text>().text = "Player " + (orderedIndex[i] + 1).ToString() + ": " + GameController.Instance.teamPoints[orderedIndex[i]].ToString();
            if(i == 0)
            {
                characterPanels[i].GetComponentInChildren<Image>().sprite = sprites_vitoria[csd.CharSelected[orderedIndex[i]]];
            }
            else
            {
                characterPanels[i].GetComponentInChildren<Image>().sprite = sprites_derrota[csd.CharSelected[orderedIndex[i]]];   
            }
        }



    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}