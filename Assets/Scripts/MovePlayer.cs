using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public Vector3 moveDirection;
    Vector3 _playerInput;
    float _horizontalInput, _verticalInput;
    private CharacterController _characterController;
    public float runSpeed; //Vitesse de déplacement
    public float jumpForce; //Force de saut
    public float gravityScale;


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }
    void Start()
    {
        
    }

    void MovePlayerTPS()

    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        _playerInput.x = _horizontalInput;
        _playerInput.z = _verticalInput;

        _characterController.Move(_playerInput * Time.deltaTime * runSpeed);

    }

    // Update is called once per frame
    void Update()
    {
        MovePlayerTPS();
    }
}
