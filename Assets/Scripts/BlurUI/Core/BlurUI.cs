using UnityEngine;
using UnityEngine.UI;

namespace BlurUI
{
    [AddComponentMenu("BlurUI/BlurUI")]
    //[RequireComponent(typeof(Canvas))]
    public class BlurUI : MonoBehaviour
    {
		//[Range(0f, 1f)]
		//public float Transparency = 0.5f;

		[SerializeField]
		public Camera mainCamera;

		public BlurOption blurOption = BlurOption.BlurBehindUI;

		public BlurKernelSize kernalSize = BlurKernelSize.Medium;

		[Range(0, 4)]
		public int DownSample = 2;

		[Range(0, 4)]
		public int Iterations = 2;

		[Range(0, 60)]
		public int UpdateFrameRate = 60;


		public bool ApplyOnChildren = false;

		public bool MobileDevice = false;

		[Range(0f, 1f)]
		public float GreyScale = 0.0f;

		[Range(-1f, 1f)]
		public float Brightness = 0.0f;

		private Image[] uiImages;
		private Image uiImage;

		private Shader shader = null;
		private Material blurringMat = null;
		private BlurUICamera blurUICamera = null;
		private BlurUICameraMobile blurUICameraMobile = null;
		private void Awake()
		{
			blurUICamera = mainCamera.gameObject.GetComponent<BlurUICamera>();
			blurUICameraMobile = mainCamera.gameObject.GetComponent<BlurUICameraMobile>();
		}

		public void AddBlurringComponent()
		{
			if (shader == null)
				shader = Shader.Find("Custom/Blurring");

			if (blurringMat)
				blurringMat = new Material(shader);


			if (ApplyOnChildren)
			{
				AddBlurringComponentOnChildren();
			}
			else
			{
				uiImage = this.GetComponent<Image>();
				if (uiImage != null)
				{
					Blurring blurring = uiImage.gameObject.GetComponent<Blurring>();
					if (blurring == null)
					{
						blurring = uiImage.gameObject.AddComponent<Blurring>();
						blurring.SetBlurringMaterial(blurringMat);
						blurring.SetGreyScale(GreyScale);
						blurring.SetBrightness(Brightness);
					}
				}
			}
		}

		public void AddBlurUICamera()
		{
			if (mainCamera)
			{
				blurUICamera = mainCamera.gameObject.GetComponent<BlurUICamera>();
				if (blurUICamera == null)
				{
					mainCamera.gameObject.AddComponent<BlurUICamera>();
				}
			}
		}

		public void RemoveBlurUICamera()
		{
			if (mainCamera)
			{
				BlurUICamera buc = mainCamera.gameObject.GetComponent<BlurUICamera>();
				if (buc != null)
				{
					DestroyImmediate(buc);
					blurUICamera = null;
				}
			}
		}

		public void AddBlurUICameraMobile()
		{
			if (mainCamera)
			{
				blurUICameraMobile = mainCamera.gameObject.GetComponent<BlurUICameraMobile>();
				if (blurUICameraMobile == null)
				{
					mainCamera.gameObject.AddComponent<BlurUICameraMobile>();
				}
			}
		}

		public void RemoveBlurUICameraMobile()
		{
			if (mainCamera)
			{
				BlurUICameraMobile buc = mainCamera.gameObject.GetComponent<BlurUICameraMobile>();
				if (buc != null)
				{
					DestroyImmediate(buc);
					blurUICameraMobile = null;
				}
			}
		}

		public void AddBlurringComponentOnChildren()
		{
			uiImages = this.GetComponentsInChildren<Image>();
			for (int i = 0; i < uiImages.Length; i++)
			{
				Blurring blurring = uiImages[i].gameObject.GetComponent<Blurring>();
				if (blurring == null)
				{
					blurring = uiImages[i].gameObject.AddComponent<Blurring>();
					blurring.SetBlurringMaterial(blurringMat);
					blurring.SetGreyScale(GreyScale);
					blurring.SetBrightness(Brightness);
				}
			}
		}

		public void RemoveBlurUI()
		{
			foreach (Image image in GetComponentsInChildren<Image>())
			{
				Blurring blurring = image.GetComponent<Blurring>();
				if (blurring != null)
				{
					DestroyImmediate(blurring);
				}
			}
			RemoveBlurUICamera();
			RemoveBlurUICameraMobile();

			BlurUI bui = GetComponent<BlurUI>();
			if (bui != null)
			{
				DestroyImmediate(bui);
			}
		}


		public void RemoveBlurringComponentFromChildren()
		{
			foreach (Image image in GetComponentsInChildren<Image>())
			{
				Blurring blurring = image.GetComponent<Blurring>();
				BlurUI blurUI = image.GetComponent<BlurUI>();
				if (blurring != null && blurUI == null)
				{
					DestroyImmediate(blurring);
				}
			}
			uiImages = this.GetComponentsInChildren<Image>();
		}

		public void ApplyCameraProperties()
		{
			if (MobileDevice)
			{
				if (blurUICameraMobile != null)
				{
					blurUICameraMobile.blurOption = blurOption;
					blurUICameraMobile.DownSample = DownSample;
					blurUICameraMobile.Iterations = Iterations;
					blurUICameraMobile.UpdateFrameRate = UpdateFrameRate;
				}
			}
			else
			{
				if (blurUICamera != null)
				{
					blurUICamera.blurOption = blurOption;
					blurUICamera.DownSample = DownSample;
					blurUICamera.Iterations = Iterations;
					blurUICamera.kernalSize = kernalSize;
					blurUICamera.UpdateFrameRate = UpdateFrameRate;
				}
			}

		}

		// Update is called once per frame
		void Update()
		{
			if (ApplyOnChildren == true)
			{
				for (int i = 0; i < uiImages.Length; i++)
				{
					Blurring blurring = uiImages[i].gameObject.GetComponent<Blurring>();
					blurring.SetGreyScale(GreyScale);
					blurring.SetBrightness(Brightness);
				}
			}
			else
			{
				if (uiImage != null)
				{
					Blurring blurring = uiImage.gameObject.GetComponent<Blurring>();
					blurring.SetGreyScale(GreyScale);
					blurring.SetBrightness(Brightness);
				}
			}


			if (blurOption == BlurOption.BlurBackground)
			{
				if (MobileDevice)
				{
					if (blurUICameraMobile != null)
					{
						blurUICameraMobile.Brightness = Brightness;
						blurUICameraMobile.GreyScale = GreyScale;
					}
				}
				else
				{
					if (blurUICamera != null)
					{
						blurUICamera.Brightness = Brightness;
						blurUICamera.GreyScale = GreyScale;
					}
				}
			}
		}
	}
}

