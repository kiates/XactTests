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
	public class Sounds
	{
		public Sound SoundPrototype;
		public List<Sound> ActiveSounds = new List<Sound>();

		public Sounds(Sound soundPrototype)
		{
			SoundPrototype = soundPrototype;
		}

		public Sound ActiveSound
		{
			get
			{
				return ActiveSounds.Count > 0 ? ActiveSounds[0] : SoundPrototype;
			}
		}

		public bool Paused
		{
			get
			{
				foreach(var x in ActiveSounds)
				{
					if (x.Paused)
					{
						return true;
					}
				}
				return false;
			}
			set
			{
				foreach (var x in ActiveSounds)
				{
					x.Paused = value;
				}
			}
		}

		public bool IsPlaying
		{
			get
			{
				foreach(var x in ActiveSounds)
				{
					if (x.IsPlaying)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsStopped
		{
			get
			{
				foreach (var x in ActiveSounds)
				{
					if (x.IsStopped)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsDisposed
		{
			get
			{
				foreach (var x in ActiveSounds)
				{
					if (x.IsDisposed)
					{
						return true;
					}
				}
				return false;
			}
		}

		public void Stop(AudioStopOptions audioStopOptions)
		{
			foreach (var x in ActiveSounds)
			{
				x.Stop(audioStopOptions);
			}
		}

		public void Dispose()
		{
			foreach (var x in ActiveSounds)
			{
				x.Dispose();
			}
		}

		public void Deactivate()
		{
			foreach (var x in ActiveSounds)
			{
				x.Deactivate();
			}
			ActiveSounds.Clear();
		}
	}

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

			private List<Sounds> cueInfos;

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
				audio = new AudioEngine(
					Path.Combine(Content.RootDirectory, "XactTests.xgs"));

				// In-memory wave bank.
				waveBank1 = new WaveBank(
					audio,
					Path.Combine(Content.RootDirectory, "Wave Bank 1.xwb"));
				soundBank1 = new SoundBank(
					audio,
					Path.Combine(Content.RootDirectory, "Sound Bank 1.xsb"));

				// Streaming wave bank.
				waveBank2 = new WaveBank(
					audio,
					Path.Combine(Content.RootDirectory, "Wave Bank 2.xwb"),
					0,
					4);
				soundBank2 = new SoundBank(
					audio,
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
			cueInfos = new List<Sounds>
			{
				new Sounds(new XactSound(soundBank1, "Cue 1", Keys.A))
			};
#else
				cueInfos = new List<Sounds>
				{
					new Sounds(new XactSound(soundBank1, "Cue 1", Keys.A)),
					new Sounds(new XactSound(soundBank1, "Cue 2", Keys.S)),
					new Sounds(new XactSound(soundBank1, "Cue 2b", Keys.D)),
					//new Sounds(new XactSound(soundBank1, "Cue 2c", Keys.D)),
					new Sounds(new XactSound(soundBank1, "Cue 4", Keys.F)),
					new Sounds(new XactSound(soundBank1, "Cue 5", Keys.G)),
					new Sounds(new XactSound(soundBank1, "Cue 6", Keys.H)),
					new Sounds(new XactSound(soundBank1, "Cue 7", Keys.J)),
					new Sounds(new XactSound(soundBank1, "Cue 8 (Doppler)", Keys.K)),
					new Sounds(new XactSound(soundBank1, "Cue 9 (Wind)", Keys.L)),
					new Sounds(new XactSound(soundBank1, "Cue 10 (Pitch)", Keys.Z)),
					new Sounds(new XactSound(soundBank1, "Cue 11 (Volume)", Keys.X)),
					new Sounds(new XactSound(soundBank1, "Cue 12 (Sound Pitch)", Keys.V)),
					new Sounds(new XnaSound(whiteNoiseSoundEffect, "Cue 10 (Pitch)", true, Keys.C)),
					new Sounds(new XactSound(soundBank2, "Music Cue 1", Keys.Q)),
					new Sounds(new XactSound(soundBank1, "Pitch Ramps", Keys.W)),
					new Sounds(new XactSound(soundBank1, "Volume Ramps", Keys.E)),
					new Sounds(new XactSound(soundBank1, "Volume & Pitch Ramps", Keys.R)),
					new Sounds(new XactSound(soundBank1, "Instance Limiting", Keys.T)),
					new Sounds(new XactSound(soundBank1, "Repeating Volume & Pitch", Keys.Y))
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
				return (keyboardState.IsKeyDown(modifierKey)
								|| (modifierKey == Keys.None && NoModifiersPresseed()))
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
						Sounds cueInfo = cueInfos[i];


						Sound nextSound = cueInfo.SoundPrototype;
						Keys soundEffectKey = nextSound.Key;

						// Prepare, start, and stop sounds.
						if (!nextSound.IsActive
								|| nextSound.IsPrepared)
						{
							// If the sound isn't active or if it is only prepared.

							// If the prepare modifier is used, then just get the cue in preparation to play, otherwise play the sound.
							if (IsKeyPressed(soundEffectKey, Keys.LeftShift))
							{
								if (nextSound is XactSound)
								{
									XactSound xactSound = nextSound as XactSound;
									xactSound.GetCue();
								}
							}
							else if (IsKeyPressed(soundEffectKey, Keys.None) 
							|| IsKeyPressed(soundEffectKey,Sound.PositionalModifierKey))
							{
								bool positional = IsKeyDown(Sound.PositionalModifierKey);
								nextSound.Play(listener, emitter, positional);

								cueInfo.ActiveSounds.Add(nextSound);
								cueInfo.SoundPrototype = (Sound) nextSound.Clone();
							}
						}

						if (cueInfo.ActiveSounds.Count > 0)
						{
							// One or more sounds are playing.

							if (IsKeyPressed(soundEffectKey, Sound.PauseModifierKey))
							{
								cueInfo.Paused = !cueInfo.Paused;
							}
							else if (IsKeyPressed(soundEffectKey, Sound.StopModifierKey))
							{
								if (cueInfo.IsPlaying)
								{
									cueInfo.Stop(AudioStopOptions.AsAuthored);
								}
								else if (cueInfo.IsStopped)
								{
									cueInfo.Dispose();
								}
								else if (cueInfo.IsDisposed)
								{
									cueInfo.Deactivate();
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
								manualXPos += direction * 25;
								emitter.Position = new Vector3(
									manualXPos,
									emitter.Position.Y,
									emitter.Position.Z);
								emitter.Velocity = new Vector3(
									direction * 2,
									emitter.Velocity.Y,
									emitter.Velocity.Z);
							}
						}

						previousKeyboardState = keyboardState;
					}

					const float maxDistance = 125;
					float periodicMovement =
						(float) Math.Sin(gameTime.TotalGameTime.TotalSeconds) * maxDistance;
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
						Sounds cueInfo = cueInfos[i];
						foreach (Sound cueSound in cueInfo.ActiveSounds)
						{
							if (cueSound.IsActive && !cueSound.IsDisposed
									&& cueSound.Positional)
							{
								cueSound.Apply3D(listener, emitter);
							}
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

				spriteBatch.DrawString(
					debugFont,
					string.Format(
						"emitterPosition={0} emitterVelocity={1}",
						emitter.Position.X,
						emitter.Velocity.X),
					position,
					Color.White);
				position.Y = position.Y + debugFont.LineSpacing;
				position.Y = position.Y + debugFont.LineSpacing;

				for (int i = 0; i < cueInfos.Count; i++)
				{
					if (cueInfos[i].ActiveSounds.Count > 0)
					{
						Sound cueSound = cueInfos[i].ActiveSounds[0];
						cueSound.DrawCueInfo(spriteBatch, debugFont, ref position);
						position.Y = position.Y + debugFont.LineSpacing * .25f;
					}
				}

				spriteBatch.End();

				base.Draw(gameTime);
			}
		}
	}
