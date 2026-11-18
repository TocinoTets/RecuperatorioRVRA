# Sistema Solar AR: steps to finish the deliverable

## 0. Install Android support (required for the APK)
Your Unity 6000.3.17f1 **does not have the Android module**. In Unity Hub: Installs > 6000.3.17f1 > ⚙ Add modules >
**Android Build Support** (with OpenJDK and Android SDK & NDK). Then restart Unity.

## 1. Set up the project (in the open Unity editor)
1. Bring the Unity window to the front: it will import AR Foundation 6.7.1 and ARCore 6.7.1 (already added to `Packages/manifest.json`).
2. Menu **Sistema Solar AR > Configurar escena**. It creates:
   - `Assets/AR/MarcadoresLibrary.asset` (4 images, 8 × 11 cm)
   - `Assets/AR/Materiales/*.mat`
   - `Assets/Scenes/SistemaSolarAR.unity` (AR Session + XR Origin + ARTrackedImageManager + SistemaSolarTracker), the only scene in the Build
   - AR Background Renderer Feature in the URP renderers
3. **File > Build Profiles > Android > Switch Platform**.
4. **Edit > Project Settings > XR Plug-in Management > Android tab > tick "Google ARCore"**.
5. **XR Plug-in Management > Project Validation > Fix All** (sets IL2CPP, ARM64, minimum API, Graphics API, etc.).

## 2. Build the APK
Build Profiles > Android > **Build** → `SistemaSolarAR.apk`. Install it on a phone with ARCore
(Google Play Services for AR).

## 3. Print the markers
`Assets/AR/Marcadores/*.png` at **8 × 11 cm** on matte paper (the 4 fit on one A4 sheet).

## 4. Video (2–4 min)
1. High Concept (~30 s, can be over the GDD's page 1).
2. Unity project (~1 min): ARTrackedImageManager + library, SistemaSolarTracker script and its list in the Inspector.
3. Screen recording on the phone (~1.5 min): one card at a time, then 2–4 at once, move them, cover/uncover one.

## Regenerate the markers
`powershell -ExecutionPolicy Bypass -File Tools\GenerarMarcadores.ps1` (if you change them, run the menu from step 1 again).
