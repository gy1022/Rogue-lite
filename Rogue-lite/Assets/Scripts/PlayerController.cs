using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;

    public float speed = 5.0f;
    public float runSpeed = 10.0f;
    public float acceleration = 10.0f;
    public float currentSpeed = 0.0f;

    public float jumpHeight = 2.0f;
    public float gravity = -9.91f;

    public float dashSpeed = 20.0f;
    public float dashTime = 2.0f;
    public float dashCooldown = 5.0f;

    private float dashCounter;
    private float dashTimer;
    private bool isDashing = false;

    public bool isAction = false;

    private Vector3 velocity;
    private bool canJumpAgain = true;
    public Transform cam;
    Vector3 moveDir;

    public ActionData[] actionDataList = new ActionData[10];
    public ComboSystem comboSystem;
    public Transform fxTransform;

    private float actionTimer = 0f;
    private ActionData currentAction = null;
    private GameObject currentFx = null;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool isGrounded = controller.isGrounded;

        if(!isAction)
        {
            MoveLogic(isGrounded);
            JumpLogic(isGrounded);
        }
        else
        {
            UpdateAction();
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }

    void UndateAction()
    {
        if (currentAction == null) return;
        actionTimer += Time.deltaTime;

    }

    void SpawnFx()
    {
        if(currentAction.fxObject != null)
        {
            currentFx = Instantiate(currentAction.fxObject, fxTransform);
            currentFx.transform.localPosition = Vector3.zero;
            currentFx.transform.localRotation = Quaternion.identity;
        }
    }

    void EndAction()
    {
        isAction = false;
        if(currentFx != null)
        {
            Destroy(currentFx);
        }
        currentAction = null;
        currentFx = null;
    }

    void UpdateAction()
    {
        if (currentAction == null) return;
        actionTimer += Time.deltaTime;

        if(actionTimer >= currentAction.fxTime && currentFx == null)
        {
            SpawnFx();
        }

        if(actionTimer >= currentAction.waitTime)
        {
            EndAction();
        }
    }

    public void MoveLogic(bool isGrounded)
    {
        if(isGrounded)
        {
            velocity.y = -2f;
            canJumpAgain = true;
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsLanding", true);
        }
        else
        {
            if(velocity.y < 0)
            {
                animator.SetBool("IsFalling", true);
            }
            else
            {
                animator.SetBool("IsFalling", false);
            }
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        animator.SetFloat("moveSpeed", Mathf.Lerp(0, 1, currentSpeed / speed));

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.x) * Mathf.Rad2Deg + cam.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            currentSpeed = Mathf.MoveTowards(currentSpeed, speed, acceleration * Time.deltaTime);

            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }
        else
        {
            currentSpeed = 0.0f;
        }

    }

    public void JumpLogic(bool isGrounded)
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(isGrounded)
            {
                animator.SetBool("IsJumping", true);
                animator.SetBool("IsFalling", false);
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            else if(canJumpAgain)
            {
                animator.SetBool("IsJumping", true);
                animator.SetBool("IsFalling", false);
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                canJumpAgain = false;
            }
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }
    }

}
