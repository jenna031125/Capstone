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
    public Sprite modernSprite;                 // Her normal clothes
    public Sprite maidSprite;                   // Her work clothes

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // Hide the blank screen at the start just in case it was left on
        if (blackScreen != null) blackScreen.SetActive(false);
        if (greyScreen != null) greyScreen.SetActive(false);

        dialogue.StartDialogue("Start");
        dialogue.onDialogueComplete = dialogueCompleteEvent;
    }

    public void DialogueComplete()
    {
        Debug.Log("Finished");
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
    public void ChangeClothes(string outfitName)
    {
        if (outfitName == "Maid")
        {
            playerSpriteRenderer.sprite = maidSprite;
            Debug.Log("Changed into Maid Costume");
        }
        else if (outfitName == "Modern")
        {
            playerSpriteRenderer.sprite = modernSprite;
            Debug.Log("Changed into Modern Clothes");
        }
        else
        {
            Debug.LogWarning("Outfit not found! Check your spelling in Yarn.");
        }
    }


}
