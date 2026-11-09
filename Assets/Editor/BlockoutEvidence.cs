using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace OrbitalRescue.Editor
{
    public static class BlockoutEvidence
    {
        [MenuItem("Orbital Rescue/Capture A1 Views")]
        public static void Capture()
        {
            EditorSceneManager.OpenScene(BlockoutBuilder.ScenePath);
            const string output = "Docs/Validation";
            Directory.CreateDirectory(output);
            var camera = Camera.main;
            Vector3 savedPosition = camera.transform.position;
            Quaternion savedRotation = camera.transform.rotation;
            CaptureView(camera, output + "/01-mission-area.png", new Vector3(0, 1.65f, 2), Quaternion.Euler(5, 0, 0));
            CaptureView(camera, output + "/02-airlock.png", new Vector3(0, 1.65f, 12.8f), Quaternion.Euler(3, 0, 0));
            CaptureView(camera, output + "/03-exterior.png", new Vector3(0, 1.9f, 18), Quaternion.Euler(6, 0, 0));
            foreach (var renderer in Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
                if (renderer.name == "Ceiling" || renderer.name.StartsWith("CeilingRib") || renderer.name == "Earth_ScalePlaceholder") renderer.enabled = false;
            camera.orthographic = true;
            camera.orthographicSize = 17;
            var overviewPosition = new Vector3(22, 31, -9);
            CaptureView(camera, output + "/04-overview.png", overviewPosition,
                Quaternion.LookRotation(new Vector3(0, 0, 14) - overviewPosition));
            camera.orthographic = false;
            camera.transform.SetPositionAndRotation(savedPosition, savedRotation);
            // Re-open instead of saving preview-only camera and roof visibility changes.
            EditorSceneManager.OpenScene(BlockoutBuilder.ScenePath);
            Debug.Log("A1 scene views captured in " + output);
        }

        private static void CaptureView(Camera camera, string path, Vector3 position, Quaternion rotation)
        {
            camera.transform.SetPositionAndRotation(position, rotation);
            var target = new RenderTexture(1600, 1000, 24);
            var previous = RenderTexture.active;
            camera.targetTexture = target;
            camera.Render();
            RenderTexture.active = target;
            var image = new Texture2D(1600, 1000, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 1600, 1000), 0, 0);
            image.Apply();
            File.WriteAllBytes(path, image.EncodeToPNG());
            camera.targetTexture = null;
            RenderTexture.active = previous;
            Object.DestroyImmediate(image);
            Object.DestroyImmediate(target);
        }
    }
}
