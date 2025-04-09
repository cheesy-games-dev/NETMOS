using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations.Rigging;

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
        private Transform _armature, _currentSelection;
        private bool _isLeftHand;

        private HandData _initialHandData = new HandData(1);
        private Animator _animator;
        private bool _isAnimatorInitialized = false;

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
            if (_animator)
            {
                if (!_isAnimatorInitialized)
                {
                    _animator.Update(0f);
                    _isAnimatorInitialized = _initialHandData.Thumb[0] != _animator.GetBoneTransform(HumanBodyBones.RightThumbProximal).localRotation;

                    if (_isAnimatorInitialized)
                    {
                        Transform hips = _animator.GetBoneTransform(HumanBodyBones.Hips);
                        Transform hand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
                        hips.position = _dummyHand.transform.position;
                        hand.rotation = _dummyHand.transform.rotation * Quaternion.Euler(0f, 0f, 0f);
                        hips.position += Vector3.down * 0.09f;

                        _initialHandData.Thumb[0] = _animator.GetBoneTransform(HumanBodyBones.RightThumbProximal).localRotation;
                        _initialHandData.Thumb[1] = _animator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate).localRotation;
                        _initialHandData.Thumb[2] = _animator.GetBoneTransform(HumanBodyBones.RightThumbDistal).localRotation;
                        _initialHandData.Index[0] = _animator.GetBoneTransform(HumanBodyBones.RightIndexProximal).localRotation;
                        _initialHandData.Index[1] = _animator.GetBoneTransform(HumanBodyBones.RightIndexIntermediate).localRotation;
                        _initialHandData.Index[2] = _animator.GetBoneTransform(HumanBodyBones.RightIndexDistal).localRotation;
                        _initialHandData.Middle[0] = _animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal).localRotation;
                        _initialHandData.Middle[1] = _animator.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate).localRotation;
                        _initialHandData.Middle[2] = _animator.GetBoneTransform(HumanBodyBones.RightMiddleDistal).localRotation;
                        _initialHandData.Ring[0] = _animator.GetBoneTransform(HumanBodyBones.RightRingProximal).localRotation;
                        _initialHandData.Ring[1] = _animator.GetBoneTransform(HumanBodyBones.RightRingIntermediate).localRotation;
                        _initialHandData.Ring[2] = _animator.GetBoneTransform(HumanBodyBones.RightRingDistal).localRotation;
                        _initialHandData.Little[0] = _animator.GetBoneTransform(HumanBodyBones.RightLittleProximal).localRotation;
                        _initialHandData.Little[1] = _animator.GetBoneTransform(HumanBodyBones.RightLittleIntermediate).localRotation;
                        _initialHandData.Little[2] = _animator.GetBoneTransform(HumanBodyBones.RightLittleDistal).localRotation;

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
            }

            if (_currentSelection && _dummyHand)
            {
                Transform offset = _animator.GetBoneTransform(HumanBodyBones.RightHand); //_armature.transform.Find("Offset");
                _armature.rotation = _currentSelection.rotation * Quaternion.Inverse(offset.rotation) * _armature.rotation;
                _armature.position += _currentSelection.position - offset.position;
            }
        }

        private void SpawnDummyHand(Vector3 spawnLocation, Quaternion spawnRotation)
        {
            _isAnimatorInitialized = false;
            DestroyImmediate(_dummyHand);
            _dummyHand = Instantiate((GameObject) DummyHandPrefab, spawnLocation, spawnRotation);
            _dummyHand.name = "DummyHand";
            _subPose = SubPose.None;

            _animator = _dummyHand.GetComponent<Animator>();
            _armature = _animator.avatarRoot;
            _animator.runtimeAnimatorController = _controller;

            foreach (SkinnedMeshRenderer renderer in _dummyHand.GetComponentsInChildren<SkinnedMeshRenderer>())
                renderer.updateWhenOffscreen = true;

            _selectedBoneRenderer = _dummyHand.AddComponent<BoneRenderer>();
            _selectedBoneRenderer.boneColor = new Color(1f, 0f, 0f, 0.5f);

            _armature.localScale = 1f / 1000f * Vector3.one;
            _animator.GetBoneTransform(HumanBodyBones.RightHand).localScale = 1000f * Vector3.one;

            _initialHandData.Thumb[0] = _animator.GetBoneTransform(HumanBodyBones.RightThumbProximal).localRotation;
            _initialHandData.Thumb[1] = _animator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate).localRotation;
            _initialHandData.Thumb[2] = _animator.GetBoneTransform(HumanBodyBones.RightThumbDistal).localRotation;
            _initialHandData.Index[0] = _animator.GetBoneTransform(HumanBodyBones.RightIndexProximal).localRotation;
            _initialHandData.Index[1] = _animator.GetBoneTransform(HumanBodyBones.RightIndexIntermediate).localRotation;
            _initialHandData.Index[2] = _animator.GetBoneTransform(HumanBodyBones.RightIndexDistal).localRotation;
            _initialHandData.Middle[0] = _animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal).localRotation;
            _initialHandData.Middle[1] = _animator.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate).localRotation;
            _initialHandData.Middle[2] = _animator.GetBoneTransform(HumanBodyBones.RightMiddleDistal).localRotation;
            _initialHandData.Ring[0] = _animator.GetBoneTransform(HumanBodyBones.RightRingProximal).localRotation;
            _initialHandData.Ring[1] = _animator.GetBoneTransform(HumanBodyBones.RightRingIntermediate).localRotation;
            _initialHandData.Ring[2] = _animator.GetBoneTransform(HumanBodyBones.RightRingDistal).localRotation;
            _initialHandData.Little[0] = _animator.GetBoneTransform(HumanBodyBones.RightLittleProximal).localRotation;
            _initialHandData.Little[1] = _animator.GetBoneTransform(HumanBodyBones.RightLittleIntermediate).localRotation;
            _initialHandData.Little[2] = _animator.GetBoneTransform(HumanBodyBones.RightLittleDistal).localRotation;

            _currentHandPose = CreateInstance<HandPose>();
            _currentHandPose = Instantiate(_currentHandPose);

            _dummyHand.transform.parent = null;
            //if (_currentSelection)
            //{
            //    SnapGrab grab = _currentSelection.GetComponent<SnapGrab>();
            //    if (grab)
            //    {
            //        _isLeftHand = grab.IsLeftHanded && !grab.IsRightHanded;

            //        _dummyHand.transform.localScale = _isLeftHand ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
            //        if (grab.HandPose)
            //        {
            //            _currentHandPose = grab.HandPose;
            //            _currentAsset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(grab.HandPose), typeof(HandPose));
            //            _subPose = SubPose.GripClosed;
            //            HandPoseToDummy(_currentHandPose);
            //            _subPose = SubPose.IndexClosed;
            //            HandPoseToDummy(_currentHandPose);
            //            _subPose = SubPose.ThumbrestTouched;
            //            HandPoseToDummy(_currentHandPose);
            //            _subPose = SubPose.None;
            //            HandPoseToDummy(_currentHandPose);
            //        }
            //    }
            //}

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
                SpawnDummyHand(spawnLocation, spawnRotation);
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
                    SpawnDummyHand(position, rotation); //Respawns at old location
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
                //Updates the hand pose
                _currentHandPose = DummyToHandPose();
                if (!_currentAsset) //If there is no file currently loaded
                {
                    SaveAs();
                }
                else
                {
                    //Overwrite existing pose
                    HandPose handPose = (HandPose)_currentAsset;
                    handPose.Thumb = _currentHandPose.Thumb;
                    handPose.Index = _currentHandPose.Index;
                    handPose.Middle = _currentHandPose.Middle;
                    handPose.Ring = _currentHandPose.Ring;
                    handPose.Little = _currentHandPose.Little;
                    EditorUtility.SetDirty(_currentAsset);
                }
            }
            if (GUILayout.Button("Save as"))
            {
                //Updates the hand pose
                _currentHandPose = DummyToHandPose();

                SaveAs();
            }
            GUILayout.Label("Misc");
            if (_isLeftHand)
            {
                if (GUILayout.Button("Swap to right hand"))
                {
                    _isLeftHand = false;
                    _dummyHand.transform.localScale = new Vector3(1f, 1f, 1f);
                }
            }
            else
            {
                if (GUILayout.Button("Swap to left hand"))
                {
                    _isLeftHand = true;
                    _dummyHand.transform.localScale = new Vector3(-1f, 1f, 1f);
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
            GUILayout.Label("Sub-Poses");
            if (GUILayout.Button("None"))
            {
                _currentHandPose = DummyToHandPose();
                _subPose = SubPose.None;
                HandPoseToDummy(_currentHandPose);
            }
            GUILayout.Label("Little, Ring, Middle");
            GUI.backgroundColor = _subPose == SubPose.GripOpen ? Color.red : Color.white;
            if (GUILayout.Button("Open"))
            {
                _currentHandPose = DummyToHandPose();
                _subPose = SubPose.GripOpen;
                HandPoseToDummy(_currentHandPose);
            }
            GUI.backgroundColor = _subPose == SubPose.GripClosed ? Color.red : Color.white;
            if (GUILayout.Button("Closed"))
            {
                _currentHandPose = DummyToHandPose();
                _subPose = SubPose.GripClosed;
                HandPoseToDummy(_currentHandPose);
            }
            GUILayout.Label("Index");
            GUI.backgroundColor = _subPose == SubPose.IndexOpen ? Color.red : Color.white;
            if (GUILayout.Button("Open"))
            {
                _currentHandPose = DummyToHandPose();
                _subPose = SubPose.IndexOpen;
                HandPoseToDummy(_currentHandPose);
            }
            GUI.backgroundColor = _subPose == SubPose.IndexTriggerTouched ? Color.red : Color.white;
            if (GUILayout.Button("Trigger touched"))
            {
                _currentHandPose = DummyToHandPose();
                _subPose = SubPose.IndexTriggerTouched;
                HandPoseToDummy(_currentHandPose);
            }
            GUI.backgroundColor = _subPose == SubPose.IndexClosed ? Color.red : Color.white;
            if (GUILayout.Button("Closed"))
            {
                _currentHandPose = DummyToHandPose();
                _subPose = SubPose.IndexClosed;
                HandPoseToDummy(_currentHandPose);
            }
            GUILayout.Label("Thumb");
            GUI.backgroundColor = _subPose == SubPose.ThumbOpen ? Color.red : Color.white;
            if (GUILayout.Button("Open"))
            {
                _currentHandPose = DummyToHandPose();
                _subPose = SubPose.ThumbOpen;
                HandPoseToDummy(_currentHandPose);
            }
            GUI.backgroundColor = _subPose == SubPose.ThumbrestTouched ? Color.red : Color.white;
            if (GUILayout.Button("Thumbrest touched"))
            {
                _currentHandPose = DummyToHandPose();
                _subPose = SubPose.ThumbrestTouched;
                HandPoseToDummy(_currentHandPose);
            }
            GUI.backgroundColor = _subPose == SubPose.ThumbstickTouched ? Color.red : Color.white;
            if (GUILayout.Button("Thumbstick touched"))
            {
                _currentHandPose = DummyToHandPose();
                _subPose = SubPose.ThumbstickTouched;
                HandPoseToDummy(_currentHandPose);
            }
            GUI.backgroundColor = _subPose == SubPose.PrimaryTouched ? Color.red : Color.white;
            if (GUILayout.Button("Primary touched"))
            {
                _currentHandPose = DummyToHandPose();
                _subPose = SubPose.PrimaryTouched;
                HandPoseToDummy(_currentHandPose);
            }
            GUI.backgroundColor = _subPose == SubPose.PrimaryButton ? Color.red : Color.white;
            if (GUILayout.Button("Primary button"))
            {
                _currentHandPose = DummyToHandPose();
                _subPose = SubPose.PrimaryButton;
                HandPoseToDummy(_currentHandPose);
            }
            GUI.backgroundColor = _subPose == SubPose.SecondaryTouched ? Color.red : Color.white;
            if (GUILayout.Button("Secondary touched"))
            {
                _currentHandPose = DummyToHandPose();
                _subPose = SubPose.SecondaryTouched;
                HandPoseToDummy(_currentHandPose);
            }
            GUI.backgroundColor = _subPose == SubPose.SecondaryButton ? Color.red : Color.white;
            if (GUILayout.Button("Secondary button"))
            {
                _currentHandPose = DummyToHandPose();
                _subPose = SubPose.SecondaryButton;
                HandPoseToDummy(_currentHandPose);
            }
            GUILayout.EndScrollView();
        }

        private void SaveAs()
        {
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
                _currentAsset = AssetDatabase.LoadAssetAtPath(path, typeof(HandPose));
            }
        }

        private void HandPoseToDummy(HandPose handPose)
        {
            List<Transform> bones = new();
            switch (_subPose)
            {
                case SubPose.GripOpen:
                    FingerPoseToDummy(handPose.Middle.Open, _animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal), _initialHandData.Middle);
                    FingerPoseToDummy(handPose.Ring.Open, _animator.GetBoneTransform(HumanBodyBones.RightRingProximal), _initialHandData.Ring);
                    FingerPoseToDummy(handPose.Little.Open, _animator.GetBoneTransform(HumanBodyBones.RightLittleProximal), _initialHandData.Little);

                    GetBones(_animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal), bones);
                    GetBones(_animator.GetBoneTransform(HumanBodyBones.RightRingProximal), bones);
                    GetBones(_animator.GetBoneTransform(HumanBodyBones.RightLittleProximal), bones);
                    break;
                case SubPose.GripClosed:
                    FingerPoseToDummy(handPose.Middle.Closed, _animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal), _initialHandData.Middle);
                    FingerPoseToDummy(handPose.Ring.Closed, _animator.GetBoneTransform(HumanBodyBones.RightRingProximal), _initialHandData.Ring);
                    FingerPoseToDummy(handPose.Little.Closed, _animator.GetBoneTransform(HumanBodyBones.RightLittleProximal), _initialHandData.Little);

                    GetBones(_animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal), bones);
                    GetBones(_animator.GetBoneTransform(HumanBodyBones.RightRingProximal), bones);
                    GetBones(_animator.GetBoneTransform(HumanBodyBones.RightLittleProximal), bones);
                    break;
                case SubPose.IndexOpen:
                    FingerPoseToDummy(handPose.Index.Open, _animator.GetBoneTransform(HumanBodyBones.RightIndexProximal), _initialHandData.Index);
                    GetBones(_animator.GetBoneTransform(HumanBodyBones.RightIndexProximal), bones);
                    break;
                case SubPose.IndexTriggerTouched:
                    FingerPoseToDummy(handPose.Index.TriggerTouched, _animator.GetBoneTransform(HumanBodyBones.RightIndexProximal), _initialHandData.Index);
                    GetBones(_animator.GetBoneTransform(HumanBodyBones.RightIndexProximal), bones);
                    break;
                case SubPose.IndexClosed:
                    FingerPoseToDummy(handPose.Index.Closed, _animator.GetBoneTransform(HumanBodyBones.RightIndexProximal), _initialHandData.Index);
                    GetBones(_animator.GetBoneTransform(HumanBodyBones.RightIndexProximal), bones);
                    break;
                case SubPose.ThumbOpen:
                    FingerPoseToDummy(handPose.Thumb.Idle, _animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), _initialHandData.Thumb);
                    GetBones(_animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), bones);
                    break;
                case SubPose.ThumbrestTouched:
                    FingerPoseToDummy(handPose.Thumb.ThumbrestTouched, _animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), _initialHandData.Thumb);
                    GetBones(_animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), bones);
                    break;
                case SubPose.PrimaryTouched:
                    FingerPoseToDummy(handPose.Thumb.PrimaryTouched, _animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), _initialHandData.Thumb);
                    GetBones(_animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), bones);
                    break;
                case SubPose.PrimaryButton:
                    FingerPoseToDummy(handPose.Thumb.PrimaryButton, _animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), _initialHandData.Thumb);
                    GetBones(_animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), bones);
                    break;
                case SubPose.SecondaryTouched:
                    FingerPoseToDummy(handPose.Thumb.SecondaryTouched, _animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), _initialHandData.Thumb);
                    GetBones(_animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), bones);
                    break;
                case SubPose.SecondaryButton:
                    FingerPoseToDummy(handPose.Thumb.SecondaryButton, _animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), _initialHandData.Thumb);
                    GetBones(_animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), bones);
                    break;
                case SubPose.ThumbstickTouched:
                    FingerPoseToDummy(handPose.Thumb.ThumbstickTouched, _animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), _initialHandData.Thumb);
                    GetBones(_animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), bones);
                    break;
            }

            _selectedBoneRenderer.transforms = bones.ToArray();
        }

        private void FingerPoseToDummy(FingerPose fingerPose, Transform rootTransform, Quaternion[] initialFingerData)
        {
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
                    handPose.Middle.Open = DummyToFingerPose(_animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal), _initialHandData.Middle);
                    handPose.Ring.Open = DummyToFingerPose(_animator.GetBoneTransform(HumanBodyBones.RightRingProximal), _initialHandData.Ring);
                    handPose.Little.Open = DummyToFingerPose(_animator.GetBoneTransform(HumanBodyBones.RightLittleProximal), _initialHandData.Little);
                    break;
                case SubPose.GripClosed:
                    handPose.Middle.Closed = DummyToFingerPose(_animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal), _initialHandData.Middle);
                    handPose.Ring.Closed = DummyToFingerPose(_animator.GetBoneTransform(HumanBodyBones.RightRingProximal), _initialHandData.Ring);
                    handPose.Little.Closed = DummyToFingerPose(_animator.GetBoneTransform(HumanBodyBones.RightLittleProximal), _initialHandData.Little);
                    break;
                case SubPose.IndexOpen:
                    handPose.Index.Open = DummyToFingerPose(_animator.GetBoneTransform(HumanBodyBones.RightIndexProximal), _initialHandData.Index);
                    break;
                case SubPose.IndexTriggerTouched:
                    handPose.Index.TriggerTouched = DummyToFingerPose(_animator.GetBoneTransform(HumanBodyBones.RightIndexProximal), _initialHandData.Index);
                    break;
                case SubPose.IndexClosed:
                    handPose.Index.Closed = DummyToFingerPose(_animator.GetBoneTransform(HumanBodyBones.RightIndexProximal), _initialHandData.Index);
                    break;
                case SubPose.ThumbOpen:
                    handPose.Thumb.Idle = DummyToFingerPose(_animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), _initialHandData.Thumb);
                    break;
                case SubPose.ThumbrestTouched:
                    handPose.Thumb.ThumbrestTouched = DummyToFingerPose(_animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), _initialHandData.Thumb);
                    break;
                case SubPose.PrimaryTouched:
                    handPose.Thumb.PrimaryTouched = DummyToFingerPose(_animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), _initialHandData.Thumb);
                    break;
                case SubPose.PrimaryButton:
                    handPose.Thumb.PrimaryButton = DummyToFingerPose(_animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), _initialHandData.Thumb);
                    break;
                case SubPose.SecondaryTouched:
                    handPose.Thumb.SecondaryTouched = DummyToFingerPose(_animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), _initialHandData.Thumb);
                    break;
                case SubPose.SecondaryButton:
                    handPose.Thumb.SecondaryButton = DummyToFingerPose(_animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), _initialHandData.Thumb);
                    break;
                case SubPose.ThumbstickTouched:
                    handPose.Thumb.ThumbstickTouched = DummyToFingerPose(_animator.GetBoneTransform(HumanBodyBones.RightThumbProximal), _initialHandData.Thumb);
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
}