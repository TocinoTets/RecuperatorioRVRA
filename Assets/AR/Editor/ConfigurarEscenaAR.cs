using System.IO;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.XR.ARSubsystems;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public static class ConfigurarEscenaAR
{
    const string Raiz = "Assets/AR";
    const string Escena = "Assets/Scenes/SistemaSolarAR.unity";
    static readonly Vector2 TamanioCarta = new(0.08f, 0.11f);

    static readonly (string nombre, Color color, float diametro, bool anillos, string dato)[] Planetas =
    {
        ("Sol",     new Color(1f, 0.6f, 0.1f),   0.09f, false, "La estrella del centro.\nTiene el 99,8 % de la masa del sistema."),
        ("Tierra",  new Color(0.2f, 0.45f, 1f),  0.05f, false, "3.er planeta. A 150 millones de km del Sol.\nEl único con agua líquida."),
        ("Marte",   new Color(0.8f, 0.25f, 0.1f), 0.035f, false, "4.º planeta. Rojo por el óxido de hierro.\nMonte Olimpo: 22 km de altura."),
        ("Saturno", new Color(0.9f, 0.75f, 0.4f), 0.07f, true,  "6.º planeta. Anillos de hielo y roca.\nEs menos denso que el agua."),
    };

    [MenuItem("Sistema Solar AR/Configurar escena")]
    public static void Configurar()
    {
        var lib = ScriptableObject.CreateInstance<XRReferenceImageLibrary>();
        AssetDatabase.CreateAsset(lib, $"{Raiz}/MarcadoresLibrary.asset");
        foreach (var p in Planetas)
        {
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>($"{Raiz}/Marcadores/{p.nombre}.png");
            if (!tex) { Debug.LogError($"Missing {Raiz}/Marcadores/{p.nombre}.png — run Tools/GenerarMarcadores.ps1"); return; }
            lib.Add();
            int i = lib.count - 1;
            lib.SetTexture(i, tex, true);
            lib.SetName(i, p.nombre);
            lib.SetSpecifySize(i, true);
            lib.SetSize(i, TamanioCarta);
        }
        EditorUtility.SetDirty(lib);

        Directory.CreateDirectory($"{Raiz}/Materiales");
        var lit = Shader.Find("Universal Render Pipeline/Lit");
        Material Mat(string n, Color c, bool emisivo = false)
        {
            var m = new Material(lit) { color = c };
            if (emisivo) { m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor", c * 1.5f); }
            AssetDatabase.CreateAsset(m, $"{Raiz}/Materiales/{n}.mat");
            return m;
        }

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        if (!EditorApplication.ExecuteMenuItem("GameObject/XR/AR Session") ||
            !EditorApplication.ExecuteMenuItem("GameObject/XR/XR Origin (Mobile AR)"))
        { Debug.LogError("The AR Foundation menus weren't found. Is the package installed?"); return; }

        var luz = new GameObject("Directional Light").AddComponent<Light>();
        luz.type = LightType.Directional;
        luz.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        var origin = Object.FindFirstObjectByType<XROrigin>();
        var manager = origin.gameObject.AddComponent<ARTrackedImageManager>();
        manager.referenceLibrary = lib;
        manager.requestedMaxNumberOfMovingImages = Planetas.Length;

        var tracker = origin.gameObject.AddComponent<SistemaSolarTracker>();
        tracker.materialAnillo = Mat("Anillo", new Color(0.85f, 0.8f, 0.65f));
        foreach (var p in Planetas)
            tracker.planetas.Add(new SistemaSolarTracker.Planeta
            {
                imagen = p.nombre, material = Mat(p.nombre, p.color, p.nombre == "Sol"),
                diametro = p.diametro, anillos = p.anillos, dato = p.dato,
            });

        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), Escena);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(Escena, true) };

        foreach (var guid in AssetDatabase.FindAssets("t:UniversalRendererData"))
            AgregarFondoAR(AssetDatabase.LoadAssetAtPath<UniversalRendererData>(AssetDatabase.GUIDToAssetPath(guid)));

        AssetDatabase.SaveAssets();
        Debug.Log("AR scene ready: " + Escena + ". Now enable ARCore under XR Plug-in Management (Android).");
    }

    static void AgregarFondoAR(UniversalRendererData data)
    {
        if (data.rendererFeatures.Exists(f => f is ARBackgroundRendererFeature)) return;
        var feature = ScriptableObject.CreateInstance<ARBackgroundRendererFeature>();
        feature.name = "ARBackgroundRendererFeature";
        AssetDatabase.AddObjectToAsset(feature, data);
        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(feature, out _, out long localId);

        var so = new SerializedObject(data);
        var lista = so.FindProperty("m_RendererFeatures");
        var mapa = so.FindProperty("m_RendererFeatureMap");
        lista.InsertArrayElementAtIndex(lista.arraySize);
        lista.GetArrayElementAtIndex(lista.arraySize - 1).objectReferenceValue = feature;
        mapa.InsertArrayElementAtIndex(mapa.arraySize);
        mapa.GetArrayElementAtIndex(mapa.arraySize - 1).longValue = localId;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(data);
    }
}
