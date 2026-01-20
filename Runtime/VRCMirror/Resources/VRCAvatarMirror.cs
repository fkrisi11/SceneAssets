#if UNITY_EDITOR
using System;
using UnityEngine;
using VRC.SDKBase;

[ExecuteInEditMode]
[HelpURL("https://creators.vrchat.com/worlds/components/vrc_mirrorreflection")]
[AddComponentMenu("VRChat/VRC Avatar Mirror Reflection")]
public class VRCAvatarMirrorReflection : VRC_MirrorReflection
{
    public MirrorClearMode clearMode = MirrorClearMode.MatchReference;

    public Material overrideSkybox;

    public Color overrideClearColor = new Color(0f, 0f, 0f, 1f);

    public enum MirrorClearMode
    {
        MatchReference = 0,
        UseSkybox = 1,
        UseSolidColor = 2,
        UseNothing = 3,
        UseDepthOnly = 4
    }

    protected override void UpdateCameraClearing(Camera referenceCamera, Camera mirrorCamera, Skybox mirrorSkybox)
    {
        if (mirrorCamera == null)
            throw new ArgumentNullException(nameof(mirrorCamera));

        switch (clearMode)
        {
            case MirrorClearMode.MatchReference:
                ApplyReferenceClear(referenceCamera, mirrorCamera, mirrorSkybox);
                return;

            case MirrorClearMode.UseSkybox:
                ApplyOverrideSkybox(mirrorCamera, mirrorSkybox);
                return;

            case MirrorClearMode.UseSolidColor:
                ApplySolidColor(mirrorCamera, mirrorSkybox, overrideClearColor);
                return;

            case MirrorClearMode.UseNothing:
                ApplyClearFlagsOnly(mirrorCamera, mirrorSkybox, CameraClearFlags.Nothing);
                return;

            case MirrorClearMode.UseDepthOnly:
                ApplyClearFlagsOnly(mirrorCamera, mirrorSkybox, CameraClearFlags.Depth);
                return;

            default:
                ApplySolidColor(mirrorCamera, mirrorSkybox, Color.black);
                return;
        }
    }

    private static void ApplyReferenceClear(Camera referenceCamera, Camera mirrorCamera, Skybox mirrorSkybox)
    {
        if (referenceCamera == null)
        {
            ApplySolidColor(mirrorCamera, mirrorSkybox, Color.black);
            return;
        }

        mirrorCamera.clearFlags = referenceCamera.clearFlags;
        mirrorCamera.backgroundColor = referenceCamera.backgroundColor;

        if (referenceCamera.clearFlags != CameraClearFlags.Skybox)
        {
            DisableSkybox(mirrorSkybox);
            return;
        }

        var srcSkybox = referenceCamera.GetComponent<Skybox>();
        if (srcSkybox == null || srcSkybox.material == null)
        {
            DisableSkybox(mirrorSkybox);
            return;
        }

        EnableSkybox(mirrorSkybox, srcSkybox.material);
    }

    private void ApplyOverrideSkybox(Camera mirrorCamera, Skybox mirrorSkybox)
    {
        if (overrideSkybox != null)
        {
            mirrorCamera.clearFlags = CameraClearFlags.Skybox;
            EnableSkybox(mirrorSkybox, overrideSkybox);
            return;
        }

        ApplySolidColor(mirrorCamera, mirrorSkybox, Color.black);
    }

    private static void ApplySolidColor(Camera mirrorCamera, Skybox mirrorSkybox, Color color)
    {
        mirrorCamera.clearFlags = CameraClearFlags.SolidColor;
        mirrorCamera.backgroundColor = color;
        DisableSkybox(mirrorSkybox);
    }

    private static void ApplyClearFlagsOnly(Camera mirrorCamera, Skybox mirrorSkybox, CameraClearFlags flags)
    {
        mirrorCamera.clearFlags = flags;
        DisableSkybox(mirrorSkybox);
    }

    private static void EnableSkybox(Skybox target, Material mat)
    {
        if (target == null)
            return;

        target.enabled = true;
        target.material = mat;
    }

    private static void DisableSkybox(Skybox target)
    {
        if (target == null)
            return;

        target.enabled = false;
    }
}
#endif