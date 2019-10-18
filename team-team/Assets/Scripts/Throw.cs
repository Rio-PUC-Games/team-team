﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
Throw: script que implementa o comportamento de um player segurar objetos e poder arremessá-los

Autores: Arthur, Krauss, Arthus James

 */
public class Throw : MonoBehaviour
{    
    [Header("Referências do Unity(não mexer, só se você souber o que está fazendo)")]
    public Transform holdpoint;
    public bool holding = false;

    [Header("Variáveis de ajuste da gameplay. Ver tooltips para mais infos")]
    [Tooltip("Variável para ajustar a velocidade em que o jogador arremessa a poção")]
    public float throwSpeed = 100.0f;
    [Tooltip("variavel que determina a qual time o jogador pertence (começa contagem do 0)")]
    //J: , e o script da poção com que se está interagindo
    public int throwerTeam;

    //Arthur, é de fato importante ter essa variável Rigidbody aqui. Mas é importante que o nome dela seja potionRigidbody, ou algo mais descritivo que só rb. Tomei a liberdade de mudar. Ass: Krauss
        //Além disso, é importante que ela seja setada quando o player pega uma poção, senão ele só poderia pegar uma única poção durante o jogo
    private Rigidbody potionRigidbody;
    

    
    private PotColi potionScript = null;

    //classe bobinha que armazena configuração de controle do jogador
    private PlayerInput playerInput;
    private PlayerEffects playerEffects;

    //O: Variável que define se o jogador pode jogar ou não os itens
    private bool canThrow = true;

    // Start is called before the first frame update
    void Start()
    {
        GameController.setThrowSpeedGlobal(throwSpeed);
        playerInput = this.GetComponent<PlayerInput>();
        playerEffects = GetComponent<PlayerEffects>();

        Debug.Assert(playerEffects != null);
    }

    // Update is called once per frame
    void Update()
    {
        //se o personagem está segurando uma poção,
        if (holding)
        {
            GetCanThrow();
            //Debug.Assert(potionRigidbody != null);
            //J: Corrige o erro de ficar impedido de pegar poção nova
            //K: não é melhor setar pra null toda vez que arremessar a poção?
            if (potionRigidbody == null)
            {
                holding = false;
            }
            else
            {
                //seta a posição da poção
                potionRigidbody.transform.position = holdpoint.position;
                potionRigidbody.useGravity = false;

                if (InputManager.GetKeyDown(playerInput.controllerScheme, "Action1") && canThrow)
                {
                    //arremessa a poção que estava sendo segurada

                    //K: O, eu alterei esta linha abaixo para fazer com que o objeto seja arremessado em várias direções, e não só para direita!
                    Vector3 throwDirection = transform.forward;
                    potionRigidbody.velocity = throwDirection * throwSpeed;
                    potionRigidbody.gameObject.transform.SetParent(null);
                    //potionRigidbody.useGravity = true;
                    holding = false;

                    //J: altera o valor Thrown da poção para true
                    potionScript.setThrown(true);
                }
                else if(InputManager.GetKeyDown(playerInput.controllerScheme, "Action2") && canThrow)
                {
                    //joga a poção em si mesmo!
                    potionScript.HitPlayer(playerEffects);
                }
            }
            
        }
            
    }


    // on TRIGGER enter
    private void OnTriggerEnter(Collider other)
    {
        //se o player colidir com a poção && o player não estiver segurando outra poção,
        PotColi pc = other.GetComponent<PotColi>();
        if(other.gameObject.CompareTag("Potion") && holding == false && pc != null && !other.GetComponent<PotColi>().getThrown())
        {
            //segura esta poção:
            holding = true;
            other.gameObject.transform.SetParent(holdpoint);
            other.gameObject.transform.position = holdpoint.position;
            potionRigidbody = other.gameObject.GetComponent<Rigidbody>();
            potionRigidbody.isKinematic = false;

            //J: adquire script da poção arremessada e altera o valor de qual time carrega/arremessa a poção
            potionScript = pc;
            potionScript.setThrower(throwerTeam);
        }
    }

    //O: Função que altera o valor da variável canThrow caso o jogador esteja ou não congelado
    private bool GetCanThrow()
    {
        bool freeze = playerEffects.HasEffect(PotionEffect.Freeze);
        if(freeze)
        {
            canThrow = false;
        }
        else
        {
            canThrow = true;
        }
        return canThrow;

        //K: não há nada de errado com a estrutura escrita acima(pode ser útil mais pra frente pra implementar efeitos visuais e tal,)
        //mas repare que do jeito que está hoje, ela poderia ser simplificada para: 
            // return  !playerEffects.HasEffect(PotionEffect.Freeze);
    }
}
