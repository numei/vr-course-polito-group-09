using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace OrbitalRescue.Editor
{
    /// <summary>Creates real editable scene objects. Nothing is generated on entering Play mode.</summary>
    public static class BlockoutBuilder
    {
        public const string ScenePath = "Assets/Scenes/Blockout.unity";
        private static Material hull, floor, frame, equipment, accent, grid, screen, earth;

        [MenuItem("Orbital Rescue/Rebuild A1 Blockout")]
        private static void RebuildFromMenu()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (File.Exists(ScenePath) && !EditorUtility.DisplayDialog("Rebuild A1 blockout?",
                "This replaces Blockout.unity with the reference layout. Save a copy first if you made manual changes.", "Rebuild", "Cancel")) return;
            Build();
        }

        [MenuItem("Orbital Rescue/Open A1 Walkthrough")]
        public static void OpenWalkthrough()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(ScenePath);
            EditorApplication.ExecuteMenuItem("Window/General/Game");
        }

        public static void Build()
        {
            foreach (var path in new[] {"Assets/Materials", "Assets/Prefabs", "Assets/Scenes"}) Directory.CreateDirectory(path);
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            hull = Material("Hull", new Color(.66f, .69f, .70f));
            floor = Material("Floor", new Color(.38f, .42f, .44f));
            frame = Material("Frame", new Color(.25f, .29f, .31f));
            equipment = Material("Equipment", new Color(.50f, .54f, .56f));
            accent = Material("Identification", new Color(.72f, .56f, .32f));
            grid = Material("ScaleGrid", new Color(.48f, .52f, .53f));
            screen = Material("DisplayPlaceholder", new Color(.30f, .44f, .49f));
            earth = Material("EarthPlaceholder", new Color(.31f, .42f, .48f));

            var environment = Group("Environment");
            MissionArea(Group("01_MissionArea", environment));
            Airlock(Group("02_Airlock", environment));
            Exterior(Group("03_Exterior", environment));
            Context(Group("Context"));
            Lighting(Group("Lighting"));
            Player();

            RenderSettings.skybox = null;
            RenderSettings.fog = false;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(.65f, .68f, .72f);
            Lightmapping.realtimeGI = false;
            Lightmapping.bakedGI = false;
            PlayerSettings.companyName = "Polito Group 09";
            PlayerSettings.productName = "Orbital Rescue";
            EditorSettings.serializationMode = SerializationMode.ForceText;
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
            EditorBuildSettings.scenes = new[] {new EditorBuildSettingsScene(ScenePath, true)};
            EditorBuildSettings.RemoveConfigObject("com.unity.input.settings.actions");
            AssetDatabase.SaveAssets();
            Debug.Log("A1 blockout saved: " + ScenePath);
        }

        private static void MissionArea(Transform root)
        {
            Floor(root, 8, 12, 6);
            Box("Wall_Left", root, new Vector3(-4.12f, 1.8f, 6), new Vector3(.24f, 3.6f, 12.48f), hull);
            Box("Wall_Right", root, new Vector3(4.12f, 1.8f, 6), new Vector3(.24f, 3.6f, 12.48f), hull);
            Box("Wall_Back", root, new Vector3(0, 1.8f, -.12f), new Vector3(8, 3.6f, .24f), hull);
            Box("Ceiling", root, new Vector3(0, 3.72f, 6), new Vector3(8.48f, .24f, 12.48f), hull);
            Hatch(root, "Hatch_Interior", 12, 8, 3.6f);
            for (int z = 2; z <= 10; z += 4)
            {
                Box("StructuralRib_Left_" + z, root, new Vector3(-3.94f, 1.8f, z), new Vector3(.12f, 3.6f, .16f), frame);
                Box("StructuralRib_Right_" + z, root, new Vector3(3.94f, 1.8f, z), new Vector3(.12f, 3.6f, .16f), frame);
                Box("CeilingRib_" + z, root, new Vector3(0, 3.52f, z), new Vector3(8, .16f, .16f), frame);
            }
            var terminal = Group("MissionTerminal", root);
            Box("TerminalVolume", terminal, new Vector3(3.05f, .65f, 3.5f), new Vector3(1.1f, 1.3f, 2.4f), equipment);
            Box("DisplayVolume", terminal, new Vector3(3.05f, 1.42f, 3.6f), new Vector3(1.05f, .55f, .16f), screen);
            var tools = Group("ToolRack", root);
            Box("RackVolume", tools, new Vector3(-3.25f, 1.05f, 5), new Vector3(.9f, 2.1f, 2), equipment);
            for (int i = 0; i < 3; i++)
                Box("ToolPlaceholder_" + i, tools, new Vector3(-2.74f, 1f + i * .28f, 5), new Vector3(.12f, .1f, .65f), accent);
            foreach (float x in new[] {-3.2f, 3.2f})
                Box(x < 0 ? "Storage_Left" : "Storage_Right", root, new Vector3(x, .7f, 9), new Vector3(1, 1.4f, 2), equipment);
            Box("ViewportPlaceholder", root, new Vector3(3.98f, 2.35f, 9), new Vector3(.05f, 1f, 1.6f), screen);
            // Handrail volumes are future XR interaction anchors; in A1 they are static geometry.
            RailSegment("InteriorHandrail_Left", root, new Vector3(-3.7f, 1.1f, 7), new Vector3(-3.7f, 1.1f, 11), .06f, accent);
            RailSegment("InteriorHandrail_Right", root, new Vector3(3.7f, 1.1f, 5.5f), new Vector3(3.7f, 1.1f, 7.5f), .06f, accent);
        }

        private static void Airlock(Transform root)
        {
            Floor(root, 4, 4, 14);
            Box("Wall_Left", root, new Vector3(-2.12f, 1.6f, 14), new Vector3(.24f, 3.2f, 4), hull);
            Box("Wall_Right", root, new Vector3(2.12f, 1.6f, 14), new Vector3(.24f, 3.2f, 4), hull);
            Box("Ceiling", root, new Vector3(0, 3.32f, 14), new Vector3(4.48f, .24f, 4), hull);
            Hatch(root, "Hatch_Exterior", 16, 4, 3.2f);
            Box("SuitLockerVolume", root, new Vector3(-1.65f, .95f, 14), new Vector3(.55f, 1.9f, 1.2f), equipment);
            Box("AirlockPanelVolume", root, new Vector3(1.96f, 1.4f, 14), new Vector3(.08f, .55f, .7f), accent);
        }

        private static void Exterior(Transform root)
        {
            Floor(root, 12, 12, 22);
            // Rails have solid lower infill, so a walking capsule cannot accidentally fall off the prototype.
            foreach (float x in new[] {-5.92f, 5.92f})
            {
                Box("RailInfill_" + x, root, new Vector3(x, .57f, 22), new Vector3(.16f, 1.14f, 12), frame);
                RailSegment("Handrail_" + x, root, new Vector3(x, 1.2f, 16), new Vector3(x, 1.2f, 28), .08f, accent);
                for (int z = 16; z <= 28; z += 2)
                    Box("RailPost_" + x + "_" + z, root, new Vector3(x, .6f, z), new Vector3(.13f, 1.2f, .13f), equipment);
            }
            Box("FarBoundary", root, new Vector3(0, .57f, 27.92f), new Vector3(12, 1.14f, .16f), frame);
            RailSegment("FarHandrail", root, new Vector3(-6, 1.2f, 27.92f), new Vector3(6, 1.2f, 27.92f), .08f, accent);
            foreach (float x in new[] {-4f, 4f})
                Box("NearBoundary_" + x, root, new Vector3(x, .57f, 16.08f), new Vector3(4, 1.14f, .16f), frame);

            var repair = Group("RepairInterface", root);
            Box("RepairCabinet", repair, new Vector3(-3.6f, .85f, 24), new Vector3(1.2f, 1.7f, 2), equipment);
            Box("ConnectorPanel", repair, new Vector3(-3.6f, 1.1f, 22.96f), new Vector3(.9f, .65f, .08f), frame);
            for (int i = 0; i < 2; i++)
                Primitive("PowerConnector_" + i, PrimitiveType.Cylinder, repair, new Vector3(-3.85f + i * .5f, 1.1f, 22.85f),
                    new Vector3(.22f, .12f, .22f), accent).transform.rotation = Quaternion.Euler(90, 0, 0);
            var bin = Group("DebrisCollection", root);
            Box("BinVolume", bin, new Vector3(3.65f, .6f, 21.25f), new Vector3(1.3f, 1.2f, 1.5f), accent);
            var debris = Group("DebrisPlaceholders", root);
            var positions = new[] {new Vector3(-1.7f, 1.25f, 22), new Vector3(1.8f, 1.6f, 23.5f), new Vector3(-2, 1.35f, 25.5f)};
            for (int i = 0; i < positions.Length; i++)
            {
                var item = Primitive("StaticDebris_" + (i + 1), i == 1 ? PrimitiveType.Sphere : PrimitiveType.Cube,
                    debris, positions[i], Vector3.one * .4f, equipment);
                item.transform.rotation = Quaternion.Euler(20 + i * 14, 35 + i * 12, 18);
            }
            Solar(Group("Solar_Left", root), -8.4f);
            Solar(Group("Solar_Right", root), 8.4f);
        }

        private static void Solar(Transform root, float x)
        {
            Box("Support", root, new Vector3(x * .72f, .45f, 24), new Vector3(4f, .2f, .3f), frame);
            Box("PanelVolume", root, new Vector3(x, 1.2f, 24), new Vector3(3.2f, .12f, 6), screen);
            for (int i = 0; i <= 3; i++)
                Box("PanelDivision_" + i, root, new Vector3(x, 1.29f, 21 + i * 2), new Vector3(3.2f, .07f, .07f), frame, false);
        }

        private static void Hatch(Transform parent, string name, float z, float width, float height)
        {
            var root = Group(name, parent);
            foreach (float sign in new[] {-1f, 1f})
            {
                Box("WallShoulder_" + sign, root, new Vector3(sign * (width / 4 + .6f), height / 2, z),
                    new Vector3(width / 2 - 1.2f, height, .24f), hull);
                Box("FrameUpright_" + sign, root, new Vector3(sign * 1.3f, 1.3f, z - .02f), new Vector3(.2f, 2.6f, .32f), frame);
            }
            Box("WallAbove", root, new Vector3(0, (height + 2.6f) / 2, z), new Vector3(2.4f, height - 2.6f, .24f), hull);
            Box("FrameHeader", root, new Vector3(0, 2.7f, z - .02f), new Vector3(2.8f, .2f, .32f), accent);
        }

        private static void Floor(Transform parent, float width, float length, float centerZ)
        {
            Box("Floor", parent, new Vector3(0, -.1f, centerZ), new Vector3(width, .2f, length), floor);
            var lines = Group("MetreGrid", parent);
            for (float x = -width / 2 + 1; x < width / 2; x++)
                Box("GridX_" + x, lines, new Vector3(x, .003f, centerZ), new Vector3(.018f, .004f, length), grid, false);
            for (float z = centerZ - length / 2 + 1; z < centerZ + length / 2; z++)
                Box("GridZ_" + z, lines, new Vector3(0, .003f, z), new Vector3(width, .004f, .018f), grid, false);
        }

        private static void Context(Transform root)
        {
            var globe = Primitive("Earth_ScalePlaceholder", PrimitiveType.Sphere, root,
                new Vector3(0, -15, 75), Vector3.one * 70, earth, false);
            var renderer = globe.GetComponent<Renderer>();
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }

        private static void Lighting(Transform root)
        {
            var sun = new GameObject("NeutralPreviewLight");
            sun.transform.SetParent(root);
            sun.transform.rotation = Quaternion.Euler(45, -35, 0);
            var light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.color = Color.white;
            light.shadows = LightShadows.Hard;
            foreach (float z in new[] {4f, 10f, 14f})
            {
                var go = new GameObject("InteriorPreviewLight_" + z);
                go.transform.SetParent(root);
                go.transform.position = new Vector3(0, 2.6f, z);
                var point = go.AddComponent<Light>();
                point.type = LightType.Point;
                point.range = 9;
                point.intensity = 6f;
                point.color = Color.white;
                point.shadows = LightShadows.None;
            }
        }

        private static void Player()
        {
            var root = new GameObject("Player");
            root.transform.position = new Vector3(0, .05f, 2);
            var body = root.AddComponent<CharacterController>();
            body.height = 1.8f;
            body.radius = .3f;
            body.center = Vector3.up * .9f;
            body.skinWidth = .03f;
            body.stepOffset = .2f;
            body.slopeLimit = 45;
            body.minMoveDistance = 0;
            var view = new GameObject("FirstPersonCamera");
            view.tag = "MainCamera";
            view.transform.SetParent(root.transform, false);
            view.transform.localPosition = Vector3.up * 1.65f;
            var camera = view.AddComponent<Camera>();
            camera.fieldOfView = 75;
            camera.nearClipPlane = .05f;
            camera.farClipPlane = 250;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.018f, .023f, .035f);
            view.AddComponent<AudioListener>();
            var controller = root.AddComponent<FirstPersonController>();
            var serialized = new SerializedObject(controller);
            serialized.FindProperty("inputActions").objectReferenceValue = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Input/OrbitalRescue.inputactions");
            serialized.FindProperty("cameraPivot").objectReferenceValue = view.transform;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            var guide = root.AddComponent<BlockoutGuide>();
            var guideSerialized = new SerializedObject(guide);
            guideSerialized.FindProperty("controller").objectReferenceValue = controller;
            guideSerialized.ApplyModifiedPropertiesWithoutUndo();
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, "Assets/Prefabs/Player.prefab");
            Object.DestroyImmediate(root);
            var instance = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
            instance.name = "Player";
        }

        private static Transform Group(string name, Transform parent = null)
        {
            var go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent, false);
            return go.transform;
        }

        private static GameObject Box(string name, Transform parent, Vector3 position, Vector3 size, Material material, bool collision = true)
            => Primitive(name, PrimitiveType.Cube, parent, position, size, material, collision);

        private static GameObject Primitive(string name, PrimitiveType type, Transform parent, Vector3 position, Vector3 size, Material material, bool collision = true)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localScale = size;
            go.GetComponent<Renderer>().sharedMaterial = material;
            if (!collision) Object.DestroyImmediate(go.GetComponent<Collider>());
            return go;
        }

        private static void RailSegment(string name, Transform parent, Vector3 a, Vector3 b, float thickness, Material material)
        {
            var go = Box(name, parent, (a + b) / 2, new Vector3(thickness, thickness, Vector3.Distance(a, b)), material);
            go.transform.rotation = Quaternion.LookRotation(b - a);
        }

        private static Material Material(string name, Color color)
        {
            string path = "Assets/Materials/" + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(material, path);
            }
            material.SetColor("_BaseColor", color);
            material.SetFloat("_Smoothness", 0);
            EditorUtility.SetDirty(material);
            return material;
        }
    }
}
