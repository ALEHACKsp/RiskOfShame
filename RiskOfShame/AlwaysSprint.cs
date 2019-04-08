﻿using UnityEngine;

namespace RiskOfShame
{
    public class AlwaysSprint : MonoBehaviour
    {
        private void Update()
        {
            var localUser = RoR2.LocalUserManager.GetFirstLocalUser();
            var controller = RoR2.PlayerCharacterMasterController.instances[0];
            var body = controller.GetField<RoR2.CharacterBody>("body");
            if (body && !body.isSprinting && !localUser.inputPlayer.GetButton("Sprint"))
                controller.SetField("sprintInputPressReceived", true);
        }
    }
}
