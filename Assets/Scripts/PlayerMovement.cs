using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 5f;

    void Start()
    {
        // Obtenemos la referencia al componente Rigidbody2D del objeto Player
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Capturamos la entrada horizontal (Teclas A/D o Flechas Izquierda/Derecha)
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Movemos el objeto horizontalmente manteniendo la velocidad vertical de la gravedad
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }
}