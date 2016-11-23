// Copyright© 2016-2016 Chad C. Yates (cyates@dynfxdigital.com)

#region Using

using System.Diagnostics.Contracts;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace XactTests
{
  /// <summary>This is the main type for your game</summary>
  public class Game1 : Game
  {
    private GraphicsDeviceManager graphics;
    private readonly AudioEngine audio;
    private readonly WaveBank waveBank;
    private readonly SoundBank soundBank;

    private SpriteFont debugFont;
    private SpriteBatch spriteBatch;

    private KeyboardState keyboardState;
    private KeyboardState previousKeyboardState;

    public Game1()
    {
      Content.RootDirectory = "Content";

      graphics = new GraphicsDeviceManager(this);

      audio = new AudioEngine(Path.Combine(Content.RootDirectory, "XactTests.xgs"));
      waveBank = new WaveBank(audio, Path.Combine(Content.RootDirectory, "Wave Bank 1.xwb"));
      soundBank = new SoundBank(audio, Path.Combine(Content.RootDirectory, "Sound Bank 1.xsb"));
    }

    /// <summary>
    ///   Allows the game to perform any initialization it needs to before starting to run. This is where it can query for any
    ///   required services and load any non-graphic related content.  Calling base.Initialize will enumerate through any
    ///   components and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      // TODO: Add your initialization logic here

      base.Initialize();
    }

    /// <summary>
    ///   LoadContent will be called once per game and is the place to load all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      spriteBatch = new SpriteBatch(GraphicsDevice);
      debugFont = Content.Load<SpriteFont>("DebugFont");

      // TODO: use this.Content to load your game content here
    }

    /// <summary>
    ///   UnloadContent will be called once per game and is the place to unload all content.
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    private Cue cue;

    public bool IsKeyPressed(Keys key)
    {
      return keyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
    }

    public bool IsKeyReleased(Keys key)
    {
      return keyboardState.IsKeyUp(key) && previousKeyboardState.IsKeyDown(key);
    }

    /// <summary>
    ///   Allows the game to run logic such as updating the world, checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      // Allows the game to exit
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
        Exit();

      // Handle input.
      {
        keyboardState = Keyboard.GetState();

        Keys startKey = Keys.S;
        if (cue == null)
        {
          if (IsKeyPressed(startKey))
          {
            cue = soundBank.GetCue("Cue 1");
            cue.Play();
          }
        }
        else
        {
          if (IsKeyReleased(startKey))
          {
            cue.Stop(AudioStopOptions.AsAuthored);
          }

          if (cue.IsStopped)
          {
            cue.Dispose();
            cue = null;
          }
        }

        previousKeyboardState = keyboardState;
      }

      // TODO: Add your update logic here
      audio.Update();

      base.Update(gameTime);
    }

    /// <summary>This is called when the game should draw itself.</summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      float numCueInstances = 0;
      float attackTime = 0;
      float releaseTime = 0;
      float dopplerPitchScalar = 0;
      float orientationAngle = 0;
      float distance = 0;

      Color clearColor;
      if (cue != null)
      {
        numCueInstances = cue.GetVariable("NumCueInstances");
        attackTime = cue.GetVariable("AttackTime");
        releaseTime = cue.GetVariable("ReleaseTime");
        dopplerPitchScalar = cue.GetVariable("DopplerPitchScalar");
        orientationAngle = cue.GetVariable("OrientationAngle");
        distance = cue.GetVariable("Distance");

        cue.SetVariable("DopplerPitchScalar", 2);

        if (cue.IsPlaying)
        {
          clearColor = Color.Gray;
        }
        else if (cue.IsStopping)
        {
          clearColor = Color.LightGray;
        }
        else
        {
          clearColor = Color.Blue;
        }
      }
      else
      {
        clearColor = Color.Black;
      }

      GraphicsDevice.Clear(clearColor);

      // TODO: Add your drawing code here

      spriteBatch.Begin();

      spriteBatch.DrawString(debugFont, string.Format("NumCueInstances={0}", numCueInstances), new Vector2(0, 0), Color.White);
      spriteBatch.DrawString(debugFont, string.Format("AttackTime={0}", attackTime), new Vector2(0, 10), Color.White);
      spriteBatch.DrawString(debugFont, string.Format("ReleaseTime={0}", releaseTime), new Vector2(0, 20), Color.White);
      spriteBatch.DrawString(debugFont, string.Format("DopplerPitchScalar={0}", dopplerPitchScalar), new Vector2(0, 30), Color.White);
      spriteBatch.DrawString(debugFont, string.Format("OrientationAngle={0}", orientationAngle), new Vector2(0, 40), Color.White);
      spriteBatch.DrawString(debugFont, string.Format("Distance={0}", distance), new Vector2(0, 50), Color.White);

      spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}