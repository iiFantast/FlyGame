using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController1 : MonoBehaviour
{
    public float thrustForce = 15f; // Сила тяги (ускорение)
    public float maxSpeed = 8f; // Максимальная скорость
    public float rotationSpeed = 150f;
    public float drag = 2f; // Скорость торможения (чем меньше, тем дольше скользит)
    public int maxHealth = 10;
    private int currentHealth;
    private float thrustInput;
    private float rotationInput;
    private Vector2 movement;
    private Rigidbody2D rb;

    public TextMeshProUGUI healthText;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        UpdateHealthUI();
        
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions.Disable();
        playerInput.actions.FindActionMap("Movement").Enable();
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        thrustInput = input.y;
        rotationInput = input.x;
    }

    public void OnFire(InputValue value)
    {
        if (Time.time >= nextFireTime)
        {
            if (firePoint != null && bulletPrefab != null)
            {
                Instantiate(bulletPrefab, firePoint.position, transform.rotation);
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void FixedUpdate()
    {
        // Поворот самолёта (A/D или стрелки влево/вправо)
        float rotation = -rotationInput * rotationSpeed * Time.fixedDeltaTime;
        rb.rotation += rotation;

        // Применяем силу для плавного ускорения
        if (thrustInput > 0)
        {
            Vector2 thrustDirection = transform.right * thrustInput;
            rb.AddForce(thrustDirection * thrustForce);

            // Ограничиваем максимальную скорость
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
        else
        {
            // Плавное замедление
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, drag * Time.fixedDeltaTime);
        }      
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Hazard"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthUI();
        Debug.Log("Player1 took damage! Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText !=null)
        {
            healthText.text = "Helath: + " + currentHealth;
        }
    }

    private void Die()
    {
        Debug.Log("Player1 is dead!");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver("Player1");
        }
        
        gameObject.SetActive(false);
    }
}