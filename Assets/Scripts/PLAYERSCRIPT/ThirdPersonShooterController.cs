using Cinemachine;
using UnityEngine;
using System.Collections;
using StarterAssets;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity, aimSensitivity;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint;

    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator animator;
    private Coroutine layerWeightCoroutine;
    private bool isAiming;
    private void Awake() => (starterAssetsInputs, thirdPersonController, animator) = (GetComponent<StarterAssetsInputs>(), GetComponent<ThirdPersonController>(), GetComponent<Animator>());

    private void Update() { HandleAiming(); HandleWeaponFire(); }

    private void HandleAiming()
    {
        if (starterAssetsInputs.aim)
        {
            UpdatePlayerRotation(Camera.main.ScreenPointToRay(Input.mousePosition).direction);
            SetupCameraAndControls(true, false, aimSensitivity, 1);
            isAiming = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            SetupCameraAndControls(false, true, normalSensitivity, 0);
            isAiming = false;
        }
    }
    private void HandleWeaponFire()
    {
        if (isAiming && Input.GetMouseButtonDown(0))        
            FireWeapon(Camera.main.ScreenPointToRay(Input.mousePosition).direction);        
    }
    private void SetupCameraAndControls(bool isAiming, bool isRotating, float sensitivity, int weight)
    {
        aimVirtualCamera.gameObject.SetActive(isAiming);
        thirdPersonController.SetSensitivity(sensitivity);
        thirdPersonController.SetRotateOnMove(isRotating);

        if (layerWeightCoroutine != null)
        {
            StopCoroutine(layerWeightCoroutine);
        }
        layerWeightCoroutine = StartCoroutine(AnimateLayerWeight(weight));
    }

    private IEnumerator AnimateLayerWeight(int targetWeight)
    {
        float currentWeight = animator.GetLayerWeight(1);
        float elapsedTime = 0f;
        const float duration = 0.2f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            animator.SetLayerWeight(1, Mathf.Lerp(currentWeight, targetWeight, elapsedTime / duration));
            yield return null;
        }

        animator.SetLayerWeight(1, targetWeight);
    }

    [PunRPC]
    private void FireWeapon(Vector3 direction)
    {
        if (fireballPrefab && firePoint)
        {
            var fireball = PhotonNetwork.Instantiate("PhotonPrefabs/BouleDeFeu", firePoint.position, Quaternion.identity);
            fireball.GetComponent<Rigidbody>().linearVelocity = direction * 40f;
        }
    }

    private void UpdatePlayerRotation(Vector3 direction)
    {
        direction.y = 0f;
        transform.forward = Vector3.Lerp(transform.forward, direction.normalized, Time.deltaTime * 20f);
    }
}
/*using Cinemachine;
using UnityEngine;
using System.Collections;
using StarterAssets;
using Photon.Pun;

public class NewMonoBehaviourScript : MonoBehaviour
{
    #region VARIABLE
    [SerializeField] CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] float normalSensitivity, aimSensitivity;
    [SerializeField] LayerMask aimColliderLayerMask;
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform firePoint;

    StarterAssetsInputs starterAssetsInputs;
    ThirdPersonController thirdPersonController;
    Animator animator;
    bool isAiming;

    GameObject fireball;
    Coroutine coroutineLayerWeight;
    #endregion

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (starterAssetsInputs.aim)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            SetPosPlayer(ray.direction);

            Setup(true, false, aimSensitivity, 1);
            isAiming = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            Setup(false, true, normalSensitivity, 0);
            isAiming = false;
        }

        if (isAiming && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            FireWeapon(ray.direction);
        }
    }
    void Setup(bool cam, bool rotate, float sensitivity, int weight)
    {
        aimVirtualCamera.gameObject.SetActive(cam);
        thirdPersonController.SetSensitivity(sensitivity);
        thirdPersonController.SetRotateOnMove(rotate);

        if (coroutineLayerWeight != null)
        {
            StopCoroutine(coroutineLayerWeight);
        }
        coroutineLayerWeight = StartCoroutine(AnimateLayerWeight(weight));
    }

    IEnumerator AnimateLayerWeight(float targetWeight)
    {
        float currentWeight = animator.GetLayerWeight(1);
        float elapsedTime = 0f;
        float duration = 0.2f; 

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newWeight = Mathf.Lerp(currentWeight, targetWeight, elapsedTime / duration);
            animator.SetLayerWeight(1, newWeight);
            yield return null;
        }

        animator.SetLayerWeight(1, targetWeight);
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
    void SetPosPlayer(Vector3 aimDirection)
    {
        aimDirection.y = 0f;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection.normalized, Time.deltaTime * 20f);
    }  
}*/
