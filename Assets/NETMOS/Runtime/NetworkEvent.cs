using Mirror;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Netmos
{
    public class NetworkEvent : NetworkBehaviour
    {
        [SerializeField]
        protected MonoBehaviour script;
        [SerializeField]
        private List<UnityEvent> unityEvents;

        void Start()
        {
            if (!script) return;
            unityEvents.Clear();
            Type exampleType = script.GetType();
            FieldInfo[] foundFields = exampleType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default);

            foreach (var field in foundFields)
            {
                if (field.FieldType == typeof(UnityEvent))
                {
                    UnityEvent unityEvent = (UnityEvent)field.GetValue(script);
                    unityEvents.Add(unityEvent);
                    unityEvent.AddListener(call: () =>
                    {
                        CallEvent(unityEvents.IndexOf(unityEvent));
                    });
                }
            }
        }

        public void CallEvent(int index)
        {
            CallEventCmd(index, NetworkServer.localConnection);
        }

        [NetCmd(requiresAuthority = false)]
        private void CallEventCmd(int index, NetworkConnectionToClient conn)
        {
            netIdentity.RemoveClientAuthority();
            netIdentity.AssignClientAuthority(conn);
            CallEventClientRpc(index);
        }

        [NetClientRpc(includeOwner = false)]
        private void CallEventClientRpc(int index)
        {
            if (!authority)
            {
                //Removes the Listener to prevent Stack Overflow https://en.wikipedia.org/wiki/Stack_overflow
                unityEvents[index].RemoveListener(() =>
                        {
                            CallEvent(index);
                        });
                unityEvents[index].Invoke();

                //Brings back the Listener
                unityEvents[index].AddListener(() =>
                        {
                            CallEvent(index);
                        });
            }
        }
    }
}