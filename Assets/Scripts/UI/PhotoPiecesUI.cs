using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class PhotoPiecesUI : MonoBehaviour
{
    [Header("Piece Images")]
    [SerializeField] private Image _piece1Image;
    [SerializeField] private Image _piece2Image;
    [SerializeField] private Image _piece3Image;

    private InMemoryVariableStorage _variableStorage;

    void Awake()
    {
        _variableStorage = FindFirstObjectByType<InMemoryVariableStorage>();
    }

    void OnEnable()
    {
        UpdatePhotoPieces();
    }

    public void UpdatePhotoPieces()
    {
        if (_variableStorage == null) return;

        // Check Piece 1
        if (_piece1Image != null)
        {
            _variableStorage.TryGetValue("$has_photo_piece_1", out bool hasPiece1);
            _piece1Image.gameObject.SetActive(hasPiece1);
        }

        // Check Piece 2 (Carpet Minigame)
        if (_piece2Image != null)
        {
            _variableStorage.TryGetValue("$has_photo_piece_2", out bool hasPiece2);
            _piece2Image.gameObject.SetActive(hasPiece2);
        }

        // Check Piece 3
        if (_piece3Image != null)
        {
            _variableStorage.TryGetValue("$has_photo_piece_3", out bool hasPiece3);
            _piece3Image.gameObject.SetActive(hasPiece3);
        }
    }
}