using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement")]
    public float maxSpeed = 10.0f;
    public float gravity = -30.0f;
    public float jumpHeight = 3.0f;
    public Vector3 velocity;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundRadius = 0.5f;
    public LayerMask groundMask;
    public bool isGrounded;

    [Header("MiniMap")] 
    public GameObject miniMap;

    [Header("Player Sounds")] 
    public AudioSource jumpSound;
    public AudioSource hitSound;


    [Header("HealthBar")] 
    public HealthBarScreenSpaceController healthBar;

    [Header("Player Abilities")] 
    [Range(0, 100)]
    public int health = 100;


    private Vector3 m_touchesEnded;

    private float x, z;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame - once every 16.6666ms

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2.0f;
        }
        //input for webgl
        // float x = Input.GetAxis("Horizontal");
        // float z = Input.GetAxis("Vertical");
        float x=0.0f;
        float z=0.0f;

        float direction = 0.0f;
        //input touch
        foreach (var touch in Input.touches)
        {
            var worldTouch = Camera.main.ScreenToViewportPoint(touch.position);
            x = worldTouch.x;
         //  z = worldTouch.y;
            m_touchesEnded = worldTouch;
            Debug.Log(m_touchesEnded.ToString());
            if (x>transform.position.x)
            {
                direction = 1.0f;
            }
            if (x < transform.position.x)
            {
                direction = -1.0f;
            }
        }

        Debug.Log(direction);
        Vector3 move = transform.right * x*direction;// + transform.forward * z;

        controller.Move(move * maxSpeed * Time.deltaTime);

        if (Input.GetButton("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
            jumpSound.Play();
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.M))
        {
            // toggle the MiniMap on/off
            miniMap.SetActive(!miniMap.activeInHierarchy);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        hitSound.Play();
        healthBar.TakeDamage(damage);
        if (health < 0)
        {
            health = 0;
        }
    }

   
}
