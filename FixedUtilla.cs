using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaTag.Rendering;
using HarmonyLib;
using UnityEngine;
using Utilla.HarmonyPatches;

namespace Utilla.HarmonyPatches
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Awake", MethodType.Normal)]
    internal static class PostInitializedPatch
    {
        private static void Postfix(Player __instance)
        {
            __instance.StartCoroutine(PostInitializedPatch.DelayCoroutine());
        }

        private static IEnumerator DelayCoroutine()
        {
            yield return 0;
            PostInitializedPatch.events.TriggerGameInitialized();
            yield break;
        }

        public static Events events;
    }
}

namespace Utilla
{
    [BepInPlugin("org.legoandmars.gorillatag.utilla", "Utilla", "1.6.7")]
    public class Utilla : BaseUnityPlugin
    {
        private static Events events = new Events();
        private void Awake()
        {
            Harmony a = new Harmony("org.legoandmars.gorillatag.utilla");
            a.PatchAll();
            PostInitializedPatch.events = Utilla.events;
        }
    }

    public class Events
    {
        public static event EventHandler<Events.RoomJoinedArgs> RoomJoined;

        public static event EventHandler<Events.RoomJoinedArgs> RoomLeft;

        public static event EventHandler GameInitialized;

        public virtual void TriggerRoomJoin(Events.RoomJoinedArgs e)
        {
            EventHandler<Events.RoomJoinedArgs> roomJoined = Events.RoomJoined;
            if (roomJoined == null)
            {
                return;
            }
            roomJoined.SafeInvoke(this, e);
        }

        public virtual void TriggerRoomLeft(Events.RoomJoinedArgs e)
        {
            EventHandler<Events.RoomJoinedArgs> roomLeft = Events.RoomLeft;
            if (roomLeft == null)
            {
                return;
            }
            roomLeft.SafeInvoke(this, e);
        }

        public virtual void TriggerGameInitialized()
        {
            EventHandler gameInitialized = Events.GameInitialized;
            if (gameInitialized == null)
            {
                return;
            }
            gameInitialized.SafeInvoke(this, EventArgs.Empty);
        }

        public class RoomJoinedArgs : EventArgs
        {
            public bool isPrivate { get; set; }
            public string Gamemode { get; set; }
        }
    }

    public static class EventHandlerExtensions
    {
        public static void SafeInvoke(this EventHandler handler, object sender, EventArgs e)
        {
            foreach (EventHandler eventHandler in (handler != null) ? handler.GetInvocationList() : null)
            {
                try
                {
                    if (eventHandler != null)
                    {
                        eventHandler(sender, e);
                    }
                }
                catch (Exception message)
                {
                    Debug.LogError(message);
                }
            }
        }

        public static void SafeInvoke<T>(this EventHandler<T> handler, object sender, T e) where T : EventArgs
        {
            foreach (EventHandler<T> eventHandler in (handler != null) ? handler.GetInvocationList() : null)
            {
                try
                {
                    if (eventHandler != null)
                    {
                        eventHandler(sender, e);
                    }
                }
                catch (Exception message)
                {
                    Debug.LogError(message);
                }
            }
        }
    }
}
