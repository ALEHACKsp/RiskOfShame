﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RiskOfShame
{
    public class ItemSpawner : MonoBehaviour
    {
        static Int32 Margin = 5;
        static Int32 ItemWidth = 400;
        Int32 ItemSelectorId;
        Rect ItemSelectorWindow = new Rect(Screen.width - ItemWidth - Margin, Margin, ItemWidth, Screen.height - Margin * 2);
        Vector2 ItemScrollPos;
        Dictionary<String, Int32> nameToIndexMap = new Dictionary<String, Int32>();
        void Start()
        {
            nameToIndexMap = typeof(RoR2.BodyCatalog).GetStaticField<Dictionary<String, Int32>>("nameToIndexMap");
            ItemSelectorId = GetHashCode();
        }
        void OnGUI()
        {
            ItemSelectorWindow = GUILayout.Window(ItemSelectorId, ItemSelectorWindow, ItemSelectorMethod, "Item Select");
        }
        void ItemSelectorMethod(Int32 id)
        {
            ItemScrollPos = GUILayout.BeginScrollView(ItemScrollPos);
            {
                var localUser = RoR2.LocalUserManager.GetFirstLocalUser();
                if (localUser == null || localUser.cachedMasterController == null || localUser.cachedMasterController.master == null) return;
                var master = localUser.cachedMasterController.master;
                var body = master.GetBody();
                for (int i = (int)RoR2.ItemIndex.Syringe; i < (int)RoR2.ItemIndex.Count; i++)
                {
                    var def = RoR2.ItemCatalog.GetItemDef((RoR2.ItemIndex)i);
                    if (GUILayout.Button(RoR2.Language.GetString(def.nameToken) + " : " + (RoR2.ItemIndex)i))
                    {
                        RoR2.PickupDropletController.CreatePickupDroplet(new RoR2.PickupIndex((RoR2.ItemIndex)i), body.transform.position + Vector3.up * 1.5f, Vector3.up * 20f + body.transform.forward * 2f);
                    }
                }
            }
            GUILayout.EndScrollView();
            GUI.DragWindow();
        }
    }
}