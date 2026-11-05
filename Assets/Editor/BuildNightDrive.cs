using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor.Events;

public static class BuildNightDrive {
 const string Dir="Assets/Generated/";
 static Material road, white, cyan, pink, yellow, red, dark, glass;
 [MenuItem("Night Drive/Rebuild Game Scene")]
 public static void Build(){
  AssetDatabase.Refresh();
  road=Mat("Asphalt",new Color(.045f,.035f,.04f)); white=Mat("White",new Color(.92f,.82f,.61f)); cyan=Mat("Cyan",new Color(1f,.47f,.08f),true); pink=Mat("Pink",new Color(.95f,.18f,.07f),true); yellow=Mat("Gold",new Color(1,.72f,.15f),true); red=Mat("Red",new Color(.42f,.13f,.08f)); dark=Mat("Dark",new Color(.055f,.035f,.04f)); glass=Mat("Glass",new Color(.08f,.11f,.14f));
  var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
  RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.22f,.16f,.16f);RenderSettings.fog=true;RenderSettings.fogColor=new Color(.035f,.022f,.035f);RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogDensity=.003f;
  var sky=AssetDatabase.LoadAssetAtPath<Material>(Dir+"NightSky.mat");if(!sky){sky=new Material(Shader.Find("Skybox/Procedural"));AssetDatabase.CreateAsset(sky,Dir+"NightSky.mat");}sky.SetFloat("_AtmosphereThickness",.3f);sky.SetColor("_SkyTint",new Color(.09f,.045f,.07f));sky.SetColor("_GroundColor",new Color(.055f,.025f,.03f));sky.SetFloat("_Exposure",.25f);EditorUtility.SetDirty(sky);RenderSettings.skybox=sky;
  var gm=new GameObject("Game Manager").AddComponent<GameManager>();
  var player=CreateCar("Player",white,true);player.transform.position=new Vector3(0,.5f,0);var pc=player.AddComponent<PlayerCar>();pc.controls=AssetDatabase.LoadAssetAtPath<InputActionAsset>(Dir+"NightDriveControls.inputactions");pc.body=player.transform.Find("Body").GetComponent<Renderer>();
  var rb=player.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;var col=player.AddComponent<BoxCollider>();col.size=new Vector3(1.8f,1.1f,3.8f);
  var glow=new GameObject("Underglow");glow.transform.SetParent(player.transform,false);glow.transform.localPosition=new Vector3(0,-.1f,0);pc.underglow=glow.AddComponent<Light>();pc.underglow.type=LightType.Point;pc.underglow.color=new Color(1f,.35f,.04f);pc.underglow.range=7;pc.underglow.intensity=3;
  for(int i=-1;i<=1;i+=2){var l=new GameObject("Headlight");l.transform.SetParent(player.transform,false);l.transform.localPosition=new Vector3(i*.6f,.1f,1.9f);var light=l.AddComponent<Light>();light.type=LightType.Spot;light.range=45;light.spotAngle=65;light.intensity=20;light.color=new Color(.7f,.85f,1);}
  pc.sparks=Particles(player.transform,"Collision Sparks",new Color(1,.55f,.1f),65,9,.5f,false);pc.pickup=Particles(player.transform,"Pickup Burst",new Color(1,.65f,.2f),45,5,.5f,false);var exhaust=Particles(player.transform,"Exhaust",new Color(1,.45f,.15f,.7f),6,1,.6f,true);exhaust.transform.localPosition=new Vector3(0,0,-2);exhaust.Play();
  var stars=new GameObject("Stars");stars.transform.SetParent(player.transform,false);
  var starRandom=new System.Random(932);
  for(int i=0;i<100;i++){float x=(float)(starRandom.NextDouble()*180-90);float y=(float)(starRandom.NextDouble()*55+18);float z=(float)(starRandom.NextDouble()*140+35);var star=GameObject.CreatePrimitive(PrimitiveType.Sphere);star.name="Star";star.transform.SetParent(stars.transform,false);star.transform.localPosition=new Vector3(x,y,z);float size=.2f+(float)starRandom.NextDouble()*.35f;star.transform.localScale=Vector3.one*size;star.GetComponent<Renderer>().sharedMaterial=white;Object.DestroyImmediate(star.GetComponent<Collider>());}
  var camera=new GameObject("Main Camera");camera.tag="MainCamera";var cam=camera.AddComponent<Camera>();cam.clearFlags=CameraClearFlags.Skybox;cam.fieldOfView=60;cam.allowHDR=true;camera.AddComponent<AudioListener>();camera.AddComponent<CameraFollow>().target=player.transform;camera.transform.position=new Vector3(0,5,-11);cam.transform.LookAt(new Vector3(0,1,12));
  var sun=new GameObject("Moonlight").AddComponent<Light>();sun.type=LightType.Directional;sun.color=new Color(.45f,.35f,.48f);sun.intensity=.5f;sun.transform.rotation=Quaternion.Euler(45,-30,0);
  var volume=new GameObject("Bloom Volume").AddComponent<Volume>();volume.isGlobal=true;var profile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(Dir+"NightVolume.asset");if(!profile){profile=ScriptableObject.CreateInstance<VolumeProfile>();AssetDatabase.CreateAsset(profile,Dir+"NightVolume.asset");}Bloom bloom;if(!profile.TryGet<Bloom>(out bloom))bloom=profile.Add<Bloom>(true);bloom.intensity.Override(1.3f);bloom.threshold.Override(.8f);EditorUtility.SetDirty(profile);volume.sharedProfile=profile;cam.GetUniversalAdditionalCameraData().renderPostProcessing=true;
  var roadPrefab=RoadPrefab();var still=ObstaclePrefab("Stationary Obstacle",false);var moving=ObstaclePrefab("Moving Obstacle",true);var coin=ItemPrefab("Coin",true);var fuel=ItemPrefab("Fuel Can",false);
  var director=new GameObject("Road Director").AddComponent<RoadDirector>();director.player=player.transform;director.road=Pool("Road Pool",roadPrefab,8);director.stationary=Pool("Stationary Pool",still,16);director.moving=Pool("Moving Pool",moving,16);director.coins=Pool("Coin Pool",coin,20);director.fuel=Pool("Fuel Pool",fuel,10);
  var fireworks=new GameObject("Sky Fireworks").AddComponent<Fireworks>();fireworks.player=player.transform;fireworks.burst=Particles(fireworks.transform,"Firework Burst",Color.magenta,120,12,1.8f,false);
  var audio=new GameObject("Audio Director").AddComponent<AudioDirector>();audio.music=AssetDatabase.LoadAssetAtPath<AudioClip>(Dir+"Music.wav");audio.engine=AssetDatabase.LoadAssetAtPath<AudioClip>(Dir+"Engine.wav");audio.collision=AssetDatabase.LoadAssetAtPath<AudioClip>(Dir+"Collision.wav");audio.collect=AssetDatabase.LoadAssetAtPath<AudioClip>(Dir+"Collect.wav");
  CreateUI(gm);
  EditorSceneManager.SaveScene(scene,"Assets/Scenes/NightDrive.unity");EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/NightDrive.unity",true)};AssetDatabase.SaveAssets();AssetDatabase.Refresh();Debug.Log("Night Drive scene generated successfully");
 }
 static Material Mat(string name,Color c,bool emissive=false){var m=AssetDatabase.LoadAssetAtPath<Material>(Dir+name+".mat");if(!m){m=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(m,Dir+name+".mat");}m.color=c;if(emissive){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",c*3);}EditorUtility.SetDirty(m);return m;}
 static GameObject Cube(string name,Transform parent,Vector3 pos,Vector3 scale,Material mat){var o=GameObject.CreatePrimitive(PrimitiveType.Cube);o.name=name;o.transform.SetParent(parent,false);o.transform.localPosition=pos;o.transform.localScale=scale;o.GetComponent<Renderer>().sharedMaterial=mat;Object.DestroyImmediate(o.GetComponent<Collider>());return o;}
 static GameObject CreateCar(string name,Material bodyMat,bool player){
  var car=new GameObject(name);
  Cube("Body",car.transform,new Vector3(0,0,0),new Vector3(1.75f,.55f,3.7f),bodyMat);
  Cube("Hood",car.transform,new Vector3(0,.25f,1.08f),new Vector3(1.55f,.18f,1.25f),bodyMat);
  Cube("Cabin",car.transform,new Vector3(0,.45f,-.35f),new Vector3(1.4f,.52f,1.75f),glass);
  Cube("Roof",car.transform,new Vector3(0,.75f,-.35f),new Vector3(1.35f,.1f,1.55f),bodyMat);
  Cube("Bumper",car.transform,new Vector3(0,-.18f,-1.91f),new Vector3(1.8f,.16f,.15f),dark);
  for(int x=-1;x<=1;x+=2){
   for(int z=-1;z<=1;z+=2){var wheel=GameObject.CreatePrimitive(PrimitiveType.Cylinder);wheel.name="Wheel";wheel.transform.SetParent(car.transform,false);wheel.transform.localPosition=new Vector3(x*.9f,-.22f,z*1.18f);wheel.transform.localRotation=Quaternion.Euler(0,0,90);wheel.transform.localScale=new Vector3(.55f,.16f,.55f);wheel.GetComponent<Renderer>().sharedMaterial=dark;Object.DestroyImmediate(wheel.GetComponent<Collider>());}
   Cube("Headlamp",car.transform,new Vector3(x*.57f,.03f,1.87f),new Vector3(.46f,.13f,.08f),white);
   Cube("Tail lamp",car.transform,new Vector3(x*.57f,.05f,-1.88f),new Vector3(.46f,.15f,.08f),pink);
  }
  return car;
 }
 static GameObject SavePrefab(GameObject obj,string name){var prefab=PrefabUtility.SaveAsPrefabAsset(obj,Dir+name+".prefab");Object.DestroyImmediate(obj);return prefab;}
 static GameObject RoadPrefab(){
  var o=new GameObject("Desert Highway Segment");
  Cube("Desert ground",o.transform,new Vector3(0,-.31f,20),new Vector3(90,.25f,40),red);
  Cube("Asphalt",o.transform,new Vector3(0,-.14f,20),new Vector3(10,.28f,40),road);
  for(int x=-1;x<=1;x+=2){
   Cube("Shoulder",o.transform,new Vector3(x*5.1f,-.03f,20),new Vector3(.34f,.12f,40),yellow);
   Cube("Guard rail",o.transform,new Vector3(x*6.1f,.55f,20),new Vector3(.14f,.14f,40),dark);
   for(int z=2;z<40;z+=6){
    Cube("Lane marking",o.transform,new Vector3(x*1.6f,.02f,z),new Vector3(.10f,.04f,3),white);
    Cube("Reflector",o.transform,new Vector3(x*5.1f,.08f,z),new Vector3(.25f,.09f,.4f),cyan);
    Cube("Rail post",o.transform,new Vector3(x*6.1f,.25f,z),new Vector3(.15f,.55f,.15f),dark);
   }
   for(int z=8;z<40;z+=20){
    Cube("Road lamp post",o.transform,new Vector3(x*7,2.5f,z),new Vector3(.12f,5,.12f),dark);
    Cube("Road lamp arm",o.transform,new Vector3(x*6.55f,5,z),new Vector3(1,.12f,.12f),dark);
    Cube("Amber lamp",o.transform,new Vector3(x*6.1f,4.9f,z),new Vector3(.35f,.12f,.45f),cyan);
   }
   for(int z=3;z<40;z+=13){
    float mx=x*(24+(z%3)*4); Cube("Mesa foothill",o.transform,new Vector3(mx,1.1f,z),new Vector3(17,2.2f,12),dark);Cube("Mesa ridge",o.transform,new Vector3(mx+x*.7f,2.7f,z),new Vector3(13,1.1f,9),dark);Cube("Mesa crown",o.transform,new Vector3(mx-x*.6f,3.5f,z+.8f),new Vector3(9,.55f,6),dark);
   }
  }
  return SavePrefab(o,"RoadSegment");
 }
 static GameObject ObstaclePrefab(string name,bool move){var o=CreateCar(name,move?pink:red,false);var c=o.AddComponent<BoxCollider>();c.size=new Vector3(1.8f,1,3.8f);c.isTrigger=true;if(move)o.AddComponent<MovingObstacle>();else o.AddComponent<StationaryObstacle>();return SavePrefab(o,move?"MovingObstacle":"StationaryObstacle");}
 static GameObject ItemPrefab(string name,bool coin){var o=new GameObject(name);if(coin){var mesh=GameObject.CreatePrimitive(PrimitiveType.Cylinder);mesh.name="Coin mesh";mesh.transform.SetParent(o.transform,false);mesh.transform.localScale=new Vector3(.9f,.12f,.9f);mesh.transform.localRotation=Quaternion.Euler(90,0,0);mesh.GetComponent<Renderer>().sharedMaterial=yellow;Object.DestroyImmediate(mesh.GetComponent<Collider>());o.AddComponent<CoinItem>();}else{Cube("Can",o.transform,Vector3.zero,new Vector3(.8f,1,.4f),red);Cube("Cap",o.transform,new Vector3(0,.6f,0),new Vector3(.3f,.2f,.3f),yellow);o.AddComponent<FuelItem>();}var c=o.AddComponent<BoxCollider>();c.size=new Vector3(1.4f,1.5f,1.4f);c.isTrigger=true;return SavePrefab(o,coin?"Coin":"Fuel");}
 static ObjectPool Pool(string name,GameObject prefab,int count){var p=new GameObject(name).AddComponent<ObjectPool>();p.prefab=prefab;p.initialSize=count;return p;}
 static ParticleSystem Particles(Transform parent,string name,Color color,int count,float speed,float life,bool looping){var o=new GameObject(name);o.transform.SetParent(parent,false);var ps=o.AddComponent<ParticleSystem>();var main=ps.main;main.loop=looping;main.playOnAwake=false;main.startColor=color;main.startLifetime=life;main.startSpeed=speed;main.startSize=.15f;main.maxParticles=300;main.simulationSpace=ParticleSystemSimulationSpace.World;var emission=ps.emission;emission.rateOverTime=looping?count:0;if(!looping)emission.SetBursts(new[]{new ParticleSystem.Burst(0,(short)count)});var shape=ps.shape;shape.shapeType=ParticleSystemShapeType.Sphere;shape.radius=.15f;var renderer=o.GetComponent<ParticleSystemRenderer>();renderer.material=white;return ps;}
 static GameObject Panel(string name,Transform parent,Color color){var o=new GameObject(name,typeof(RectTransform),typeof(Image));o.transform.SetParent(parent,false);var rt=o.GetComponent<RectTransform>();rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=rt.offsetMax=Vector2.zero;o.GetComponent<Image>().color=color;return o;}
 static Text TextAt(Transform parent,string name,string value,Vector2 anchor,Vector2 pos,int size,TextAnchor alignment=TextAnchor.MiddleCenter){var o=new GameObject(name,typeof(RectTransform),typeof(Text));o.transform.SetParent(parent,false);var rt=o.GetComponent<RectTransform>();rt.anchorMin=rt.anchorMax=anchor;rt.anchoredPosition=pos;rt.sizeDelta=new Vector2(550,70);var t=o.GetComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=size;t.horizontalOverflow=HorizontalWrapMode.Overflow;t.verticalOverflow=VerticalWrapMode.Overflow;t.alignment=alignment;t.color=Color.white;t.text=value;return t;}
 static void Button(Transform parent,string label,Vector2 pos,UnityEngine.Events.UnityAction action){var o=new GameObject(label,typeof(RectTransform),typeof(Image),typeof(Button));o.transform.SetParent(parent,false);var rt=o.GetComponent<RectTransform>();rt.anchorMin=rt.anchorMax=new Vector2(.25f,.5f);rt.anchoredPosition=pos;rt.sizeDelta=new Vector2(300,58);o.GetComponent<Image>().color=new Color(.58f,.24f,.07f,.98f);var b=o.GetComponent<Button>();UnityEventTools.AddPersistentListener(b.onClick,action);var t=TextAt(o.transform,"Label",label,new Vector2(.5f,.5f),Vector2.zero,21);t.rectTransform.sizeDelta=rt.sizeDelta;}
 static void CreateUI(GameManager gm){var canvas=new GameObject("HUD and Menus",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));canvas.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;var scaler=canvas.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);
 var eventSystem=new GameObject("Event System",typeof(EventSystem),typeof(InputSystemUIInputModule));eventSystem.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
 var ui=canvas.AddComponent<GameUI>();ui.hud=Panel("HUD",canvas.transform,Color.clear);ui.speed=TextAt(ui.hud.transform,"Speed","",new Vector2(1,0),new Vector2(-300,100),42,TextAnchor.MiddleRight);ui.lives=TextAt(ui.hud.transform,"Lives","",new Vector2(0,1),new Vector2(300,-70),24,TextAnchor.MiddleLeft);ui.score=TextAt(ui.hud.transform,"Score","",new Vector2(1,1),new Vector2(-300,-70),24,TextAnchor.MiddleRight);ui.best=TextAt(ui.hud.transform,"Best","",new Vector2(1,1),new Vector2(-300,-105),18,TextAnchor.MiddleRight);ui.coins=TextAt(ui.hud.transform,"Coins","",new Vector2(0,1),new Vector2(300,-105),18,TextAnchor.MiddleLeft);
 TextAt(ui.hud.transform,"Fuel Label","FUEL",new Vector2(0,0),new Vector2(300,115),19,TextAnchor.MiddleLeft);var bg=new GameObject("Fuel Bar",typeof(RectTransform),typeof(Image));bg.transform.SetParent(ui.hud.transform,false);var br=bg.GetComponent<RectTransform>();br.anchorMin=br.anchorMax=new Vector2(0,0);br.anchoredPosition=new Vector2(190,70);br.sizeDelta=new Vector2(320,20);bg.GetComponent<Image>().color=new Color(.08f,.12f,.2f);var fill=new GameObject("Fill",typeof(RectTransform),typeof(Image));fill.transform.SetParent(bg.transform,false);var fr=fill.GetComponent<RectTransform>();fr.anchorMin=Vector2.zero;fr.anchorMax=Vector2.one;fr.offsetMin=fr.offsetMax=Vector2.zero;ui.fuelFill=fill.GetComponent<Image>();ui.fuelFill.color=new Color(1,.55f,.12f);ui.fuelFill.sprite=AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");ui.fuelFill.type=Image.Type.Filled;ui.fuelFill.fillMethod=Image.FillMethod.Horizontal;
 ui.menu=Panel("Main Menu",canvas.transform,new Color(.055f,.025f,.025f,.78f));var title=TextAt(ui.menu.transform,"Title","DUSTLINE",new Vector2(.5f,.5f),new Vector2(-480,235),78);title.font=AssetDatabase.LoadAssetAtPath<Font>(Dir+"DustlineDisplay.ttf");TextAt(ui.menu.transform,"Subtitle","THE DESERT DOES NOT END",new Vector2(.5f,.5f),new Vector2(-480,160),22);TextAt(ui.menu.transform,"Controls","A / D OR ARROW KEYS TO STEER     •     ESC TO PAUSE",new Vector2(.5f,.5f),new Vector2(-480,-190),17);Button(ui.menu.transform,"DRIVE",new Vector2(0,30),gm.StartGame);Button(ui.menu.transform,"QUIT",new Vector2(0,-45),gm.Quit);
 ui.pause=Panel("Pause Menu",canvas.transform,new Color(.055f,.025f,.025f,.91f));TextAt(ui.pause.transform,"Title","PAUSED",new Vector2(.5f,.5f),new Vector2(-480,170),64);Button(ui.pause.transform,"RESUME",new Vector2(0,60),gm.TogglePause);Button(ui.pause.transform,"RESTART",new Vector2(0,-15),gm.Restart);Button(ui.pause.transform,"MAIN MENU",new Vector2(0,-90),gm.MainMenu);
 ui.over=Panel("Game Over",canvas.transform,new Color(.055f,.025f,.025f,.94f));TextAt(ui.over.transform,"Title","GAME OVER",new Vector2(.5f,.5f),new Vector2(-480,200),68);ui.reason=TextAt(ui.over.transform,"Reason","",new Vector2(.5f,.5f),new Vector2(-480,125),25);ui.finalScore=TextAt(ui.over.transform,"Final Score","",new Vector2(.5f,.5f),new Vector2(-480,55),25);Button(ui.over.transform,"RESTART",new Vector2(0,-45),gm.Restart);Button(ui.over.transform,"MAIN MENU",new Vector2(0,-120),gm.MainMenu);
 }
}
