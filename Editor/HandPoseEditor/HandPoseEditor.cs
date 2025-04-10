using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.XR;

namespace BIMOS
{
    // Monolithic class - I'm so sorry
    public class HandPoseEditor : EditorWindow
    {
        public Object DummyHandPrefab;

        [SerializeField]
        private GameObject _mirrorPrefab;

        [SerializeField]
        private AnimatorController _controller;

        private Vector2 _scrollPosition = Vector2.zero;
        private GameObject _dummyHand, _mirror;
        private Transform _armature, _currentSelection, _palm;

        private Animator _animator;
        private bool _isAnimatorInitialized = false;

        private DummyHand _leftHand;
        private DummyHand _rightHand;
        private DummyHand _currentHand;

        private BoneRenderer _selectedBoneRenderer;

        HandPose _currentHandPose;

        private enum SubPose
        {
            None,
            GripOpen, GripClosed,
            IndexOpen, IndexTriggerTouched, IndexClosed,
            ThumbOpen, ThumbrestTouched, PrimaryTouched, PrimaryButton, SecondaryTouched, SecondaryButton, ThumbstickTouched
        };
        SubPose _subPose = SubPose.None;
        Object _currentAsset;

        [MenuItem("Window/BIMOS/Hand Pose Editor")]
        public static void ShowEditor()
        {
            GetWindow<HandPoseEditor>("Hand Pose Editor");
        }

        private void Update()
        {
            if (!_animator)
                return;

            Transform hips = _animator.GetBoneTransform(HumanBodyBones.Hips);
            Transform hand = null;
            if (_currentHand != null)
                hand = _currentHand.GetBoneTransform(HandBones.Hand);

            if (!_isAnimatorInitialized)
            {
                _animator.Update(0f);
                Debug.Log(_currentHand.GetBoneTransform(HandBones.ThumbProximal).localRotation);
                _isAnimatorInitialized = _currentHand.InitialData.Thumb[0] != _currentHand.GetBoneTransform(HandBones.ThumbProximal).localRotation;

                if (_isAnimatorInitialized)
                {
                    hips.position = _dummyHand.transform.position;
                    _leftHand.GetBoneTransform(HandBones.Hand).rotation = _dummyHand.transform.rotation;
                    _rightHand.GetBoneTransform(HandBones.Hand).rotation = _dummyHand.transform.rotation;
                    hips.position += Vector3.down * 0.09f;

                    _leftHand.InitialData.Thumb[0] = _leftHand.GetBoneTransform(HandBones.ThumbProximal).localRotation;
                    _leftHand.InitialData.Thumb[1] = _leftHand.GetBoneTransform(HandBones.ThumbIntermediate).localRotation;
                    _leftHand.InitialData.Thumb[2] = _leftHand.GetBoneTransform(HandBones.ThumbDistal).localRotation;
                    _leftHand.InitialData.Index[0] = _leftHand.GetBoneTransform(HandBones.IndexProximal).localRotation;
                    _leftHand.InitialData.Index[1] = _leftHand.GetBoneTransform(HandBones.IndexIntermediate).localRotation;
                    _leftHand.InitialData.Index[2] = _leftHand.GetBoneTransform(HandBones.IndexDistal).localRotation;
                    _leftHand.InitialData.Middle[0] = _leftHand.GetBoneTransform(HandBones.MiddleProximal).localRotation;
                    _leftHand.InitialData.Middle[1] = _leftHand.GetBoneTransform(HandBones.MiddleIntermediate).localRotation;
                    _leftHand.InitialData.Middle[2] = _leftHand.GetBoneTransform(HandBones.MiddleDistal).localRotation;
                    _leftHand.InitialData.Ring[0] = _leftHand.GetBoneTransform(HandBones.RingProximal).localRotation;
                    _leftHand.InitialData.Ring[1] = _leftHand.GetBoneTransform(HandBones.RingIntermediate).localRotation;
                    _leftHand.InitialData.Ring[2] = _leftHand.GetBoneTransform(HandBones.RingDistal).localRotation;
                    _leftHand.InitialData.Little[0] = _leftHand.GetBoneTransform(HandBones.LittleProximal).localRotation;
                    _leftHand.InitialData.Little[1] = _leftHand.GetBoneTransform(HandBones.LittleIntermediate).localRotation;
                    _leftHand.InitialData.Little[2] = _leftHand.GetBoneTransform(HandBones.LittleDistal).localRotation;

                    _rightHand.InitialData.Thumb[0] = _rightHand.GetBoneTransform(HandBones.ThumbProximal).localRotation;
                    _rightHand.InitialData.Thumb[1] = _rightHand.GetBoneTransform(HandBones.ThumbIntermediate).localRotation;
                    _rightHand.InitialData.Thumb[2] = _rightHand.GetBoneTransform(HandBones.ThumbDistal).localRotation;
                    _rightHand.InitialData.Index[0] = _rightHand.GetBoneTransform(HandBones.IndexProximal).localRotation;
                    _rightHand.InitialData.Index[1] = _rightHand.GetBoneTransform(HandBones.IndexIntermediate).localRotation;
                    _rightHand.InitialData.Index[2] = _rightHand.GetBoneTransform(HandBones.IndexDistal).localRotation;
                    _rightHand.InitialData.Middle[0] = _rightHand.GetBoneTransform(HandBones.MiddleProximal).localRotation;
                    _rightHand.InitialData.Middle[1] = _rightHand.GetBoneTransform(HandBones.MiddleIntermediate).localRotation;
                    _rightHand.InitialData.Middle[2] = _rightHand.GetBoneTransform(HandBones.MiddleDistal).localRotation;
                    _rightHand.InitialData.Ring[0] = _rightHand.GetBoneTransform(HandBones.RingProximal).localRotation;
                    _rightHand.InitialData.Ring[1] = _rightHand.GetBoneTransform(HandBones.RingIntermediate).localRotation;
                    _rightHand.InitialData.Ring[2] = _rightHand.GetBoneTransform(HandBones.RingDistal).localRotation;
                    _rightHand.InitialData.Little[0] = _rightHand.GetBoneTransform(HandBones.LittleProximal).localRotation;
                    _rightHand.InitialData.Little[1] = _rightHand.GetBoneTransform(HandBones.LittleIntermediate).localRotation;
                    _rightHand.InitialData.Little[2] = _rightHand.GetBoneTransform(HandBones.LittleDistal).localRotation;

                    _subPose = SubPose.GripClosed;
                    HandPoseToDummy(_currentHandPose);
                    _subPose = SubPose.IndexClosed;
                    HandPoseToDummy(_currentHandPose);
                    _subPose = SubPose.ThumbrestTouched;
                    HandPoseToDummy(_currentHandPose);
                    _subPose = SubPose.None;
                    HandPoseToDummy(_currentHandPose);
                }
            }

            if (_currentSelection && _dummyHand)
            {
                Vector3 pos = _currentSelection.TransformPoint(_palm.InverseTransformPoint(hand.position));
                Quaternion rot = _currentSelection.rotation * Quaternion.Inverse(_palm.rotation) * hand.rotation;
                hips.transform.position = pos;
                hand.transform.rotation = rot;
            }
        }

        private void CreateDummyHand()
        {
            _isAnimatorInitialized = false;
            DestroyImmediate(_dummyHand);
            _dummyHand = Instantiate((GameObject) DummyHandPrefab);
            _dummyHand.name = "DummyHand";
            _subPose = SubPose.None;

            _animator = _dummyHand.GetComponent<Animator>();
            _armature = _animator.avatarRoot;
            _animator.runtimeAnimatorController = _controller;
            _rightHand = new DummyHand(_animator, false);
            _leftHand = new DummyHand(_animator, true);

            foreach (SkinnedMeshRenderer renderer in _dummyHand.GetComponentsInChildren<SkinnedMeshRenderer>())
                renderer.updateWhenOffscreen = true;

            _currentHand = new DummyHand(_animator, false);

            _currentHand.InitialData.Thumb[0] = _currentHand.GetBoneTransform(HandBones.ThumbProximal).localRotation;

            _selectedBoneRenderer = _dummyHand.AddComponent<BoneRenderer>();
            _selectedBoneRenderer.boneColor = new Color(1f, 0f, 0f, 0.5f);

            _armature.localScale = 1f / 1000f * Vector3.one;

            _currentHandPose = CreateInstance<HandPose>();
            _currentHandPose = Instantiate(_currentHandPose);

            _dummyHand.transform.parent = null;

            if (_currentSelection)
            {
                SnapGrab grab = _currentSelection.GetComponent<SnapGrab>();
                if (grab)
                {
                    if (grab.IsLeftHanded && !grab.IsRightHanded)
                        _currentHand = _leftHand;
                    else
                        _currentHand = _rightHand;

                    if (grab.HandPose)
                    {
                        _currentHandPose = grab.HandPose;
                        _currentAsset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(grab.HandPose), typeof(HandPose));
                    }
                }
            }
        }

        private void InitializeDummyHand(Vector3 position, Quaternion rotation)
        {
            _dummyHand.transform.SetPositionAndRotation(position, rotation);

            _leftHand.GetBoneTransform(HandBones.Hand).localScale
                = _rightHand.GetBoneTransform(HandBones.Hand).localScale
                    = Vector3.one;

            _currentHand.GetBoneTransform(HandBones.Hand).localScale = 1000f * Vector3.one;

            Transform hand = _currentHand.GetBoneTransform(HandBones.Hand);
            Transform middleProximal = _currentHand.GetBoneTransform(HandBones.MiddleProximal);

            if (_palm)
                DestroyImmediate(_palm.gameObject);

            GameObject palm = new();
            _palm = palm.transform;
            _palm.transform.parent = hand;
            _palm.transform.SetPositionAndRotation(
                Vector3.Lerp(hand.position, middleProximal.position, 0.5f),
                Quaternion.Lerp(hand.rotation, middleProximal.rotation, 0.5f)
            );

            HandPoseToDummy(_currentHandPose);
        }

        private void GetBones(Transform parent, List<Transform> bones, bool first = false)
        {
            if (!first)
                bones.Add(parent);

            foreach (Transform bone in parent)
            {
                bones.Add(bone);
                GetBones(bone, bones);
            }
        }

        private void OnGUI()
        {
            minSize = new Vector2(150f, 351f);
            maxSize = new Vector2(265f, 576f);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUIStyle.none);

            //All the controls for the editor
            GUILayout.Label("Character Model");
            DummyHandPrefab = EditorGUILayout.ObjectField(DummyHandPrefab, typeof(GameObject), true);
            if (GUILayout.Button("Select"))
            {
                _currentSelection = Selection.activeTransform;

                //Insert the dummy hand into the scene 0.25m in front of the camera
                Vector3 spawnLocation = SceneView.lastActiveSceneView.camera.transform.TransformPoint(Vector3.forward * 0.25f);
                Quaternion spawnRotation = Quaternion.LookRotation(Vector3.Cross(SceneView.lastActiveSceneView.camera.transform.right, Vector3.down));
                CreateDummyHand();
                InitializeDummyHand(spawnLocation, spawnRotation);
            }
            GUILayout.Label("File Handling");
            if (GUILayout.Button("New"))
            {
                _currentAsset = null;

                //Reset the dummy hand's pose (by deleting and creating a new one in its position)
                if (_dummyHand != null)
                {
                    Vector3 position = _dummyHand.transform.position;
                    Quaternion rotation = _dummyHand.transform.rotation;
                    DestroyImmediate(_dummyHand); //Deletes dummy hand
                    InitializeDummyHand(position, rotation); //Respawns at old location
                }
            }
            if (GUILayout.Button("Open"))
            {
                //Opens file explorer for the user to navigate to their file
                string path = EditorUtility.OpenFilePanel("Open hand pose", "", "asset");
                path = path.Replace(Application.dataPath, "Assets"); //Converts absolute path to relative path

                if (path.Length > 0)
                { //Check that path isn't empty
                    _currentAsset = AssetDatabase.LoadAssetAtPath(path, typeof(HandPose));
                    HandPose handPose = (HandPose)_currentAsset;
                    _currentHandPose = handPose;
                    _subPose = SubPose.GripClosed;
                    HandPoseToDummy(_currentHandPose);
                    _subPose = SubPose.IndexClosed;
                    HandPoseToDummy(_currentHandPose);
                    _subPose = SubPose.ThumbrestTouched;
                    HandPoseToDummy(_currentHandPose);
                    _subPose = SubPose.None;
                    HandPoseToDummy(_currentHandPose);
                }
            }
            if (GUILayout.Button("Save"))
            {
                if (!_currentAsset) //If there is no file currently loaded
                    SaveAs();
                else
                    Save();
            }
            if (GUILayout.Button("Save as"))
                SaveAs();
            GUILayout.Label("Misc");
            if (_currentHand == _leftHand)
            {
                if (GUILayout.Button("Swap to right hand"))
                {
                    _currentHand = _rightHand;
                    InitializeDummyHand(_dummyHand.transform.position, _dummyHand.transform.rotation);
                }
            }
            else
            {
                if (GUILayout.Button("Swap to left hand"))
                {
                    _currentHand = _leftHand;
                    InitializeDummyHand(_dummyHand.transform.position, _dummyHand.transform.rotation);
                }
            }
            if (!_mirror)
            {
                if (GUILayout.Button("Spawn mirror"))
                {
                    Transform parent = Selection.activeTransform.parent;

                    if (Selection.activeTransform.parent)
                        parent = Selection.activeTransform.parent;

                    _mirror = Instantiate(_mirrorPrefab, parent.position, parent.rotation, parent);
                    _mirror.name = "Mirror";
                }
            }
            else
            {
                if (GUILayout.Button("Mirror grabs"))
                {
                    foreach (GameObject grab in Selection.gameObjects)
                    {
                        if (!grab.GetComponent<Grab>())
                            return;

                        GameObject mirroredGrab = Instantiate(grab, grab.transform.parent);
                        mirroredGrab.name = grab.name
                            .Replace("Left", "R I G H T")
                            .Replace("Right", "L E F T")
                            .Replace("R I G H T", "Right")
                            .Replace("L E F T", "Left");

                        foreach (Transform child in mirroredGrab.transform)
                            DestroyImmediate(child.gameObject);

                        MirrorGrab(mirroredGrab.GetComponent<Grab>());
                    }
                }
            }
            SubPoseButtons();
            GUILayout.EndScrollView();
        }

        private void SubPoseButtons()
        {
            GUILayout.Label("Sub-Poses");
            SubPoseButton("None", SubPose.None);
            GUILayout.Label("Little, Ring, Middle");
            SubPoseButton("Open", SubPose.GripOpen);
            SubPoseButton("Closed", SubPose.GripClosed);
            GUILayout.Label("Index");
            SubPoseButton("Open", SubPose.IndexOpen);
            SubPoseButton("TriggerTouched", SubPose.IndexTriggerTouched);
            SubPoseButton("Closed", SubPose.IndexClosed);
            GUILayout.Label("Thumb");
            SubPoseButton("Open", SubPose.ThumbOpen);
            SubPoseButton("Thumbrest touched", SubPose.ThumbrestTouched);
            SubPoseButton("Thumbstick touched", SubPose.ThumbstickTouched);
            SubPoseButton("Secondary button", SubPose.PrimaryTouched);
            SubPoseButton("Primary button", SubPose.PrimaryButton);
            SubPoseButton("Secondary touched", SubPose.SecondaryTouched);
            SubPoseButton("Secondary button", SubPose.SecondaryButton);
        }

        private void SubPoseButton(string text, SubPose subPose)
        {
            GUI.backgroundColor = _subPose == subPose ? Color.red : Color.white;
            if (!GUILayout.Button(text))
                return;

            _currentHandPose = DummyToHandPose();
            _subPose = subPose;
            HandPoseToDummy(_currentHandPose);
        }

        private void SaveAs()
        {
            _currentHandPose = DummyToHandPose();

            //Opens the file explorer for the user to navigate to/save their file
            string path = EditorUtility.SaveFilePanel("Save hand pose", "", "MyHandPose", "asset");
            path = path.Replace(Application.dataPath, "Assets"); //Converts absolute path to relative path
            if (path.Length > 0) //Check that path isn't empty
            {
                Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(HandPose));
                HandPose handPose = (HandPose)asset;
                if (!handPose) //If the hand pose does not exist yet
                {
                    //Create new pose
                    AssetDatabase.CreateAsset(_currentHandPose, path);
                }
                else
                {
                    //Overwrite existing pose
                    handPose.Thumb = _currentHandPose.Thumb;
                    handPose.Index = _currentHandPose.Index;
                    handPose.Middle = _currentHandPose.Middle;
                    handPose.Ring = _currentHandPose.Ring;
                    handPose.Little = _currentHandPose.Little;
                    EditorUtility.SetDirty(asset);
                }
                _currentAsset = asset;
            }
        }

        private void Save()
        {
            _currentHandPose = DummyToHandPose();

            //Overwrite existing pose
            HandPose handPose = (HandPose)_currentAsset;
            handPose.Thumb = _currentHandPose.Thumb;
            handPose.Index = _currentHandPose.Index;
            handPose.Middle = _currentHandPose.Middle;
            handPose.Ring = _currentHandPose.Ring;
            handPose.Little = _currentHandPose.Little;
            EditorUtility.SetDirty(_currentAsset);
        }

        private void HandPoseToDummy(HandPose handPose)
        {
            List<Transform> bones = new();
            switch (_subPose)
            {
                case SubPose.GripOpen:
                    FingerPoseToDummy(handPose.Middle.Open, _currentHand.GetBoneTransform(HandBones.MiddleProximal), _currentHand.InitialData.Middle);
                    FingerPoseToDummy(handPose.Ring.Open, _currentHand.GetBoneTransform(HandBones.RingProximal), _currentHand.InitialData.Ring);
                    FingerPoseToDummy(handPose.Little.Open, _currentHand.GetBoneTransform(HandBones.LittleProximal), _currentHand.InitialData.Little);

                    GetBones(_currentHand.GetBoneTransform(HandBones.MiddleProximal), bones);
                    GetBones(_currentHand.GetBoneTransform(HandBones.RingProximal), bones);
                    GetBones(_currentHand.GetBoneTransform(HandBones.LittleProximal), bones);
                    break;
                case SubPose.GripClosed:
                    FingerPoseToDummy(handPose.Middle.Closed, _currentHand.GetBoneTransform(HandBones.MiddleProximal), _currentHand.InitialData.Middle);
                    FingerPoseToDummy(handPose.Ring.Closed, _currentHand.GetBoneTransform(HandBones.RingProximal), _currentHand.InitialData.Ring);
                    FingerPoseToDummy(handPose.Little.Closed, _currentHand.GetBoneTransform(HandBones.LittleProximal), _currentHand.InitialData.Little);

                    GetBones(_currentHand.GetBoneTransform(HandBones.MiddleProximal), bones);
                    GetBones(_currentHand.GetBoneTransform(HandBones.RingProximal), bones);
                    GetBones(_currentHand.GetBoneTransform(HandBones.LittleProximal), bones);
                    break;
                case SubPose.IndexOpen:
                    FingerPoseToDummy(handPose.Index.Open, _currentHand.GetBoneTransform(HandBones.IndexProximal), _currentHand.InitialData.Index);
                    GetBones(_currentHand.GetBoneTransform(HandBones.IndexProximal), bones);
                    break;
                case SubPose.IndexTriggerTouched:
                    FingerPoseToDummy(handPose.Index.TriggerTouched, _currentHand.GetBoneTransform(HandBones.IndexProximal), _currentHand.InitialData.Index);
                    GetBones(_currentHand.GetBoneTransform(HandBones.IndexProximal), bones);
                    break;
                case SubPose.IndexClosed:
                    FingerPoseToDummy(handPose.Index.Closed, _currentHand.GetBoneTransform(HandBones.IndexProximal), _currentHand.InitialData.Index);
                    GetBones(_currentHand.GetBoneTransform(HandBones.IndexProximal), bones);
                    break;
                case SubPose.ThumbOpen:
                    FingerPoseToDummy(handPose.Thumb.Idle, _currentHand.GetBoneTransform(HandBones.ThumbProximal), _currentHand.InitialData.Thumb);
                    GetBones(_currentHand.GetBoneTransform(HandBones.ThumbProximal), bones);
                    break;
                case SubPose.ThumbrestTouched:
                    FingerPoseToDummy(handPose.Thumb.ThumbrestTouched, _currentHand.GetBoneTransform(HandBones.ThumbProximal), _currentHand.InitialData.Thumb);
                    GetBones(_currentHand.GetBoneTransform(HandBones.ThumbProximal), bones);
                    break;
                case SubPose.PrimaryTouched:
                    FingerPoseToDummy(handPose.Thumb.PrimaryTouched, _currentHand.GetBoneTransform(HandBones.ThumbProximal), _currentHand.InitialData.Thumb);
                    GetBones(_currentHand.GetBoneTransform(HandBones.ThumbProximal), bones);
                    break;
                case SubPose.PrimaryButton:
                    FingerPoseToDummy(handPose.Thumb.PrimaryButton, _currentHand.GetBoneTransform(HandBones.ThumbProximal), _currentHand.InitialData.Thumb);
                    GetBones(_currentHand.GetBoneTransform(HandBones.ThumbProximal), bones);
                    break;
                case SubPose.SecondaryTouched:
                    FingerPoseToDummy(handPose.Thumb.SecondaryTouched, _currentHand.GetBoneTransform(HandBones.ThumbProximal), _currentHand.InitialData.Thumb);
                    GetBones(_currentHand.GetBoneTransform(HandBones.ThumbProximal), bones);
                    break;
                case SubPose.SecondaryButton:
                    FingerPoseToDummy(handPose.Thumb.SecondaryButton, _currentHand.GetBoneTransform(HandBones.ThumbProximal), _currentHand.InitialData.Thumb);
                    GetBones(_currentHand.GetBoneTransform(HandBones.ThumbProximal), bones);
                    break;
                case SubPose.ThumbstickTouched:
                    FingerPoseToDummy(handPose.Thumb.ThumbstickTouched, _currentHand.GetBoneTransform(HandBones.ThumbProximal), _currentHand.InitialData.Thumb);
                    GetBones(_currentHand.GetBoneTransform(HandBones.ThumbProximal), bones);
                    break;
            }

            _selectedBoneRenderer.transforms = bones.ToArray();
        }

        private void FingerPoseToDummy(FingerPose fingerPose, Transform rootTransform, Quaternion[] initialFingerData)
        {
            if (_currentHand == _leftHand)
                fingerPose = fingerPose.Mirrored();

            Transform root = rootTransform;
            Transform middle = root.GetChild(0);
            Transform tip = middle.GetChild(0);

            //Sets dummy finger bone rotations to those in the fingerPose
            root.localRotation = initialFingerData[0] * fingerPose.RootBone;
            middle.localRotation = initialFingerData[1] * fingerPose.MiddleBone;
            tip.localRotation = initialFingerData[2] * fingerPose.TipBone;
        }

        private HandPose DummyToHandPose()
        {
            HandPose handPose = _currentHandPose;
            handPose = Instantiate(handPose);

            switch (_subPose)
            {
                case SubPose.GripOpen:
                    handPose.Middle.Open = DummyToFingerPose(_currentHand.GetBoneTransform(HandBones.MiddleProximal), _currentHand.InitialData.Middle);
                    handPose.Ring.Open = DummyToFingerPose(_currentHand.GetBoneTransform(HandBones.RingProximal), _currentHand.InitialData.Ring);
                    handPose.Little.Open = DummyToFingerPose(_currentHand.GetBoneTransform(HandBones.LittleProximal), _currentHand.InitialData.Little);
                    break;
                case SubPose.GripClosed:
                    handPose.Middle.Closed = DummyToFingerPose(_currentHand.GetBoneTransform(HandBones.MiddleProximal), _currentHand.InitialData.Middle);
                    handPose.Ring.Closed = DummyToFingerPose(_currentHand.GetBoneTransform(HandBones.RingProximal), _currentHand.InitialData.Ring);
                    handPose.Little.Closed = DummyToFingerPose(_currentHand.GetBoneTransform(HandBones.LittleProximal), _currentHand.InitialData.Little);
                    break;
                case SubPose.IndexOpen:
                    handPose.Index.Open = DummyToFingerPose(_currentHand.GetBoneTransform(HandBones.IndexProximal), _currentHand.InitialData.Index);
                    break;
                case SubPose.IndexTriggerTouched:
                    handPose.Index.TriggerTouched = DummyToFingerPose(_currentHand.GetBoneTransform(HandBones.IndexProximal), _currentHand.InitialData.Index);
                    break;
                case SubPose.IndexClosed:
                    handPose.Index.Closed = DummyToFingerPose(_currentHand.GetBoneTransform(HandBones.IndexProximal), _currentHand.InitialData.Index);
                    break;
                case SubPose.ThumbOpen:
                    handPose.Thumb.Idle = DummyToFingerPose(_currentHand.GetBoneTransform(HandBones.ThumbProximal), _currentHand.InitialData.Thumb);
                    break;
                case SubPose.ThumbrestTouched:
                    handPose.Thumb.ThumbrestTouched = DummyToFingerPose(_currentHand.GetBoneTransform(HandBones.ThumbProximal), _currentHand.InitialData.Thumb);
                    break;
                case SubPose.PrimaryTouched:
                    handPose.Thumb.PrimaryTouched = DummyToFingerPose(_currentHand.GetBoneTransform(HandBones.ThumbProximal), _currentHand.InitialData.Thumb);
                    break;
                case SubPose.PrimaryButton:
                    handPose.Thumb.PrimaryButton = DummyToFingerPose(_currentHand.GetBoneTransform(HandBones.ThumbProximal), _currentHand.InitialData.Thumb);
                    break;
                case SubPose.SecondaryTouched:
                    handPose.Thumb.SecondaryTouched = DummyToFingerPose(_currentHand.GetBoneTransform(HandBones.ThumbProximal), _currentHand.InitialData.Thumb);
                    break;
                case SubPose.SecondaryButton:
                    handPose.Thumb.SecondaryButton = DummyToFingerPose(_currentHand.GetBoneTransform(HandBones.ThumbProximal), _currentHand.InitialData.Thumb);
                    break;
                case SubPose.ThumbstickTouched:
                    handPose.Thumb.ThumbstickTouched = DummyToFingerPose(_currentHand.GetBoneTransform(HandBones.ThumbProximal), _currentHand.InitialData.Thumb);
                    break;
            }

            return handPose;
        }

        private FingerPose DummyToFingerPose(Transform rootTransform, Quaternion[] initialFingerData)
        {
            FingerPose fingerPose = new FingerPose();

            Transform root = rootTransform;
            Transform middle = root.GetChild(0);
            Transform tip = middle.GetChild(0);

            fingerPose.RootBone = Quaternion.Inverse(initialFingerData[0]) * root.localRotation;
            fingerPose.MiddleBone = Quaternion.Inverse(initialFingerData[1]) * middle.localRotation;
            fingerPose.TipBone = Quaternion.Inverse(initialFingerData[2]) * tip.localRotation;

            if (_currentHand == _leftHand)
                fingerPose = fingerPose.Mirrored();

            return fingerPose;
        }

        private void OnDestroy()
        {
            //Remove the dummy hand
            DestroyImmediate(_dummyHand);
            DestroyImmediate(_mirror);
            _currentAsset = null;
        }

        private void MirrorGrab(Grab snapGrab)
        {
            //Position
            Plane plane = new Plane(-_mirror.transform.right, _mirror.transform.position);
            var mirrorPoint = plane.ClosestPointOnPlane(snapGrab.transform.position);
            snapGrab.transform.position = Vector3.LerpUnclamped(snapGrab.transform.position, mirrorPoint, 2f);

            //Rotation
            Vector3 mirroredForwardVector = Vector3.Reflect(snapGrab.transform.forward, plane.normal);
            Vector3 mirroredUpVector = Vector3.Reflect(snapGrab.transform.up, plane.normal);
            snapGrab.transform.rotation = Quaternion.LookRotation(mirroredForwardVector, mirroredUpVector);

            //Flipping grab
            if (snapGrab.IsLeftHanded)
            {
                snapGrab.IsLeftHanded = false;
                snapGrab.IsRightHanded = true;
            }
            else if (snapGrab.IsRightHanded)
            {
                snapGrab.IsRightHanded = false;
                snapGrab.IsLeftHanded = true;
            }
        }
    }
    
    public struct HandData
    {
        public Quaternion[]
            Thumb,
            Index,
            Middle,
            Ring,
            Little;

        public HandData(int param)
        {
            Thumb = new Quaternion[3];
            Index = new Quaternion[3];
            Middle = new Quaternion[3];
            Ring = new Quaternion[3];
            Little = new Quaternion[3];
        }
    }

    class DummyHand
    {
        public HandData InitialData;
        private readonly Dictionary<HandBones, Transform> _handBones = new();

        public DummyHand(Animator animator, bool isLeftHand)
        {
            InitialData = new HandData(1);
            if (isLeftHand)
            {
                _handBones.Add(HandBones.Hand, animator.GetBoneTransform(HumanBodyBones.LeftHand));
                _handBones.Add(HandBones.ThumbProximal, animator.GetBoneTransform(HumanBodyBones.LeftThumbProximal));
                _handBones.Add(HandBones.ThumbIntermediate, animator.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate));
                _handBones.Add(HandBones.ThumbDistal, animator.GetBoneTransform(HumanBodyBones.LeftThumbDistal));
                _handBones.Add(HandBones.IndexProximal, animator.GetBoneTransform(HumanBodyBones.LeftIndexProximal));
                _handBones.Add(HandBones.IndexIntermediate, animator.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate));
                _handBones.Add(HandBones.IndexDistal, animator.GetBoneTransform(HumanBodyBones.LeftIndexDistal));
                _handBones.Add(HandBones.MiddleProximal, animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal));
                _handBones.Add(HandBones.MiddleIntermediate, animator.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate));
                _handBones.Add(HandBones.MiddleDistal, animator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal));
                _handBones.Add(HandBones.RingProximal, animator.GetBoneTransform(HumanBodyBones.LeftRingProximal));
                _handBones.Add(HandBones.RingIntermediate, animator.GetBoneTransform(HumanBodyBones.LeftRingIntermediate));
                _handBones.Add(HandBones.RingDistal, animator.GetBoneTransform(HumanBodyBones.LeftRingDistal));
                _handBones.Add(HandBones.LittleProximal, animator.GetBoneTransform(HumanBodyBones.LeftLittleProximal));
                _handBones.Add(HandBones.LittleIntermediate, animator.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate));
                _handBones.Add(HandBones.LittleDistal, animator.GetBoneTransform(HumanBodyBones.LeftLittleDistal));
            }
            else
            {
                _handBones.Add(HandBones.Hand, animator.GetBoneTransform(HumanBodyBones.RightHand));
                _handBones.Add(HandBones.ThumbProximal, animator.GetBoneTransform(HumanBodyBones.RightThumbProximal));
                _handBones.Add(HandBones.ThumbIntermediate, animator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate));
                _handBones.Add(HandBones.ThumbDistal, animator.GetBoneTransform(HumanBodyBones.RightThumbDistal));
                _handBones.Add(HandBones.IndexProximal, animator.GetBoneTransform(HumanBodyBones.RightIndexProximal));
                _handBones.Add(HandBones.IndexIntermediate, animator.GetBoneTransform(HumanBodyBones.RightIndexIntermediate));
                _handBones.Add(HandBones.IndexDistal, animator.GetBoneTransform(HumanBodyBones.RightIndexDistal));
                _handBones.Add(HandBones.MiddleProximal, animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal));
                _handBones.Add(HandBones.MiddleIntermediate, animator.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate));
                _handBones.Add(HandBones.MiddleDistal, animator.GetBoneTransform(HumanBodyBones.RightMiddleDistal));
                _handBones.Add(HandBones.RingProximal, animator.GetBoneTransform(HumanBodyBones.RightRingProximal));
                _handBones.Add(HandBones.RingIntermediate, animator.GetBoneTransform(HumanBodyBones.RightRingIntermediate));
                _handBones.Add(HandBones.RingDistal, animator.GetBoneTransform(HumanBodyBones.RightRingDistal));
                _handBones.Add(HandBones.LittleProximal, animator.GetBoneTransform(HumanBodyBones.RightLittleProximal));
                _handBones.Add(HandBones.LittleIntermediate, animator.GetBoneTransform(HumanBodyBones.RightLittleIntermediate));
                _handBones.Add(HandBones.LittleDistal, animator.GetBoneTransform(HumanBodyBones.RightLittleDistal));
            }
        }

        public Transform GetBoneTransform(HandBones bone)
        {
            return _handBones[bone];
        }
    }

    public enum HandBones
    {
        Hand,
        ThumbProximal, ThumbIntermediate, ThumbDistal,
        IndexProximal, IndexIntermediate, IndexDistal,
        MiddleProximal, MiddleIntermediate, MiddleDistal,
        RingProximal, RingIntermediate, RingDistal,
        LittleProximal, LittleIntermediate, LittleDistal,
    }
}