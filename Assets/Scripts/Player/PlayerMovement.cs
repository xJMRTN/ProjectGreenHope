using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    [SerializeField] float movementMultiplier;
    [SerializeField] float airMultiplier;
    [SerializeField] float movementDrag;
    [SerializeField] float airDrag;
    public float jumpForce;


    [Header("Sprinting")]
    public float walkSpeed;
    public float sprintSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float boostForce;

    [Header("Misc")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform orientation;
    [SerializeField] CameraController cameraController;

    float horizontalMovement;
    float verticalMovement;
    
    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    bool isGrounded;

    public bool isDriving = false;

    public bool Frozen = false;

    RaycastHit groundHit;

    Vector4 movementValues;

    void Start(){
        rb.freezeRotation = true;
        movementValues = new Vector4(moveSpeed, walkSpeed, sprintSpeed, jumpForce);
    }

    void Update(){
        if(Frozen) return;
        if(isDriving) return;
        isGrounded = isPlayerOnGround();
        moveDirection = GetPlayerInput();
        CalculateDrag();
        CalculateSpeed();
        CalculateJump();
        CalculateSlope();
    }

    void FixedUpdate(){
        MovePlayer();
    }

    Vector3 GetPlayerInput(){
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");
        return (orientation.forward * verticalMovement) + (orientation.right * horizontalMovement);
    }

    void MovePlayer(){
        if(isGrounded && !OnSlope()) rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        else if(isGrounded && OnSlope())  rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        else rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
    }

    void CalculateDrag(){
        if(isGrounded) rb.drag = movementDrag;
         else rb.drag = airDrag;             
    }

    bool isPlayerOnGround(){
        if(Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out groundHit)){
            if(groundHit.distance < 0.2f){
                return true;
            }else  return false;    
        }
        return false;    
    }

    void CalculateJump(){
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded && !Input.GetKeyDown(KeyCode.LeftControl)){
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    bool OnSlope(){
         if(Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out groundHit)){
             if(groundHit.normal != Vector3.up){
                 return true;
             }else return false;
         }
         return false;
    }

    void CalculateSlope(){
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, groundHit.normal);
    }

    void CalculateSpeed(){
        if(Input.GetKey(KeyCode.LeftShift) && isGrounded){
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }else moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
    }

    public void Intro(){
        moveSpeed = 1f;
        walkSpeed = 1f;
        sprintSpeed = 1f;
        jumpForce = 0.1f;
    }

    public void Reset(){
        moveSpeed = movementValues.x;
        walkSpeed =movementValues.y;
        sprintSpeed = movementValues.z;
        jumpForce = movementValues.w;
    }

    public void ToggleMouse(bool locked){
        cameraController.locked = locked;
        if(locked)Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;    
    }
}