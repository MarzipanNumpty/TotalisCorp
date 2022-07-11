using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wrenchScript : MonoBehaviour
{
    Animator anim;
    AudioSource audioData;
    int dnaHealth = 5;
    [SerializeField] GameObject endGameCanvas;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        audioData = GetComponent<AudioSource>();
    }

    public void stopSwinging()
    {
        anim.SetBool("swing1", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("enemy"))
        {
            audioData.Play(0);
            other.GetComponent<enemyScript>().takeDamage(3);
        }
        if (other.CompareTag("meleeEnemy"))
        {
            audioData.Play(0);
            other.GetComponent<meleeEnemyScript>().takeDamage(2);
        }
        if (other.CompareTag("dna"))
        {
            audioData.Play(0);
            dnaHealth--;
            if(dnaHealth <= 0)
            {
                endGameCanvas.SetActive(true);
                PlayerManager.instance.player.GetComponent<Animator>().SetBool("end", true);
            }
        }
    }
}
