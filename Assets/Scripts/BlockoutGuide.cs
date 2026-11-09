using UnityEngine;

namespace OrbitalRescue
{
    /// <summary>Small walkthrough legend; no legacy input or UI package is required.</summary>
    public sealed class BlockoutGuide : MonoBehaviour
    {
        [SerializeField] private FirstPersonController controller;
        private GUIStyle title, text;

        private void OnGUI()
        {
            if (title == null)
            {
                title = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold };
                text = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true };
                title.normal.textColor = text.normal.textColor = Color.white;
            }
            float width = Mathf.Min(580, Screen.width - 32);
            GUI.color = new Color(0, 0, 0, .65f);
            GUI.DrawTexture(new Rect(16, 16, width, 32), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(16, Screen.height - 76, width, 60), Texture2D.whiteTexture);
            GUI.color = Color.white;
            float z = controller != null ? controller.transform.position.z : 0;
            string zone = z >= 16 ? "03 EXTERIOR" : z >= 12 ? "02 AIRLOCK" : "01 MISSION AREA";
            GUI.Label(new Rect(26, 20, width - 20, 28), "ORBITAL RESCUE / A1 / " + zone, title);
            string controls = controller != null && !controller.IsCaptured
                ? "Cursor released. Click or press gamepad A to resume. R / Select returns to spawn."
                : "WASD / arrows: move   Mouse: look   Shift: sprint   Esc: release cursor\nR: reset   Gamepad: sticks move/look, L3 sprint, Start cursor, Select reset";
            GUI.Label(new Rect(26, Screen.height - 70, width - 20, 52), controls, text);
        }
    }
}
