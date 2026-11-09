using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OrbitalRescue.Tests
{
    public class BlockoutSceneTests
    {
        private const string ScenePath = "Assets/Scenes/Blockout.unity";

        [Test]
        public void SavedSceneCanBeOpenedAndHasCompletePlayerCamera()
        {
            Assert.That(File.Exists(ScenePath), Is.True, "A1 needs a committed, editable Blockout.unity scene.");
            EditorSceneManager.OpenScene(ScenePath);
            var player = GameObject.Find("Player");
            Assert.That(player, Is.Not.Null);
            Assert.That(player.GetComponent<CharacterController>(), Is.Not.Null);
            Assert.That(player.GetComponent("FirstPersonController"), Is.Not.Null);
            var cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
            Assert.That(cameras.Length, Is.EqualTo(1));
            Assert.That(cameras[0].CompareTag("MainCamera"), Is.True);
            Assert.That(cameras[0].transform.IsChildOf(player.transform), Is.True);
            Assert.That(Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None).Length, Is.EqualTo(1));
        }

        [Test]
        public void WholeSceneHasNoMissingScriptsOrMaterials()
        {
            Assert.That(File.Exists(ScenePath), Is.True, "Scene has not been built.");
            var scene = EditorSceneManager.OpenScene(ScenePath);
            foreach (var root in scene.GetRootGameObjects())
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                Assert.That(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject), Is.Zero, transform.name);
            foreach (var renderer in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
            foreach (var material in renderer.sharedMaterials)
            {
                Assert.That(material, Is.Not.Null, renderer.name);
                Assert.That(material.shader, Is.Not.Null, renderer.name);
                Assert.That(ShaderUtil.ShaderHasError(material.shader), Is.False, material.name);
            }
        }

        [Test]
        public void FullHeightPlayerFitsBothHatchesAndEntireCentralRoute()
        {
            Assert.That(File.Exists(ScenePath), Is.True, "Scene has not been built.");
            EditorSceneManager.OpenScene(ScenePath);
            var player = GameObject.Find("Player");
            Assert.That(player, Is.Not.Null);
            player.SetActive(false);
            Physics.SyncTransforms();
            // Capsule clear of floor: 1.8 m tall, radius 0.30 m. Test each approach and both return directions.
            foreach (var start in new[] { new Vector3(0, 0, 2), new Vector3(0, 0, 27) })
            {
                var direction = start.z < 10 ? Vector3.forward : Vector3.back;
                bool blocked = Physics.CapsuleCast(start + Vector3.up * .33f, start + Vector3.up * 1.53f,
                    .30f, direction, out var hit, 25f, ~0, QueryTriggerInteraction.Ignore);
                Assert.That(blocked, Is.False, blocked ? "Route blocked by " + hit.collider.name : "");
            }
            foreach (var position in new[] {new Vector3(0, 0, 2), new Vector3(0, 0, 14), new Vector3(0, 0, 26)})
                Assert.That(Physics.Raycast(position + Vector3.up, Vector3.down, 2f), Is.True, "Missing floor at " + position);
        }

        [Test]
        public void InputAssetSupportsMovementLookAndCaptureWithoutLegacyInput()
        {
            var asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Input/OrbitalRescue.inputactions");
            Assert.That(asset, Is.Not.Null, "New Input System action asset must be saved.");
            foreach (var action in new[] {"Move", "MouseLook", "StickLook", "Sprint", "ReleaseCursor", "CaptureCursor", "Reset"})
                Assert.That(asset.FindAction("Gameplay/" + action), Is.Not.Null, action);
            var paths = asset.FindAction("Gameplay/Move").bindings.Select(b => b.path).ToArray();
            Assert.That(paths, Does.Contain("<Keyboard>/w"));
            Assert.That(paths, Does.Contain("<Gamepad>/leftStick"));
        }
    }
}
