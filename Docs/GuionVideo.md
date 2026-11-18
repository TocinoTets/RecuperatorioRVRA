# Guion del video – Sistema Solar AR (≈ 3 min 15 s)

> Grabá la pantalla de la PC con OBS (o con la grabadora de la Xbox Game Bar: `Win + Alt + R`) y tu voz con el micrófono.
> Para la parte de AR, usá XR Simulation en Unity (Play) o grabá la pantalla del celular con el APK.
> Exportá en MP4 y subilo a YouTube como **oculto** o a Drive con **acceso público**.

---

## 1. Presentación y High Concept · 0:00 – 0:35
**Pantalla:** página 1 del GDD (PDF) o las 4 cartas impresas sobre la mesa.

**Decir:**
> "Hola, soy Ignacio Rodríguez. Mi proyecto se llama **Sistema Solar AR**. Es un recurso educativo de realidad aumentada: un mazo de cartas impresas que, al apuntarles con el celular, hacen aparecer los planetas en 3D sobre la mesa, girando y con un dato clave encima.
> Resuelve un problema concreto: en la escuela el sistema solar se enseña con láminas planas, donde cuesta entender el volumen y las diferencias de tamaño. Está pensado para chicos de 9 a 14 años y para docentes que necesitan un recurso barato: solo hay que imprimir cuatro cartas."

---

## 2. Las imágenes marcadoras · 0:35 – 0:55
**Pantalla:** carpeta `Assets/AR/Marcadores` en Unity, o la sección 5 del GDD.

**Decir:**
> "Diseñé mis propias cartas: Sol, Tierra, Marte y Saturno. No usé fotos reales porque tienen fondos negros y degradados que ARCore rastrea mal. Estas tienen alto contraste, bordes duros y un patrón distinto en cada carta, así se detectan rápido y no se confunden entre sí."

---

## 3. El proyecto de Unity · 0:55 – 2:00
Mostrá cada cosa haciendo clic en la Hierarchy / Project y señalando el Inspector.

| Tiempo | Mostrar | Decir |
|---|---|---|
| 0:55 | **Package Manager** → AR Foundation y Google ARCore XR Plugin | "Uso Unity 6.3 con URP, AR Foundation 6.7 y el plugin de ARCore para Android." |
| 1:05 | **Project Settings → XR Plug-in Management**: pestaña Android (ARCore) y pestaña PC (XR Simulation) | "En Android está activado ARCore, y en PC XR Simulation para probar sin el celular." |
| 1:15 | Hierarchy: **AR Session** y **XR Origin** | "La escena tiene el AR Session, que maneja el ciclo de vida de AR, y el XR Origin con la cámara." |
| 1:25 | **MarcadoresLibrary** en el Inspector | "Esta es la Reference Image Library con las cuatro cartas. A cada una le declaré el tamaño físico, 8 por 11 centímetros, para que la detección sea más estable." |
| 1:35 | **XR Origin → AR Tracked Image Manager** | "El AR Tracked Image Manager usa esa librería y permite seguir hasta 4 imágenes en movimiento a la vez: este es el seguimiento de imágenes múltiples." |
| 1:45 | **Sistema Solar Tracker** abierto en el Inspector (lista Planetas desplegada) | "Mi script, Sistema Solar Tracker, tiene la configuración de cada planeta: material, diámetro, si tiene anillos y el dato que muestra." |
| 1:50 | Abrí **SistemaSolarTracker.cs** | "Cuando el manager detecta una imagen nueva, busco su nombre en la lista y creo el planeta encima de la carta. Si la carta deja de rastrearse bien, oculto el planeta para que no quede flotando en un lugar equivocado. Además, el planeta se escala según el tamaño real de la carta detectada, y el cartel siempre mira a la cámara." |

---

## 4. Demo funcionando · 2:00 – 3:00
**Pantalla:** Play con XR Simulation (o el celular).
Moverse: **clic derecho + WASD**, mouse para mirar, **Q/E** bajar/subir.

1. **2:00** Acercate a una sola carta (Tierra). Esperá que aparezca. *"Al detectar la carta de la Tierra aparece el planeta girando, con su dato."*
2. **2:15** Recorré las otras tres de a una (Sol, Marte, Saturno con sus anillos). *"Cada carta activa su propio planeta."*
3. **2:35** Alejate para ver **varias cartas a la vez**. *"Acá se ve el seguimiento múltiple: varias imágenes rastreadas al mismo tiempo, y se pueden comparar los tamaños: el Sol es el más grande y Marte el más chico."*
4. **2:50** Movete y volvé a una carta (o tapala con la mano en el celular). *"El seguimiento es estable: si la pierdo de vista, el planeta se oculta, y al volver a enfocarla reaparece en su lugar."*

---

## 5. Cierre · 3:00 – 3:15
**Pantalla:** las 4 cartas con sus planetas o la página del GDD.

**Decir:**
> "Como mejoras a futuro, agregaría el resto de los planetas, preguntas tipo trivia al juntar dos cartas y órbitas animadas. Gracias por mirar."

---

### Checklist antes de grabar
- [ ] Unity sin errores en la Console.
- [ ] Probaste el Play una vez entero y todas las cartas se detectan.
- [ ] Ventana de Unity maximizada y la fuente legible (zoom del Inspector si hace falta).
- [ ] Duración final entre 2 y 4 minutos.
