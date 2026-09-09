using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Yarn.Unity;

public class PlayerDialogue : MonoBehaviour
{

    public DialogueRunner dialogue;
    public UnityEvent dialogueCompleteEvent;

    public GameObject blackScreen;
    public GameObject greyScreen;

    // --- NEW: Sprite change variables ---
    public SpriteRenderer playerSpriteRenderer; // The component that draws the player
    public GameObject modernSprite;                 // Her normal clothes
    public GameObject maidSprite;                   // Her work clothes

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // Hide the blank screen at the start just in case it was left on
        if (blackScreen != null) blackScreen.SetActive(false);
        if (greyScreen != null) greyScreen.SetActive(false);

        dialogue.StartDialogue("Start");
        dialogue.onDialogueComplete = dialogueCompleteEvent;
    }


    [YarnCommand("play_animation")]

    public IEnumerator PlayAnimation(string animationName)
    {
        // (Your existing animation code stays exactly the same here)
        if (animationName == "Black") { if (blackScreen != null) blackScreen.SetActive(true); }
        else if (animationName == "Grey") { if (greyScreen != null) greyScreen.SetActive(true); }

        yield return new WaitForSeconds(3f);

        if (blackScreen != null) blackScreen.SetActive(false);
        if (greyScreen != null) greyScreen.SetActive(false);
    }

    // --- NEW: Yarn Command to change clothes ---
    [YarnCommand("change_clothes")]
    public static void ChangeClothes(string outfitName)
    {
        PlayerDialogue player = FindFirstObjectByType<PlayerDialogue>();
        if (player == null) return;

        if (outfitName == "Maid")
        {
            player.maidSprite.SetActive(true);
            player.modernSprite.SetActive(false);
            Debug.Log("Changed into Maid Costume");
        }
        else if (outfitName == "Modern")
        {
            player.maidSprite.SetActive(false);
            player.modernSprite.SetActive(true);
            Debug.Log("Changed into Modern Clothes");
        }
        else
        {
            Debug.LogWarning("Outfit not found! Check your spelling in Yarn.");
        }
    }

}
