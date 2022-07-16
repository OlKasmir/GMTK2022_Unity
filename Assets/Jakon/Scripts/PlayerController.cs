using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 100;
    [SerializeField] AudioClip duesenSound;

    Rigidbody2D rb;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        rb= GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            Antrieb();
        }
        else
        {
            StopSound();
        }
    }

    private void Antrieb()
    {
        rb.AddRelativeForce(Vector2.up * Time.deltaTime * speed);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(duesenSound);
        }
    }

    private void StopSound()
    {
        audioSource.Stop();
    }
}
