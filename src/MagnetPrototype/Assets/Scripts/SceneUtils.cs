
    using Cinemachine;
    using UnityEngine;

    public static class SceneUtils
    {
        public static void SwitchCamera(string name)
        {
            var cameras = GameObject.FindObjectsOfType<CinemachineVirtualCamera>(true);

            foreach (var camera in cameras)
            {
                camera.gameObject.SetActive(camera.name == name);
            }
        }
    }
