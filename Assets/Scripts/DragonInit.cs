using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class DragonInit : MonoBehaviour
{
    //Buttons
    [SerializeField] Button lifePointsPlus;
    [SerializeField] Button lifePointsMinus;
    [SerializeField] Button armorPointsPlus;
    [SerializeField] Button armorPointsMinus;
    [SerializeField] Button attackPointsPlus;
    [SerializeField] Button attackPointsMinus;
    [SerializeField] Button finish;
    //Textdisplay
    [SerializeField] TextMeshProUGUI _LifePoints;
    [SerializeField] TextMeshProUGUI _ArmorPoints;
    [SerializeField] TextMeshProUGUI _AttackPoints;
    //values for the player
    private int lifePoints=10;
    private int armorPoints=5;
    private int attackPoints=0;
    private GameObject target=null;
    private GameObject DungeonMaster=null;

    void Start()
    {
        DungeonMaster = GameObject.FindGameObjectWithTag("DM");

        //lifePoints Changes:
        lifePointsPlus.onClick.AddListener(() =>
        {
            lifePoints++;
            _LifePoints.SetText("Life Points: " + lifePoints.ToString());
        });
        lifePointsMinus.onClick.AddListener(() =>
        {
            if (lifePoints > 0) lifePoints--;
            _LifePoints.SetText("Life Points: " + lifePoints.ToString());
        });

        //armorClass Changes:
        armorPointsPlus.onClick.AddListener(() =>
        {
            armorPoints++;
            _ArmorPoints.SetText("Armor class: " + armorPoints.ToString());
        });
        armorPointsMinus.onClick.AddListener(() =>
        {
            if (armorPoints > 0) armorPoints--;
            _ArmorPoints.SetText("Armor class: " + armorPoints.ToString());
        });

        //attackBonus Changes:
        attackPointsPlus.onClick.AddListener(() =>
        {
            attackPoints++;
            _AttackPoints.SetText("Attack Bonus: " + attackPoints.ToString());
        });
        attackPointsMinus.onClick.AddListener(() =>
        {
            if (attackPoints > 0) attackPoints--;
            _AttackPoints.SetText("Attack Bonus: " + attackPoints.ToString());
        });
        
        //Set the new Settings for enemies created by the Dungeon Master:
        finish.onClick.AddListener(() =>
        {
            if (target == null) DungeonMaster.GetComponent<Dragon>().setVals(lifePoints, armorPoints, attackPoints);
            else target.GetComponent<Dragon>().setVals(lifePoints, armorPoints, attackPoints);
            this.gameObject.GetComponent<Canvas>().enabled=false;
        });
    }
    private void OnBecameVisible()
    {
        //update UI in case values changed since last visible
        if (target != null){ 
            var variables = target.GetComponent<Dragon>().getVals();
            lifePoints = variables[0];
                _LifePoints.SetText(_LifePoints.ToString());
            armorPoints = variables[1];
                _ArmorPoints.SetText(_ArmorPoints.ToString());
            attackPoints = variables[2];
                _AttackPoints.SetText(_AttackPoints.ToString());
        }
    }
    public void setTarget(GameObject obj)
    {
        target= obj;
    }
}
