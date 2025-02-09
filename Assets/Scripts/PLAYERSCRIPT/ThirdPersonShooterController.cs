using Cinemachine;
using UnityEngine;
using System.Collections;
using StarterAssets;
using Photon.Pun;
using UnityEngine.Animations;

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
    private GameObject currentFireball;
    private ParentConstraint fireballParentConstraint;
    private float fireballHoldTime;
    private const float holdDuration = 1f;

    private void Awake() => (starterAssetsInputs, thirdPersonController, animator) = (GetComponent<StarterAssetsInputs>(), GetComponent<ThirdPersonController>(), GetComponent<Animator>());

    private void Start() => SetupCameraAndControls(false, true, normalSensitivity, 0);

    private void Update()
    {
        HandleAiming();
        HandleWeaponFire();
    }

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
            StartHoldingFireball();

        if (isAiming && Input.GetMouseButton(0))
        {
            fireballHoldTime += Time.deltaTime;
        }

        if (isAiming && Input.GetMouseButtonUp(0))
        {
            if (fireballHoldTime >= holdDuration)
                ReleaseFireball();
            else
                DestroyFireball();  // Destroy fireball if not held for 1 second

            fireballHoldTime = 0f;
        }
    }

    [PunRPC]
    private void StartHoldingFireball()
    {
        if (fireballPrefab && firePoint)
        {
            currentFireball = PhotonNetwork.Instantiate("PhotonPrefabs/BouleDeFeu", firePoint.position, Quaternion.identity);
            fireballParentConstraint = currentFireball.GetComponent<ParentConstraint>();

            if (fireballParentConstraint != null)
            {
                ConstraintSource source = new ConstraintSource
                {
                    sourceTransform = firePoint,
                    weight = 1f
                };
                fireballParentConstraint.AddSource(source);
                fireballParentConstraint.constraintActive = true;
            }

            currentFireball.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    [PunRPC]
    private void ReleaseFireball()
    {
        if (currentFireball && fireballParentConstraint != null)
        {
            fireballParentConstraint.constraintActive = false;
            currentFireball.GetComponent<Rigidbody>().isKinematic = false;

            Vector3 direction = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
            currentFireball.GetComponent<Rigidbody>().linearVelocity = direction * 40f;

            currentFireball = null;
        }
    }

    private void DestroyFireball()
    {
        if (currentFireball != null)
        {
            PhotonNetwork.Destroy(currentFireball);  // Destroys the fireball if not held for 1 second
            currentFireball = null;
        }
    }

    private void SetupCameraAndControls(bool Aim, bool isRotating, float sensitivity, int weight)
    {
        aimVirtualCamera.gameObject.SetActive(Aim);
        thirdPersonController.SetSensitivity(sensitivity);
        thirdPersonController.SetRotateOnMove(isRotating);
        if (layerWeightCoroutine != null)
            StopCoroutine(layerWeightCoroutine);
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

    private void UpdatePlayerRotation(Vector3 direction)
    {
        direction.y = 0f;
        transform.forward = Vector3.Lerp(transform.forward, direction.normalized, Time.deltaTime * 20f);
    }
}
