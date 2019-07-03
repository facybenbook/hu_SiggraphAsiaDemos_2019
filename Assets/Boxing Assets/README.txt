************************************
*     Boxing Game Starter Kit      *
************************************

Hi, Thanks for purchasing Boxing Game Starter Kit.
This package is ready for use. No setup is needed.

Scenes
======

There are 3 scenes located in scene folder.
- Intro: Is the main menu scene.
- Scene: Is the fighting scene.
- Results: Is the final scene when someone wins.

All of this scenes are ready to play.

Prefabs
=======

The most important prefabs, located in Prefabs folder, are:
- LevelManager prefab: This prefab manage all the Boxing game. Rounds per Game, Time per Rounds, Winners.
- Player prefab: This prefab is the player character, fully scripted, ready to use.
- PlayerEnemyIA prefab: This prefab is the PC character, fully scripted, ready to use.

You can change the exposed variables in this prefabs as you like.

If you want to setup a new Project from 0, you need to do the follow:
- Import this package.
- Create an empty game object. GameObject -> Create Empty. Name it as LevelManager.
- Asign the LevelManager script located in Scripts folder to this game object.
- Drag and Drop Player and PlayerEnemyIA to the scene.
- In Player Inspector View -> PlayerStatus, asign the Enemy (the PlayerEnemyIA).
- In PlayerEnemyIA Inspector View -> AiScript, asign the Enemy (the Player).
- In LevelManager Inspector View -> LevelManager, asign the Player and Enemy game objects.

- Optionaly, tweak all the exposed variables as you like.


FINAL
=====

If you have any question, contact me at esteban.traina@atomwork.net

Thanks you!
Esteban Trania
AtomWork Game Studio