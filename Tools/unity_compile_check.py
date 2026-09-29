#!/usr/bin/env python3
"""
Approximate Unity compile check WITHOUT a Unity install.

Compiles every Assets/**/*.asmdef as its own assembly (respecting asmdef
references, so circular/missing references surface) against public
UnityEngine reference DLLs from NuGet (UnityEngine.Modules 2021.3 +
UnityEngine.UI 2018 as a stand-in for com.unity.ugui). TextMeshPro is a tiny
stub. Only built-in modules listed in Packages/manifest.json are referenced,
so a missing com.unity.modules.* entry shows up as a compile error.

It is NOT a substitute for a real Unity build (API surface is 2021-era, no
IL2CPP/stripping), but it catches syntax errors, missing asmdef references,
bad `using`s and wrong define branches. Run for each platform define:

    python3 Tools/unity_compile_check.py            # Editor-ish (UNITY_EDITOR off)
    python3 Tools/unity_compile_check.py --android  # UNITY_ANDROID player branches
"""
import glob, json, os, re, shutil, subprocess, sys, urllib.request, zipfile

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
CACHE = os.path.expanduser("~/.cache/heartline-unity-ref")
WORK = "/tmp/heartline-compile-check"
DOTNET = shutil.which("dotnet") or os.path.expanduser("~/.dotnet/dotnet")
PKGS = [("unityengine.modules", "2021.3.33"), ("unity3d.unityengine.ui", "2018.3.5.1")]

# Built-in modules that Unity always links regardless of manifest.
ALWAYS = {"CoreModule", "SharedInternalsModule", "InputLegacyModule", "InputModule", "TextRenderingModule",
          "TextCoreFontEngineModule", "TextCoreTextEngineModule", "SubsystemsModule",
          "RuntimeInitializeOnLoadManagerInitializerModule", "HotReloadModule", "StreamingModule",
          "GridModule", "ScreenCaptureModule", "UnityCurlModule", "TLSModule", "UIElementsNativeModule",
          "AndroidJNIModule"}
MODULE_MAP = {  # com.unity.modules.X -> DLL module names
    "ai": ["AIModule"], "androidjni": ["AndroidJNIModule"], "animation": ["AnimationModule", "DirectorModule"],
    "assetbundle": ["AssetBundleModule"], "audio": ["AudioModule"], "cloth": ["ClothModule"],
    "director": ["DirectorModule"], "imageconversion": ["ImageConversionModule"], "imgui": ["IMGUIModule"],
    "jsonserialize": ["JSONSerializeModule"], "particlesystem": ["ParticleSystemModule"],
    "physics": ["PhysicsModule"], "physics2d": ["Physics2DModule"], "screencapture": ["ScreenCaptureModule"],
    "terrain": ["TerrainModule"], "terrainphysics": ["TerrainPhysicsModule"], "tilemap": ["TilemapModule"],
    "ui": ["UIModule"], "uielements": ["UIElementsModule"], "umbra": ["UmbraModule"],
    "unityanalytics": ["UnityAnalyticsModule", "UnityAnalyticsCommonModule"],
    "unitywebrequest": ["UnityWebRequestModule"], "unitywebrequestassetbundle": ["UnityWebRequestAssetBundleModule"],
    "unitywebrequestaudio": ["UnityWebRequestAudioModule"], "unitywebrequesttexture": ["UnityWebRequestTextureModule"],
    "unitywebrequestwww": ["UnityWebRequestWWWModule"], "vehicles": ["VehiclesModule"], "video": ["VideoModule"],
    "vr": ["VRModule"], "wind": ["WindModule"], "xr": ["XRModule"], "accessibility": ["AccessibilityModule"],
}

TMP_STUB = """
namespace TMPro {
  public class TMP_Text : UnityEngine.UI.MaskableGraphic { public virtual string text { get; set; } }
  public class TextMeshProUGUI : TMP_Text {}
}
"""


def fetch():
    dirs = []
    for pid, ver in PKGS:
        d = os.path.join(CACHE, f"{pid}.{ver}")
        if not os.path.isdir(d):
            os.makedirs(d, exist_ok=True)
            url = f"https://api.nuget.org/v3-flatcontainer/{pid}/{ver}/{pid}.{ver}.nupkg"
            path = d + ".nupkg"
            urllib.request.urlretrieve(url, path)
            zipfile.ZipFile(path).extractall(d)
        dirs.append(d)
    dlls = {}
    for d in dirs:
        for f in glob.glob(os.path.join(d, "**", "*.dll"), recursive=True):
            dlls.setdefault(os.path.basename(f)[:-4], f)
    return dlls


def main():
    android = "--android" in sys.argv
    dlls = fetch()
    manifest = json.load(open(os.path.join(ROOT, "Packages/manifest.json")))["dependencies"]
    mods = set(ALWAYS)
    for k in manifest:
        if k.startswith("com.unity.modules."):
            mods.update(MODULE_MAP.get(k[len("com.unity.modules."):], []))
    has_ugui = "com.unity.ugui" in manifest
    engine_refs = [dlls["UnityEngine"]] + [dlls["UnityEngine." + m] for m in sorted(mods) if "UnityEngine." + m in dlls]

    shutil.rmtree(WORK, ignore_errors=True)
    os.makedirs(WORK)
    defines = ["UNITY_6000_0_OR_NEWER", "DEVELOPMENT_BUILD"] + (["UNITY_ANDROID"] if android else ["UNITY_STANDALONE"])

    def proj(name, srcs, refs, extra_dll=()):
        pdir = os.path.join(WORK, name)
        os.makedirs(pdir)
        items = "\n".join(f'<Compile Include="{s}" />' for s in srcs)
        prefs = "\n".join(f'<ProjectReference Include="../{r}/{r}.csproj" />' for r in refs)
        hrefs = "\n".join(f'<Reference Include="{os.path.basename(d)[:-4]}"><HintPath>{d}</HintPath></Reference>'
                          for d in list(engine_refs) + list(extra_dll))
        open(os.path.join(pdir, name + ".csproj"), "w").write(f"""<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><TargetFramework>netstandard2.1</TargetFramework><LangVersion>9.0</LangVersion>
<EnableDefaultCompileItems>false</EnableDefaultCompileItems><AssemblyName>{name}</AssemblyName>
<DefineConstants>{';'.join(defines)}</DefineConstants><Nullable>disable</Nullable>
<NoWarn>CS0414;CS0169;CS0649;CS0618;CS0067;CS0219;CS0168</NoWarn></PropertyGroup>
<ItemGroup>{items}</ItemGroup><ItemGroup>{prefs}</ItemGroup><ItemGroup>{hrefs}</ItemGroup></Project>""")

    ui = [dlls["UnityEngine.UI"]] if has_ugui else []
    open(os.path.join(WORK, "tmp.cs"), "w").write(TMP_STUB)
    proj("Unity.TextMeshPro", [os.path.join(WORK, "tmp.cs")], [], ui)

    asmdefs = {}
    for f in glob.glob(os.path.join(ROOT, "Assets", "**", "*.asmdef"), recursive=True):
        j = json.load(open(f))
        asmdefs[j["name"]] = (os.path.dirname(f), j)
    dirs = {v[0]: k for k, v in asmdefs.items()}

    def owner(cs):
        d = os.path.dirname(cs)
        while d.startswith(os.path.join(ROOT, "Assets")):
            if d in dirs: return dirs[d]
            d = os.path.dirname(d)
        return "Assembly-CSharp"

    srcs = {}
    for cs in glob.glob(os.path.join(ROOT, "Assets", "**", "*.cs"), recursive=True):
        if "/Editor/" in cs: continue  # editor-only code not in player builds
        srcs.setdefault(owner(cs), []).append(cs)

    known = set(asmdefs) | {"Unity.TextMeshPro", "UnityEngine.UI"}
    for name, (d, j) in asmdefs.items():
        if j.get("includePlatforms") and ("Android" not in j["includePlatforms"] if android else True):
            continue
        refs = []
        for r in j.get("references", []):
            if r not in known:
                print(f"WARNING {name}: unknown asmdef reference {r}")
            elif r == "UnityEngine.UI":
                continue
            else:
                refs.append(r)
        proj(name, srcs.get(name, []), refs, ui)
    if "Assembly-CSharp" in srcs:
        proj("Assembly-CSharp", srcs["Assembly-CSharp"],
             [n for n, v in asmdefs.items() if v[1].get("autoReferenced", True)] + ["Unity.TextMeshPro"], ui)

    sln = os.path.join(WORK, "all.proj")
    projs = "\n".join(f'<ProjectReference Include="{p}" />' for p in glob.glob(os.path.join(WORK, "*", "*.csproj")))
    open(sln, "w").write(f'<Project Sdk="Microsoft.Build.Traversal/4.1.0"><ItemGroup>{projs}</ItemGroup></Project>')
    r = subprocess.run([DOTNET, "build", sln, "-nologo", "-v:q", "-clp:NoSummary"], capture_output=True, text=True)
    out = r.stdout + r.stderr
    errs = sorted(set(l.strip() for l in out.splitlines() if ": error " in l))
    for e in errs:
        print(e.replace(ROOT + "/", ""))
    print(f"[{'android' if android else 'standalone'}] {'OK' if r.returncode == 0 and not errs else 'FAILED'} ({len(errs)} errors)")
    sys.exit(0 if r.returncode == 0 and not errs else 1)


if __name__ == "__main__":
    main()
