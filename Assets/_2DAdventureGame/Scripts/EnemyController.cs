using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2;
    Rigidbody2D rb;
    public bool vertical;
    public float changeTime = 3;
    private int direction = 1;
    private float timer;
    public float changeTimeVertical;
    Animator animator;
    private bool broken = true;
    public bool isBrooken { get { return broken; } }

    private AudioSource audioSource;
    public AudioClip enemyFixClip;

    public ParticleSystem smokeParticleEffect;
    public ParticleSystem fixedEffect;

    public event Action OnFixed;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timer = changeTime;
        changeTimeVertical = UnityEngine.Random.Range(1, 3);
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        timer -= Time.deltaTime;
        changeTimeVertical -= Time.deltaTime;
        if (changeTimeVertical < 0)
        {
            changeTimeVertical = UnityEngine.Random.Range(1, 3);
            vertical = !vertical;

        }
        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }
    void FixedUpdate()
    {
        if (!broken)
        {
            return;
        }
        Vector2 position = rb.position;
        if (vertical)
        {
            position.y += speed * Time.deltaTime * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x += speed * Time.deltaTime * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        rb.MovePosition(position);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }
    public void Fix()
    {
        OnFixed?.Invoke();
        broken = false;
        rb.simulated = false;
        animator.SetTrigger("Fixed");
        audioSource.Stop();
        audioSource.PlayOneShot(enemyFixClip);
        smokeParticleEffect.Stop();
        Instantiate(fixedEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
    }
}
