// Copyright© 2016-2016 Chad C. Yates (cyates@dynfxdigital.com)

#define XACT_SIMPLE

#region Using

using System;
using System.Collections.Generic;
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

    private SoundEffect whiteNoiseSoundEffect;

    private readonly AudioListener listener = new AudioListener();
    private readonly AudioEmitter emitter = new AudioEmitter();
    Vector3 previousEmitterPosition = new Vector3();

    private SpriteFont debugFont;
    private SpriteBatch spriteBatch;

    private KeyboardState keyboardState;
    private KeyboardState previousKeyboardState;


    private List<Sound> cueInfos;
    private bool autoMovement = true;
    private float manualXPos = 0;

    public Game1()
    {
      Content.RootDirectory = "Content";

      graphics = new GraphicsDeviceManager(this);


#if !XACT_SIMPLE
      audio = new AudioEngine(Path.Combine(Content.RootDirectory, "XactTestsSimple.xgs"));
      waveBank = new WaveBank(audio, Path.Combine(Content.RootDirectory, "Wave Bank 2.xwb"));
      soundBank = new SoundBank(audio, Path.Combine(Content.RootDirectory, "Sound Bank 2.xsb"));

      cueInfos = new List<XactSound>
      {
        new XactSound("Cue 1", false, Keys.A)
      };
#else
      audio =
        new AudioEngine(Path.Combine(Content.RootDirectory, "XactTests.xgs"));
      waveBank = new WaveBank(audio,
        Path.Combine(Content.RootDirectory, "Wave Bank 1.xwb"));
      soundBank = new SoundBank(audio,
        Path.Combine(Content.RootDirectory, "Sound Bank 1.xsb"));
#endif
    }

    /// <summary>
    ///   Allows the game to perform any initialization it needs to before starting to run. This is where it can query for any
    ///   required services and load any non-graphic related content.  Calling base.Initialize will enumerate through any
    ///   components and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      base.Initialize();
    }

    /// <summary>
    ///   LoadContent will be called once per game and is the place to load all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      whiteNoiseSoundEffect = Content.Load<SoundEffect>("WhiteNoise");

      // Create a new SpriteBatch, which can be used to draw textures.
      spriteBatch = new SpriteBatch(GraphicsDevice);
      debugFont = Content.Load<SpriteFont>("DebugFont");

      cueInfos = new List<Sound>
      {
        new XactSound(soundBank, "Cue 1", false, Keys.A),
        new XactSound(soundBank, "Cue 2", false, Keys.S),
        new XactSound(soundBank, "Cue 3", false, Keys.D),
        new XactSound(soundBank, "Cue 4", false, Keys.F),
        new XactSound(soundBank, "Cue 5", false, Keys.G),
        new XactSound(soundBank, "Cue 6", false, Keys.H),
        new XactSound(soundBank, "Cue 7", false, Keys.J),
        new XactSound(soundBank, "Cue 8 (Doppler)", true, Keys.K),
        new XactSound(soundBank, "Cue 9 (Wind)", false, Keys.L),
        new XactSound(soundBank, "Cue 10 (Pitch)", false, Keys.Z),
        new XnaSound(whiteNoiseSoundEffect, "Cue 10 (Pitch)", true, false, Keys.X)
      };
    }

    /// <summary>
    ///   UnloadContent will be called once per game and is the place to unload all content.
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

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
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
          || IsKeyPressed(Keys.Escape))
        Exit();

      // Handle input.
      {
        keyboardState = Keyboard.GetState();

        for (int i = 0; i < cueInfos.Count; ++i)
        {
          Sound cueSound = cueInfos[i];

          // Start and stop sounds.
          Keys soundEffectKey = cueSound.Key;
          if (!cueSound.IsActive)
          {
            if (IsKeyPressed(soundEffectKey))
            {
              cueSound.Play(listener, emitter);
            }
          }
          else
          {
            if (cueSound.IsPlaying)
            {
              if (IsKeyPressed(soundEffectKey))
              {
                cueSound.Stop(AudioStopOptions.AsAuthored);
              }
            }
            else if (cueSound.IsStopped)
            {
              cueSound.Dispose();
            }
          }
        }

        // Handle listener/emitter updates.
        {
          listener.Position = new Vector3(0, 0, 0);
          listener.Velocity = new Vector3(0, 0, 0);

          // Toggle automatic/manual emitter movement.
          if (IsKeyPressed(Keys.T))
          {
            autoMovement = !autoMovement;
            if (!autoMovement)
            {
              manualXPos = 0;
              emitter.Position = new Vector3(manualXPos, emitter.Position.Y,
                emitter.Position.Z);
              emitter.Velocity = Vector3.Zero;
            }
          }

          if (!autoMovement)
          {
            float direction = IsKeyPressed(Keys.Left)
              ? -1.0f
              : IsKeyPressed(Keys.Right) ? 1.0f : 0.0f;

            if (Math.Abs(direction) > float.Epsilon)
            {
              manualXPos += direction*25;
              emitter.Position = new Vector3(manualXPos, emitter.Position.Y,
                emitter.Position.Z);
              emitter.Velocity = new Vector3(direction*2, emitter.Velocity.Y,
                emitter.Velocity.Z);
            }
          }

          previousKeyboardState = keyboardState;
        }

        const float maxDistance = 125;
        float periodicMovement =
          (float) Math.Sin(gameTime.TotalGameTime.TotalSeconds)*maxDistance;
        float xPos = autoMovement ? periodicMovement : manualXPos;

        if (autoMovement)
        {
          emitter.Position = new Vector3(xPos, 0, 10);
          emitter.Velocity =
            new Vector3(emitter.Position.X - previousEmitterPosition.X, 0, 0);
        }
        emitter.DopplerScale = 20;

        previousEmitterPosition = emitter.Position;
      }

      // Update sounds.
      {
        for (int i = 0; i < cueInfos.Count; i++)
        {
          Sound cueSound = cueInfos[i];

          if (cueSound.IsActive && cueSound.Positional)
          {
            cueSound.Apply3D(listener, emitter);
          }
        }

        audio.Update();
      }

      base.Update(gameTime);
    }

    /// <summary>This is called when the game should draw itself.</summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      spriteBatch.Begin();

      Color clearColor = Color.Black;
      GraphicsDevice.Clear(clearColor);

      Vector2 position = new Vector2(0, 0);

      AudioCategory category = audio.GetCategory("Music");
      category.SetVolume(1);

      spriteBatch.DrawString(debugFont,
        string.Format("emitterPosition={0} emitterVelocity={1}",
          emitter.Position.X, emitter.Velocity.X), position, Color.White);
      position.Y = position.Y + debugFont.LineSpacing;
      position.Y = position.Y + debugFont.LineSpacing;

      for (int i = 0; i < cueInfos.Count; i++)
      {
        Sound cueSound = cueInfos[i];

        cueSound.DrawCueInfo(spriteBatch, debugFont, ref position);
        position.Y = position.Y + debugFont.LineSpacing;
      }

      spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}