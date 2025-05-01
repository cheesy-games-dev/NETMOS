using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.Rendering.Universal;

namespace BIMOS.Editor
{
    public class ProjectValidation : MonoBehaviour
    {
        private const string _category = "BIMOS";
        const string _projectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";

        private const string _openXRLoaderTypeName = "UnityEngine.XR.OpenXR.OpenXRLoader";

        static readonly BuildTargetGroup[] s_BuildTargetGroups =
            ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();

        static readonly List<BuildValidationRule> s_BuildValidationRules = new()
        {
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Must be using Unity 6 or greater",
                Category = _category,
                CheckPredicate = () =>
                {
                    var major = int.Parse(Application.unityVersion.Split(".")[0]);
                    return major >= 6000;
                },
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Plug-in Provider must be OpenXR for PC",
                Category = _category,
                CheckPredicate = () =>
                {
                    EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out XRGeneralSettingsPerBuildTarget buildTargetSettings);
                    XRGeneralSettings settings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.Standalone);
                    var activeLoaders = settings.Manager.activeLoaders;
                    if (activeLoaders.Count <= 0)
                        return false;

                    var activeLoader = activeLoaders[0];
                    var loaderTypeName = activeLoader.GetType().FullName;
                    return activeLoader.GetType().FullName.Equals(_openXRLoaderTypeName);
                },
                FixIt = () =>
                {
                    EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out XRGeneralSettingsPerBuildTarget buildTargetSettings);
                    XRGeneralSettings settings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.Standalone);
                    XRPackageMetadataStore.AssignLoader(settings.Manager, _openXRLoaderTypeName, BuildTargetGroup.Standalone);
                },
                FixItAutomatic = true,
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Plug-in Provider must be OpenXR for Android",
                Category = _category,
                CheckPredicate = () =>
                {
                    EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out XRGeneralSettingsPerBuildTarget buildTargetSettings);
                    XRGeneralSettings settings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.Android);
                    var activeLoaders = settings.Manager.activeLoaders;
                    if (activeLoaders.Count <= 0)
                        return false;

                    var activeLoader = activeLoaders[0];
                    var loaderTypeName = activeLoader.GetType().FullName;
                    return activeLoader.GetType().FullName.Equals(_openXRLoaderTypeName);
                },
                FixIt = () =>
                {
                    EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out XRGeneralSettingsPerBuildTarget buildTargetSettings);
                    XRGeneralSettings settings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.Android);
                    XRPackageMetadataStore.AssignLoader(settings.Manager, _openXRLoaderTypeName, BuildTargetGroup.Android);
                },
                FixItAutomatic = true,
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Must enable PC controller profiles and features",
                Category = _category,
                CheckPredicate = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Standalone);
                    if (!settings.GetFeature<OculusTouchControllerProfile>().enabled)
                        return false;
                    else if (!settings.GetFeature<HTCViveControllerProfile>().enabled)
                        return false;
                    else if (!settings.GetFeature<HPReverbG2ControllerProfile>().enabled)
                        return false;
                    else if (!settings.GetFeature<ValveIndexControllerProfile>().enabled)
                        return false;
                    else if (!settings.GetFeature<PalmPoseInteraction>().enabled)
                        return false;

                    return true;
                },
                FixIt = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Standalone);
                    settings.GetFeature<OculusTouchControllerProfile>().enabled
                        = settings.GetFeature<HTCViveControllerProfile>().enabled
                        = settings.GetFeature<HPReverbG2ControllerProfile>().enabled
                        = settings.GetFeature<ValveIndexControllerProfile>().enabled
                        = settings.GetFeature<PalmPoseInteraction>().enabled
                        = true;
                },
                FixItAutomatic = true,
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Must enable Android controller profiles and features",
                Category = _category,
                CheckPredicate = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
                    if (!settings.GetFeature<OculusTouchControllerProfile>().enabled)
                        return false;
                    else if (!settings.GetFeature<MetaQuestTouchPlusControllerProfile>().enabled)
                        return false;
                    else if (!settings.GetFeature<MetaQuestTouchProControllerProfile>().enabled)
                        return false;
                    else if (!settings.GetFeature<PalmPoseInteraction>().enabled)
                        return false;
                    else if (!settings.GetFeature<MetaQuestFeature>().enabled)
                        return false;

                    return true;
                },
                FixIt = () =>
                {
                    var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
                    settings.GetFeature<OculusTouchControllerProfile>().enabled
                        = settings.GetFeature<MetaQuestTouchPlusControllerProfile>().enabled
                        = settings.GetFeature<MetaQuestTouchProControllerProfile>().enabled
                        = settings.GetFeature<PalmPoseInteraction>().enabled
                        = settings.GetFeature<MetaQuestFeature>().enabled
                        = true;
                },
                FixItAutomatic = true,
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Must have a layer called \"BIMOSRig\"",
                Category = _category,
                CheckPredicate = () =>
                {
                    for (int i = 0; i < 32; i++)
                        if (LayerMask.LayerToName(i).Equals("BIMOSRig"))
                            return true;

                    return false;
                },
                FixIt = () =>
                {
                    SettingsService.OpenProjectSettings("Project/Tags and Layers");
                },
                FixItAutomatic = true,
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Must have a layer called \"BIMOSMenu\"",
                Category = _category,
                CheckPredicate = () =>
                {
                    for (int i = 0; i < 32; i++)
                        if (LayerMask.LayerToName(i).Equals("BIMOSMenu"))
                            return true;

                    return false;
                },
                FixIt = () =>
                {
                    SettingsService.OpenProjectSettings("Project/Tags and Layers");
                },
                FixItAutomatic = true,
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Must enable the Decal feature in URP asset for bullet holes",
                Category = _category,
                CheckPredicate = () =>
                {
                    for (int currentLevel = 0; currentLevel < QualitySettings.count; currentLevel++)
                    {
                        var asset = QualitySettings.GetRenderPipelineAssetAt(currentLevel) as UniversalRenderPipelineAsset;
                        if (!asset)
                            continue;

                        foreach (var rendererData in asset.rendererDataList)
                            if (!rendererData.TryGetRendererFeature<DecalRendererFeature>(out _))
                                return false;
                    }

                    return true;
                },
                FixIt = () =>
                {
                    for (int currentLevel = 0; currentLevel < QualitySettings.count; currentLevel++)
                    {
                        var asset = QualitySettings.GetRenderPipelineAssetAt(currentLevel) as UniversalRenderPipelineAsset;
                        if (!asset)
                            continue;

                        foreach (var rendererData in asset.rendererDataList)
                        {
                            if (rendererData.TryGetRendererFeature<DecalRendererFeature>(out _))
                                continue;

                            AddRendererFeature(rendererData, typeof(DecalRendererFeature));
                        }
                    }
                },
                FixItAutomatic = true,
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => true,
                Message = "Must enable Adaptive Probe Volumes for Sample Scene",
                Category = _category,
                CheckPredicate = () =>
                {
                    for (int currentLevel = 0; currentLevel < QualitySettings.count; currentLevel++)
                    {
                        var asset = QualitySettings.GetRenderPipelineAssetAt(currentLevel) as UniversalRenderPipelineAsset;
                        if (!asset)
                            continue;

                        if (asset.lightProbeSystem != LightProbeSystem.ProbeVolumes)
                            return false;
                    }

                    return true;
                },
                FixIt = () =>
                {
                    for (int currentLevel = 0; currentLevel < QualitySettings.count; currentLevel++)
                    {
                        var asset = QualitySettings.GetRenderPipelineAssetAt(currentLevel) as UniversalRenderPipelineAsset;
                        if (!asset)
                            continue;

                        if (asset.lightProbeSystem == LightProbeSystem.ProbeVolumes)
                            continue;

                        var serializedObject = new SerializedObject(asset);
                        var lightProbeSystemProp = serializedObject.FindProperty("m_LightProbeSystem");

                        Undo.RecordObject(asset, "Modified Light Probe System in " + asset.name);
                        lightProbeSystemProp.intValue = (int) LightProbeSystem.ProbeVolumes;

                        // Force save / refresh
                        if (EditorUtility.IsPersistent(asset))
                            AssetDatabase.SaveAssetIfDirty(asset);

                        serializedObject.ApplyModifiedProperties();
                    }
                },
                FixItAutomatic = true,
                Error = true
            }
        };

        [InitializeOnLoadMethod]
        static void RegisterProjectValidationRules()
        {
            foreach (var buildTargetGroup in s_BuildTargetGroups)
                BuildValidator.AddRules(buildTargetGroup, s_BuildValidationRules);

            // Delay evaluating conditions for issues to give time for Package Manager and UPM cache to fully initialize.
            EditorApplication.delayCall += ShowWindowIfIssuesExist;
        }

        static void ShowWindowIfIssuesExist()
        {
            foreach (var validation in s_BuildValidationRules)
                if (validation.CheckPredicate == null || !validation.CheckPredicate.Invoke())
                {
                    ShowWindow();
                    return;
                }
        }

        internal static void ShowWindow()
        {
            // Delay opening the window since sometimes other settings in the player settings provider redirect to the
            // project validation window causing serialized objects to be nullified.
            EditorApplication.delayCall += () => SettingsService.OpenProjectSettings(_projectValidationSettingsPath);
        }

        private static void AddRendererFeature(ScriptableRendererData data, Type type)
        {
            var serializedObject = new SerializedObject(data);

            var rendererFeaturesProp = serializedObject.FindProperty("m_RendererFeatures");
            var rendererFeatureMapProp = serializedObject.FindProperty("m_RendererFeatureMap");

            serializedObject.Update();

            var feature = ScriptableObject.CreateInstance(type);
            feature.name = type.Name;
            Undo.RegisterCreatedObjectUndo(feature, "Add Renderer Feature");

            // Store this new effect as a sub-asset so we can reference it safely afterwards
            // Only when we're not dealing with an instantiated asset
            if (EditorUtility.IsPersistent(data))
                AssetDatabase.AddObjectToAsset(feature, data);

            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(feature, out _, out long localId);

            // Grow the list first, then add - that's how serialized lists work in Unity
            rendererFeaturesProp.arraySize++;
            var componentProp = rendererFeaturesProp.GetArrayElementAtIndex(rendererFeaturesProp.arraySize - 1);
            componentProp.objectReferenceValue = feature;

            // Update GUID Map
            rendererFeatureMapProp.arraySize++;
            var guidProp = rendererFeatureMapProp.GetArrayElementAtIndex(rendererFeatureMapProp.arraySize - 1);
            guidProp.longValue = localId;

            // Force save / refresh
            if (EditorUtility.IsPersistent(data))
                AssetDatabase.SaveAssetIfDirty(data);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
