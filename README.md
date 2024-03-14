# HIGHLANDER

**There can be only one**

## Rules: 

World: Board (X x Y)

The game works on a turn-based system.

The game starts with a board and a series of characters on it, in random positions.

Number of characters: 12

On each turn, the characters make a random move to any of the 8 adjacent squares.

Whenever they meet, they fight.

Only one can remain - The loser dies decapitated.

*The characters have the following characteristics:*

> Age
> - Increases by 1 per turn
> 
> Health
> - Increases by 1 per turn
> - Increases by 2 for each combat (if you win)
> 
> Strength
> - Increases by 1 per turn
> - Adds the defeated-in-combat opponent's strength to itself
 
The characteristics are randomly assigned when the character appears on the board, i.e., at the beginning of the game.
 
The duels are between two characters. If three characters coincide, one of the three is excluded from the fight.

Ways to calculate who wins (choose one for the Kata)
- The fight is 90% health and strength (3 health points, 1 strength point) and 10% luck.
- The fight is one-third of current health + strength + 10% luck

## RESTRICTIONS for second/third iterations of the KATA:

1. Sanctuary squares (holy ground) where no fights occur - they appear randomly at the beginning of the game.

2. At the beginning of the game, not all characters appear on the board, only some, and in each turn a new immortal may or may not appear on the board (until reaching 12).

3. Each square where a fight has occurred becomes a sanctuary square.
