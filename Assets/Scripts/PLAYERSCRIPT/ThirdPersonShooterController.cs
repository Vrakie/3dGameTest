using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using StarterAssets;
using Photon.Pun;
using UnityEngine.Animations;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity, aimSensitivity;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Slider fireballChargeSlider;
    [SerializeField] private Image sliderFillImage;

    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator animator;
    private Coroutine layerWeightCoroutine;
    private bool isAiming;
    private bool isCharging;
    private GameObject currentFireball;
    private ParentConstraint fireballParentConstraint;
    private GameObject currentFlamethower;
    private float fireballHoldTime;
    private const float holdDuration = 1f;
    private int sort = 1;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        SetupCameraAndControls(false, true, normalSensitivity, 0);
        if (fireballChargeSlider) fireballChargeSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleAiming();
        HandleWeaponFire();

        // Make the flamethrower follow the aim
        if (currentFlamethower != null)
        {
            Vector3 lookDirection = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
            lookDirection.y = 0f; // To keep the flamethrower level
            currentFlamethower.transform.forward = lookDirection.normalized;
        }
    }

    private void HandleAiming()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            sort = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            sort = 2;

        if (Input.GetMouseButtonUp(0) && currentFlamethower != null)
            Destroy(currentFlamethower);

        if (starterAssetsInputs.aim)
        {
            if (Camera.main != null)
                UpdatePlayerRotation(Camera.main.ScreenPointToRay(Input.mousePosition).direction);

            SetupCameraAndControls(true, false, aimSensitivity, 1);
            isAiming = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (isCharging)
            {
                if (fireballHoldTime >= holdDuration)
                    ReleaseFireball();
                else
                    DestroyFireball();
            }

            ResetFireballCharge();
            SetupCameraAndControls(false, true, normalSensitivity, 0);
            isAiming = false;

            if (currentFlamethower != null)
                Destroy(currentFlamethower);
        }
    }

    private void HandleWeaponFire()
    {
        if (isAiming && Input.GetMouseButtonDown(0))
        {
            if (sort == 1)
            {
                StartHoldingFireball();
                isCharging = true;
                if (fireballChargeSlider) fireballChargeSlider.gameObject.SetActive(true);
            }
            else if (sort == 2)
            {
                currentFlamethower = PhotonNetwork.Instantiate("PhotonPrefabs/LanceFlamme", firePoint.position, Quaternion.identity);
                currentFlamethower.transform.SetParent(firePoint);
            }
        }

        if (isCharging && Input.GetMouseButton(0))
        {
            fireballHoldTime += Time.deltaTime;
            UpdateChargeUI();
        }

        if (isCharging && Input.GetMouseButtonUp(0))
        {
            if (fireballHoldTime >= holdDuration)
                ReleaseFireball();
            else
                DestroyFireball();

            ResetFireballCharge();
        }
    }

    private void ResetFireballCharge()
    {
        fireballHoldTime = 0f;
        isCharging = false;
        if (fireballChargeSlider) fireballChargeSlider.gameObject.SetActive(false);
        if (sliderFillImage) sliderFillImage.color = Color.blue;
    }

    private void UpdateChargeUI()
    {
        if (fireballChargeSlider)
        {
            fireballChargeSlider.value = fireballHoldTime / holdDuration;
            if (sliderFillImage)
                sliderFillImage.color = (fireballChargeSlider.value >= 1f) ? Color.green : Color.blue;
        }
    }

    [PunRPC]
    private void StartHoldingFireball()
    {
        currentFireball = PhotonNetwork.Instantiate("PhotonPrefabs/BouleDeFeu", firePoint.position, Quaternion.identity);
        fireballParentConstraint = currentFireball.GetComponent<ParentConstraint>();

        if (fireballParentConstraint != null)
        {
            ConstraintSource source = new ConstraintSource { sourceTransform = firePoint, weight = 1f };
            fireballParentConstraint.AddSource(source);
            fireballParentConstraint.constraintActive = true;
        }
        currentFireball.GetComponent<Rigidbody>().isKinematic = true;
    }

    [PunRPC]
    private void ReleaseFireball()
    {
        if (currentFireball != null && fireballParentConstraint != null)
        {
            fireballParentConstraint.constraintActive = false;
            Rigidbody rb = currentFireball.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            if (Camera.main != null)
                rb.linearVelocity = Camera.main.ScreenPointToRay(Input.mousePosition).direction * 40f;

            currentFireball = null;
        }
    }

    private void DestroyFireball()
    {
        if (currentFireball != null)
        {
            PhotonNetwork.Destroy(currentFireball);
            currentFireball = null;
        }
    }

    private void SetupCameraAndControls(bool aim, bool isRotating, float sensitivity, int weight)
    {
        aimVirtualCamera.gameObject.SetActive(aim);
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