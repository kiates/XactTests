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

		private AudioListener listener = new AudioListener();
		private AudioEmitter emitter = new AudioEmitter();
		Vector3 previousEmitterPosition = new Vector3();

    private SpriteFont debugFont;
    private SpriteBatch spriteBatch;

    private KeyboardState keyboardState;
    private KeyboardState previousKeyboardState;

    class CueInfo
    {
      public CueInfo(string name, bool positional, Keys key)
      {
        Name = name;
	      Positional = positional;
        Key = key;
        Cue = null;
      }

      public readonly string Name;
	    public readonly bool Positional;
      public readonly Keys Key;
      public Cue Cue;
    }

    private readonly List<CueInfo> cueInfos;
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

      cueInfos = new List<CueInfo>
      {
        new CueInfo("Cue 1", false, Keys.A)
      };
#else
      audio = new AudioEngine(Path.Combine(Content.RootDirectory, "XactTests.xgs"));
      waveBank = new WaveBank(audio, Path.Combine(Content.RootDirectory, "Wave Bank 1.xwb"));
      soundBank = new SoundBank(audio, Path.Combine(Content.RootDirectory, "Sound Bank 1.xsb"));

      cueInfos = new List<CueInfo>
      {
        new CueInfo("Cue 1", false, Keys.A),
        new CueInfo("Cue 2", false,Keys.S),
        new CueInfo("Cue 3", false,Keys.D),
        new CueInfo("Cue 4", false,Keys.F),
        new CueInfo("Cue 5", false,Keys.G),
        new CueInfo("Cue 6", false,Keys.H),
				new CueInfo("Cue 7", false,Keys.J),
				new CueInfo("Cue 8 (Doppler)", true, Keys.K)
			};
#endif
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

        for (int i = 0; i < cueInfos.Count; ++i)
        {
          CueInfo cueInfo = cueInfos[i];

          Keys soundEffectKey = cueInfo.Key;
          if (cueInfo.Cue == null)
          {
            if (IsKeyPressed(soundEffectKey))
            {
              cueInfo.Cue = soundBank.GetCue(cueInfo.Name);
	            if (cueInfo.Positional)
	            {
		            cueInfo.Cue.Apply3D(listener, emitter);
	            }
	            cueInfo.Cue.Play();
            }
          }
          else
          {
            if (IsKeyReleased(soundEffectKey))
            {
              cueInfo.Cue.Stop(AudioStopOptions.AsAuthored);
            }

            if (cueInfo.Cue.IsStopped)
            {
              cueInfo.Cue.Dispose();
              cueInfo.Cue = null;
            }
          }
        }

        if (IsKeyPressed(Keys.T))
        {
          autoMovement = !autoMovement;
          if (!autoMovement)
          {
            manualXPos = 0;
            emitter.Position = new Vector3(manualXPos, emitter.Position.Y, emitter.Position.Z);
            emitter.Velocity = Vector3.Zero;
          }
        }

        if (!autoMovement)
        {
          float direction = IsKeyPressed(Keys.Left) ? -1.0f : IsKeyPressed(Keys.Right) ? 1.0f : 0.0f;

          if (Math.Abs(direction) > float.Epsilon)
          {
            manualXPos += direction*25;
            emitter.Position = new Vector3(manualXPos, emitter.Position.Y, emitter.Position.Z);
            emitter.Velocity = new Vector3(direction*2, emitter.Velocity.Y, emitter.Velocity.Z);
          }
        }

        previousKeyboardState = keyboardState;
      }

	    listener.Position = new Vector3(0, 0, 0);
			listener.Velocity = new Vector3(0,0,0);

      const float maxDistance = 125;
      float periodicMovement = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds)*maxDistance;
      float xPos = autoMovement ? periodicMovement : manualXPos;

      if (autoMovement)
      {
        emitter.Position = new Vector3(xPos, 0, 10);
        emitter.Velocity = new Vector3(emitter.Position.X - previousEmitterPosition.X, 0, 0);
      }
	    emitter.DopplerScale = 20;

			previousEmitterPosition = emitter.Position;

			for (int i = 0; i < cueInfos.Count; i++)
			{
				CueInfo cueInfo = cueInfos[i];

				if (cueInfo.Cue != null && cueInfo.Positional)
				{
					cueInfo.Cue.Apply3D(listener, emitter);
				}
			}

			// TODO: Add your update logic here
			audio.Update();

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

			spriteBatch.DrawString(debugFont, string.Format("emitterPosition={0} emitterVelocity={1}", emitter.Position.X, emitter.Velocity.X), position, Color.White);
			position.Y = position.Y + lineSpacing;
			position.Y = position.Y + lineSpacing;

			for (int i = 0; i < cueInfos.Count; i++)
      {
        CueInfo cueInfo = cueInfos[i];

        DrawCueInfo(cueInfo, ref position);
        position.Y = position.Y + lineSpacing;
      }

      spriteBatch.End();

      base.Draw(gameTime);
    }
    public const float lineSpacing = 10;

    private void DrawCueInfo(CueInfo cueInfo, ref Vector2 position)
    {
      float numCueInstances = 0;
      float attackTime = 0;
      float releaseTime = 0;
      float dopplerPitchScalar = 0;
      float orientationAngle = 0;
      float distance = 0;

      bool isPlaying = false;
      bool isStopping = false;
      bool isStopped = true;

      Color clearColor = Color.LightGray;
      if (cueInfo.Cue != null)
      {
        numCueInstances = cueInfo.Cue.GetVariable("NumCueInstances");
        attackTime = cueInfo.Cue.GetVariable("AttackTime");
        releaseTime = cueInfo.Cue.GetVariable("ReleaseTime");
        dopplerPitchScalar = cueInfo.Cue.GetVariable("DopplerPitchScalar");
        orientationAngle = cueInfo.Cue.GetVariable("OrientationAngle");
        distance = cueInfo.Cue.GetVariable("Distance");

        isPlaying = cueInfo.Cue.IsPlaying;
        isStopping = cueInfo.Cue.IsStopping;
        isStopped = cueInfo.Cue.IsStopped;

        if (isPlaying)
        {
          clearColor = Color.White;
        }
        else if (isStopping)
        {
          clearColor = Color.LightGray;
        }
        else if (isStopped)
        {
          clearColor = Color.Gray;
        }
      }
      else
      {
        clearColor = Color.DarkGray;
      }

			spriteBatch.DrawString(debugFont, string.Format("NumCueInstances={0}, AttackTime={1}, ReleaseTime={2}, DopplerPitchScalar={3}, OrientationAngle={4}, Distance={5}",
				numCueInstances, attackTime, releaseTime, dopplerPitchScalar, orientationAngle, distance), position, clearColor);
			position.Y = position.Y + lineSpacing;

			/*
      spriteBatch.DrawString(debugFont, string.Format("AttackTime={0}", attackTime), position, clearColor);
      position.Y = position.Y + lineSpacing;

      spriteBatch.DrawString(debugFont, string.Format("ReleaseTime={0}", releaseTime), position, clearColor);
      position.Y = position.Y + lineSpacing;

      spriteBatch.DrawString(debugFont, string.Format("DopplerPitchScalar={0}", dopplerPitchScalar), position,
        clearColor);
      position.Y = position.Y + lineSpacing;

      spriteBatch.DrawString(debugFont, string.Format("OrientationAngle={0}", orientationAngle), position, clearColor);
      position.Y = position.Y + lineSpacing;

      spriteBatch.DrawString(debugFont, string.Format("Distance={0}", distance), position, clearColor);
      position.Y = position.Y + lineSpacing;
			*/

			spriteBatch.DrawString(debugFont,
        string.Format("CueName={3}, IsPlaying={0}, IsStopping={1}, IsStopped={2}", isPlaying, isStopping, isStopped,
          cueInfo.Name), position, clearColor);
      position.Y = position.Y + lineSpacing;
    }
  }
}