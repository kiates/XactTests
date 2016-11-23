using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace XactTests
{
  /// <summary>
  /// This is the main type for your game
  /// </summary>
  public class Game1 : Microsoft.Xna.Framework.Game
  {
    GraphicsDeviceManager graphics;
    AudioEngine audio;
    SpriteBatch spriteBatch;
    private WaveBank waveBank;
    private SoundBank soundBank;

    private KeyboardState keyboardState;
    private KeyboardState previousKeyboardState;

    public Game1()
    {
      Content.RootDirectory = "Content";

      graphics = new GraphicsDeviceManager(this);

      audio = new AudioEngine(Path.Combine(Content.RootDirectory ,"XactTests.xgs"));
      waveBank = new WaveBank(audio, Path.Combine(Content.RootDirectory, "Wave Bank 1.xwb"));
      soundBank = new SoundBank(audio, Path.Combine(Content.RootDirectory, "Sound Bank 1.xsb"));
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      // TODO: Add your initialization logic here

      base.Initialize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      spriteBatch = new SpriteBatch(GraphicsDevice);

      // TODO: use this.Content to load your game content here
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// all content.
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
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      // Allows the game to exit
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
        this.Exit();

      // Handle input.
      {
        keyboardState = Keyboard.GetState();

        var startKey = Keys.S;
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

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      // TODO: Add your drawing code here

      base.Draw(gameTime);
    }
  }
}
