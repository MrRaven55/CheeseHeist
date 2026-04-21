using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;

    [Header("Input (per-player controls)")]
    [SerializeField] private KeyCode upButton = KeyCode.W;
    [SerializeField] private KeyCode downButton = KeyCode.S;
    [SerializeField] private KeyCode rightButton = KeyCode.D;
    [SerializeField] private KeyCode leftButton = KeyCode.A;

    [Header("Movement / identity")]
    [SerializeField] private int playerID = 1;
    [SerializeField] private float speed = 10f;

    public int PlayerID => playerID;
    public float Speed
    {
        get => speed;
        set => speed = Mathf.Max(0f, value);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        if (!isInMenu)
        {
            Movement();
        }
    }

    /// <summary>
    /// Simple input-based movement using AddForce(Impulse).
    /// </summary>
    private void Movement()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(upButton)) move.y += 1;
        if (Input.GetKey(downButton)) move.y -= 1;
        if (Input.GetKey(rightButton)) move.x += 1;
        if (Input.GetKey(leftButton)) move.x -= 1;

        move = move.normalized;
        rb.AddForce(move * speed, ForceMode.Impulse);
    }
}
