using System;
using System.Linq;
using System.Collections.Generic;
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
        private const float flightZoomStep = 0.5f;
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
            LoadReflectionFields();
        }

        public void UpdateCameraProperties(float camPitch, float camYaw, float camZoom, float camSensitivity)
        {
            switch (CameraManager.Instance.currentCameraMode)
            {
                case CameraManager.CameraMode.Flight:
                {
                    FlightCamera.CamHdg += camYaw * camSensitivity;
                    FlightCamera.CamPitch += -camPitch * camSensitivity;
                    FlightCamera.fetch.SetDistance(FlightCamera.fetch.Distance + camZoom * flightZoomStep);
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
                case CameraManager.CameraMode.Internal:
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
                       // InternalCamera.Instance.SetFOV(Utility.Clamp(InternalCamera.Instance.camera.fieldOfView + camZoom * ivaFovStep, ivaFovMin, ivaFovMax));
                    }
                    break;
                }
                default:
                    Debug.LogWarning("AFBW - Unsupported CameraMode: "+ CameraManager.Instance.currentCameraMode.ToString());
                    break;
            }
        }

        public void NextIVACamera()
        {
            if (CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.IVA
                && FlightGlobals.ActiveVessel.GetCrewCount() > 1)
            {
                CameraManager.Instance.NextCameraIVA();
            }
        }

        public void FocusIVAWindow()
        {
            switch (CameraManager.Instance.currentCameraMode)
            {
                case CameraManager.CameraMode.IVA:
                {
                    Kerbal kerbal = GetActiveIVAKerbal();

                    if (kerbal != null && kerbal.InPart != null && kerbal.InPart.internalModel != null)
                    {
                        //Debug.Log("Kerbal: " + kerbal.crewMemberName + " inside "+ kerbal.InPart.internalModel.internalName);
                        InternalModel intModel = kerbal.InPart.internalModel;
                        Transform camTransform = InternalCamera.Instance.transform;
                        Ray ray = new Ray(camTransform.position, camTransform.forward);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 5f))
                        {
                            //Debug.Log("RaycastHit: " + hit.collider.name + " (dist: " + hit.distance + ")");
                            foreach (InternalCameraSwitch intCam in intModel.FindModelComponents<InternalCameraSwitch>())
                            {
                                if (hit.collider.name.Equals(intCam.colliderTransformName))
                                {
                                    intCam.Button_OnDoubleTap();
                                }
                            }
                        }
                    }
                    break;
                }
                case CameraManager.CameraMode.Internal:
                {
                    CameraManager.Instance.SetCameraIVA();
                    break;
                }
            }
        }

        private Kerbal GetActiveIVAKerbal()
        {
            List<ProtoCrewMember> vesselCrew = FlightGlobals.fetch.activeVessel.GetVesselCrew();
            foreach (ProtoCrewMember pcm in vesselCrew)
            {
                if (pcm.KerbalRef.eyeTransform == InternalCamera.Instance.transform.parent)
                {
                    return pcm.KerbalRef;
                }
            }
            return null;
        }

        private void LoadReflectionFields()
        {
            List<FieldInfo> fields = new List<FieldInfo>(typeof(InternalCamera).GetFields(
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            fields = new List<FieldInfo>(fields.Where(f => f.FieldType.Equals(typeof(float))));
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
               // InternalCamera.Instance.SetFOV(Utility.Clamp(InternalCamera.Instance.camera.fieldOfView + camZoom * IVAscale, fovMinIVA, fovMaxIVA));
            }
        }
    }
}
