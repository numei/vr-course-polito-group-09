using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace OrbitalRescue.Tests
{
    public class FirstPersonTests
    {
        private Keyboard keyboard;
        private Mouse mouse;
        private Gamepad gamepad;
        private Transform player;
        private Camera camera;
        private int oldCaptureRate;
        private InputSettings.BackgroundBehavior oldBackgroundBehavior;
#if UNITY_EDITOR
        private InputSettings.EditorInputBehaviorInPlayMode oldEditorInputBehavior;
#endif

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            Assert.That(Application.CanStreamedLevelBeLoaded("Blockout"), Is.True, "Saved Blockout scene must be in build settings.");
            oldCaptureRate = Time.captureFramerate;
            Time.captureFramerate = 60;
            oldBackgroundBehavior = InputSystem.settings.backgroundBehavior;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
#if UNITY_EDITOR
            // Batch mode has no focused Game view. Route virtual keyboard/mouse events to the player.
            oldEditorInputBehavior = InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
#endif
            keyboard = InputSystem.AddDevice<Keyboard>();
            mouse = InputSystem.AddDevice<Mouse>();
            gamepad = InputSystem.AddDevice<Gamepad>();
            yield return SceneManager.LoadSceneAsync("Blockout");
            player = GameObject.Find("Player").transform;
            camera = Camera.main;
            Assert.That(player.GetComponent("FirstPersonController"), Is.Not.Null, "Missing desktop controller.");
            InputSystem.QueueStateEvent(mouse, new MouseState().WithButton(MouseButton.Left));
            yield return null;
            InputSystem.QueueStateEvent(mouse, new MouseState());
            yield return Frames(3);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (keyboard != null && keyboard.added) InputSystem.RemoveDevice(keyboard);
            if (mouse != null && mouse.added) InputSystem.RemoveDevice(mouse);
            if (gamepad != null && gamepad.added) InputSystem.RemoveDevice(gamepad);
            Time.captureFramerate = oldCaptureRate;
            InputSystem.settings.backgroundBehavior = oldBackgroundBehavior;
#if UNITY_EDITOR
            InputSystem.settings.editorInputBehaviorInPlayMode = oldEditorInputBehavior;
#endif
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            yield return null;
        }

        [UnityTest]
        public IEnumerator WasdMovesForwardAndStaysOnFloor()
        {
            var start = player.position;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.W));
            yield return Frames(30);
            Assert.That(player.position.z - start.z, Is.InRange(1.1f, 1.4f));
            Assert.That(player.position.y, Is.InRange(-.1f, .15f));
        }

        [UnityTest]
        public IEnumerator DiagonalDoesNotMoveFasterThanStraight()
        {
            var start = player.position;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.W));
            yield return Frames(24);
            float straight = HorizontalDistance(start, player.position);
            Assert.That(straight, Is.GreaterThan(.5f), "Straight movement must work before comparing diagonal speed.");
            yield return Reset();
            start = player.position;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.W, Key.D));
            yield return Frames(24);
            Assert.That(HorizontalDistance(start, player.position), Is.EqualTo(straight).Within(.08f));
        }

        [UnityTest]
        public IEnumerator InteriorWallsStopThePlayerCapsule()
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.A));
            yield return Frames(180);
            Assert.That(player.position.x, Is.InRange(-3.75f, -3.45f));
            Assert.That(player.position.y, Is.GreaterThan(-.1f));
        }

        [UnityTest]
        public IEnumerator WalksThroughBothHatchesAndReturnsFromExterior()
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.W, Key.LeftShift));
            yield return Frames(360);
            Assert.That(player.position.z, Is.GreaterThan(24f), "Cannot reach exterior through both hatches.");
            Assert.That(player.position.z, Is.LessThan(28f), "Exterior boundary failed.");
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.S, Key.LeftShift));
            yield return Frames(270);
            Assert.That(player.position.z, Is.LessThan(9f), "Cannot return to mission area.");
            Assert.That(player.position.y, Is.GreaterThan(-.1f));
        }

        [UnityTest]
        public IEnumerator EscapeStopsWalkingAndClickResumes()
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.W, Key.Escape));
            yield return Frames(3);
            var paused = player.position;
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.W));
            yield return Frames(20);
            Assert.That(HorizontalDistance(paused, player.position), Is.LessThan(.01f));
            Assert.That(Cursor.lockState, Is.EqualTo(CursorLockMode.None));
            InputSystem.QueueStateEvent(mouse, new MouseState().WithButton(MouseButton.Left));
            yield return Frames(20);
            Assert.That(player.position.z - paused.z, Is.GreaterThan(.5f));
        }

        [UnityTest]
        public IEnumerator MouseRotatesYawAndClampsPitch()
        {
            InputSystem.QueueStateEvent(mouse, new MouseState { delta = new Vector2(120f, 0f) });
            yield return Frames(2);
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(0, player.eulerAngles.y)), Is.GreaterThan(5f));
            InputSystem.QueueStateEvent(mouse, new MouseState { delta = new Vector2(0, 10000f) });
            yield return Frames(2);
            float pitch = Mathf.DeltaAngle(0, camera.transform.localEulerAngles.x);
            Assert.That(pitch, Is.InRange(-85f, -60f), "Look must clamp without flipping.");
        }

        [UnityTest]
        public IEnumerator ResetRestoresSpawnAndView()
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.W, Key.D));
            InputSystem.QueueStateEvent(mouse, new MouseState { delta = new Vector2(100f, 100f) });
            yield return Frames(20);
            yield return Reset();
            Assert.That(Vector3.Distance(player.position, new Vector3(0, .05f, 2)), Is.LessThan(.12f));
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(0, player.eulerAngles.y)), Is.LessThan(.1f));
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(0, camera.transform.localEulerAngles.x)), Is.LessThan(.1f));
        }

        [UnityTest]
        public IEnumerator GamepadSticksMoveAndLook()
        {
            var start = player.position;
            InputSystem.QueueStateEvent(gamepad, new GamepadState { leftStick = Vector2.up, rightStick = Vector2.right * .6f });
            yield return Frames(30);
            Assert.That(HorizontalDistance(start, player.position), Is.GreaterThan(.7f));
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(0, player.eulerAngles.y)), Is.GreaterThan(15f));
        }

        [UnityTest]
        public IEnumerator FocusLossStopsNavigationUntilExplicitResume()
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.W));
            yield return Frames(5);
            player.gameObject.SendMessage("OnApplicationFocus", false);
            var stopped = player.position;
            yield return Frames(12);
            Assert.That(HorizontalDistance(stopped, player.position), Is.LessThan(.01f));
            player.gameObject.SendMessage("OnApplicationFocus", true);
            yield return Frames(12);
            Assert.That(HorizontalDistance(stopped, player.position), Is.LessThan(.01f), "Focus regain must not capture automatically.");
            InputSystem.QueueStateEvent(mouse, new MouseState().WithButton(MouseButton.Left));
            yield return Frames(12);
            Assert.That(player.position.z - stopped.z, Is.GreaterThan(.3f));
        }

        [UnityTest]
        public IEnumerator GamepadLookHasSameAngularSpeedAt30And120Fps()
        {
            foreach (int rate in new[] {30, 120})
            {
                InputSystem.QueueStateEvent(gamepad, new GamepadState());
                yield return Reset();
                Time.captureFramerate = rate;
                InputSystem.QueueStateEvent(gamepad, new GamepadState {rightStick = Vector2.right});
                yield return Frames(rate / 2);
                Assert.That(Mathf.DeltaAngle(0, player.eulerAngles.y), Is.EqualTo(60f).Within(4.1f), "Angular speed at " + rate + " fps");
            }
        }

        private IEnumerator Reset()
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.R));
            InputSystem.QueueStateEvent(mouse, new MouseState());
            yield return Frames(2);
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return Frames(2);
        }

        private static IEnumerator Frames(int count)
        {
            for (int i = 0; i < count; i++) yield return null;
        }

        private static float HorizontalDistance(Vector3 a, Vector3 b)
        {
            a.y = b.y = 0;
            return Vector3.Distance(a, b);
        }
    }
}
