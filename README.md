Millionaire-3D
==============

An attempt to create a 100% replica of a favorite TV show "Who Wants To Be a Millionaire" using Unity 3D.

All fans are welcome to contributing - either by writing code, designing graphics replicas, creating your questions, providing voiceovers for characters. 


What is done so far
-------------------

* Created question bar (logenze) for classic Millionaire format using new unity UI (you must have Unity 4.6).

![Main scene](https://raw.githubusercontent.com/ua2004/Millionaire-3D/master/Extras/screen1.png)

* The source logenze image Assets/Graphics/Logenze/Classic.png is Full-HD and is divided into 9 sprites:
  1. Question
  2. Left answer (either A or C) active (with an orange diamond)
  3. Right answer (either B or D) active (with an orange diamond)
  4. Left answer inactive (when the answer is removed by a 50x50 lifeline)
  5. Right answer inactive (when the answer is removed by a 50x50 lifeline)
  6. Left answer final (orange background)
  7. Right answer final (orange background)
  8. Left answer correct (green background)
  9. Right answer correct (green background)

![Logense](https://raw.githubusercontent.com/ua2004/Millionaire-3D/master/Assets/Graphics/Logenze/Classic.png)


What is planned
---------------
* Classic game format (15 questions, 3 lifelines, no clock), as appeared in 1998.
* Super Millionaire.
* Clock format (US and UK).
* Hot Seat format.
* All other formats that ever appeared on TV (if possible).

As you can see, there is a lot of work and we need your help.


Author's notes
-------------------
I have a job so I can work on this project only in my free time. Considering this and the fact that I'm lazy, there is no specific date for this game release. Please don't ask about it. Instead, fork and help it appear faster faster by creating pull requests if you want.

Unity 3D was chosen due to its cross-platform features. You will need Unity 4.6 or higher because earlier versions don't have the new UI support.
