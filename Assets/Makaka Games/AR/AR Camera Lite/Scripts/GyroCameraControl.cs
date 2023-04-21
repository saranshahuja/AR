/*
===================================================================
Unity Assets by MAKAKA GAMES: https://makaka.org/o/all-unity-assets
===================================================================

Online Docs (Latest): https://makaka.org/unity-assets
Offline Docs: You have a PDF file in the package folder.

=======
SUPPORT
=======

First of all, read the docs. If it didn’t help, get the support.

Web: https://makaka.org/support
Email: info@makaka.org

If you find a bug or you can’t use the asset as you need, 
please first send email to info@makaka.org
before leaving a review to the asset store.

I am here to help you and to improve my products for the best.
*/

using UnityEngine;
using UnityEngine.Events;

[HelpURL("https://makaka.org/unity-assets")]
public class GyroCameraControl : MonoBehaviour 
{
	private Gyroscope gyro;
	private bool isGyroSupportedNotInEditor = false;
	private bool isRotationWithGyro = false;
	private Quaternion rotationFix = new Quaternion (0f, 0f, 1f, 0f);

	[SerializeField]
	private Transform gyroCamera;

	[Tooltip("To Reset GYRO Data.")]
	[SerializeField]
	private bool isGyroDisabledOnDestroy = false;

	[Header("Testing")]
	[SerializeField]
	private bool isGyroUnsupportedNotInEditorTest = false;

#if UNITY_EDITOR

	[SerializeField]
	private bool isGyroSupportedInEditorTest = true;

	[Space]
	[SerializeField]
	private bool isMovementWASDQEInEditorTest = false;

	[SerializeField]
	private Vector3 movementWASDQESpeed = new Vector3(4f, 5f, 5f);

	private readonly string xAxisName = "Horizontal";
	private readonly string zAxisName = "Vertical";

#endif

	[Header("Events")]
	[Space]
	[SerializeField]
	private UnityEvent OnGyroInitialized;

	[Space]
	[SerializeField]
	private UnityEvent OnGyroIsNotSupported;

	[Space]
	[SerializeField]
	private UnityEvent OnInitializedNotInEditor = null;

#if UNITY_EDITOR

	[Space]
	[SerializeField]
	private UnityEvent OnInitializedInEditor = null;

#endif

	[Header("Accelerometer Settings")]
	[Tooltip("1f => no vibrations")]
	[Range(1f, 20f)]
	[SerializeField]
	private float accelerometerSensitivityXZ = 5f;

	[Tooltip("if > 1f => use it for smooth motion")]
	[Range(0f, 5f)]
	[SerializeField]
	private float accelerometerSmoothLimitXZ = 0.5f;

	private readonly float accelerometerRotationalAngleFactorXZ = -90f;
	private Vector3 accelerometerCurrentRotationXZ;
	private Quaternion accelerometerResultRotationXZ;

	[SerializeField]
	private float accelerometerSensitivityY = 0.11f;

	// Rotational Speed: Left and Right
	[SerializeField]
	private float accelerometerRotationalSpeedFactorY = 50f;
	private float accelerometerRotationalSpeedY;

	private Vector3 accelerometerDirNormalized;

	private bool isRotationWithAccelerometer = false;
	private bool isAccelerometerSupportedNotInEditor = false;

	private void Start() 
	{

#if UNITY_EDITOR

		InitInEditor();

#else

		InitNotInEditor();

#endif

	}

	private void InitNotInEditor()
    {
		isGyroSupportedNotInEditor = SystemInfo.supportsGyroscope;

		if (isGyroSupportedNotInEditor && isGyroUnsupportedNotInEditorTest)
        {
			isGyroSupportedNotInEditor = false;
		}

		isAccelerometerSupportedNotInEditor = SystemInfo.supportsGyroscope;

		if (isGyroSupportedNotInEditor)
		{
			gyroCamera.parent.transform.rotation =
				Quaternion.Euler(90f, 180f, 0f);

			gyro = Input.gyro;
			gyro.enabled = true;

			OnGyroInitialized.Invoke();
		}
		else
		{
			OnGyroIsNotSupported.Invoke();
		}

		OnInitializedNotInEditor.Invoke();
	}

#if UNITY_EDITOR

	private void InitInEditor()
	{
		if (isGyroSupportedInEditorTest)
		{
			OnGyroInitialized.Invoke();
		}
		else
		{
			OnGyroIsNotSupported.Invoke();
		}

		OnInitializedInEditor.Invoke();
	}

	private void LateUpdate()
	{
        if (isMovementWASDQEInEditorTest)
        {
			MoveXUpdateInEditor();
			MoveYUpdateInEditor();
			MoveZUpdateInEditor();
		}
	}

	private void MoveXUpdateInEditor()
	{
		transform.position += gyroCamera.right
			* Input.GetAxis(xAxisName) * movementWASDQESpeed.x * Time.deltaTime;
	}

	private void MoveZUpdateInEditor()
	{
		transform.position +=
			Vector3.ProjectOnPlane(gyroCamera.forward, Vector3.up).normalized
			* Input.GetAxis(zAxisName) * movementWASDQESpeed.z * Time.deltaTime;
	}

	private void MoveYUpdateInEditor()
	{
		if (Input.GetKey(KeyCode.Q))
		{
			TranslateYAxis(-movementWASDQESpeed.y);
		}

		if (Input.GetKey(KeyCode.E))
		{
			TranslateYAxis(movementWASDQESpeed.y);
		}
	}

	private void TranslateYAxis(float speed)
	{
		transform.Translate(0f, speed * Time.deltaTime, 0f);
	}

#else

	private void Update() 
	{
		UpdateNotInEditor();
	}

#endif

	private void UpdateNotInEditor()
    {
		if (isGyroSupportedNotInEditor && isRotationWithGyro)
		{
			gyroCamera.localRotation = gyro.attitude * rotationFix;

			//DebugPrinter.Print(gyro.attitude);
		}
		else if (isAccelerometerSupportedNotInEditor
			&& isRotationWithAccelerometer)
		{
			RotateYWithAccelerometer();
			RotateXZWithAccelerometer();
		}
	}

	private void RotateYWithAccelerometer()
	{
		accelerometerRotationalSpeedY =
			Input.acceleration.x * accelerometerRotationalSpeedFactorY;

		accelerometerDirNormalized = Input.acceleration.normalized;

		if (accelerometerDirNormalized.x >= accelerometerSensitivityY
			|| accelerometerDirNormalized.x <= -accelerometerSensitivityY)
		{
			gyroCamera.Rotate(
				0f,
				accelerometerSensitivityY * accelerometerRotationalSpeedY,
				0f);
		}
	}

	private void RotateXZWithAccelerometer()
	{
		accelerometerCurrentRotationXZ.y = gyroCamera.localEulerAngles.y;

		accelerometerCurrentRotationXZ.x =
			Input.acceleration.z * accelerometerRotationalAngleFactorXZ;

		accelerometerCurrentRotationXZ.z =
			Input.acceleration.x * accelerometerRotationalAngleFactorXZ;

		accelerometerResultRotationXZ = Quaternion.Slerp(
			gyroCamera.localRotation,
			Quaternion.Euler(accelerometerCurrentRotationXZ),
			Time.deltaTime * accelerometerSensitivityXZ
			);

		if (Quaternion.Angle(gyroCamera.rotation, accelerometerResultRotationXZ)
			> accelerometerSmoothLimitXZ)
		{
			gyroCamera.localRotation = accelerometerResultRotationXZ;
		}
		else
		{
			gyroCamera.localRotation = Quaternion.Slerp(
				gyroCamera.localRotation,
				Quaternion.Euler(accelerometerCurrentRotationXZ),
				Time.deltaTime
				);
		}
	}

	private void OnDestroy()
	{
		if (gyro != null && isGyroDisabledOnDestroy)
		{
			gyro.enabled = false;

			//print("Reset Gyro!");
		} 
	}

	public void SetPositionAndRotation (Transform transform)
	{
		gyroCamera.position  = transform.position;
		gyroCamera.rotation = transform.rotation;
	}

	public void SetRotationWithGyroActive(bool isActive)
	{
		isRotationWithGyro = isActive;
	}

	public void SetRotationWithAccelerometerActive(bool isActive)
	{
		isRotationWithAccelerometer = isActive;
	}

}
