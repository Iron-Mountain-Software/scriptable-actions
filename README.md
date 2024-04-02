# Scriptable Actions
*Version: 1.0.6*
## Description: 
Scriptable object actions that can be stored and invoked.
## Package Mirrors: 
[<img src='https://img.itch.zone/aW1nLzEzNzQ2ODk4LnBuZw==/original/Rv4m96.png'>](https://iron-mountain.itch.io/scriptable-actions)[<img src='https://img.itch.zone/aW1nLzEzNzQ2ODkyLnBuZw==/original/Fq0ORM.png'>](https://www.npmjs.com/package/com.iron-mountain.scriptable-actions)[<img src='https://img.itch.zone/aW1nLzEzNzQ2ODg3LnBuZw==/original/npRUfq.png'>](https://github.com/Iron-Mountain-Software/scriptable-actions)
---
## Key Scripts & Components: 
1. public class **DebugLogAction** : ScriptableAction
   * Methods: 
      * public override void ***Invoke***()
      * public override String ***ToString***()
      * public override Boolean ***HasErrors***()
1. public class **LoadSceneByIndexAction** : ScriptableAction
   * Methods: 
      * public override void ***Invoke***()
      * public override String ***ToString***()
      * public override Boolean ***HasErrors***()
1. public class **LoadSceneByNameAction** : ScriptableAction
   * Methods: 
      * public override void ***Invoke***()
      * public override String ***ToString***()
      * public override Boolean ***HasErrors***()
1. public abstract class **ScriptableAction** : ScriptableObject
   * Methods: 
      * public abstract void ***Invoke***()
      * public abstract String ***ToString***()
      * public abstract Boolean ***HasErrors***()
1. public class **UnityEventAction** : ScriptableAction
   * Methods: 
      * public override void ***Invoke***()
      * public override String ***ToString***()
      * public override Boolean ***HasErrors***()
