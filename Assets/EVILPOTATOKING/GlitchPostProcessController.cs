using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[DisallowMultipleComponent]
[RequireComponent(typeof(Volume))]
public class GlitchPostProcessController : MonoBehaviour
{
    private Volume _volume;
    private ChromaticAberration _chroma;
    private LensDistortion _distortion;
    private FilmGrain _grain;
    private ColorAdjustments _color;
    private SplitToning _split;
    private MotionBlur _motionBlur;

    [Header("总强度（所有效果的总开关）")]
    [Range(0f, 1f)] public float globalIntensity = 1f;

    [Header("A 电子干扰 / TV 故障")]
    public bool enableBroadcastGlitch = true;
    [Range(0f, 1f)] public float broadcastIntensity = 0.6f;

    [Header("C 残影延迟 / Afterimage")]
    public bool enableAfterimage = false;
    [Range(0f, 1f)] public float afterimageIntensity = 0.5f;

    [Header("D 迷幻色彩 / Psychedelic")]
    public bool enablePsychedelic = false;
    [Range(0f, 1f)] public float psychedelicIntensity = 0.5f;

    [Header("动画速度控制")]
    public float flickerSpeed = 18f;     // A：闪动噪声速度
    public float breathingSpeed = 0.7f;  // C / D：呼吸节奏

    private void Awake()
    {
        _volume = GetComponent<Volume>();
        if (_volume.profile == null)
        {
            _volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
        }

        VolumeProfile profile = _volume.profile;

        if (!profile.TryGet(out _chroma))
            _chroma = profile.Add<ChromaticAberration>(true);

        if (!profile.TryGet(out _distortion))
            _distortion = profile.Add<LensDistortion>(true);

        if (!profile.TryGet(out _grain))
            _grain = profile.Add<FilmGrain>(true);

        if (!profile.TryGet(out _color))
            _color = profile.Add<ColorAdjustments>(true);

        if (!profile.TryGet(out _split))
            _split = profile.Add<SplitToning>(true);

        // MotionBlur 在 2D 里作用有限，但保留一点可选的“拖影”感觉
        if (!profile.TryGet(out _motionBlur))
            _motionBlur = profile.Add<MotionBlur>(false);

        _chroma.active = true;
        _distortion.active = true;
        _grain.active = true;
        _color.active = true;
        _split.active = true;
        if (_motionBlur != null) _motionBlur.active = true;
    }

    private void Update()
    {
        float t = Time.unscaledTime;
        float global = Mathf.Clamp01(globalIntensity);

        if (global <= 0.0001f)
        {
            DisableAll();
            return;
        }

        // 累积各效果的最终值
        float chromaVal = 0f;
        float lensVal = 0f;
        float lensScale = 1f;
        float grainVal = 0f;
        float saturationVal = 0f;
        float contrastVal = 0f;
        float hueShiftVal = 0f;
        float hueShadows = 0f, satShadows = 0f;
        float hueHighlights = 0f, satHighlights = 0f;
        float splitBalance = 0f;
        float motionVal = 0f;

        // ========= A) 电子干扰 / TV Broadcast =========
        if (enableBroadcastGlitch && broadcastIntensity > 0f)
        {
            float a = broadcastIntensity * global;

            float noise = Mathf.PerlinNoise(t * flickerSpeed, 0.0f) * 2f - 1f;
            float pulsate = 0.5f + 0.5f * Mathf.Sin(t * flickerSpeed * 0.5f);
            float local = a * (0.7f + 0.3f * pulsate);

            chromaVal += Mathf.Lerp(0f, 0.55f, local + 0.15f * noise);
            lensVal += Mathf.Lerp(0f, -0.35f, local);
            lensScale *= Mathf.Lerp(1.0f, 1.08f, local);
            grainVal += Mathf.Lerp(0f, 0.45f, local + 0.2f * noise);

            contrastVal += Mathf.Lerp(0f, 18f, local);
            saturationVal += Mathf.Lerp(0f, -12f, local);
        }

        // ========= C) 残影延迟 / Afterimage =========
        // 重新设计为明显一点的“画面呼吸 + 轻微拖影 + 轻微晕眩”
        if (enableAfterimage && afterimageIntensity > 0f)
        {
            float c = afterimageIntensity * global;

            // 快一点的呼吸 & 晃动
            float breathFast = 0.5f + 0.5f * Mathf.Sin(t * breathingSpeed * 3.0f);
            float wobble = Mathf.Sin(t * breathingSpeed * 6.0f);
            float local = c * (0.6f + 0.4f * breathFast);

            // 镜头轻微放大缩小 + 扭曲，感觉像世界在呼吸
            lensScale *= Mathf.Lerp(1.0f, 1.06f, local);
            lensVal += Mathf.Lerp(0f, -0.18f, local) * (0.8f + 0.2f * wobble);

            // 轻微色差 + 颗粒增加，模拟视线对不准
            chromaVal += Mathf.Lerp(0f, 0.25f, local);
            grainVal += Mathf.Lerp(0f, 0.15f, local);

            // 饱和度稍微下降一点，让“残影”感觉更灰
            saturationVal += Mathf.Lerp(0f, -10f, local);

            // 如果你开启 MotionBlur，这里给一点拖影感（不开也不会报错）
            motionVal += Mathf.Lerp(0f, 0.6f, local);
        }

        // ========= D) 迷幻色彩 / Psychedelic =========
        // 改成“颜色持续缓慢变化的滤镜”，而不是全部覆盖
        if (enablePsychedelic && psychedelicIntensity > 0f)
        {
            float d = psychedelicIntensity * global;

            // 慢速色相摆动，±maxHueShift 度
            float hueWave = Mathf.Sin(t * breathingSpeed * 1.2f);   // -1..1
            float maxHueShift = 22f; // 最大色相偏移角度（越大越夸张）
            float local = d;

            // 轻微彩边，加强“梦境感”，但不要太炸
            chromaVal += Mathf.Lerp(0f, 0.30f, local);

            // 保留原图，只加一点饱和和对比作为“滤镜加强”
            saturationVal += Mathf.Lerp(0f, 15f, local);
            contrastVal += Mathf.Lerp(0f, 8f, local);

            // 用 hueShift 做整体颜色偏移（来回晃动）
            hueShiftVal += Mathf.Lerp(-maxHueShift, maxHueShift, 0.5f + 0.5f * hueWave) * local;

            // SplitToning 做轻微色调滤镜（阴影偏紫，高光偏青）
            hueShadows = 280f;               // 稍微偏紫
            satShadows = Mathf.Lerp(0f, 18f, local);
            hueHighlights = 150f;               // 偏青
            satHighlights = Mathf.Lerp(0f, 14f, local);
            splitBalance = Mathf.Lerp(-8f, 8f, hueWave) * local;
        }

        // ========= 把累计结果写回 Volume =========
        ApplyChroma(chromaVal);
        ApplyLens(lensVal, lensScale);
        ApplyGrain(grainVal);
        ApplyColor(saturationVal, contrastVal, hueShiftVal);
        ApplySplit(hueShadows, satShadows, hueHighlights, satHighlights, splitBalance);
        ApplyMotionBlur(motionVal);
    }

    private void DisableAll()
    {
        if (_chroma != null) _chroma.active = false;
        if (_distortion != null) _distortion.active = false;
        if (_grain != null) _grain.active = false;
        if (_color != null) _color.active = false;
        if (_split != null) _split.active = false;
        if (_motionBlur != null) _motionBlur.active = false;
    }

    private void ApplyChroma(float val)
    {
        if (_chroma == null) return;
        val = Mathf.Clamp01(val);
        _chroma.active = val > 0.0001f;
        _chroma.intensity.overrideState = true;
        _chroma.intensity.value = val;
    }

    private void ApplyLens(float intensity, float scale)
    {
        if (_distortion == null) return;
        _distortion.active =
            Mathf.Abs(intensity) > 0.0001f ||
            Mathf.Abs(scale - 1f) > 0.0001f;

        _distortion.intensity.overrideState = true;
        _distortion.intensity.value = Mathf.Clamp(intensity, -1f, 1f);

        _distortion.scale.overrideState = true;
        _distortion.scale.value = Mathf.Clamp(scale, 0.85f, 1.2f);
    }

    private void ApplyGrain(float val)
    {
        if (_grain == null) return;
        val = Mathf.Clamp01(val);
        _grain.active = val > 0.0001f;

        _grain.intensity.overrideState = true;
        _grain.intensity.value = val;

        _grain.response.overrideState = true;
        _grain.response.value = Mathf.Lerp(0.8f, 1.0f, val);
    }

    private void ApplyColor(float saturation, float contrast, float hueShift)
    {
        if (_color == null) return;

        bool any =
            Mathf.Abs(saturation) > 0.001f ||
            Mathf.Abs(contrast) > 0.001f ||
            Mathf.Abs(hueShift) > 0.001f;

        _color.active = any;

        _color.saturation.overrideState = true;
        _color.saturation.value = Mathf.Clamp(saturation, -100f, 100f);

        _color.contrast.overrideState = true;
        _color.contrast.value = Mathf.Clamp(contrast, -100f, 100f);

        _color.hueShift.overrideState = true;
        _color.hueShift.value = Mathf.Clamp(hueShift, -180f, 180f);
    }

    private void ApplySplit(float hueShadow, float satShadow,
                            float hueHighlight, float satHighlight,
                            float balance)
    {
        if (_split == null) return;

        bool any = satShadow > 0.01f || satHighlight > 0.01f;
        _split.active = any;

        _split.shadows.overrideState = true;
        _split.shadows.value = new Vector4(
            Mathf.Repeat(hueShadow, 360f),
            Mathf.Clamp(satShadow, 0f, 100f),
            0f, 0f);

        _split.highlights.overrideState = true;
        _split.highlights.value = new Vector4(
            Mathf.Repeat(hueHighlight, 360f),
            Mathf.Clamp(satHighlight, 0f, 100f),
            0f, 0f);

        _split.balance.overrideState = true;
        _split.balance.value = Mathf.Clamp(balance, -100f, 100f);
    }

    private void ApplyMotionBlur(float val)
    {
        if (_motionBlur == null) return;
        val = Mathf.Clamp01(val);
        _motionBlur.active = val > 0.01f;

        _motionBlur.intensity.overrideState = true;
        _motionBlur.intensity.value = val;
    }

    // ====== 对外接口（如果你想用代码控制开关） ======

    public void SetBroadcastEnabled(bool on) => enableBroadcastGlitch = on;
    public void SetAfterimageEnabled(bool on) => enableAfterimage = on;
    public void SetPsychedelicEnabled(bool on) => enablePsychedelic = on;

    public void SetGlobalIntensity(float value)
    {
        globalIntensity = Mathf.Clamp01(value);
    }
}
