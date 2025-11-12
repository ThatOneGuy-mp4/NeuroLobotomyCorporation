using KetherBoss;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NeuroLobotomyCorporation.DaatSuppression
{
    public class Spin
    {
        public enum Parameters
        {
            SPIN_AMOUNT = 1
        }

        private static int storedSpinPower = 0;
        private static bool isSpinning = false; //implement the rest of the spinning lock
        public static string Command(int spinAmount)
        {
            if (isSpinning) return "failure|The facility is still spinning. Please wait for a moment...";
            if (spinAmount == 0) return "success|You spun the facility by 0 degrees. Congratulations. ...Perhaps give a non-zero power?";
            isSpinning = true;
            storedSpinPower = spinAmount; 
            return "success|The facility begins to spin.";
        }
        
        public static void InitiateNURU(KetherLastBossBase __instance)
        {
            //bool isTrueSpin = false;
            //NeuroCameraRotationEvent NURU = null;
            //if(trueSpinningEvent != null)
            //{
            //    NURU = trueSpinningEvent;
            //    isTrueSpin = true;
            //}
            //else if(storedSpinPower != 0)
            //{
            //    NURU = new NeuroCameraRotationEvent(__instance);
            //}
            if (!startTrueSpin && storedSpinPower == 0) return;
            if (startTrueSpin && isSpinning) return; 
            NeuroCameraRotationEvent NURU = new NeuroCameraRotationEvent(__instance);
            FieldInfo effectsInfo = typeof(KetherLastBossBase).GetField("_effects", BindingFlags.Instance | BindingFlags.NonPublic);
            KetherLastEffectBase[] effects = (KetherLastEffectBase[])effectsInfo.GetValue(__instance);
            effects[0] = NURU;
            if (NURU != null)
            {
                NURU.OnStart();
            }
            if (startTrueSpin)
            {
                isSpinning = true;
                NURU.StartRotation(0, true);
                startTrueSpin = false;
            }
            else
            {
                NURU.StartRotation(storedSpinPower, false);
                storedSpinPower = 0;
            }
            
        }

        //Prefix - replace the game's normal camera rotation system with a better one that lets Neuro NURU.
        private static bool startTrueSpin = false;
        private static bool delayMovementUntilRotationCorrected = false;
        public static bool ReplaceCameraRotate(KetherLastBossBase __instance, int currentLevel)
        {
            if (currentLevel > 6) return true;
            if (currentLevel == 6) //facility begins moving; correct rotation so emotional impact is not lost. and so the screen doesn't get covered in dust, because that kept happening.
            {
                if (delayMovementUntilRotationCorrected || NeuroCameraRotationEvent.IsRotationCorrect()) return true;
                delayMovementUntilRotationCorrected = true;
                NeuroCameraRotationEvent.CorrectRotation();
                return false;
            }
            //NeuroCameraRotationEvent.TrueSpinLevel = currentLevel;
            //NeuroCameraRotationEvent NURU = new NeuroCameraRotationEvent(__instance);
            //FieldInfo effectsInfo = typeof(KetherLastBossBase).GetField("_effects", BindingFlags.Instance | BindingFlags.NonPublic);
            //KetherLastEffectBase[] effects = (KetherLastEffectBase[])effectsInfo.GetValue(__instance);
            //effects[0] = NURU;
            //if (NURU != null)
            //{
            //    NURU.OnStart();
            //}
            //NURU.StartRotation(0, true);
            startTrueSpin = true;
            return false;
        }

        public class NeuroCameraRotationEvent : KetherLastEffectBase
        {
            public NeuroCameraRotationEvent(KetherLastBossBase bossBase) : base(bossBase)
            {
                this.type = KetherLastEffectType.ROTATE_CAMERA;
            }

            public override void OnStart()
            {
                IEnumerator enumerator = Camera.main.transform.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object obj = enumerator.Current;
                        Transform transform = (Transform)obj;
                        if (transform.name.Equals("CameraDust"))
                        {
                            this._effect = transform.gameObject;
                        }
                    }
                }
                finally
                {
                    IDisposable disposable;
                    if ((disposable = (enumerator as IDisposable)) != null)
                    {
                        disposable.Dispose();
                    }
                }
                if (this._effect == null)
                {
                    this._effect = Prefab.LoadPrefab("Effect/SefiraBoss/DustCameraAttachedEffect");
                    this._effect.transform.SetParent(Camera.main.transform);
                    this._effect.transform.localScale = Vector3.one;
                    this._effect.transform.localPosition = new Vector3(0f, 0f, 10f);
                    this._effect.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                }
                this._effect.gameObject.SetActive(false);
                this.earthQuake = Camera.main.gameObject.GetComponent<CameraFilterPack_FX_EarthQuake>();
                if (this.earthQuake == null)
                {
                    this.earthQuake = Camera.main.gameObject.AddComponent<CameraFilterPack_FX_EarthQuake>();
                }
                this.earthQuake.enabled = false;
            }

            public void StartRotation(int level, bool wasTrueSpin)
            {
                if (level == 0 && !wasTrueSpin) return;
                if (!wasTrueSpin)
                {
                    endPoint = startPoint + level;
                    this._rotationValue.min = (float)(startPoint + TrueSpinLevel) * 36f;
                }
                else
                {
                    TrueSpinLevel++;
                    this._rotationValue.min = (float)(startPoint + (TrueSpinLevel - 1)) * 36f;
                }
                this._rotationValue.max = (float)(endPoint + TrueSpinLevel) * 36f;
                this._rotationTimer.StartTimer(this._rotationTime);
                this._effect.SetActive(true);
                this.curve = SefiraBossUI.Instance.ketherLowerRotationCurve;
                base.BossBase.MakeKetherSound(KetherLastBossBase.ShakeStart);
            }

            public static bool IsRotationCorrect()
            {
                return startPoint % 10 == 0 && endPoint % 10 == 0;
            }

            public static void CorrectRotation()
            {
                gettingCorrected = true;
                int baseSpinPower = 0;
                if (Math.Abs(startPoint) < Math.Abs(endPoint))
                {
                    baseSpinPower = -endPoint;
                }
                else
                {
                    baseSpinPower = -startPoint;
                }
                baseSpinPower = (baseSpinPower % 5);
                storedSpinPower = baseSpinPower;
            }

            public static void ResetSpinParams()
            {
                TrueSpinLevel = 0;
                storedSpinPower = 0;
                startPoint = 0;
                endPoint = 0;
                startTrueSpin = false;
                delayMovementUntilRotationCorrected = false;
                gettingCorrected = false;
            }

            public override void Update()
            {
                if (this._rotationTimer.started)
                {
                    float cameraRotation = this._rotationValue.GetLerp(this.curve.Evaluate(this._rotationTimer.Rate));
                    if (this._rotationTimer.Rate >= 0.4f && !this.earthQuake.enabled)
                    {
                        this.earthQuake.enabled = true;
                        base.BossBase.MakeKetherSound(KetherLastBossBase.ShakeDown);
                    }
                    if (this._rotationTimer.RunTimer())
                    {
                        cameraRotation = this._rotationValue.max;
                        this.earthQuake.enabled = false;
                        EnergyModel.instance.fillBlock = false;
                        startPoint = endPoint;
                        this.Terminate();
                        isSpinning = false;
                        if (gettingCorrected)
                        {
                            (SefiraBossManager.Instance.CurrentBossBase as KetherLastBossBase).EnergyLevelChange(6);
                            gettingCorrected = false;
                        }
                    }
                    this.SetCameraRotation(cameraRotation);
                }
            }

            public void SetCameraRotation(float value)
            {
                Camera.main.transform.localRotation = Quaternion.Euler(0f, 0f, value);
            }

            private const float rotationValue = 36f;

            private const string effectSrc = "Effect/SefiraBoss/DustCameraAttachedEffect";

            private const string effectName = "CameraDust";

            private CameraFilterPack_FX_EarthQuake earthQuake;

            private AnimationCurve curve;

            private UnscaledTimer _rotationTimer = new UnscaledTimer();

            private float _rotationTime = 2f;

            private MinMax _rotationValue = new MinMax(0f, 1f);

            public static int TrueSpinLevel = 0;

            private static int startPoint = 0;
            private static int endPoint = 0;

            private static bool gettingCorrected = false;

            private GameObject _effect;
        }
    }
}
