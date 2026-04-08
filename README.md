# 🔬 VR Molecular Chemistry Lab

A VR application built in Unity 6 for the Meta Quest platform where students can interactively combine atomic elements to form valid molecules using VR hand controllers.

---

## 📽️ Demo Video

https://youtu.be/Ol5ycmCz-IM
---

## 📦 APK Download

>(https://drive.google.com/file/d/13Moimgkc4N5swyX8GhzZxsjBEMF8_knz/view?usp=drive_link)

---

## 🧪 Molecules Implemented

| # | Molecule | Formula | Elements | Bond Type |
|---|----------|---------|----------|-----------|
| 1 | Water | H₂O | 2H + 1O | Single Covalent |
| 2 | Hydrogen Gas | H₂ | 2H | Single Covalent |
| 3 | Oxygen Gas | O₂ | 2O | Double Covalent |
| 4 | Nitrogen Gas | N₂ | 2N | Triple Covalent |
| 5 | Ammonia | NH₃ | 1N + 3H | Single Covalent |
| 6 | Carbon Dioxide | CO₂ | 1C + 2O | Double Covalent |
| 7 | Methane | CH₄ | 1C + 4H | Single Covalent |
| 8 | Hydrogen Cyanide | HCN | 1H + 1C + 1N | Triple Covalent |
| 9 | Nitric Oxide | NO | 1N + 1O | Single Covalent |
| 10 | Hydroxyl Radical | OH | 1O + 1H | Single Covalent |
| 11 | Carbon Monoxide | CO | 1C + 1O | Triple Covalent |
| 12 | Diazene | N₂H₂ | 2N + 2H | Double Covalent |
| 13 | Hydrazine | N₂H₄ | 2N + 4H | Single Covalent |
| 14 | Cyanogen | C₂N₂ | 2C + 2N | Triple Covalent |
| 15 | Formaldehyde | CH₂O | 1C + 2H + 1O | Double Covalent |
| 16 | Methanol | CH₄O | 1C + 4H + 1O | Single Covalent |
| 17 | Urea | CH₄N₂O | 1C + 4H + 2N + 1O | Mixed Covalent |
| 18 | Glycine | C₂H₅NO₂ | 2C + 5H + 1N + 2O | Mixed Covalent |

✅ All 7 required molecules implemented  
✅ All 18 total molecules implemented

---

## 🛠️ Technical Stack

| Tool | Version |
|------|---------|
| Unity | 6 (Latest Stable) |
| XR Interaction Toolkit | 3.x |
| OpenXR Plugin | Latest |
| Render Pipeline | Universal Render Pipeline (URP) |
| Scripting Backend | IL2CPP |
| Target Platform | Android (Meta Quest 2/3/Pro) |
| Min Android API | 29 |
| Target Architecture | ARM64 |
| TextMeshPro | Latest |
| DOTween | Latest (Free) |

---

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Data/
│   │   ├── AtomType.cs           # Enum: H, O, C, N
│   │   ├── MoleculeData.cs       # ScriptableObject per molecule
│   │   └── MoleculeDatabase.cs   # Master list of all molecules
│   ├── Atoms/
│   │   ├── AtomController.cs     # XR grab + distance-based proximity
│   │   └── AtomSpawner.cs        # Spawns atoms on tray, handles refill
│   ├── Molecules/
│   │   ├── BondManager.cs        # Validates combos, spawns molecules
│   │   └── MoleculeObject.cs     # Molecule grab, inspector, break-apart
│   ├── UI/
│   │   ├── UIManager.cs          # World-space library panel
│   │   └── UIAnimator.cs         # DOTween animations
│   └── Audio/
│       └── AudioManager.cs       # Singleton sound manager
├── ScriptableObjects/
│   └── Molecules/                # 18 x MoleculeData assets
├── Prefabs/
│   ├── Atoms/                    # H, O, C, N atom prefabs
│   └── Molecules/                # 18 molecule prefabs
├── Materials/                    # Atom and environment materials
├── Audio/                        # Sound effect clips
└── Scenes/
    └── MainScene.unity
```

---

## ⚙️ Architecture Overview

### MoleculeData (ScriptableObject)
Each molecule is defined as a ScriptableObject asset storing its name, formula, atom counts (H/O/C/N), bond type, and 3D prefab. No hardcoded values anywhere in scripts.

### AtomController
Handles XR grab interactions using XRIT 3.x callbacks. Uses a **distance-based proximity system** (polling against a static registry of all active atoms) instead of trigger colliders — this ensures reliable bond detection on Meta Quest hardware where physics triggers can be inconsistent.

### BondManager
On atom release, counts nearby atoms by type and queries `MoleculeDatabase.FindMatch()`. On a valid match, instantiates the molecule prefab at the midpoint, consumes the used atoms, triggers audio and UI updates.

### UIManager + UIAnimator
World-space Canvas panels designed for VR. `UIAnimator` uses DOTween scale animations (not anchoredPosition) to correctly animate top-level World Space Canvases. Library panel shows all discovered molecules with subscript-formatted formulas via TMP rich text.

---

## 🚀 Setup Instructions

### Prerequisites
- Unity 6 (latest stable)
- Android Build Support module installed
- Meta Quest 2, 3, or Pro headset (or Meta XR Simulator)

### Steps

**1. Clone the repository**
```bash
git clone https://github.com/YOURNAME/VR-Molecular-Lab.git
cd VR-Molecular-Lab
```

**2. Open in Unity**
- Open Unity Hub → Add → select the cloned folder
- Open with Unity 6

**3. Install dependencies**
Unity will auto-import packages. If any are missing, install via Window → Package Manager:
- XR Interaction Toolkit 3.x
- OpenXR Plugin
- TextMeshPro
- DOTween (from Asset Store — free)

**4. Configure OpenXR**
- Edit → Project Settings → XR Plug-in Management → Android
- Enable **OpenXR**
- Add **Meta Quest Touch Pro Controller** interaction profile

**5. Build APK**
- File → Build Settings → Android → Switch Platform
- Player Settings:
  - Min API Level: 29
  - Scripting Backend: IL2CPP
  - Target Architecture: ARM64
- Click **Build** → save as `MolecularLab.apk`

**6. Install on Quest**
```bash
adb install -r MolecularLab.apk
```
Or drag and drop via Meta Quest Developer Hub.

**7. Launch**
On Quest → App Library → Unknown Sources → **VR Molecular Lab**

---

## 🎮 How to Play

1. **Pick up atoms** from the tray using the grip button on your controller
2. **Bring atoms close together** — they glow cyan when close enough to bond
3. **Release** the atom — if it's a valid molecule combination, it forms!
4. **Check the Library panel** on your Front to see discovered molecules
5. **Tap Refill** button to get fresh atoms on the tray
6. **Shake a molecule** to break it apart and reuse the atoms

---

## 🤖 AI Tools Used

### 1. Claude (Anthropic) — Architecture 
Used Claude extensively throughout development to design the  C# architecture and generate  scripts . Specific examples:

- **Architecture planning**: Claude designed the modular system (MoleculeData ScriptableObject → MoleculeDatabase → BondManager → AtomController) ensuring no hardcoded values and clean separation of concerns
- **Quest bug fix**: When bond detection failed on device (worked in editor only), Claude diagnosed the `OnTriggerEnter` unreliability on Quest and rewrote `AtomController` to use a distance-based polling system with a static atom registry — fixing the issue completely
- **DOTween animation system**: Claude generated the full `UIAnimator.cs` with scale-based panel animations (correctly handling World Space Canvas at 0.001 scale) and molecule pop/break effects
- **Formula subscript rendering**: Claude identified that Unicode subscript characters (₂) don't render in TMP and provided the `FormatFormula()` helper using TMP rich text tags

### 2. GitHub Copilot — Code Completion & Boilerplate
Used Copilot during implementation for:
- Auto-completing XRIT 3.x event listener patterns (`selectEntered.AddListener`)
- Generating repetitive ScriptableObject property setters
- Completing DOTween sequence chains
- Boilerplate null-check patterns throughout the codebase

### 3. Claude (Anthropic) — Debugging & Molecule Data
- **Molecule database**: Generated all 18 molecule entries with correct atom counts (H/O/C/N), bond types, and validated against real molecular chemistry
- **UIAnimator scale bug**: Diagnosed why `Vector3.one` was making the panel huge (World Space Canvas uses 0.001 scale) and fixed `SlideIn()` to use `_originalScale` captured in `Awake()`
- **Grace period fix**: Identified that molecule prefabs were disappearing instantly because spawn velocity was triggering the shake-detection threshold, and added the `_canBreak` grace period pattern

---

## 🏗️ Scene Structure

```
MainScene
├── XR Origin
│   ├── Camera Offset
│   │   └── Main Camera
│   ├── Left Controller
│   │   ├── XR Ray Interactor
│   │   └── XR Direct Interactor
│   └── Right Controller
│       ├── XR Ray Interactor
│       └── XR Direct Interactor
├── Managers
│   ├── BondManager
│   ├── AudioManager
│   └── UIManager
├── Environment
│   ├── Floor
│   ├── Lab Table
│   └── Directional Light
├── AtomTray (AtomSpawner)
├── LibraryPanel (World Space Canvas)
└── LibraryToggleButton (World Space Canvas)
```

---

## ✅ Evaluation Criteria Coverage

| Criteria | Implementation |
|----------|---------------|
| Unity & XR Development | Unity 6, XRIT 3.x, OpenXR, URP, Quest-optimized |
| Molecular Chemistry Accuracy | All 18 molecules with correct formulas and bond types |
| VR UI/UX Design | World-space canvases, ray interactor support, DOTween animations |
| C# Code Quality | Modular architecture, ScriptableObjects, no hardcoded values |
| AI Tool Usage | Claude + Copilot — documented above with specific examples |
| Git & Documentation | This README + commit history |

---

## 📝 Notes

- Atoms use distance-based proximity detection (not trigger colliders) for reliable Quest compatibility
- All UI is World Space — no screen-space overlays
- Molecule formulas use TMP rich text for proper subscript rendering
- DOTween animations use `_originalScale` to correctly handle 0.001 World Space Canvas scale

---

*Built for XR Developer Assessment — 48 Hour Challenge*
