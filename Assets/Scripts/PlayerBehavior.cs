using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerBehavior : MonoBehaviour
{
    public Text allyHPStatus;
    const int BULLET_DAMAGE = 10;
    public int hp;

    void Start()
    {
        hp = 100;
    }

    void Update()
    {
        if (hp <= 0)
            gameOver();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Bullet"))
        {    
            hp -= BULLET_DAMAGE;
            
            if (hp < 0)
                return;
            
            if (hp == 0)
                {
                   allyHPStatus.GetComponent<Text>().text = allyHPStatus.GetComponent<Text>().text.Replace("Player: 010", "Player: 000");
                   return; 
                }
                
            string oldHPString = allyHPStatus.GetComponent<Text>().text.Substring(17,11);
            string newHPString = "Player: 0" + hp;
            allyHPStatus.GetComponent<Text>().text = allyHPStatus.GetComponent<Text>().text.Replace(oldHPString, newHPString);
            
            
        }        
    }

    void gameOver()
    {
        print("I am dead");
    }
}
