using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickTarget : MonoBehaviour
{

    public static int claireScore;
    public static int mouseScore;
    public static int racerScore;
    private string claireScoreText;
    private string mouseScoreText;
    private string racerScoreText;
    public Text scoreText;
    public AudioSource pickTargetSound;

    private Vector3[] targetLocations = {
        new Vector3(3.43549f, 21.19f, 49.67f),
        new Vector3(-28.564f, 22.35f, 49.92f),
        new Vector3(26.4354f, 22.85f, 69.17f),
        new Vector3(34.29f, 22.35f, 42.42f),
        new Vector3(-23.75f, 1.1f, 85.75f),
        new Vector3(25.25f, 1.1f, 72.5f)        
    };

    void OnTriggerEnter(Collider other)
    {
        int index = Random.Range(0, targetLocations.Length);
        this.gameObject.transform.position = targetLocations[index];
        switch (other.tag)
        {
            case "Claire":
                claireScore++;
                claireScoreText = "Claire: " + claireScore + "\n";
            break;

            case "Mouse":
                mouseScore++;
                mouseScoreText = "Mouse: " + mouseScore + "\n";
            break;

            case "Racer":
                racerScore++;
                racerScoreText = "Racer: " + racerScore;
            break;
        }
        scoreText.GetComponent<Text>().text = claireScoreText + mouseScoreText + racerScoreText;
        pickTargetSound.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        claireScore = 0;
        mouseScore = 0;
        racerScore = 0;
        pickTargetSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
