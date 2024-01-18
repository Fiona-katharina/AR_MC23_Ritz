using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Collections;
using System.Threading.Tasks;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;

public class DwarfInit : NetworkBehaviour
{
    //UI
    [SerializeField] Button lifePointsPlus;
    [SerializeField] Button lifePointsMinus;
    [SerializeField] Button armorPointsPlus;
    [SerializeField] Button armorPointsMinus;
    [SerializeField] Button attackPointsPlus;
    [SerializeField] Button attackPointsMinus;
    [SerializeField] Button finish;
    [SerializeField] Button claimPlayer;
    [SerializeField] Button attackButton;
    [SerializeField] TextMeshProUGUI _LifePoints;
    [SerializeField] TextMeshProUGUI _ArmorPoints;
    [SerializeField] TextMeshProUGUI _AttackPoints;
    [SerializeField] TextMeshProUGUI name;
    //external links
    [SerializeField] GameObject LogMessages;
    [SerializeField] GameObject markerEdit;
    [SerializeField] GameObject markerEnemy;
    [SerializeField] GameObject Dice;
    //values
    private int lifePoints ;
    private int armorPoints;
    private int attackPoints;
    private GameObject currentEdit;
    private GameObject currentEnemy;
    private int Dicepoints;
    public bool attackRolled;

    public void setEditTarget(GameObject obj)
    {
        currentEdit=obj;
        markerEdit.SetActive(true);
        markerEdit.transform.position = obj.transform.position + new Vector3(0,0.055f,0);
        markerEdit.transform.parent = currentEdit.transform.parent;
        claimPlayer.enabled = true;
    }

    public void setEnemyTarget(GameObject obj)
    {
        if (!attackRolled)
        {
            currentEnemy = obj;
            markerEnemy.SetActive(true);
            markerEnemy.transform.position = obj.transform.position + new Vector3(0.005f, 0.055f, 0);
            markerEnemy.transform.parent = currentEnemy.transform.parent;
            attackButton.enabled = true;
        }
        else
        {
            LogMessages.GetComponent<ErrorLog>().setLogMsg("Action not possible because you're already attacking another enemy");
        }
    }

    public void placeTarget()
    {
        if (currentEdit != null)
        {
            currentEdit.GetComponent<ClientPlayer>().setPositionServerRpc(Dice.transform.position);
            markerEdit.transform.position = Dice.transform.position + new Vector3(0, 0.055f, 0);
        }
    }
    public void checkAttack()
    {
        //Check if the rolled dice value breaks through the armor of the enemy
        if (currentEnemy != null){
            if (!attackRolled)
            {
                if (Dicepoints > currentEnemy.GetComponent<Dragon>().getVals()[1])
                {
                    attackRolled = true;
                    LogMessages.GetComponent<ErrorLog>().setLogMsg("Please roll again");
                }
                else LogMessages.GetComponent<ErrorLog>().setLogMsg("The enemy blocked your attack...");
            }
        }
        else
        {
            LogMessages.GetComponent<ErrorLog>().setLogMsg("Please Select an enemy first!");
        }
    }
    public void AttackEnemy(int damage)
    {
        if (currentEdit != null)
        {
            currentEnemy.GetComponent<Dragon>().attackServerRpc(attackPoints + damage);
            LogMessages.GetComponent<ErrorLog>().setLogMsg("The enemy was hit with " + (attackPoints + Dicepoints).ToString() + " damage.");
            currentEnemy = null;
            markerEnemy.SetActive(false);
            attackRolled = false;
        }
        else LogMessages.GetComponent<ErrorLog>().setLogMsg("Please select a Player object first!");
    }

    public void SetDicePoints(int p)
    {
        Dicepoints = p;
        if (attackRolled)
        {
            AttackEnemy(p);
        }
    }
    void Start()
    {
        attackRolled = false;
        Dicepoints = 0;
        attackButton.onClick.AddListener(() =>
        {
            checkAttack();
        });
        //lifePoints Changes:
        lifePointsPlus.onClick.AddListener(() =>
        {
            lifePoints++;
        });
        lifePointsMinus.onClick.AddListener(() =>
        {
            if (lifePoints > 0) lifePoints--;
        });

        //armorClass Changes:
        armorPointsPlus.onClick.AddListener(() =>
        {
            armorPoints++;
        });
        armorPointsMinus.onClick.AddListener(() =>
        {
            if (armorPoints > 0) armorPoints--;
        });

        //attackBonus Changes:
        attackPointsPlus.onClick.AddListener(() =>
        {
            attackPoints++;
        });
        attackPointsMinus.onClick.AddListener(() =>
        {
            if (attackPoints > 0) armorPoints--;
        });

        //Set the new Settings for the dwarf selected:
        finish.onClick.AddListener(() =>
        {
            string z= name.text;
            if (currentEdit != null)
            {
                currentEdit.GetComponent<ClientPlayer>().setVals(lifePoints, armorPoints, attackPoints, new FixedString32Bytes(z));
                claimPlayer.GetComponentInChildren<TextMeshProUGUI>().text = "Edit";
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().claimedPlayer = currentEdit;
                this.gameObject.GetComponent<Canvas>().enabled = false;
            }
            //This means the Host has deleted the player object so the player needs to claim a new one
            else
            {
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().claimedPlayer = null;
                LogMessages.GetComponent<ErrorLog>().setLogMsg("Please select a Player object first!");
                claimPlayer.GetComponentInChildren<TextMeshProUGUI>().text = "Claim";
                this.gameObject.GetComponent<Canvas>().enabled = false;
            }
        });

    }

    public void SetVals(int lifeP, int attackB, int armorClass)
    {
        lifePoints = lifeP;
        attackPoints=attackB;
        armorPoints = armorClass;
    }

    private void Update()
    {
        _LifePoints.SetText("Life Points: " + lifePoints.ToString());
        _AttackPoints.SetText("Attack Bonus Points: " + attackPoints.ToString());
        _ArmorPoints.SetText("Armor Class: " + armorPoints.ToString());
    }
}
