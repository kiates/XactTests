// Copyright© 2016-2016 Chad C. Yates (cyates@dynfxdigital.com)

//#define XACT_SIMPLE

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
    private readonly WaveBank waveBank1;
    private readonly SoundBank soundBank1;
#if !XACT_SIMPLE
    private readonly WaveBank waveBank2;
    private readonly SoundBank soundBank2;
#endif
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

      graphics = new GraphicsDeviceManager(this)
      {
        PreferredBackBufferWidth = 1280,
        PreferredBackBufferHeight = 720
      };

#if XACT_SIMPLE
      audio = new AudioEngine(Path.Combine(Content.RootDirectory, "XactTestsSimple.xgs"));
      waveBank1 = new WaveBank(audio, Path.Combine(Content.RootDirectory, "Wave Bank 3.xwb"));
      soundBank1 = new SoundBank(audio, Path.Combine(Content.RootDirectory, "Sound Bank 3.xsb"));
#else
      audio =
        new AudioEngine(Path.Combine(Content.RootDirectory, "XactTests.xgs"));

      // In-memory wave bank.
      waveBank1 = new WaveBank(audio,
        Path.Combine(Content.RootDirectory, "Wave Bank 1.xwb"));
      soundBank1 = new SoundBank(audio,
        Path.Combine(Content.RootDirectory, "Sound Bank 1.xsb"));

      // Streaming wave bank.
      waveBank2 = new WaveBank(audio,
        Path.Combine(Content.RootDirectory, "Wave Bank 2.xwb"), 0, 4);
      soundBank2 = new SoundBank(audio,
        Path.Combine(Content.RootDirectory, "Sound Bank 2.xsb"));
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

#if XACT_SIMPLE
      cueInfos = new List<Sound>
      {
        new XactSound(soundBank1, "Cue 1", false, Keys.A)
      };
#else
      cueInfos = new List<Sound>
      {
        new XactSound(soundBank1, "Cue 1", Keys.A),
        new XactSound(soundBank1, "Cue 2", Keys.S),
        new XactSound(soundBank1, "Cue 2b", Keys.D),
        new XactSound(soundBank1, "Cue 4", Keys.F),
        new XactSound(soundBank1, "Cue 5", Keys.G),
        new XactSound(soundBank1, "Cue 6", Keys.H),
        new XactSound(soundBank1, "Cue 7", Keys.J),
        new XactSound(soundBank1, "Cue 8 (Doppler)", Keys.K),
        new XactSound(soundBank1, "Cue 9 (Wind)", Keys.L),
        new XactSound(soundBank1, "Cue 10 (Pitch)", Keys.Z),
        new XactSound(soundBank1, "Cue 11 (Volume)", Keys.X),
        new XnaSound(whiteNoiseSoundEffect, "Cue 10 (Pitch)", true, Keys.C),
        new XactSound(soundBank2, "Music Cue 1", Keys.Q),
      };
#endif
    }

    /// <summary>
    ///   UnloadContent will be called once per game and is the place to unload all content.
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    public bool IsKeyPressed(Keys key, Keys modifierKey)
    {
      return (keyboardState.IsKeyDown(modifierKey) || (modifierKey == Keys.None && NoModifiersPresseed())) 
        && keyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
    }

    public bool NoModifiersPresseed()
    {
      return keyboardState.IsKeyUp(Keys.LeftShift)
             && keyboardState.IsKeyUp(Keys.LeftControl)
             && keyboardState.IsKeyUp(Keys.LeftAlt)
             && keyboardState.IsKeyUp(Keys.RightShift)
             && keyboardState.IsKeyUp(Keys.RightControl)
             && keyboardState.IsKeyUp(Keys.RightAlt);
    }

    public bool IsKeyPressed(Keys key)
    {
      return keyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
    }

    public bool IsKeyReleased(Keys key)
    {
      return keyboardState.IsKeyUp(key) && previousKeyboardState.IsKeyDown(key);
    }

    public bool IsKeyDown(Keys key)
    {
      return keyboardState.IsKeyDown(key);
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

          Keys soundEffectKey = cueSound.Key;

          // Prepare, start, and stop sounds.
          if (!cueSound.IsActive || cueSound.IsPrepared)
          {
            // If the sound isn't active or if it is only prepared.

            // If the prepare modifier is used, then just get the cue in preparation to play, otherwise play the sound.
            if (IsKeyPressed(soundEffectKey, Keys.LeftShift))
            {
              if (cueSound is XactSound)
              {
                XactSound xactSound = cueSound as XactSound;
                xactSound.GetCue();
              }
            }
            else if (IsKeyPressed(soundEffectKey))
            {
              bool positional = IsKeyDown(Sound.PositionalModifierKey);
              cueSound.Play(listener, emitter, positional);
            }
          }
          else
          {
            // The sound is playing.

            if (IsKeyPressed(soundEffectKey, Keys.LeftShift))
            {
              cueSound.Paused = !cueSound.Paused;
            }
            else if (IsKeyPressed(soundEffectKey))
            {
              if (cueSound.IsPlaying)
              {
                cueSound.Stop(AudioStopOptions.AsAuthored);
              }
              else if (cueSound.IsStopped)
              {
                cueSound.Dispose();
              }
              else if (cueSound.IsDisposed)
              {
                cueSound.Deactivate();
              }
            }
          }
        }

        // Handle listener/emitter updates.
        {
          listener.Position = new Vector3(0, 0, 0);
          listener.Velocity = new Vector3(0, 0, 0);

          // Toggle automatic/manual emitter movement.
          if (IsKeyPressed(Keys.M))
          {
            autoMovement = !autoMovement;
            if (!autoMovement)
            {
              manualXPos = 0;
              emitter.Position = new Vector3(manualXPos, 0, 0);
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
        position.Y = position.Y + debugFont.LineSpacing*.25f;
      }

      spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}