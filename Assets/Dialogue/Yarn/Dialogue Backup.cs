/*
 * ===
title: Office_Desk
-- -
<<if $is_after_shift == false >>
    Player: Should I start working?
    -> Yes
        Player: ......(Sounds of typing and organizing paperwork...)
        Player: My work is finished!
        <<set $is_after_shift = true>>
    -> No
        Player: I'll finish this up later.
<<else>>
    Player: I've already finished my shift for today. I should look around or head to bed.
<<endif>>
*/