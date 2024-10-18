using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private Player player;
    private float footStepsTimer;
    private float foorStepsTimerMax = .1f;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        footStepsTimer -= Time.deltaTime;
        if (footStepsTimer < 0f)
        {
            footStepsTimer = foorStepsTimerMax;

            if(player.IsWalking())
            {
                float volume = 1f;
                SoundManager.Instance.PlayerFootSepsSound(player.transform.position, volume);

            }
        }
    }

}
