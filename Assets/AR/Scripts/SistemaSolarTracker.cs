using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class SistemaSolarTracker : MonoBehaviour
{
    [System.Serializable]
    public class Planeta
    {
        public string imagen;
        public Material material;
        public float diametro = 0.06f;
        public float velocidadGiro = 30f;
        public bool anillos;
        [TextArea] public string dato;
    }

    public List<Planeta> planetas = new();
    public Material materialAnillo;

    ARTrackedImageManager manager;
    readonly Dictionary<TrackableId, (Transform planeta, Transform etiqueta, float giro)> activos = new();

    void Awake() => manager = GetComponent<ARTrackedImageManager>();
    void OnEnable() => manager.trackablesChanged.AddListener(OnCambios);
    void OnDisable() => manager.trackablesChanged.RemoveListener(OnCambios);

    void OnCambios(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var img in args.added) Crear(img);
        foreach (var img in args.updated)
            if (activos.TryGetValue(img.trackableId, out var p))
                p.planeta.parent.gameObject.SetActive(img.trackingState == TrackingState.Tracking);
        foreach (var par in args.removed)
            activos.Remove(par.Key);
    }

    void Crear(ARTrackedImage img)
    {
        var datos = planetas.Find(p => p.imagen == img.referenceImage.name);
        if (datos == null) { Debug.LogWarning($"No planet configured for image '{img.referenceImage.name}'"); return; }

        var raiz = new GameObject(datos.imagen).transform;
        raiz.SetParent(img.transform, false);
        float escala = img.size.x > 0f ? img.size.x / AnchoCartaReferencia : 1f;
        raiz.localScale = Vector3.one * escala;

        var esfera = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        Destroy(esfera.GetComponent<Collider>());
        esfera.SetParent(raiz, false);
        esfera.localScale = Vector3.one * datos.diametro;
        esfera.localPosition = Vector3.up * (datos.diametro * 0.5f + 0.02f);
        esfera.GetComponent<Renderer>().sharedMaterial = datos.material;

        if (datos.anillos)
        {
            var anillo = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
            Destroy(anillo.GetComponent<Collider>());
            anillo.SetParent(esfera, false);
            anillo.localScale = new Vector3(2.2f, 0.01f, 2.2f);
            anillo.localRotation = Quaternion.Euler(20f, 0f, 0f);
            anillo.GetComponent<Renderer>().sharedMaterial = materialAnillo;
        }

        var etiqueta = new GameObject("Etiqueta").transform;
        etiqueta.SetParent(raiz, false);
        etiqueta.localPosition = Vector3.up * (datos.diametro + 0.05f);
        var texto = $"<b>{datos.imagen.ToUpper()}</b>\n{datos.dato}";
        CrearTexto(etiqueta, texto, Color.white, Vector3.zero);
        const float o = 0.0007f;
        foreach (var d in new[] { new Vector3(o, 0), new Vector3(-o, 0), new Vector3(0, o), new Vector3(0, -o) })
            CrearTexto(etiqueta, texto, Color.black, d + Vector3.forward * 0.001f);

        activos[img.trackableId] = (esfera, etiqueta, datos.velocidadGiro);
    }

    const float AnchoCartaReferencia = 0.08f;

    static void CrearTexto(Transform padre, string texto, Color color, Vector3 offset)
    {
        var go = new GameObject("Texto");
        go.transform.SetParent(padre, false);
        go.transform.localPosition = offset;
        var tm = go.AddComponent<TextMesh>();
        tm.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        go.GetComponent<MeshRenderer>().sharedMaterial = tm.font.material;
        tm.text = texto;
        tm.anchor = TextAnchor.LowerCenter;
        tm.alignment = TextAlignment.Center;
        tm.characterSize = 0.0018f;
        tm.fontSize = 80;
        tm.color = color;
    }

    void Update()
    {
        var cam = Camera.main ? Camera.main.transform : null;
        foreach (var p in activos.Values)
        {
            p.planeta.Rotate(Vector3.up, p.giro * Time.deltaTime, Space.Self);
            if (cam) p.etiqueta.rotation = Quaternion.LookRotation(p.etiqueta.position - cam.position);
        }
    }
}
