using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Input : MonoBehaviour
{
    public CharacterNavMeshMovement local_CharacterNavMeshMovement;
    public MouseShooting local_MouseShooting;
    public Pause_Manager local_Pause_manager;
	public CharacterAnimationHandler charAnimHandler;
    public Player_Abilities local_Player_Abilities;
    private Vector2 input;
    private Vector2 input2;
    float curr_Stun_Duration = 0f;
    //set this variable from the movement script or other places when the player cannot move
    public bool can_Move = true;

    //set this variable when input is entirely disabled
    public bool input_Enabled = true;

    private bool rTDown = false;

    // Update is called once per frame
    void Update()
    {
  
        if (input_Enabled)
        {
            Check_For_Input();

            //rotate the character, should only need to be updated whenever there is mouse input
            if (!local_CharacterNavMeshMovement.usingController)
            {
                local_CharacterNavMeshMovement.RotateCharacter();
            }

            if (rTDown)
            {
                if (Input.GetAxis("Fire2") == 0)
                {
                    rTDown = false;
                }
            }
        }

        if (Input.GetButtonDown("Escape"))
        {
            //pause game
            local_Pause_manager.Toggle_Pause();
        }
    }

    void Check_For_Input()
    {
		//waits until the player presses any key so that checks and input axis are not set every frame
		if (Input.anyKey || local_CharacterNavMeshMovement.usingController)
		{
			//check the player is allowed to enter input

			if (can_Move)
			{
				//movement check and set axis
				input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

				//controller right stick axes
				input2 = new Vector2(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"));

				charAnimHandler.UpdateAnimationParameters(input);

				if (input.x != 0 || input.y != 0)
				{
					//move here
					local_CharacterNavMeshMovement.Move(input.x, input.y);
				}

				if (input.x == 0 && input.y == 0)
				{
					//stop moving
					local_CharacterNavMeshMovement.speed = 0;
				}


				if (input2.x != 0 || input2.y != 0)
				{
					//rotating
					local_CharacterNavMeshMovement.RotateCharacter(input2.x, input2.y);
				}

				//dash check
				if (Input.GetButtonDown("Jump"))
				{
					//dash here
					local_CharacterNavMeshMovement.Dash(input.x, input.y);
				}

				//Hoverboard Check
				if (Input.GetButtonDown("Fire3"))
				{
					local_CharacterNavMeshMovement.ToggleHoverboard();
					StartCoroutine(local_CharacterNavMeshMovement.HoverboardToggleCooldown());
				}
			}

			if (Input.GetButtonDown("Fire1"))
			{
				//call parry
				local_MouseShooting.StartParry();
			}

			if (!local_CharacterNavMeshMovement.usingController)
			{
				if (Input.GetButtonDown("Fire2"))
				{
					//call thrower/catch
					local_MouseShooting.ThrowCatch();
				}
			}
			else
			{
				if (Input.GetAxis("Fire2") < 0 && rTDown == false)
				{
					//call thrower/catch
					local_MouseShooting.ThrowCatch();
					rTDown = true;
				}
			}

			if (Input.GetButtonDown("Fire4"))
			{
				//call player ability
				local_Player_Abilities.local_Ability_Type = Player_Abilities.Ability_Type.DISC_EXPLOSION;
				local_Player_Abilities.Cast_Player_Ability();
			}

			if (Input.GetButtonDown("Fire5"))
			{
				//call player ability
				local_Player_Abilities.local_Ability_Type = Player_Abilities.Ability_Type.DISC_STOPPER;
				local_Player_Abilities.Cast_Player_Ability();
			}
		}
		else {
			if (charAnimHandler.CheckParameters() > 0)
			{
				charAnimHandler.ZeroOutParameters();
			}
		}
    }

    public void End_Stun_Public( float t, Enemy_Attack_Crowd_Control enemy_Cc)
    {
        curr_Stun_Duration = t;
        StartCoroutine("End_Stun" , enemy_Cc);
    }

     IEnumerator End_Stun( Enemy_Attack_Crowd_Control enemy_Attack_Crowd_Control)
    {
        yield return new WaitForSeconds(curr_Stun_Duration);
        input_Enabled = true;
        enemy_Attack_Crowd_Control.have_Stunned = false;
    }
}
