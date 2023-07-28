using UnityEngine;
using System.Collections;

public class PlayerAnimations : MonoBehaviour 
{
    public Animator animator;       //Player's Animator component;
    private Player player;  

    void Start()
    {
        //Caching player controls script;
        player = GetComponent<Player>();
    }

	void Update () 
    {
        //Setting Animator parameters;
        animator.SetBool("Jump", PlayerInput.Jump);
        animator.SetBool("Grounded", player.IsGrounded());
        animator.SetBool("Dead", player.IsDead());
	}
}
