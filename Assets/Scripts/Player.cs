using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    //Values specific for player
    private string name;
    private int lifePoints;
    private int armorClass;
    private int attackBonus;
    public GameObject claimedPlayer;

    void Start()
    {
        setName("Player");
        lifePoints = 0;
        armorClass = 0;
        attackBonus = 0;
        claimedPlayer = null;
    }
    public void setName(string name)
    {
        this.name = name;
    }
    public void setVals(int lP, int aP, int atP)
    { 
        this.lifePoints = lP;
        this.armorClass = aP;
        this.attackBonus = atP;
    }
    //possibility for expansion
    public void hurt()
    {    }
    public bool HasClaimedPLayer()
    {
        //allows for a player to only claim one Player object in the game
        if (claimedPlayer == null) return false;
        else return true;
    }

}
