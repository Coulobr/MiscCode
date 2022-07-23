using System;
using System.Collections;
using UnityEngine;

namespace HitTrax.Bataround
{
	public class BataroundSprayChartPanel : Panel<BataroundSprayChartPanel>
	{
		public RenderTexture targetRenderTexture;

		private Camera sprayChartReportCam; // Camera for the spray chart
		private Rect originalSparyChartReportCameraCoords;
		private Color originalSparyChartReportColor;
		private SprayChart sprayChart;
		private IEnumerator cameraRectCR;

		protected override void OnOpened()
		{
			base.OnOpened();

			BataroundGameManager.Instance.OnHitterChanged += OnHitterChanged;

			var obj = Objects.Instance;
			sprayChart = obj.sprayChart;
			sprayChartReportCam = obj.camSprayChartReport().GetComponent<Camera>();

			originalSparyChartReportCameraCoords = sprayChartReportCam.rect;
			originalSparyChartReportColor = sprayChartReportCam.backgroundColor;

			sprayChartReportCam.gameObject.SetActive(true);

			sprayChartReportCam.targetTexture = targetRenderTexture;
			UpdateCameraRect(sprayChartReportCam, new Vector4(0, 0, 1, 1));
			sprayChartReportCam.cullingMask = LayerMask.GetMask("UI", "SprayChart", "Baseball Trail");
			sprayChartReportCam.depth = 0.9f;
			sprayChartReportCam.clearFlags = CameraClearFlags.SolidColor;
			sprayChartReportCam.backgroundColor = new Color(0f, 0f, 0f, 0f);
		}

		private void OnHitterChanged(User obj)
		{
			sprayChartReportCam.clearFlags = CameraClearFlags.SolidColor;
			sprayChartReportCam.backgroundColor = new Color(0f, 0f, 0f, 0f);
		}

		public override void SetupPanel()
		{
			base.SetupPanel();
		}

		protected override void OnClosed()
		{
			base.OnClosed();

			BataroundGameManager.Instance.OnHitterChanged -= OnHitterChanged;

			sprayChartReportCam.gameObject.SetActive(false);
			sprayChartReportCam.targetTexture = null;
			sprayChartReportCam.rect = originalSparyChartReportCameraCoords;
			sprayChartReportCam.backgroundColor = originalSparyChartReportColor;
			sprayChartReportCam.depth = 2f;
		}

		private void UpdateCameraRect(Camera desiredCam, Vector4 xywh)
		{
			if (cameraRectCR == null)
			{
				cameraRectCR = UpdateCameraRectCR(desiredCam, xywh);
				StartCoroutine(cameraRectCR);
			}
		}

		private IEnumerator UpdateCameraRectCR(Camera desiredCam, Vector4 xywh)
		{
			yield return null;
			desiredCam.rect = new Rect(xywh.x, xywh.y, xywh.z, xywh.w);
			cameraRectCR = null;
		}
	}
}