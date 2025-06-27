using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using NUnit.Framework;

public class Movement : NetworkBehaviour
{
    [SerializeField] CharacterController controller = null;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpHeight = 2f;
    float gravity = -9.81f;

    [SerializeField] Transform Feet = null;
    float CheckRadius = 0.4f;
    [SerializeField] LayerMask GroundMask = new LayerMask();
    Vector3 velocity;
    bool isGrounded;

    [SerializeField] Animator FpsAnimator = null, TpsAnimator = null;

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        isGrounded = Physics.CheckSphere(Feet.transform.position, CheckRadius, GroundMask);

        if (isGrounded && velocity.y <= 0)
        {
            velocity.y = -2f; //stick player to ground
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x != 0 || z != 0)
        {
            FpsAnimator.SetBool("isWalking", true);
            TpsAnimator.SetBool("isWalking", true);
        }
        else
        {
            FpsAnimator.SetBool("isWalking", false);
            TpsAnimator.SetBool("isWalking", false);
        }

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
