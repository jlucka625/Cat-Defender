using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : NetworkBehaviour
{
    public float MovingMaxVelocity = 10f;
    public float JumpMaxVelcoity = 10f;
    public bool IsDiscreteJump = true;

    public string HorizontalAxis = "Horizontal";
    public string JumpAxis = "Jump";

    public float MinimumControlHeight = 0.1f;

    private Rigidbody2D theRigidbody = null;
    // Facing vector
    private float facing = 1f;
    private bool controllable = true;

    // Use this for initialization
    void Start () {
        theRigidbody = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(2, 8);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backslash))
        {
            Application.LoadLevel(1);
        }
    }

	void FixedUpdate () {
        if (IsAbleToControl())
        {
            Vector2 velocity = theRigidbody.velocity;
            // Get the left or right
            float move = Input.GetAxis(HorizontalAxis);
            if (move != 0f)
            {
                velocity.x = move * MovingMaxVelocity;

                facing = move < 0 ? -1 : 1;
            }

            float jump = Input.GetAxis(JumpAxis);
            if (jump != 0f)
            {
                jump = IsDiscreteJump && jump > 0f ? 1f : jump;
                velocity.y = jump * JumpMaxVelcoity;
            }

            theRigidbody.velocity = velocity;
        }
    }

    public float GetFaceing()
    {
        return facing;
    }

    public bool IsAbleToControl()
    {
        return controllable;
    }
}
