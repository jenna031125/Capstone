using UnityEngine;
using Yarn.Unity;

public class OfficeWorldManager : MonoBehaviour
{
    [System.Serializable]
    public class WorldObjectState
    {
        public SpriteRenderer spriteRenderer;
        public Sprite dirtySprite;
        public Sprite cleanSprite;
    }

    [Header("World Objects Setup")]
    public WorldObjectState desk;
    public WorldObjectState flowerPot;
    public WorldObjectState fireplace;
    public WorldObjectState frame;
    public WorldObjectState carpet;
    public WorldObjectState floorLeft;
    public WorldObjectState floorRight;

    private InMemoryVariableStorage _yarnVariables;

    void Awake()
    {
        _yarnVariables = FindFirstObjectByType<InMemoryVariableStorage>();
    }

    void Update()
    {
        // 매 프레임 Yarn 변수 상태를 확인하여 월드 스프라이트를 즉시 업데이트!
        UpdateAllObjectSprites();
    }

    public void UpdateAllObjectSprites()
    {
        if (_yarnVariables == null)
            _yarnVariables = FindFirstObjectByType<InMemoryVariableStorage>();

        if (_yarnVariables == null) return;

        // 1. Yarn 변수 값 읽어오기
        int dayCount = 0;
        if (_yarnVariables.TryGetValue("$day_count", out float dayCountFloat))
        {
            dayCount = Mathf.RoundToInt(dayCountFloat);
        }

        _yarnVariables.TryGetValue("$is_desk_cleaned_day1", out bool desk1);
        _yarnVariables.TryGetValue("$is_desk_cleaned_day3", out bool desk3);
        _yarnVariables.TryGetValue("$is_pot_watered", out bool pot);
        _yarnVariables.TryGetValue("$is_fireplace_cleaned_day2", out bool fire2);
        _yarnVariables.TryGetValue("$is_fireplace_cleaned_day3", out bool fire3);
        _yarnVariables.TryGetValue("$is_frame_cleaned", out bool frameClean);
        _yarnVariables.TryGetValue("$is_carpet_cleaned", out bool carpetClean);
        _yarnVariables.TryGetValue("$is_floor_left_clean", out bool floorL);
        _yarnVariables.TryGetValue("$is_floor_right_clean", out bool floorR);

        // --- 1. 책상 (Day 1, Day 3 청소 대상) ---
        if (desk.spriteRenderer != null)
        {
            if (dayCount == 1)
                desk.spriteRenderer.sprite = desk1 ? desk.cleanSprite : desk.dirtySprite;
            else if (dayCount == 2)
                desk.spriteRenderer.sprite = desk.cleanSprite; // 2일차에는 무조건 깨끗함
            else if (dayCount >= 3)
                desk.spriteRenderer.sprite = desk3 ? desk.cleanSprite : desk.dirtySprite;
            else
                desk.spriteRenderer.sprite = desk.cleanSprite;
        }

        // --- 2. 화분 (Day 1 청소 대상) ---
        if (flowerPot.spriteRenderer != null)
        {
            if (dayCount == 1)
                flowerPot.spriteRenderer.sprite = pot ? flowerPot.cleanSprite : flowerPot.dirtySprite;
            else
                flowerPot.spriteRenderer.sprite = flowerPot.cleanSprite;
        }

        // --- 3. 벽난로 (Day 2, Day 3 청소 대상) ---
        if (fireplace.spriteRenderer != null)
        {
            if (dayCount == 2)
                fireplace.spriteRenderer.sprite = fire2 ? fireplace.cleanSprite : fireplace.dirtySprite;
            else if (dayCount >= 3)
                fireplace.spriteRenderer.sprite = fire3 ? fireplace.cleanSprite : fireplace.dirtySprite;
            else
                fireplace.spriteRenderer.sprite = fireplace.cleanSprite;
        }

        // --- 4. 액자 (Day 2 청소 대상) ---
        if (frame.spriteRenderer != null)
        {
            if (dayCount == 2)
                frame.spriteRenderer.sprite = frameClean ? frame.cleanSprite : frame.dirtySprite;
            else
                frame.spriteRenderer.sprite = frame.cleanSprite;
        }

        // --- 5. 카펫 (Day 2 청소 대상) ---
        if (carpet.spriteRenderer != null)
        {
            if (dayCount == 2)
                carpet.spriteRenderer.sprite = carpetClean ? carpet.cleanSprite : carpet.dirtySprite;
            else
                carpet.spriteRenderer.sprite = carpet.cleanSprite;
        }

        // --- 6. 바닥 먼지 (Left: Day 1 / Right: Day 3) ---
        if (floorLeft.spriteRenderer != null)
        {
            if (dayCount == 1)
            {
                floorLeft.spriteRenderer.sprite = floorL ? floorLeft.cleanSprite : floorLeft.dirtySprite;
            }
            else
            {
                floorLeft.spriteRenderer.sprite = floorLeft.cleanSprite;
            }
        }

        if (floorRight.spriteRenderer != null)
        {
            if (dayCount >= 3)
            {
                floorRight.spriteRenderer.sprite = floorR ? floorRight.cleanSprite : floorRight.dirtySprite;
            }
            else
            {
                floorRight.spriteRenderer.sprite = floorRight.cleanSprite;
            }
        }
    }
}