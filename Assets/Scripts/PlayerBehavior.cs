using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerBehavior : MonoBehaviour
{
    public Text allyHPStatus;
    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject HorrorMusic;
    public GameObject DirectionalLight;
    public GameObject WinningImage;
    public GameObject LosingImage;

    private bool wasRotatedAlready = false;

    const int BULLET_DAMAGE = 10;
    const float WAR_THRESHOLD = 90f;
    const int BIG_NUM = 200000;

    public int hp;

    void Start()
    {
        WinningImage.SetActive(false);
        LosingImage.SetActive(false);
        hp = 100;
        DirectionalLight.transform.Rotate(28.38f, 368.563f, -20.9f);
    }

    void Update()
    {
        checkGameStatus();
        
        bool areAnyOfTheEnemiesNearby = getDistanceFrom(Enemy1) <= WAR_THRESHOLD || getDistanceFrom(Enemy2) <= WAR_THRESHOLD;
        if (areAnyOfTheEnemiesNearby && !wasRotatedAlready)
        {
            DirectionalLight.transform.Rotate(-47.5f, 197.163f, 163f);
            wasRotatedAlready = true;
            AudioSource horrorMusic = HorrorMusic.GetComponent<AudioSource>();
            horrorMusic.Play();
        }      
    }

    void checkGameStatus()
    {
        if (hp <= 0)
            StartCoroutine(gameOver());
        else if (ClaireBehavior.hp <= 0  && RacerBehavior.hp <= 0)
            StartCoroutine(win());
    }

    float getDistanceFrom(GameObject target)
    {
        return target != null ? Vector3.Distance(this.transform.position, target.transform.position) : BIG_NUM;
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

    IEnumerator gameOver()
    {
        LosingImage.SetActive(true);
        yield return new WaitForSeconds(3);
        QuitGame();
        yield return 0;
    }

    IEnumerator win()
    {
        WinningImage.SetActive(true);
        yield return new WaitForSeconds(3);
        QuitGame();
        yield return 0;
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
