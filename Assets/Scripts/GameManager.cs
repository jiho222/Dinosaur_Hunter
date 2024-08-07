using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public PlayerMovement player;
    public Enemy enemy;
    public TextMeshProUGUI playerHealthText;
    public RectTransform energyhealth;

    // void LateUpdate()
    // {     
    //     playerHealthText.text = player.health + " / " + player.maxHealth;
    //     energyhealth.localScale = new Vector3((float)enemy.curHealth / enemy.maxHealth, 1, 1);
    // }

}
