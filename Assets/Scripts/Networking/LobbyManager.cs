﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Assets.Scripts.Networking;
using Facepunch.Steamworks;
using SteamNetworking.GUI;
using SteamNetworking.Avatar;
using UnityEngine.SceneManagement;

namespace SteamNetworking 
{
    public class LobbyManager : MonoBehaviour
    {
        public GameObject friendAvatarPrefab;
        public GameObject lobbyAvatarPrefab;

        public Transform layoutLobby;
        public Transform layoutFriends;

        public UnityEngine.UI.Text textLobby;
        public UnityEngine.UI.Text textFriends;
        public UnityEngine.UI.Button readyButton;

        [Header("Change this to your development scene(s)")]
        public string serverSceneName = "Server";
        public string clientSceneName = "Client";

        private Dictionary<ulong, LobbyAvatar> lobbyAvatars = new Dictionary<ulong, LobbyAvatar>();

        private ulong lobbyIDToJoin;
        private bool gameStarted = false;
        private bool ready = false;
        private void OnLobbyStateChanged(Lobby.MemberStateChange stateChange, ulong steamID, ulong affectedSteamID)
        {
            if (stateChange == Lobby.MemberStateChange.Entered)
            {
                lobbyAvatars[steamID] = InstantiateLobbyAvatar(Client.Instance.Friends.Get(steamID));
            }
            else
            {
                Destroy(lobbyAvatars[steamID].gameObject);
                lobbyAvatars.Remove(steamID);
            }
        }
        private void OnLobbyMemberDataUpdated(ulong steamID)
        {
            lobbyAvatars[steamID].Refresh();
            //CheckForEveryoneReady();
        }
        private void OnMessageLobbyStartGame(byte[] data, ulong steamID)
        {
            LoadingScreen.Instantiate();

            SceneManager.LoadScene(clientSceneName);

            if (Client.Instance.Lobby.Owner == Client.Instance.SteamId)
            {
                SceneManager.LoadScene(serverSceneName, LoadSceneMode.Additive);
            }
        }

        private void OnLobbyCreatedOrJoined(bool success)
        {
            if (success && Client.Instance.Lobby.IsValid)
            {
                Debug.Log("Created/joined lobby \"" + Client.Instance.Lobby.Name + "\"");
                InitializeLobby();
            }
            else
            {
                Debug.LogError("Failed to create/join lobby. Trying to recreate the default lobby...");
                CreateDefaultLobby();
            }
        }
        private void OnUserInvitedToLobby(ulong lobbyID, ulong otherUserID)
        {
            Debug.Log("Got invitation to the lobby " + lobbyID + " from user " + otherUserID);
            lobbyIDToJoin = lobbyID;
            string message = "Player " + Client.Instance.Friends.Get(otherUserID).Name + " invited you to a lobby.\nDo you want to join?";
            DialogBox.Show(message, true, true, AcceptLobbyInvitation, null);
        }

        private void CreateDefaultLobby()
        {
            Client.Instance.Lobby.Create(Lobby.Type.FriendsOnly, 4);
            Client.Instance.Lobby.Name = Client.Instance.Username + "'s Lobby";
        }

        private void AcceptLobbyInvitation()
        {
            Client.Instance.Lobby.Leave();
            Client.Instance.Lobby.Join(lobbyIDToJoin);
        }
        private void InitializeLobby()
        {
            foreach (LobbyAvatar lobbyAvatar in lobbyAvatars.Values)
            {
                Destroy(lobbyAvatar.gameObject);
            }

            lobbyAvatars.Clear();

            ulong[] lobbyMemberIDs = Client.Instance.Lobby.GetMemberIDs();

            foreach (ulong steamID in lobbyMemberIDs)
            {
                lobbyAvatars[steamID] = InstantiateLobbyAvatar(Client.Instance.Friends.Get(steamID));
                lobbyAvatars[steamID].Refresh();
            }

            textLobby.text = Client.Instance.Lobby.Name;
            Client.Instance.Lobby.SetMemberData("Ready", ready.ToString());
        }
        private void CheckForEveryoneReady()
        {
            if (!gameStarted && Client.Instance.Lobby.NumMembers > 0)
            {
                if (Client.Instance.SteamId == Client.Instance.Lobby.Owner)
                {
                    bool everyoneReady = true;

                    foreach (LobbyAvatar lobbyAvatar in lobbyAvatars.Values)
                    {
                        if (!lobbyAvatar.ready)
                        {
                            everyoneReady = false;
                            break;
                        }
                    }

                    if (everyoneReady)
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes("LobbyStartGame");
                        NetworkManager.Instance.SendToAllClients(data, NetworkMessageType.StartGame, SendType.Reliable);
                        gameStarted = true;
                    }
                }
            }
        }

        public void Ready()
        {
            ready = !ready;
            Client.Instance.Lobby.SetMemberData("Ready", ready.ToString());
            readyButton.GetComponent<UnityEngine.UI.Image>().color = ready ? new UnityEngine.Color(0, 0.25f, 0) : new UnityEngine.Color(0.25f, 0, 0);
            readyButton.GetComponentInChildren<UnityEngine.UI.Text>().text = ready ? "Ready" : "Click to Ready up";
        }

        private IEnumerator RefreshFriendAvatars()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.25f);

                FriendAvatar[] friendAvatars = layoutFriends.GetComponentsInChildren<FriendAvatar>();

                Dictionary<ulong, bool> friendsToStay = new Dictionary<ulong, bool>();

                foreach (FriendAvatar f in friendAvatars)
                {
                    friendsToStay[f.steamID] = false;
                }

                Client.Instance.Friends.Refresh();
                IEnumerable<SteamFriend> friends = Client.Instance.Friends.All;

                foreach (SteamFriend friend in friends)
                {
                    if (friend.IsOnline)
                    {
                        if (!friendsToStay.ContainsKey(friend.Id))
                        {
                            InstantiateFriendAvatar(friend);
                        }

                        friendsToStay[friend.Id] = true;
                    }
                }

                foreach (FriendAvatar f in friendAvatars)
                {
                    if (!friendsToStay[f.steamID])
                    {
                        Destroy(f.gameObject);
                    }
                }
            }
        }
        private LobbyAvatar InstantiateLobbyAvatar(SteamFriend friend)
        {
            LobbyAvatar tmp = Instantiate(lobbyAvatarPrefab, layoutLobby, false).GetComponent<LobbyAvatar>();
            tmp.gameObject.name = friend.Name;
            tmp.steamID = friend.Id;
            return tmp;
        }
        private FriendAvatar InstantiateFriendAvatar(SteamFriend friend)
        {
            FriendAvatar tmp = Instantiate(friendAvatarPrefab, layoutFriends, false).GetComponent<FriendAvatar>();
            tmp.gameObject.name = friend.Name;
            tmp.steamID = friend.Id;
            return tmp;
        }
        private void OnDestroy()
        {
            if (Client.Instance != null)
            {
                Client.Instance.Lobby.OnLobbyCreated -= OnLobbyCreatedOrJoined;
                Client.Instance.Lobby.OnLobbyJoined -= OnLobbyCreatedOrJoined;
                Client.Instance.Lobby.OnUserInvitedToLobby -= OnUserInvitedToLobby;
                Client.Instance.Lobby.OnLobbyMemberDataUpdated -= OnLobbyMemberDataUpdated;
                Client.Instance.Lobby.OnLobbyStateChanged -= OnLobbyStateChanged;

                NetworkManager.Instance.clientMessageEvents[NetworkMessageType.StartGame] -= OnMessageLobbyStartGame;
            }
        }

    } 
}
