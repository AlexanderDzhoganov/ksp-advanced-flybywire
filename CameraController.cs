using System;
using System.Reflection;
using UnityEngine;

namespace KSPAdvancedFlyByWire
{
    class CameraController
    {
        private const float ivaFovMax = 90f;
        private const float ivaFovMin = 7.5f;
        private const float ivaFovStep = 5f;
        private const float ivaPanStep = 100.0f;
        private const float mapZoomStep = 0.05f;

        private bool ivaCamFieldsLoaded = true;
        private FieldInfo ivaPitchField;
        private FieldInfo ivaYawField;
        private static CameraController instance;

        public static CameraController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CameraController();
                }
                return instance;
            }
        }

        public CameraController()
        {
            LoadIVAFields();
        }

        public void UpdateCameraProperties(float camPitch, float camYaw, float camZoom, float camSensitivity)
        {
            switch (CameraManager.Instance.currentCameraMode)
            {
                case CameraManager.CameraMode.Flight:
                {
                    FlightCamera.CamHdg += camYaw * camSensitivity;
                    FlightCamera.CamPitch += -camPitch * camSensitivity;
                    FlightCamera.fetch.SetDistance(FlightCamera.fetch.Distance + camZoom);
                    break;
                }
                case CameraManager.CameraMode.Map:
                {
                    PlanetariumCamera cam = PlanetariumCamera.fetch;
                    cam.camHdg += camYaw * camSensitivity;
                    cam.camPitch += -camPitch * camSensitivity;
                    cam.SetDistance(Utility.Clamp(cam.Distance + (cam.Distance * camZoom * mapZoomStep), cam.minDistance, cam.maxDistance));
                    break;
                }
                case CameraManager.CameraMode.IVA:
                {
                    //Hack: access private field that holds pitch/yaw in degrees before being applied to the camera.
                    if (this.ivaCamFieldsLoaded)
                    {
                        InternalCamera cam = InternalCamera.Instance;
                        ivaPitchField.SetValue(cam, (float)ivaPitchField.GetValue(cam) + camPitch * camSensitivity * ivaPanStep);
                        ivaYawField.SetValue(cam, (float)ivaYawField.GetValue(cam) + camYaw * camSensitivity * ivaPanStep);
                    }
                    if (camZoom != 0)
                    {
                        InternalCamera.Instance.SetFOV(Utility.Clamp(InternalCamera.Instance.camera.fieldOfView + camZoom * ivaFovStep, ivaFovMin, ivaFovMax));
                    }
                    break;
                }
                default:
                    Debug.LogWarning("AFBW - Unsupported CameraMode: "+ CameraManager.Instance.currentCameraMode.ToString());
                    break;
            }
        }

        private void LoadIVAFields()
        {
            FieldInfo[] fields = typeof(InternalCamera).GetFields(
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            this.ivaPitchField = fields[3];
            this.ivaYawField = fields[4];
            if (ivaPitchField == null || ivaYawField == null)
            {
                this.ivaCamFieldsLoaded = false;
                Debug.LogWarning("AFBW - Failed to acquire pitch/yaw fields in InternalCamera");
            }
        }

        // Old IVA cam updater. It takes exclusive control of the camera and doesn't reset on vehicle change.
        // It also needs to update rotation on EVERY Update() call or Squad will overwrite it before render.
        // Inspiration from: https://github.com/pizzaoverhead/KerbTrack/blob/master/KerbTrack/KerbTrack.cs#L402-459
        private void OldUpdateIVACamera(float camPitch, float camYaw, float camZoom, float camSensitivity)
        {
            float fovMaxIVA = 90f;
            float fovMinIVA = 10f;
            float pitchMaxIVA = 60f;
            float pitchMinIVA = -30f;
            float maxRot = 60f;
            float IVAscale = 10.0f;

            float pitchChange = Mathf.Clamp(camPitch * IVAscale * Time.deltaTime * 30f, pitchMinIVA, pitchMaxIVA);
            float yawChange = Mathf.Clamp(camYaw * IVAscale * Time.deltaTime * 30f, -maxRot, maxRot);

            InternalCamera cam = InternalCamera.Instance;
            cam.transform.localRotation *= Quaternion.Euler(-pitchChange, yawChange, 0);
            FlightCamera.fetch.transform.rotation = InternalSpace.InternalToWorld(cam.transform.rotation);
            Debug.Log("Cam rotation: " + cam.transform.localRotation.ToString());

            if (camZoom != 0)
            {
                InternalCamera.Instance.SetFOV(Utility.Clamp(InternalCamera.Instance.camera.fieldOfView + camZoom * IVAscale, fovMinIVA, fovMaxIVA));
            }
        }
    }
}
