using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class PlayerInit : MonoBehaviour
{
    //UI
    [SerializeField] TMP_InputField nameB;
    [SerializeField] Button lifePointsPlus;
    [SerializeField] Button lifePointsMinus;
    [SerializeField] Button armorPointsPlus;
    [SerializeField] Button armorPointsMinus;
    [SerializeField] Button attackPointsPlus;
    [SerializeField] Button attackPointsMinus;
    [SerializeField] Button finish;
    [SerializeField] TextMeshProUGUI _Name;
    [SerializeField] TextMeshProUGUI _LifePoints2;
    //link to handling of Player objects in Game
    [SerializeField] DwarfInit Settings;
    //values
    private int lpoints=0;
    private int armorPoints = 0;
    private int attackPoints = 0;

    void Start()
    {
        //Set Name:
        nameB.onValueChanged.AddListener(t =>
        {
            _Name.SetText(nameB.text);
        });

        //Set Life Points:
        lifePointsPlus.onClick.AddListener(() =>
        {
            lpoints++;
            lifePointsPlus.gameObject.transform.parent.GetChild(2).GetComponent<TextMeshProUGUI>().SetText("Life Points: " + lpoints.ToString());
            _LifePoints2.SetText("Life Points: " + lpoints.ToString());
        });
        lifePointsMinus.onClick.AddListener(() =>
        {
            if(lpoints>0) lpoints--;
            lifePointsPlus.gameObject.transform.parent.GetChild(2).GetComponent<TextMeshProUGUI>().SetText("Life Points: " + lpoints.ToString());
            _LifePoints2.SetText("Life Points: " + lpoints.ToString());
        });

        //armorClass Changes:
        armorPointsPlus.onClick.AddListener(() =>
        {
            armorPoints++;
            armorPointsPlus.gameObject.transform.parent.GetChild(2).GetComponent<TextMeshProUGUI>().SetText("Armor Class: "+ armorPoints.ToString());
        });
        armorPointsMinus.onClick.AddListener(() =>
        {
            if (armorPoints > 0) armorPoints--;
            armorPointsPlus.gameObject.transform.parent.GetChild(2).GetComponent<TextMeshProUGUI>().SetText("Armor Class: " + armorPoints.ToString());
        });

        //attackBonus Changes:
        attackPointsPlus.onClick.AddListener(() =>
        {
            attackPoints++;
            attackPointsPlus.gameObject.transform.parent.GetChild(2).GetComponent<TextMeshProUGUI>().SetText("Attack Bonus: " + attackPoints.ToString());
        });
        attackPointsMinus.onClick.AddListener(() =>
        {
            if (attackPoints > 0) attackPoints--;
            attackPointsPlus.gameObject.transform.parent.GetChild(2).GetComponent<TextMeshProUGUI>().SetText("Attack Bonus: " + attackPoints.ToString());
        });

        //Set Finish Button and save stats for Player:
        finish.onClick.AddListener(() =>
        {
            string name = nameB.text;
            if (name.Length<1)nameB.text= "Random Unicorn";
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().setName(name);
            Settings.SetVals(lpoints, attackPoints, armorPoints);
        });
    }
}
