using BIMOS;
using Epic.OnlineServices.Lobby;
using Mirror;
using System.Collections;
using UnityEngine;

namespace  Netmos
{
    [System.Serializable]
    public struct PlayerInfo
    {
        public NetworkConnectionToClient connection;
        public Transform head;
        public Transform leftHand;
        public Transform rightHand;

        public string Nickname;
        public Color Color;
    }

    public class NetworkManager : Mirror.NetworkManager
    {
        [HideInInspector]
        private PlayerInfo _localPlayer;

        public new static NetworkManager singleton;
        public static PlayerInfo LocalPlayer;

        private static bool gameStart = false;

        public static NetworkManager Netmos;

        private IEnumerator Start()
        {
            singleton = this;
            
            _localPlayer.Nickname = "Player" + Random.Range(1111, 9999);
            _localPlayer.Color = Random.ColorHSV(0,1,1,1,1,1,1,1);

            LocalPlayer = singleton._localPlayer;
            lobby = FindAnyObjectByType<EOSLobby>();
            yield return new WaitForSeconds(0.5f);
            if (!gameStart) {
                gameStart = true;
                CreateLobby();
            }
        }

        private ControllerRig controllerRig;
        private PhysicsRig physicsRig;

        public override void Update() {
            if (!controllerRig) {
                controllerRig = FindAnyObjectByType<ControllerRig>();
                physicsRig = FindAnyObjectByType<PhysicsRig>();
            }
            else {
                LocalPlayer.head = controllerRig.CameraTransform;
                LocalPlayer.leftHand = physicsRig.LeftHandRigidbody.transform;
                LocalPlayer.rightHand = physicsRig.RightHandRigidbody.transform;
            }
            if (!lobby)
                lobby = FindAnyObjectByType<EOSLobby>();
        }

        public static void ChangeMaxConnections(int maxConnections) {
            singleton.maxConnections = maxConnections;
        }
        public static EOSLobby lobby;
        public static void CreateLobby() => CreateLobby(true);
        public static void CreateLobby(bool publicLobby) {
            LobbyPermissionLevel permission = publicLobby ? LobbyPermissionLevel.Publicadvertised : LobbyPermissionLevel.Inviteonly;
            lobby.CreateLobby((uint)singleton.maxConnections, permission, publicLobby);
            singleton.networkAddress = Random.Range(111111, 999999).ToString();
            singleton.StartHost();
            print(lobby.ConnectedLobbyDetails);
        }

        public static void JoinLobby(string address) {
            lobby.JoinLobbyByID(address);
            singleton.StartClient();
        }

        public static void JoinRandomLobby() {
            lobby.FindLobbies();
            lobby.FindLobbiesSucceeded += Lobby_FindLobbiesSucceeded;
            singleton.StartClient();
        }

        private static void Lobby_FindLobbiesSucceeded(System.Collections.Generic.List<LobbyDetails> foundLobbies) {
            lobby.JoinLobby(foundLobbies[0]);
        }

        private void OnGUI() {
            if (!Debug.isDebugBuild)
                return;
            if (GUILayout.Button("Create Lobby"))
                CreateLobby();
            if (GUILayout.Button("Join Random Lobby"))
                JoinRandomLobby();
        }
    }
}

