using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlurUI
{
	[AddComponentMenu("BlurUI/Blurring")]
	[RequireComponent(typeof(Image))]
	public class Blurring : MonoBehaviour
	{
		private Image image = null;
		private Material blurMat = null;
		private Material blurringMat = null;
		private BlurUICamera blurUICamera = null;
		private BlurUICameraMobile blurUICameraMobile = null;

		[Range(0f, 1f)]
		public float Transparency = 0.5f;

		[Range(0f, 1f)]
		public float GreyScale = 0.0f;

		[Range(-1f, 1f)]
		public float Brightness = 0.0f;

		private int _BlurTexID;
		private int _GreyScaleID;
		private int _BrightnessID;
		private bool _initialized = false;

		void Start()
		{
			blurUICamera = FindObjectOfType<BlurUICamera>();
			if (blurUICamera != null)
			{
				image = this.GetComponent<Image>();
				if (blurUICamera.blurOption == BlurOption.BlurBehindUI)
				{
					if (blurringMat != null)
					{
						image.material = blurringMat;
					}
					else
					{
						Shader blurImage = Shader.Find("Custom/Blurring");
						blurMat = new Material(blurImage);
						image.material = blurMat;
					}
					_BlurTexID = Shader.PropertyToID("_BlurTex");
					_GreyScaleID = Shader.PropertyToID("_GreyScale");
					_BrightnessID = Shader.PropertyToID("_Brightness");
				}
			}
			else
			{
				blurUICameraMobile = FindObjectOfType<BlurUICameraMobile>();
				if (blurUICameraMobile != null)
				{
					image = this.GetComponent<Image>();
					if (blurUICameraMobile.blurOption == BlurOption.BlurBehindUI)
					{
						if (blurringMat != null)
						{
							image.material = blurringMat;
						}
						else
						{
							Shader blurImage = Shader.Find("Custom/Blurring");
							blurMat = new Material(blurImage);
							image.material = blurMat;
						}
						_BlurTexID = Shader.PropertyToID("_BlurTex");
						_GreyScaleID = Shader.PropertyToID("_GreyScale");
						_BrightnessID = Shader.PropertyToID("_Brightness");
					}
				}
			}
			if (image)
			{
				Color color = image.color;
				Transparency = 1 - color.a;
			}
			_initialized = true;
		}

		public void SetBlurringMaterial(Material mat)
		{
			blurringMat = mat;
		}

		/*public void SetTransparency(float transparency)
		{
			Transparency = transparency;
		}*/

		public bool initialized
		{
			get
			{
				return _initialized;
			}
		}

		public bool UseBlurMaterial
		{
            set
            {
				if (initialized)
                {
					if (value)
					{
						image.material = blurringMat != null ? blurringMat : blurMat;
					}
					else
					{
						image.material = null;
					}
				}
			}
		}

		public void SetGreyScale(float greyScale)
		{
			GreyScale = greyScale;
		}

		public void SetBrightness(float brightness)
		{
			Brightness = brightness;
		}

		void LateUpdate()
		{
			if (blurUICamera && blurUICamera.blurOption == BlurOption.BlurBehindUI)
			{
				if (blurUICamera.BlurRT != null)
				{
					image.materialForRendering.SetTexture(_BlurTexID, blurUICamera.BlurRT);
					//image.material.SetTexture (_BlurTexID, blurUICamera.BlurRT);
					image.material.SetFloat(_GreyScaleID, GreyScale);
					image.material.SetFloat(_BrightnessID, Brightness);
				}
			}
			else if (blurUICameraMobile && blurUICameraMobile.blurOption == BlurOption.BlurBehindUI)
			{
				if (blurUICameraMobile.BlurRT != null)
				{
					image.materialForRendering.SetTexture(_BlurTexID, blurUICameraMobile.BlurRT);
					//image.material.SetTexture (_BlurTexID, blurUICameraMobile.BlurRT);
					image.material.SetFloat(_GreyScaleID, GreyScale);
					image.material.SetFloat(_BrightnessID, Brightness);
				}
			}
			if (image)
			{
				Color color = image.color;
				Transparency = 1 - color.a;
				//color.a = 1.0f - Transparency;
				//image.color = color;
			}
		}
	}
}
