using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Health : MonoBehaviour
{
    public Image healthBar;
    public float health = 1;
    public GameObject GameOver;

    private void Update()
    {
        if(health <= 0)
        {
            GameOver.SetActive(true);
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(20);
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            Healing(10);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.fillAmount = health / 1;
    }


    public void Healing(float healPoints)
    {
        health += healPoints;
        health = Mathf.Clamp(health, 0, 1);

        healthBar.fillAmount = health / 1;
    }
}
