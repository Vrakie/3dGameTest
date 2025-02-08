using Cinemachine;
using UnityEngine;
using StarterAssets;
using Photon.Pun;
using UnityEngine.Animations;

public class NewMonoBehaviourScript : MonoBehaviour
{
    #region VARIABLE
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity, aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask;
    [SerializeField] private LayerMask aimPlayerLayerMask;
    [SerializeField] private GameObject particle;
    [SerializeField] private AudioClip fireAudio;
    [SerializeField] private health stats;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private ParentConstraint fireballcontraint;
    [SerializeField] private Transform firePoint;
    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator animator;
    private bool isAiming;
    private Vector3 mouseWorldPosition;
    GameObject fireball;
    #endregion
    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
    }
    private void Start() => particle.SetActive(false);
    private void Update()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
            mouseWorldPosition = raycastHit.point;
        if (starterAssetsInputs.aim)
            Aim(true, ray);
        else
            Aim(false, ray);
        if (Input.GetMouseButtonDown(0) && isAiming)
            FireWeapon(ray.direction);
    }
    private void Aim(bool isAiming, Ray ray)
    {
        if (isAiming)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            this.isAiming = true;
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
            this.isAiming = false;
        }
    }
    [PunRPC]
    private void FireWeapon(Vector3 direction)
    {
        if (fireballPrefab != null && firePoint != null)
        {
            fireball = PhotonNetwork.Instantiate("PhotonPrefabs/BouleDeFeu", firePoint.position, Quaternion.identity);
            fireball.GetComponent<Rigidbody>().linearVelocity = direction * 40f;
        }
    }
}
