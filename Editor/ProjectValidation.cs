using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace BIMOS.Editor
{
    public class ProjectValidation : MonoBehaviour
    {
        private const string _category = "BIPED";
        const string _projectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";

        private const string _metaDisplayName = "Meta Movement";
        private const string _metaPackageName = "com.meta.movement";
        private const string _metaPackageUrl = "https://github.com/oculus-samples/Unity-Movement.git";
        private static AddRequest _metaPackageAddRequest;

        private const string _viveDisplayName = "VIVE OpenXR Plugin";
        private const string _vivePackageName = "com.htc.upm.vive.openxr";
        private const string _vivePackageUrl = "https://github.com/ViveSoftware/VIVE-OpenXR.git?path=com.htc.upm.vive.openxr";
        private static AddRequest _vivePackageAddRequest;

        static readonly BuildTargetGroup[] s_BuildTargetGroups =
            ((BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))).Distinct().ToArray();

        static readonly List<BuildValidationRule> s_BuildValidationRules = new()
        {
            new()
            {
                IsRuleEnabled = () => _metaPackageAddRequest == null || _metaPackageAddRequest.IsCompleted,
                Message = $"{_metaDisplayName} package ({_metaPackageName}) must be installed to use BIPED.",
                Category = _category,
                CheckPredicate = () => PackageVersionUtility.IsPackageInstalled(_metaPackageName),
                FixIt = () =>
                {
                    _metaPackageAddRequest = Client.Add(_metaPackageUrl);
                    if (_metaPackageAddRequest.Error != null)
                        Debug.LogError($"Package installation error: {_metaPackageAddRequest.Error}: {_metaPackageAddRequest.Error.message}");
                    else
                        Client.Resolve();
                },
                FixItAutomatic = true,
                Error = true
            },
            new()
            {
                IsRuleEnabled = () => _vivePackageAddRequest == null || _vivePackageAddRequest.IsCompleted,
                Message = $"{_viveDisplayName} package ({_vivePackageName}) must be installed to use BIPED.",
                Category = _category,
                CheckPredicate = () => PackageVersionUtility.IsPackageInstalled(_vivePackageName),
                FixIt = () =>
                {
                    _vivePackageAddRequest = Client.Add(_vivePackageUrl);
                    if (_vivePackageAddRequest.Error != null)
                        Debug.LogError($"Package installation error: {_vivePackageAddRequest.Error}: {_vivePackageAddRequest.Error.message}");
                    else
                        Client.Resolve();
                },
                FixItAutomatic = true,
                Error = true
            }
        };

        [InitializeOnLoadMethod]
        static void RegisterProjectValidationRules()
        {
            foreach (var buildTargetGroup in s_BuildTargetGroups)
            {
                BuildValidator.AddRules(buildTargetGroup, s_BuildValidationRules);
            }

            // Delay evaluating conditions for issues to give time for Package Manager and UPM cache to fully initialize.
            EditorApplication.delayCall += ShowWindowIfIssuesExist;
        }

        static void ShowWindowIfIssuesExist()
        {
            foreach (var validation in s_BuildValidationRules)
            {
                if (validation.CheckPredicate == null || !validation.CheckPredicate.Invoke())
                {
                    ShowWindow();
                    return;
                }
            }
        }

        internal static void ShowWindow()
        {
            // Delay opening the window since sometimes other settings in the player settings provider redirect to the
            // project validation window causing serialized objects to be nullified.
            EditorApplication.delayCall += () =>
            {
                SettingsService.OpenProjectSettings(_projectValidationSettingsPath);
            };
        }
    }
}
