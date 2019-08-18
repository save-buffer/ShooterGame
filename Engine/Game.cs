using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Threading;
namespace Engine
{
    public class Element
    {
        public Engine.PointF Position;

        public readonly string Name;
        public int ZOrder = 0;
        public bool Visible = true;

        public Element(string Name)
        {
            Position = new PointF(0, 0);
            this.Name = Name;
        }

        public virtual void Draw(Graphics g, PointF Dimensions)
        {

        }
        public virtual void Update()
        {
        }
    }
    public static class Game
    {
        private static game_obj game;
        public static Size Dimensions;
        public static uint FrameRate; //The framerate

        public static PointF GameToScreenCoordinates(PointF Position, PointF Dimensions, int Width, int Height)
        {
            return new PointF((Position.X + Dimensions.X / 2f) - Width / 2f,
                       (-Position.Y + Dimensions.Y / 2f) - Height / 2f);
        }

        public static PointF GameToScreenCoordinates(PointF Position, PointF Dimensions, float Width, float Height)
        {
            return new PointF((Position.X + Dimensions.X / 2f) - Width / 2f,
                       (-Position.Y + Dimensions.Y / 2f) - Height / 2f);
        }

        public static State CurrentState
        {
            get
            {
                return game.CurrentState;          
            }
            set
            {
                game.CurrentState = value;
                game.CurrentState.Initialize();
            }
        }
        public static void Initialize(State InitialState, string Name, int Width, int Height, uint Framerate)
        {
            game = new game_obj((uint)Width, (uint)Height, (uint)Framerate, Name);
            Dimensions = new Size(Width, Height);
            CurrentState = InitialState;
            while (game.Running)
                game.Update();
        }
        public static void Quit()
        {
            game.Running = false;
        }

        public static void AddElement(Element e)
        {
            game.CurrentState.Elements.Add(e.Name, e);
        }
        public static void RemoveElement(string name)
        {
            game.CurrentState.Elements.Remove(name);
        }
        public static Element GetElement(string name)
        {
            return game.CurrentState.Elements[name];
        }
        #region Framerate
        private static class FrameRateController
        {

            public static volatile bool ProceedToNextFrame; //Tells Game whether or not it can proceed to the next frame
            public static Thread FrameRateControlThread; //The timer for each frame runs on a separate thread
            private static Stopwatch watch; //The stopwatch to be used
            public static void Initialize(uint Framerate)
            {
                FrameRate = Framerate;
                watch = new Stopwatch();
                watch.Start(); //Start the watch
                FrameRateControlThread = new Thread(new ThreadStart(() => //Run the stopwatch until the Framerate Controller gets deinitialized.
                {
                    while (true)
                    {
                        if (watch.ElapsedMilliseconds >= 1000 / FrameRate)
                        {
                            watch.Restart();
                            ProceedToNextFrame = true;
                        }
                    }
                }));
                ProceedToNextFrame = true;
                FrameRateControlThread.Start();
            }
            public static void Deinitialize()
            {
                FrameRateControlThread.Abort();
            }
        }
        #endregion

        #region Input
        const int NumberOfKeys = 194; //That is the number of keys in the Keys enum
        public static bool[] KeysCurrentFrame = new bool[NumberOfKeys]; //All of the keys that are pressed this frame
        public static bool[] KeysLastFrame = new bool[NumberOfKeys]; //All of the keys that were pressed last frame
        public static bool[] KeyBuffer = new bool[NumberOfKeys]; //The asynchronous key buffer that is written into whenever the event fires
        public static void OnKeyDown(object Sender, KeyEventArgs e)
        {
            if (!(e.KeyValue > NumberOfKeys))
                KeyBuffer[e.KeyValue] = true;
        }
        public static void OnKeyReleased(object Sender, KeyEventArgs e)
        {
            if (!(e.KeyValue > NumberOfKeys))
                KeyBuffer[e.KeyValue] = false;
        }
        public static bool IsKeyPressed(Keys Key)
        {
            return KeysCurrentFrame[(int)Key];
        }
        public static bool IsKeyTriggered(Keys Key)
        {
            return KeysCurrentFrame[(int)Key] & !KeysLastFrame[(int)Key];
        }
        public static bool IsKeyReleased(Keys Key)
        {
            return !KeysCurrentFrame[(int)Key] & KeysLastFrame[(int)Key];
        }
        #endregion

        #region Game
        private class game_obj : Form
        {
            private bool running = true;
            public State CurrentState;
            public bool Running
            {
                get
                {
                    return running;
                }
                set
                {
                    if (!value)
                        this.OnFormClosing(new FormClosingEventArgs(CloseReason.UserClosing, false));
                }
            }//If Quit is true, the game will exit

            private System.ComponentModel.IContainer Components = null;

            public static PointF Dimensions; //The dimensions of the game
            public game_obj(uint Width, uint Height, uint FrameRate, string Name)
            {
                FrameRateController.Initialize(FrameRate); //Controls the Framerate to be at 60 FPS

                Components = new System.ComponentModel.Container();             //Form Setup
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.Text = Name;
                this.Width = (int)Width;
                this.Height = (int)Height;
                this.Visible = true;
                this.MinimizeBox = false;
                this.MaximizeBox = false;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.Activate();
                this.BackColor = Color.Black;
                this.DoubleBuffered = true;
                this.Paint += Game_Paint;

                this.KeyDown += new KeyEventHandler(Game_KeyDown); //Keyboard Input Handler setup
                this.KeyUp += new KeyEventHandler(Game_KeyUp);

                Dimensions = new PointF(Width, Height); //Sets the dimensions of the game
                XInputWrapper.XboxController.StartPolling(); //Start checking the XBoxController for input and updating the state

                this.FormClosing += (object sender, FormClosingEventArgs e) =>
                {
                    XInputWrapper.XboxController.StopPolling();
                    running = false;
                    FrameRateController.Deinitialize();
                };
            }
            void Game_Paint(object sender, PaintEventArgs e)
            {
                if (CurrentState != null && CurrentState.Elements != null)
                {
                    foreach (KeyValuePair<string, Element> Obj in CurrentState.Elements.OrderBy(Key => Key.Value.ZOrder)) //Order the gameobjects by ZOrder, drawing the lowest first
                    {
                        if (Obj.Value.Visible)
                        {
                            Obj.Value.Draw(e.Graphics, Dimensions);
                        }
                    }
                }
            }

            void Game_KeyDown(object sender, KeyEventArgs e)
            {
                Game.OnKeyDown(sender, e);
            }

            void Game_KeyUp(object sender, KeyEventArgs e)
            {
                Game.OnKeyReleased(sender, e);
            }

            new public void Update()
            {
                Application.DoEvents();
                base.Update();
                if (FrameRateController.ProceedToNextFrame) //If we can move onto the next frame
                {
                    for (int i = 0; i < KeyBuffer.Length; i++) //Copy the asynchronous buffer into the synchronous one
                    {
                        KeysCurrentFrame[i] = KeyBuffer[i];
                    }
                    this.Invalidate(); //Invalidate the current image. This calls the Game_Paint event
                    if (CurrentState != null)
                        CurrentState.Update(); //Update the current GameState
                    FrameRateController.ProceedToNextFrame = false; //Notify the FramerateController that the frame has been executed

                    for (int i = 0; i < KeysLastFrame.Length; i++) //Copy the synchronous buffer into the previous frame buffer
                    {
                        KeysLastFrame[i] = KeysCurrentFrame[i];
                    }
                }

            }

            private void InitializeComponent()
            {

                //DebugLabel.Size = new System.Drawing.Size(35, 13);
                //Game Setup
                this.ClientSize = new System.Drawing.Size(490, 301);
                this.DoubleBuffered = true;
                this.Name = "Game";
                this.ResumeLayout(false);
                this.PerformLayout();

            }

        }
    }
    #endregion
}
