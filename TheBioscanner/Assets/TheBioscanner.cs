using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class TheBioscanner : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] Buttons;
    public KMSelectable StartButton;

    public GameObject[] Glyphs;
    public GameObject StartButtonObject;
    public GameObject MovingAll;

    public Material ColorForSwitching;

    public Renderer[] ButtonOutlinesButColors;
    public Renderer Space;

    public TextMesh NumbersOfShifting;
    public TextMesh ProtocolText;

    public Sprite[] TheGlyphsButSprites;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    int[] GlyphNumbers = new int[3];
    int[] GlyphRandomized = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
    int[] GlyphNumbersRandomized = new int[52];
    int EdgeworkBullshit;
    int GlyphOffset;

    float RandomXOffset;
    float RandomYOffset;
    float XOffset;
    float YOffset;
    float TheTimerForThis = 30f;

    string[][] NatoIGuess = new string[17][] {
      new string[1] {"Alpha "},
      new string[2] {"Bravo ", "Beta "},
      new string[1] {"Charlie "},
      new string[1] {"Delta "},
      new string[2] {"Echo ", "Epsilon "},
      new string[1] {"Hotel "},
      new string[1] {"Lima "},
      new string[1] {"Nancy "},
      new string[1] {"Oscar "},
      new string[1] {"Quartz "},
      new string[1] {"Romeo "},
      new string[1] {"Sierra "},
      new string[1] {"Tango "},
      new string[1] {"Umbrella "},
      new string[1] {"X-Ray "},
      new string[1] {"Yankee "},
      new string[1] {"Zephyr "}
    };
    string[] NumbersForProtocol = {"Zero" ,"One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine"};
    string Protocol = "Initiating security Protocol:\n";
    string SN;
    string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    bool[] Pressed = new bool[10];
    bool Started;
    bool Checking;
    bool Struck;
    #pragma warning disable 0649
    bool TwitchPlaysActive;
    #pragma warning restore 0649

    Vector3 FUCKINGMOVE = new Vector3(0f, 1f, 0f);

    TheBioscannerSettings Settings = new TheBioscannerSettings();
#pragma warning disable 414
    private static Dictionary<string, object>[] TweaksEditorSettings = new Dictionary<string, object>[]
    {
      new Dictionary<string, object>
      {
        { "Filename", "TheBioscannerSettings.json"},
        { "Name", "The Bioscanner" },
        { "Listings", new List<Dictionary<string, object>>
        {
          new Dictionary<string, object>
          {
            { "Key", "Timer" },
            { "Text", "Edit how long the timer runs (in seconds)."}
          }
        }}
      }
    };
#pragma warning restore 414
    void Awake () {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable Button in Buttons) {
            KMSelectable Button2 = Button; // The person who made c# is a fucking retard
            Button.OnInteract += delegate () { ButtonPress(Button2); return false; };
        }
        StartButton.OnInteract += delegate () {StartPress(); return false;};
        ModConfig<TheBioscannerSettings> modConfig = new ModConfig<TheBioscannerSettings>("TheBioscannerSettings");
        Settings = modConfig.Read();
        // If there are any isues, write the default settings
        modConfig.Write(Settings);
    }

    void Start () {
      TheTimerForThis = Settings.Timer;
      MovingAll.gameObject.SetActive(false);
      StartButtonObject.gameObject.SetActive(true);
      RandomXOffset = UnityEngine.Random.Range(0f, 2f);
      RandomYOffset = UnityEngine.Random.Range(0f, 2f);
      SN = Bomb.GetSerialNumber();
      switch (UnityEngine.Random.Range(2,4)) {
        case 2:
        int weed = 0;
        weed = UnityEngine.Random.Range(0, NatoIGuess.Length);
        Protocol += NatoIGuess[weed][UnityEngine.Random.Range(0, NatoIGuess[weed].Length)];
        Protocol += NumbersForProtocol[UnityEngine.Random.Range(0, 10)];
        break;
        case 3:
        int ASSWIPE = 0;
        ASSWIPE = UnityEngine.Random.Range(0, NatoIGuess.Length);
        Protocol += NatoIGuess[ASSWIPE][UnityEngine.Random.Range(0, NatoIGuess[ASSWIPE].Length)];
        ASSWIPE = UnityEngine.Random.Range(0, NatoIGuess.Length);
        Protocol += NatoIGuess[ASSWIPE][UnityEngine.Random.Range(0, NatoIGuess[ASSWIPE].Length)];
        Protocol += "\n" + NumbersForProtocol[UnityEngine.Random.Range(0, 10)];
        break;
      }
      ProtocolText.text = Protocol;
      for (int i = 0; i < 6; i++)
        for (int j = 0; j < Alphabet.Length; j++)
          if (SN[i] == Alphabet[j]) {
            GlyphNumbers[0] += j % 5;
            goto BreakFromLoop;
          }
      BreakFromLoop:
      EdgeworkBullshit = ((Bomb.GetSerialNumberNumbers().ToArray()[0]) * (Bomb.GetBatteryCount() + Bomb.GetPortCount())) % 11;
      GlyphNumbers[0] += EdgeworkBullshit * 5;
      for (int i = 0; i < 6; i++)
        for (int j = 0; j < Alphabet.Length; j++)
          if (SN[4].ToString() == Alphabet[j].ToString()) {
            GlyphNumbers[1] += j % 5;
            goto SecondBreakFromLoop;
          }
      SecondBreakFromLoop:
      EdgeworkBullshit = ((Bomb.GetSerialNumberNumbers().ToArray().Last()) * (Bomb.GetIndicators().Count())) % 11;
      GlyphNumbers[1] += EdgeworkBullshit * 5;
      if (Bomb.IsPortPresent(Port.StereoRCA))
        GlyphNumbers[2] += Bomb.GetPortCount(Port.StereoRCA);
      else if (Bomb.IsPortPresent(Port.Serial))
        GlyphNumbers[2] += Bomb.GetPortCount(Port.Serial);
      else if (Bomb.IsPortPresent(Port.RJ45))
        GlyphNumbers[2] += Bomb.GetPortCount(Port.RJ45);
      else if (Bomb.IsPortPresent(Port.PS2))
        GlyphNumbers[2] += Bomb.GetPortCount(Port.PS2);
      else if (Bomb.IsPortPresent(Port.Parallel))
        GlyphNumbers[2] += Bomb.GetPortCount(Port.Parallel);
      else if (Bomb.IsPortPresent(Port.DVI))
        GlyphNumbers[2] += Bomb.GetPortCount(Port.DVI);
      EdgeworkBullshit = Bomb.GetBatteryCount() % 11;
      GlyphNumbers[2] += EdgeworkBullshit * 5;
      Restart:
      if (GlyphNumbers[0] == GlyphNumbers[1]) {
        GlyphNumbers[1] += 5;
        GlyphNumbers[1] %= 55;
      }
      if (GlyphNumbers[1] == GlyphNumbers[2]) {
        GlyphNumbers[2] += 5;
        GlyphNumbers[2] %= 55;
      }
      if (GlyphNumbers[0] == GlyphNumbers[2]) {
        GlyphNumbers[2] += 5;
        GlyphNumbers[2] %= 55;
        goto Restart;
      }
      if (RandomXOffset == RandomYOffset)
        RandomXOffset += .1f;
      StartCoroutine(MovingSpace());
      Debug.LogFormat("[The Bioscanner #{0}] The first coordinate is {1}{2}.", moduleId, Alphabet[GlyphNumbers[0] % 5], (GlyphNumbers[0] / 5) + 1);
      Debug.LogFormat("[The Bioscanner #{0}] The second coordinate is {1}{2}.", moduleId, Alphabet[GlyphNumbers[1] % 5], (GlyphNumbers[1] / 5) + 1);
      Debug.LogFormat("[The Bioscanner #{0}] The third coordinate is {1}{2}.", moduleId, Alphabet[GlyphNumbers[2] % 5], (GlyphNumbers[2] / 5) + 1);
    }

    void StartPress () {
      if (TwitchPlaysActive)
        TheTimerForThis = Settings.Timer + 15f;
      else
        TheTimerForThis = Settings.Timer;
      Started = true;
      StartButtonObject.gameObject.SetActive(false);
      StartButton.transform.localPosition -= FUCKINGMOVE;
      MovingAll.gameObject.SetActive(true);
      GenerateGlyphs();
    }

    void Update () {
      if (Started) {
        TheTimerForThis -= Time.deltaTime;
        if (TheTimerForThis < 0) {
          StartButtonObject.gameObject.SetActive(true);
          MovingAll.gameObject.SetActive(false);
          if (TwitchPlaysActive)
            TheTimerForThis = Settings.Timer + 15f;
          else
            TheTimerForThis = Settings.Timer;
          StartButton.transform.localPosition += FUCKINGMOVE;
          if (!Struck)
            GetComponent<KMBombModule>().HandleStrike();
          Struck = false;
          for (int shitknocker = 0; shitknocker < 10; shitknocker++) {
            Pressed[shitknocker] = false;
            ButtonOutlinesButColors[shitknocker].material.color = new Color32(255, 0, 0, 255);
          }
          Started = false;
        }
      }
    }

    void ButtonPress (KMSelectable Button) {
      if (Checking || !Started)
        return;
      if (moduleSolved) {
        for (int i = 0; i < Buttons.Length; i++) {
          if (Button == Buttons[i]) {
            Audio.PlaySoundAtTransform("FuckYouNumbers", Buttons[i].transform);
            StartCoroutine(ColorChanger(i, Pressed[i]));
            Pressed[i] = !Pressed[i];
          }
        }
      }
      else {
        for (int i = 0; i < Buttons.Length; i++) {
          if (Button == Buttons[i]) {
            Audio.PlaySoundAtTransform("FuckYouNumbers", Buttons[i].transform);
            StartCoroutine(ColorChanger(i, Pressed[i]));
            Pressed[i] = !Pressed[i];
            int NumberOfPressed = 0;
            for (int j = 0; j < 10; j++) {
              if (Pressed[j]) {
                NumberOfPressed++;
              }
            }
            if (NumberOfPressed == 3)
              StartCoroutine(Waiting());
            }
          }
      }
      }

    IEnumerator Waiting () {
      Checking = true;
      TheTimerForThis += 3f;
      yield return new WaitForSecondsRealtime(3f);
      if (Pressed[GlyphRandomized[0]] && Pressed[GlyphRandomized[1]] && Pressed[GlyphRandomized[2]]) {
        GetComponent<KMBombModule>().HandlePass();
        Started = false;
        moduleSolved = true;
      }
      else {
        GetComponent<KMBombModule>().HandleStrike();
        Struck = true;
        for (int shitknocker = 0; shitknocker < 10; shitknocker++) {
          Pressed[shitknocker] = false;
          ButtonOutlinesButColors[shitknocker].material.color = new Color32(255, 0, 0, 255);
        }
    }
    Checking = false;
  }

    IEnumerator ColorChanger (int Weed, bool PressedOfI) {
      if (!PressedOfI) {
        byte Red = 255;
        byte Green = 0;
        for (int j = 0; j < 51; j++) {
          Red -= 5;
          Green += 5;
          ButtonOutlinesButColors[Weed].material.color = new Color32(Red, Green, 0, 255);
          yield return new WaitForSecondsRealtime(.005f);
        }
      }
      else {
        byte Red = 0;
        byte Green = 255;
        for (int j = 0; j < 51; j++) {
          Red += 5;
          Green -= 5;
          ButtonOutlinesButColors[Weed].material.color = new Color32(Red, Green, 0, 255);
          yield return new WaitForSecondsRealtime(.005f);
        }
      }
    }

    IEnumerator MovingSpace () {
      bool AddOrSubtract = false;
      if (UnityEngine.Random.Range(0, 2) == 1)
        AddOrSubtract = !AddOrSubtract;
      while (true) {
        if (AddOrSubtract) {
          XOffset += RandomXOffset / 100;
          YOffset += RandomYOffset / 100;
        }
        else {
          XOffset -= RandomXOffset / 100;
          YOffset -= RandomYOffset / 100;
        }
        Space.material.SetTextureOffset("_MainTex", new Vector2(XOffset, YOffset));
        yield return new WaitForSeconds(.01f);
      }
    }

    void GenerateGlyphs () {
      GlyphOffset = UnityEngine.Random.Range(0,6);
      NumbersOfShifting.text = GlyphOffset.ToString();
      for (int i = 0; i < 52; i++)
        if ((i + GlyphOffset) % 55 != GlyphNumbers[0] + GlyphOffset % 55 && (i + GlyphOffset) % 55 != GlyphNumbers[1] + GlyphOffset % 55 && (i + GlyphOffset) % 55 != GlyphNumbers[2] + GlyphOffset % 55)
          GlyphNumbersRandomized[i] = (i + GlyphOffset) % 55;
      GlyphNumbersRandomized.Shuffle();
      GlyphRandomized.Shuffle();
      Glyphs[GlyphRandomized[0]].GetComponent<SpriteRenderer>().sprite = TheGlyphsButSprites[(GlyphNumbers[0] + GlyphOffset) % 55];
      Glyphs[GlyphRandomized[1]].GetComponent<SpriteRenderer>().sprite = TheGlyphsButSprites[(GlyphNumbers[1] + GlyphOffset) % 55];
      Glyphs[GlyphRandomized[2]].GetComponent<SpriteRenderer>().sprite = TheGlyphsButSprites[(GlyphNumbers[2] + GlyphOffset) % 55];
      for (int i = 3; i < 10; i++)
        Glyphs[GlyphRandomized[i]].GetComponent<SpriteRenderer>().sprite = TheGlyphsButSprites[GlyphNumbersRandomized[i]];
      Debug.LogFormat("[The Bioscanner #{0}] The current offset is {1}. This means the current coordinates are {2}{3}, {4}{5}, and {6}{7}.", moduleId, GlyphOffset, Alphabet[(((GlyphNumbers[0] + GlyphOffset) % 55)) % 5], ((((GlyphNumbers[0] + GlyphOffset) % 55)) / 5) + 1, Alphabet[((GlyphNumbers[1] + GlyphOffset) % 55) % 5], (((GlyphNumbers[1] + GlyphOffset) % 55) / 5) + 1, Alphabet[((GlyphNumbers[2] + GlyphOffset) % 55) % 5], (((GlyphNumbers[2] + GlyphOffset) % 55) / 5) + 1);
      Debug.LogFormat("[The Bioscanner #{0}] Fake glyphs are:", moduleId);
      for (int i = 3; i < 10; i++)
        Debug.LogFormat("[The Bioscanner #{0}] {1}{2}.", moduleId, Alphabet[GlyphNumbersRandomized[i] % 5], (GlyphNumbersRandomized[i] / 5) + 1);
      Debug.LogFormat("[The Bioscanner #{0}] Press buttons:", moduleId);
      for (int i = 0; i < 3; i++) {
        switch (GlyphRandomized[i]) {
          case 0:
          Debug.LogFormat("[The Bioscanner #{0}] B1.", moduleId);
          break;
          case 1:
          Debug.LogFormat("[The Bioscanner #{0}] B2.", moduleId);
          break;
          case 2:
          Debug.LogFormat("[The Bioscanner #{0}] B3.", moduleId);
          break;
          case 3:
          Debug.LogFormat("[The Bioscanner #{0}] B4.", moduleId);
          break;
          case 4:
          Debug.LogFormat("[The Bioscanner #{0}] A1.", moduleId);
          break;
          case 5:
          Debug.LogFormat("[The Bioscanner #{0}] A2.", moduleId);
          break;
          case 6:
          Debug.LogFormat("[The Bioscanner #{0}] A3.", moduleId);
          break;
          case 7:
          Debug.LogFormat("[The Bioscanner #{0}] C1.", moduleId);
          break;
          case 8:
          Debug.LogFormat("[The Bioscanner #{0}] C2.", moduleId);
          break;
          case 9:
          Debug.LogFormat("[The Bioscanner #{0}] C3.", moduleId);
          break;
        }
      }
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} X# to select a coordinate on the module. Use !{0} start to start the module.";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand (string Command) {
      Command = Command.Trim().ToUpper();
      yield return null;
      if (Command == "START" || Command == "ACTIVATE")
        StartButton.OnInteract();
      else if (Command == "A1")
        Buttons[4].OnInteract();
      else if (Command == "A2")
        Buttons[5].OnInteract();
      else if (Command == "A3")
        Buttons[6].OnInteract();
      else if (Command == "B1")
        Buttons[0].OnInteract();
      else if (Command == "B2")
        Buttons[1].OnInteract();
      else if (Command == "B3")
        Buttons[2].OnInteract();
      else if (Command == "B4")
        Buttons[3].OnInteract();
      else if (Command == "C1")
        Buttons[7].OnInteract();
      else if (Command == "C2")
        Buttons[8].OnInteract();
      else if (Command == "C3")
        Buttons[9].OnInteract();
      else {
        yield return null;
        yield return "sendtochaterror I don't understand!";
        yield break;
      }
    }

    IEnumerator TwitchHandleForcedSolve () {
      if (!Started)
        StartButton.OnInteract();
      TheTimerForThis = 30f;
      for (int i = 0; i < 10; i++)
        if (Pressed[i]) {
          Buttons[i].OnInteract();
          yield return new WaitForSecondsRealtime(.1f);
        }
      Buttons[GlyphRandomized[0]].OnInteract();
      yield return new WaitForSecondsRealtime(.1f);
      Buttons[GlyphRandomized[1]].OnInteract();
      yield return new WaitForSecondsRealtime(.1f);
      Buttons[GlyphRandomized[2]].OnInteract();
      yield return new WaitForSecondsRealtime(.1f);
    }

    class TheBioscannerSettings
    {
      public float Timer = 30f;
    }
}
