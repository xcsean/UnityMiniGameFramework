using System;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class TotalManager : MonoBehaviour
    {
        private ClientSocket _clientSocket;
        private void Awake()
        {
            _clientSocket = ClientSocket.GetInstance();
            _clientSocket.Init();
        }

        private void Update()
        {
            _clientSocket?.Run();
        }
    }
}